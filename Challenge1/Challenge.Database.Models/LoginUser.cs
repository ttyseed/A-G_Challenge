using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Database.Models
{
    [Table("login_user")]
    public partial class LoginUser
    {
        [Key]
        [Column("login_user_id")]
        public Guid LoginUserId { get; set; }
        [Column("login_id")]
        [MaxLength(30)]
        public string LoginId { get; set; } = null!;
        [Column("login_name")]
        [MaxLength(100)]
        public string LoginName { get; set; } = null!;
        [Column("login_type")]
        public short LoginType { get; set; }
        [Column("reference_id")]
        public Guid? ReferenceId { get; set; } = null!;
		[Column("reference_type")]
		public short ReferenceType { get; set; }
		[Column("last_login_code")]
		[MaxLength(320)]
		public string? LastLoginCode { get; set; } = null!;
		[Column("last_login_date", TypeName = "timestamp without time zone")]
		public DateTime? LastLoginDate { get; set; } = null!;
		[Column("last_logout_date", TypeName = "timestamp without time zone")]
		public DateTime? LastLogoutDate { get; set; } = null!;
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

        [NotMapped]
        public int? IntError { get; set; }
    }

	//Generate create script for the table login_user for ms sql server
	//    CREATE TABLE[dbo].[login_user]
	//    (
	//      [login_user_id][uniqueidentifier] NOT NULL,
	//      [login_id] [nvarchar] (30) NOT NULL,
	//      [login_name] [nvarchar] (100) NOT NULL,
	//		[login_type] [smallint],
	//      [reference_id] [uniqueidentifier] NULL,
	//		[reference_type] [smallint],
	//      [last_login_code] [nvarchar] (320) NULL,
	//      [last_login_date] [datetime] NULL,
	//      [last_logout_date] [datetime] NULL,
	//		[status] [smallint],
	//      [is_deleted] [bit] NOT NULL,
	//      [remark] [nvarchar] (1000) NULL,
	//      [created_by_id][nvarchar] (40) NOT NULL,
	//      [created_by_name] [nvarchar] (66) NOT NULL,
	//      [created_date] [datetime] NOT NULL,
	//      [updated_by_id] [nvarchar] (40) NULL,
	//      [updated_by_name][nvarchar] (66) NULL,
	//      [updated_date][datetime] NULL,
	//    NOT NULL,
	//      CONSTRAINT[PK_login_user] PRIMARY KEY CLUSTERED
	//      (
	//          [login_user_id] ASC
	//      )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
	//      ) ON[PRIMARY]
	//      GO

	//Generate insert script for the table login_user for ms sql server
	//INSERT INTO[dbo].[login_user]
	//([login_user_id], [login_id], [login_name], [login_type], [reference_id], [reference_type], [last_login_code], [last_login_date], [last_logout_date], [status], 
	//[is_deleted], [remark], [created_by_id], [created_by_name], [created_date], [updated_by_id], [updated_by_name], [updated_date])
	//VALUES
	//('00000000-0000-0000-0000-000000000001', 'WOG001', 'Administrator', 1, '', 0, NULL, NULL, NULL, 1
	//false, NULL, '0', 'System', '2021-01-01 00:00:00', NULL, NULL, NULL)
	//    GO

}
