namespace challenge1.Application.Filter.Common
{

    public class GetBackendUsersFilter
    {
        public List<Guid>? UserId { get; set; }
        public short? Status { get; set; }
        public string? LoginId { get; set; }
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
