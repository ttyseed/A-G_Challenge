using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Database.Models
{
    [Table("backend_user")]
    public partial class BackendUser
    {
        [Key]
        [Column("user_id")]
        public Guid UserId { get; set; }
        [Column("login_id")]
        [MaxLength(30)]
        public string LoginId { get; set; } = null!;
        [Column("full_name")]
        [MaxLength(66)]
        public string FullName { get; set; } = null!;
        [Column("telephone_no")]
        [MaxLength(19)]
        public string? TelephoneNo { get; set; }
        [Column("email_address")]
        [MaxLength(320)]
        public string EmailAddress { get; set; } = null!;
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
        [Column("status")]
        public short Status { get; set; }
        [Column("password_hash")]
        [MaxLength(128)]
        public string? PasswordHash { get; set; }

        [NotMapped]
        public int? IntError { get; set; }

        //Child Table(s)
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    //Generate create script for the table backend_user for ms sql server
    //    CREATE TABLE[dbo].[backend_user]
    //    (
    //      [user_id][uniqueidentifier] NOT NULL,
    //      [login_id] [nvarchar] (30) NOT NULL,
    //      [full_name] [nvarchar] (66) NOT NULL,
    //      [telephone_no] [nvarchar] (19) NULL,
    //      [email_address][nvarchar] (320) NOT NULL,
    //      [is_deleted] [bit]
    //    NOT NULL,
    //      [remark] [nvarchar] (1000) NULL,
    //      [created_by_id][nvarchar] (40) NOT NULL,
    //      [created_by_name] [nvarchar] (66) NOT NULL,
    //      [created_date] [datetime]
    //    NOT NULL,
    //      [updated_by_id] [nvarchar] (40) NULL,
    //#pragma warning restore
    //      [updated_by_name][nvarchar] (66) NULL,
    //      [updated_date][datetime] NULL,
    //      [status]
    //    [smallint]
    //    NOT NULL,
    //      CONSTRAINT[PK_backend_user] PRIMARY KEY CLUSTERED
    //      (
    //          [user_id] ASC
    //      )WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]
    //      ) ON[PRIMARY]
    //      GO

    //Generate insert script for the table backend_user for ms sql server
    //INSERT INTO[dbo].[backend_user]
    //([user_id], [login_id], [full_name], [telephone_no], [email_address], [is_deleted], [remark], [created_by_id], [created_by_name], [created_date], [updated_by_id], [updated_by_name], [updated_date], [status])
    //VALUES
    //('00000000-0000-0000-0000-000000000001', 'admin', 'Administrator', NULL, 'admin@localhost', 0, NULL, '00000000-0000-0000-0000-000000000001', 'Administrator', '2021-01-01 00:00:00', NULL, NULL, NULL, 1)
    //    GO

}
