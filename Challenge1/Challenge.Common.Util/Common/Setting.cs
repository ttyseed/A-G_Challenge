namespace challenge1.Common.Util.Common
{
    public static class Setting
    {
        #region DEV
        public const string DBSchema = "template"; //To change to actual project name
        public const string Env = Constant.Env.DEV;
        public const string SecretName = "";
        public const string CookieDomainName = "";
        public const bool CookieOptionsHttpOnly = false;
        public const bool CookieOptionsSecure = false;
        public const string AssumeRoleArn = "";
        public const bool EnableDebugLog = true;

        #endregion

        #region UAT
        //public const string DBSchema = "SimsysTemplate";
        //public const string Env = Constant.Env.UAT;
        //public const string SecretName = "";
        //public const string CookieDomainName = "";
        //public const bool CookieOptionsHttpOnly = false;
        //public const bool CookieOptionsSecure = false;
        //public const string AssumeRoleArn = "";
        //public const bool EnableDebugLog = true;
        #endregion

        #region PRD
        //public const string DBSchema = "SimsysTemplate";
        //public const string Env = Constant.Env.PRD;
        //public const string SecretName = "";
        //public const string CookieDomainName = "";
        //public const bool CookieOptionsHttpOnly = false;
        //public const bool CookieOptionsSecure = false;
        //public const string AssumeRoleArn = "";
        //public const bool EnableDebugLog = false;
        #endregion

    }
}