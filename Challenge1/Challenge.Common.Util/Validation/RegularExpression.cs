namespace challenge1.Common.Util.Validation
{
    public static class RegularExpression
    {
        public static readonly string Date = @"^((((0?[1-9]|[12]\d|3[01])[\.\-\/](0?[13578]|1[02])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|[12]\d|30)[\.\-\/](0?[13456789]|1[012])[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|((0?[1-9]|1\d|2[0-8])[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?\d{2}))|(29[\.\-\/]0?2[\.\-\/]((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00)))|(((0[1-9]|[12]\d|3[01])(0[13578]|1[02])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|[12]\d|30)(0[13456789]|1[012])((1[6-9]|[2-9]\d)?\d{2}))|((0[1-9]|1\d|2[0-8])02((1[6-9]|[2-9]\d)?\d{2}))|(2902((1[6-9]|[2-9]\d)?(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00)|00)))) ?((20|21|22|23|[01]\d|\d)(([:.][0-5]\d){1,2}))?$";
        public static readonly string Decimal = @"^((-?[1-9]+)|[0-9]+)(\.?|\,?)([0-9]*)$";
        public static readonly string Email = @"^([a-z0-9_\.\-]{3,})@([\da-z\.\-]{3,})\.([a-z\.]{2,6})$";
        public static readonly string Hex = "^#?([a-f0-9]{6}|[a-f0-9]{3})$";
        public static readonly string Hour = @"^(20|21|22|23|[01]\d|\d)(([:.][0-5]\d){1,2})$";
        public static readonly string Integer = "^((-?[1-9]+)|[0-9]+)$";
        public static readonly string Login = "^[a-z0-9_-]{10,50}$";
        public static readonly string Pass = @"^.*(?=.{10,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&+=]).*$";
        public static readonly string Url = @"http[s]?://(([^/:\.[:space:]]+(\.[^/:\.[:space:]]+)*)|([0-9](\.[0-9]{3})))(:[0-9]+)?((/[^?#[:space:]]+)(\?[^#[:space:]]+)?(\#.+)?)?";
        public static readonly string Phone = @"\+(9[976]\d|8[987530]\d|6[987]\d|5[90]\d|42\d|3[875]\d|2[98654321]\d|9[8543210]|8[6421]|6[6543210]|5[87654321]|4[987654310]|3[9643210]|2[70]|7|1)\d{1,14}$";
        public static readonly string NRIC = @"^[STFG]\d{7}[A-Z]$";
        public static readonly string PostalCode = @"^\d{6}$";
        public static readonly string UEN = @"^\d{9}[A-Z]$";

        //Create function to validate date from string
        public static bool IsDate(string date)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(date, Date);
        }

        //Function to validate decimal from string
        public static bool IsDecimal(string dec)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(dec, Decimal);
        }

        //Function to validate email from string 
        public static bool IsEmail(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email, Email);
        }

        //Function to validate hex from string
        public static bool IsHex(string hex)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(hex, Hex);
        }

        //Function to validate hour from string
        public static bool IsHour(string hour)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(hour, Hour);
        }

        //Function to validate integer from string
        public static bool IsInteger(string integer)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(integer, Integer);
        }

        //Function to validate login from string
        public static bool IsLogin(string login)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(login, Login);
        }

        //Function to validate password from string
        public static bool IsPass(string pass)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(pass, Pass);
        }

        //Function to validate NRIC from string
        public static bool IsNRIC(string nric)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(nric, NRIC);
        }

        //Function to validate PostalCode from string
        public static bool IsPostalCode(string postalCode)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(postalCode, PostalCode);
        }

        //Function to validate UEN from string
        public static bool IsUEN(string uen)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(uen, UEN);
        }
    }
}
