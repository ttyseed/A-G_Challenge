using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace challenge1.Application.DTO.Common
{
    public class GetLoginUserByLoginIdResponseDTO
    {
        public Guid LoginUserId { get; set; }
        public string LoginId { get; set; } = null!;
        public string LoginName { get; set; } = null!;
        public short LoginType { get; set; }
        public Guid? ReferenceId { get; set; } = null!;
        public short ReferenceType { get; set; }
        public string? LastLoginCode { get; set; } = null!;
        public DateTime? LastLoginDate { get; set; } = null!;
        public DateTime? LastLogoutDate { get; set; } = null!;
        public short Status { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class GetLoginUsersResponseDTO
    {
        public Guid LoginUserId { get; set; }
        public string LoginId { get; set; } = null!;
        public string LoginName { get; set; } = null!;
        public short LoginType { get; set; }
        public Guid? ReferenceId { get; set; } = null!;
        public short ReferenceType { get; set; }
        public string? LastLoginCode { get; set; } = null!;
        public DateTime? LastLoginDate { get; set; } = null!;
        public DateTime? LastLogoutDate { get; set; } = null!;
        public short Status { get; set; }
        public bool IsDeleted { get; set; }
    }
}
