using System;
using System.Collections.Generic;
using System.Text;

namespace challenge1.Application.Filter.Common
{
    public class GetLoginUsersFilter
    {
        public List<Guid>? LoginUserId { get; set; }
        public string? LoginId { get; set; }
        public string? LoginName { get; set; }
        public short? LoginType { get; set; }
        public Guid? ReferenceId { get; set; }
        public short? ReferenceType { get; set; }
        public string? LastLoginCode { get; set; }
        public DateTime? LastLoginDateStart { get; set; }
        public DateTime? LastLoginDateEnd { get; set; }
        public DateTime? LastLogoutDateStart { get; set; }
        public DateTime? LastLogoutDateEnd { get; set; }
        public short? Status { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
