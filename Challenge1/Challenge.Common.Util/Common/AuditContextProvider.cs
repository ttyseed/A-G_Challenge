using System;
using System.Collections.Generic;
using System.Text;

namespace challenge1.Common.Util.Common
{
    public static class AuditContextProvider
    {
        // These values would be set manually per request in your application logic
        public static string? CurrentUserId { get; set; }
        public static string? CurrentUserName { get; set; }
        public static string? IPAddress { get; set; }
        public static string? UserAgent { get; set; }
        public static string? Url { get; set; }
        public static string? Referer { get; set; }
        public static string? SourceMethod { get; set; }
    }
}
