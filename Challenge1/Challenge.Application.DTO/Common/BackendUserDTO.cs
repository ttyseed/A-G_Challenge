using challenge1.Common.Util.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace challenge1.Application.DTO.Common
{
    public class GetBackendUserByLoginIdResponseDTO
    {
        public string UserId { get; set; } = null!;
        public string LoginId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string TelephoneNo { get; set; } = null!;
        public short Status { get; set; }
    }

    public class GetBackendUsersResponseDTO
    {
        public string UserId { get; set; } = null!;
        public string LoginId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string TelephoneNo { get; set; } = null!;
        public short Status { get; set; }
    }

    public class CreateBackendUserRequestDTO
    {
        public string LoginId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? TelephoneNo { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string CreatedById { get; set; } = null!;
        public string CreatedByName { get; set; } = null!;
    }

    public class UpdateBackendUserRequestDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? TelephoneNo { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string UpdatedById { get; set; } = null!;
        public string UpdatedByName { get; set; } = null!;
    }

    public class SoftDeleteBackendUserRequestDTO
    {
        public Guid UserId { get; set; }
        public string UpdatedById { get; set; } = null!;
        public string UpdatedByName { get; set; } = null!;
    }

    public class GetBackendUsersRolesResponseDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? TelephoneNo { get; set; }
        public string EmailAddress { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
