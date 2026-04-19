using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Application.DTO.Common
{

    public class SoftDeleteUserRoleRequestDTO
    { 
        public Guid UserRoleId { get; set; }
        public string UpdatedById { get; set; } = null!;
        public string UpdatedByName { get; set; } = null!;
    }

    public class UpdateUserRoleRequestDTO
    {
        public Guid UserRoleId { get; set; }
        public short RoleNumber { get; set; }
        public string? UpdatedById { get; set; }
        public string? UpdatedByName { get; set; }
    }
}
