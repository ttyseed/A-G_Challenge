using System.Runtime.CompilerServices;
using System.Text;

namespace challenge1.Common.Util.Common
{
    public abstract class Constant
    {
        public const string DEFAULT_PUBLIC_USER_ID = "0";
        public const string DEFAULT_PUBLIC_USER_NAME = "Public";

        public static class Env
        {
            public const string DEV = "DEV";
            public const string UAT = "UAT";
            public const string PRD = "PRD";
            public const string Staging = "Staging";
        }

        public static class ModuleType
		{
			public const short Admin = 0;
			public const short COMMON = 99;
		}

		public static class UserType
		{
			public const short BACKEND_USER = 1;
			public const short E_SERVICE_USER = 2;
			public const short SYSTEM = 99;
		}

        public static class GenericStatus
        {
            public const short Active = 1;
            public const short Inactive = 0;
        }
        public static class LogMessage
        {
            public const string StateStart = "state:start";
            public const string StateEnd = "state:end";
            public const string StateError = "state:error";
        }

        public static class Pagination
        {
            public const short DEFAULT_PAGE_SIZE = 5;
            public const short DEFAULT_PAGE_NUMBER = 1;
        }

        //To change to actual project name
        public static class Audit
        {
            public static string GenerateAuditActionCode(object component, string filePath = "", [CallerMemberName] string methodName = "")
            {
                return $"SimsysTemplate.{component.GetType().Name}.{Path.GetFileNameWithoutExtension(filePath)}.{methodName}";
            }

            public static string GenerateAuditActionName(object component, string filePath = "", [CallerMemberName] string methodName = "", string? description = null)
            {
                var baseName = $"SimsysTemplate-{component.GetType().Name}.{Path.GetFileNameWithoutExtension(filePath)}.{methodName}";
                return string.IsNullOrEmpty(description) ? baseName : $"{baseName}-{description}";
            }

            #region Audit Action Type Mapping Dictionary

            // Define constants for action keys
            public static class AuditActionKeys
            {
                //Login, Forgot Password & Setting
                public const string SettingEdit = "/Setting/Edit/OnPostEditSetting";
                public const string UserLogin = "/Login/";
                public const string UserLoginOnGet = "/Login/OnGet";
                public const string UserLoginOnPostLogin = "/Login/OnPostLogin";
            }

            // Mapper dictionary: maps keys to user-friendly names
            public static class AuditActionTypeMapper
            {
                public static readonly Dictionary<string, string> ActionTypeKey = new Dictionary<string, string>
                {
                    { AuditActionKeys.SettingEdit, "Edit Setting" },
                    { AuditActionKeys.UserLogin, "Login/Log Out" },
                    { AuditActionKeys.UserLoginOnGet, "Login/Log Out" },
                    { AuditActionKeys.UserLoginOnPostLogin, "Login/Log Out" },
                };
            }

            #endregion
        }
    }
}
