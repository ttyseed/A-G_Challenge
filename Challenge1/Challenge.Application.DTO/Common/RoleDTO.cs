using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace challenge1.Application.DTO.Common
{
    public class GetRolesResponseDTO
    {
        public Guid RoleId { get; set; }
        public short RoleNumber { get; set; }
        public string RoleName { get; set; } = null!;
    }

    public class GetRoleByIdResponseDTO
    {
        public Guid RoleId { get; set; }
        public short RoleNumber { get; set; }
        public string RoleName { get; set; } = null!;
        public short UserType { get; set; }
        public short ModuleType { get; set; }
    }

    public class CreateRoleRequestDTO
    {
        public short RoleNumber { get; set; }
        public string RoleName { get; set; } = null!;
        public short UserType { get; set; }
        public short ModuleType { get; set; }
        public string CreatedById { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
    }

    public class UpdateRoleRequestDTO
    {
        public Guid RoleId { get; set; }
        public short RoleNumber { get; set; }
        public string RoleName { get; set; } = null!;
        public string UpdatedById { get; set; } = null!;
        public string UpdatedByName { get; set; } = null!;
    }

    public class SoftDeleteRoleRequestDTO
    {
        public Guid RoleId { get; set; }
        public string UpdatedById { get; set; } = null!;
        public string UpdatedByName { get; set; } = null!;
    }
}
