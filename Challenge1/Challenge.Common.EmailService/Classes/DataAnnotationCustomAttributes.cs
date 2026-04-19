using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace challenge1.Common.EmailService.Classes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    internal class EmailListAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var emails = value as string;

            if (string.IsNullOrWhiteSpace(emails))
                return ValidationResult.Success;

            var emailList = emails.Split(';', StringSplitOptions.RemoveEmptyEntries)
                                  .Select(e => e.Trim());

            var invalidEmails = emailList.Where(email =>
            {
                try
                {
                    var addr = new MailAddress(email);
                    return addr.Address != email;
                }
                catch
                {
                    return true;
                }
            }).ToList();

            if (invalidEmails.Any())
            {
                return new ValidationResult($"Invalid email(s): {string.Join(", ", invalidEmails)}");
            }

            return ValidationResult.Success;
        }
    }
}

