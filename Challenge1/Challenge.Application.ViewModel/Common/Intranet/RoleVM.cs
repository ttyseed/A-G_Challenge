namespace challenge1.Application.ViewModel.Common.Intranet
{
    public class RoleVM
    {
        public Guid RoleId { get; set; }
        public short RoleNumber { get; set; }
        public string RoleName { get; set; } = null!;
        public short UserType { get; set; }
        public short ModuleType { get; set; }
        public string? RoleRemark { get; set; }
        public bool IsDeleted { get; set; }
        public string? Remark { get; set; }
        public string CreatedById { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public string? UpdatedById { get; set; }
        public string? UpdatedByName { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
