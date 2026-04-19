using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace challenge1.Database.Models
{
	[Table("user_role")]
	public partial class UserRole
	{
		[Key]
		[Column("user_role_id")]
		public Guid UserRoleId { get; set; }
		[Column("user_id")]
		public Guid UserId { get; set; }
		[Column("role_number")]
		public short RoleNumber { get; set; }
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
        public BackendUser? BackendUser { get; set; }

        //Child Table(s)
        public Role Role { get; set; } = new Role();
    }
}
