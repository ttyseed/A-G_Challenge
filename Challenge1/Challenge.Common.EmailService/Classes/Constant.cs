namespace challenge1.Common.EmailService.Classes
{
    public static class Constant
    {
        public static class DbProviders
        {
            public const string PostgreSQL = "PostgreSQL";
            public const string SqlServer = "SqlServer";
        }

        private static string? _emailCSS;
        internal static string EmailCSS
        {
            get
            {
                if (string.IsNullOrEmpty(_emailCSS))
                {
                    var assembly = typeof(Constant).Assembly;

                    using var stream = assembly.GetManifestResourceStream("challenge1.Common.EmailService.Resources.EmailTemplate.css");
                    using var reader = new StreamReader(stream!);
                    _emailCSS = reader.ReadToEnd();

                }
                return _emailCSS;
            }
        }

        internal static class EmailStatus
        {
            public const string Pending = "Pending";
            public const string Sent = "Sent";
            public const string Failed = "Failed";
            public const string Cancelled = "Cancelled";
        }

        internal static class SendType
        {
            public const string Manual = "Manual";
            public const string BatchJob = "BatchJob";
        }

        internal static class AttachmentType
        {
            public const string Database = "Database";
            public const string S3 = "S3";
        }

        internal static class EmailLogType
        {
            public const string Information = "Information";
            public const string Error = "Error";
        }

        internal static string MapAttachmentType(AttachmentTypeEnum type)
        {
            return type switch
            {
                AttachmentTypeEnum.Database => AttachmentType.Database,
                AttachmentTypeEnum.S3 => AttachmentType.S3,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        internal static AttachmentTypeEnum MapAttachmentTypeEnum(string type)
        {
            return type switch
            {
                AttachmentType.Database => AttachmentTypeEnum.Database,
                AttachmentType.S3 => AttachmentTypeEnum.S3,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}
