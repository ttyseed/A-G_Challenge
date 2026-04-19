using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;

namespace challenge1.Application.Bll.Common.Interfaces
{
    public interface ILoginUserBll
    {
        Task<GetLoginUserByLoginIdResponseDTO?> GetLoginUserByLoginIdAsync(string loginId);
        Task<List<GetLoginUsersResponseDTO>?> GetLoginUsersAsync(GetLoginUsersFilter request);
    }
}
