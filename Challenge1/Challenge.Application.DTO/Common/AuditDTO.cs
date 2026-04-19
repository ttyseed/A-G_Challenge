namespace challenge1.Application.DTO.Common
{
    public class GetAuditListRequestDTO
    {
        public string ActionCode { get; set; } = "";
        public string ActionName { get; set; } = "";
    }

    public class AddAuditRequestDTO
    {
        public Guid AuditId { get; set; }
        public string ActionCode { get; set; } = null!;
        public string ActionName { get; set; } = null!;
        public string? ItemId { get; set; }
        public string? ItemTitle { get; set; }
        public string? ItemContent { get; set; }
        public string? ItemContentHistory { get; set; }
        public string? UserAgent { get; set; }
        public string? Url { get; set; }
        public string? Referer { get; set; }
        public string? IP { get; set; }
        public short UserType { get; set; }
        public short ModuleType { get; set; }
        public bool? IsVisible { get; set; }
        public string? Remark { get; set; }
        public string CreatedById { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
        public string CreatedByLoginId { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }
}
