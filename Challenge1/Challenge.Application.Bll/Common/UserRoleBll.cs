using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;
using challenge1.Database.Repositories.Repositories.Interfaces;

namespace challenge1.Application.Bll.Common
{
    public class UserRoleBll : IUserRoleBll
    {
        private readonly ILogging _logging;
        private readonly IUserRoleRepository _userRoleRepository;

        public UserRoleBll(ILogging logging, IUserRoleRepository userRoleRepository)
        {
            _logging = logging;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<List<short>?> GetUserRoleNumbersByUserIdAsync(Guid userId)
        {
            try
            {
                return await _userRoleRepository.GetUserRoleNumbersByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<bool> SoftDeleteUserRoleAsync(SoftDeleteUserRoleRequestDTO request)
        {
            try
            {
                var userRole = await _userRoleRepository.GetByIdAsync(request.UserRoleId);
                if (userRole == null)
                {
                    return false;
                }

                userRole.IsDeleted = true;
                userRole.UpdatedById = request.UpdatedById;
                userRole.UpdatedByName = request.UpdatedByName;
                userRole.UpdatedDate = DateTime.Now;

                return await _userRoleRepository.UpdateAsync(userRole);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return false;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(UpdateUserRoleRequestDTO request)
        {
            try
            {
                var userRole = await _userRoleRepository.GetByIdAsync(request.UserRoleId);
                if (userRole == null)
                {
                    return false;
                }

                userRole.RoleNumber = request.RoleNumber;
                userRole.UpdatedById = request.UpdatedById;
                userRole.UpdatedByName = request.UpdatedByName;
                userRole.UpdatedDate = DateTime.Now;

                var updateSuccess = await _userRoleRepository.UpdateAsync(userRole);
                if (!updateSuccess)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return false;
            }
        }
    }
}
