using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace challenge1.Database.Models
{
	[Table("role")]
	public partial class Role
	{
		[Key]
		[Column("role_id")]
		public Guid RoleId { get; set; }
		[Column("role_number")]
		public short RoleNumber { get; set; }
		[Column("role_name")]
		[MaxLength(100)]
		public string RoleName { get; set; } = null!;
		[Column("user_type")]
		public short UserType { get; set; }
		[Column("module_type")]
		public short ModuleType { get; set; }
		[Column("role_remark")]
		[MaxLength(200)]
		public string? RoleRemark { get; set; }
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

        //Parent Table
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
