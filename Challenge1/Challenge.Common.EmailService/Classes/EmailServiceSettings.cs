namespace challenge1.Common.EmailService.Classes
{
    /// <summary>
    /// Represents configuration settings required for sending emails via an SMTP server. Use Dependency Injection to inject.
    /// Example:
    /// services.Configure<EmailServiceSettings>(options =>
    /// {
    ///     options.Environment = "DEV";
    ///     options.SmtpHost = "smtp.gmail.com";
    ///     options.SmtpPort = 587;
    ///     options.SmtpUsername = "myapp@domain.com";
    ///     options.SmtpPassword = "supersecure";
    ///     options.SmtpSender = "noreply@domain.com";
    /// });
    /// </summary>
    /// <remarks>This class encapsulates SMTP connection details and retry behavior for email delivery. It is
    /// typically used to configure email services in different application environments, such as development, staging,
    /// or production. All properties should be set according to the requirements of the target SMTP server and
    /// environment.</remarks>
    public class EmailServiceSettings
    {
        /// <summary>
        /// Environment in which the email service is running (e.g., "DEV", "UAT", "PROD").
        /// </summary>
        public string Environment { get; set; } = string.Empty;
        /// <summary>
        /// Database schema (optional)
        /// </summary>
        public string? DBSchema { get; set; } = string.Empty;
        /// <summary>
        /// SMTP server host address.
        /// </summary>
        public string SmtpHost { get; set; } = string.Empty;
        /// <summary>
        /// SMTP server port number.
        /// </summary>
        public int? SmtpPort { get; set; }
        /// <summary>
        /// SMTP username for authentication.
        /// </summary>
        public string? SmtpUsername { get; set; } = string.Empty;
        /// <summary>
        /// SMTP password for authentication.
        /// </summary>
        public string? SmtpPassword { get; set; } = string.Empty;
        /// <summary>
        /// Default sender email address.
        /// </summary>
        public string? SmtpSender { get; set; } = string.Empty;

        internal const short MaxRetryCount = 3;
        internal const short MaxRecipientsPerBatch = 50;
        internal const string BatchJobId = "EmailServiceBatchJob";
        internal const string BatchJobName = "Email Service Batch Job";
    }
}
