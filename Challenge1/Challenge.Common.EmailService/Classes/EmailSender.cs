using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using challenge1.Common.EmailService.Classes.Interfaces;
using challenge1.Common.EmailService.DTOs;

namespace challenge1.Common.EmailService.Classes
{
    internal class EmailSender : IEmailSender
    {
        private readonly IOptions<EmailServiceSettings> _options;
        private readonly IS3Helper _s3Helper;
        private readonly IUtilities _utilities;

        public EmailSender(IS3Helper s3Helper, IOptions<EmailServiceSettings> options, IUtilities utilities)
        {
            _s3Helper = s3Helper;
            _options = options;
            _utilities = utilities;
        }

        public async Task SendEmailAsync(string subject, string? body, string recipients, string? cc, string? bcc, List<AttachmentDTO>? attachments, int? maxRecipientsPerBatch, CancellationToken cancellationToken = default)
        {
            try
            {
                var smtpUsername = _options.Value.SmtpUsername;
                var smtpPw = _options.Value.SmtpPassword;
                var smtpHost = _options.Value.SmtpHost;
                var smtpPort = _options.Value.SmtpPort;
                var smtpSender = _options.Value.SmtpSender;

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpSender))
                {
                    throw new Exception("SMTP settings are not properly configured.");
                }

                body = Constant.EmailCSS + body;

                // Build recipient list (split by semicolon and remove empties)
                var recipientList = (recipients ?? string.Empty)
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();

                if (!recipientList.Any())
                {
                    throw new Exception("No recipients provided.");
                }

                // Preload attachments into memory so they can be reused across batches
                var preloadedAttachments = new List<(string FileName, byte[] Content)>();
                if (attachments != null && attachments.Count > 0)
                {
                    foreach (var attachment in attachments)
                    {
                        if (attachment.AttachmentType == AttachmentTypeEnum.Database && attachment.Content != null)
                        {
                            preloadedAttachments.Add((attachment.FileName, attachment.Content));
                        }
                        else if (attachment.AttachmentType == AttachmentTypeEnum.S3 && !string.IsNullOrEmpty(attachment.AttachmentLocation))
                        {
                            var s3Info = _utilities.ParseS3Path(attachment.AttachmentLocation);
                            var s3Response = await _s3Helper.DownloadFromBucketAsync(s3Info.bucketName, s3Info.key);
                            if (s3Response != null)
                            {
                                using var ms = new MemoryStream();
                                await s3Response.CopyToAsync(ms, cancellationToken);
                                preloadedAttachments.Add((attachment.FileName, ms.ToArray()));
                            }
                        }
                    }
                }

                // Determine batch size (if null, <=0 or >= total, send in one batch)
                int batchSize;
                if (maxRecipientsPerBatch == null || maxRecipientsPerBatch <= 0 || maxRecipientsPerBatch >= recipientList.Count)
                {
                    batchSize = recipientList.Count;
                }
                else
                {
                    batchSize = maxRecipientsPerBatch.Value;
                }

                // Create SMTP client and connect once, then send messages for each batch
                using var smtp = new SmtpClient();
                if (_options.Value.SmtpPort != null)
                {
                    await smtp.ConnectAsync(smtpHost, _options.Value.SmtpPort.Value, MailKit.Security.SecureSocketOptions.Auto, cancellationToken);
                }
                else
                {
                    await smtp.ConnectAsync(smtpHost, 0, MailKit.Security.SecureSocketOptions.None, cancellationToken);
                }

                if (!string.IsNullOrEmpty(smtpUsername) && !string.IsNullOrEmpty(smtpPw))
                {
                    await smtp.AuthenticateAsync(smtpUsername, smtpPw, cancellationToken);
                }

                try
                {
                    for (int i = 0; i < recipientList.Count; i += batchSize)
                    {
                        var batch = recipientList.Skip(i).Take(batchSize).ToList();

                        var email = new MimeMessage();

                        foreach (string s in smtpSender.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            email.From.Add(MailboxAddress.Parse(s.Trim()));
                        }

                        email.Subject = subject;

                        foreach (string r in batch)
                        {
                            email.To.Add(MailboxAddress.Parse(r));
                        }

                        if (!string.IsNullOrEmpty(cc))
                        {
                            foreach (string s in cc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                email.Cc.Add(MailboxAddress.Parse(s.Trim()));
                            }
                        }

                        if (!string.IsNullOrEmpty(bcc))
                        {
                            foreach (string s in bcc.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                email.Bcc.Add(MailboxAddress.Parse(s.Trim()));
                            }
                        }

                        var builder = new BodyBuilder
                        {
                            HtmlBody = body
                        };

                        // Attach preloaded attachments for this batch
                        if (preloadedAttachments.Any())
                        {
                            foreach (var att in preloadedAttachments)
                            {
                                builder.Attachments.Add(att.FileName, att.Content);
                            }
                        }

                        email.Body = builder.ToMessageBody();

                        var response = await smtp.SendAsync(email, cancellationToken);
                        //Temp
                        Console.WriteLine($"Email Response: {response}");
                    }
                }
                finally
                {
                    await smtp.DisconnectAsync(true, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to send email.", ex);
            }
        }
    }
}
