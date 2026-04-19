namespace challenge1.Application.ViewModel.Common.Intranet
{
    public class LoginUserVM
    {
        public Guid LoginUserId { get; set; }
        public string LoginId { get; set; } = "";
        public string LoginName { get; set; } = "";
        public short LoginType { get; set; }
        public Guid? ReferenceId { get; set; } = null;
        public short ReferenceType { get; set; }
        public string LastLoginCode { get; set; } = "";
        public DateTime? LastLoginDate { get; set; } = null;
        public DateTime? LastLogoutDate { get; set; } = null;
        public short Status { get; set; }
    }
}
