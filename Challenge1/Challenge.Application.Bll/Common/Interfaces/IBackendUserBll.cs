using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;
using challenge1.Application.ViewModel.Common.Intranet;

namespace challenge1.Application.Bll.Common.Interfaces
{
    public interface IBackendUserBll
    {
        Task<GetBackendUserByLoginIdResponseDTO?> GetBackendUserByLoginIdAsync(string loginId);
        Task<List<GetBackendUsersResponseDTO>?> GetBackendUsersAsync(GetBackendUsersFilter request);
        Task<Guid?> CreateBackendUserAsync(CreateBackendUserRequestDTO request);
        Task<bool> UpdateBackendUserAsync(UpdateBackendUserRequestDTO request);
        Task<bool> SoftDeleteBackendUserAsync(SoftDeleteBackendUserRequestDTO request);
        Task<List<GetBackendUsersRolesVM>?> GetBackendUsersRolesAsync(Guid userId);
    }
}
