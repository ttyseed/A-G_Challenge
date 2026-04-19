using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace challenge1.Database.Models
{
	[Table("audit_code")]
	public partial class AuditCode
	{
		[Key]
		[Column("audit_code_id")]
		public Guid AuditCodeId { get; set; }
		[Column("action_code")]
		[MaxLength(50)]
		public string ActionCode { get; set; } = null!;
		[Column("action_name")]
		[MaxLength(100)]
		public string ActionName { get; set; } = null!;
		[Column("user_type")]
		public short UserType { get; set; }
		[Column("module_type")]
		public short ModuleType { get; set; }
		[Column("sort_order")]
		public int SortOrder { get; set; }
		[Column("status")]
		public short Status { get; set; }
		[Column("is_deleted")]
		public bool IsDeleted { get; set; }
		[Column("remark")]
		[MaxLength(1000)]
		public string? Remark { get; set; }
		[Column("created_by_id")]
		[MaxLength(40)]
		public string CreatedById { get; set; } = null!;
		[Column("created_by_name")]
		[MaxLength(66)]
		public string CreatedByName { get; set; } = null!;
		[Column("created_date", TypeName = "timestamp without time zone")]
		public DateTime CreatedDate { get; set; }
		[Column("updated_by_id")]
		[MaxLength(40)]
		public string? UpdatedById { get; set; }
		[Column("updated_by_name")]
		[MaxLength(66)]
		public string? UpdatedByName { get; set; }
		[Column("updated_date", TypeName = "timestamp without time zone")]
		public DateTime? UpdatedDate { get; set; }
	}
}
