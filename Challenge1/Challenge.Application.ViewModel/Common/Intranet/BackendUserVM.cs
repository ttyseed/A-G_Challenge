namespace challenge1.Application.ViewModel.Common.Intranet
{
    public class GetBackendUsersRolesVM
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? TelephoneNo { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
