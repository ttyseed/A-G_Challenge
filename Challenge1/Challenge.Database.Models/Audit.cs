using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace challenge1.Database.Models
{
	[Table("audit")]
	[Index("ActionCode", Name = "idx_audit_action_code")]
	public partial class Audit
	{
		[Key]
		[Column("audit_id")]
		public Guid AuditId { get; set; }
		[Column("action_code")]
		[MaxLength(200)]
		public string ActionCode { get; set; } = null!;
		[Column("action_name")]
		[MaxLength(300)]
		public string ActionName { get; set; } = null!;
		[Column("item_id")]
		[MaxLength(40)]
		public string? ItemId { get; set; }
		[Column("item_title")]
		[MaxLength(200)]
		public string? ItemTitle { get; set; }
		[Column("item_content")]
		public string? ItemContent { get; set; }
		[Column("item_content_history")]
		public string? ItemContentHistory { get; set; }
		[Column("user_agent")]
		[MaxLength(500)]
		public string? UserAgent { get; set; }
		[Column("url")]
		[MaxLength(500)]
		public string? Url { get; set; }
		[Column("referer")]
		[MaxLength(500)]
		public string? Referer { get; set; }
		[Column("ip")]
		[MaxLength(1000)]
		public string? IP { get; set; }
		[Column("user_type")]
		public short UserType { get; set; }
		[Column("module_type")]
		public short ModuleType { get; set; }
		[Required]
		[Column("is_visible")]
		public bool? IsVisible { get; set; }
		[Column("remark")]
		[MaxLength(1000)]
		public string? Remark { get; set; }
		[Column("created_by_id")]
		[MaxLength(40)]
		public string CreatedById { get; set; } = null!;
		[Column("created_by_name")]
		[MaxLength(66)]
		public string CreatedByName { get; set; } = null!;
		[Column("created_by_login_id")]
		[MaxLength(200)]
		public string CreatedByLoginId { get; set; } = "";
		[Column("created_date", TypeName = "timestamp without time zone")]
		public DateTime CreatedDate { get; set; }
	}
}
