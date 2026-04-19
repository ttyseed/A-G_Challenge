using challenge1.Common.Logging;
using challenge1.Database.Models;
using AutoMapper;
using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Database.Repositories.Repositories.Interfaces;
using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;
using challenge1.Common.Util.Common;
using challenge1.Application.ViewModel.Common.Intranet;

namespace challenge1.Application.Bll.Common
{
    public class BackendUserBll : IBackendUserBll
    {
        private readonly IMapper _mapper;
        private readonly ILogging _logging;
        private readonly IBackendUserRepository _backendUserRepository;

        public BackendUserBll(IMapper mapper, ILogging logging, IBackendUserRepository backendUserRepository)
        {
            _mapper = mapper;
            _logging = logging;
            _backendUserRepository = backendUserRepository;
        }

        public async Task<GetBackendUserByLoginIdResponseDTO?> GetBackendUserByLoginIdAsync(string loginId)
        {
            var backendUser = await _backendUserRepository.GetBackendUserByLoginIdAsync(loginId);
            return _mapper.Map<GetBackendUserByLoginIdResponseDTO>(backendUser);
        }

        public async Task<List<GetBackendUsersResponseDTO>?> GetBackendUsersAsync(GetBackendUsersFilter request)
        {
            var backendUsers = await _backendUserRepository.GetBackendUsersAsync(request);
            return _mapper.Map<List<GetBackendUsersResponseDTO>>(backendUsers);
        }

        public async Task<Guid?> CreateBackendUserAsync(CreateBackendUserRequestDTO request)
        {
            try
            {
                var backendUser = _mapper.Map<BackendUser>(request);

                backendUser.UserId = Guid.NewGuid();
                backendUser.CreatedDate = DateTime.Now;
                backendUser.Status = Constant.GenericStatus.Active;

                backendUser = await _backendUserRepository.CreateAsync(backendUser);
                if (backendUser == null)
                {
                    return null;
                }

                return backendUser.UserId;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<bool> UpdateBackendUserAsync(UpdateBackendUserRequestDTO request)
        {
            try
            {
                var backendUser = await _backendUserRepository.GetByIdAsync(request.UserId);
                if (backendUser == null)
                {
                    return false;
                }

                backendUser.EmailAddress = request.EmailAddress ?? "";
                backendUser.FullName = request.FullName ?? "";
                backendUser.TelephoneNo = request.TelephoneNo;
                backendUser.UpdatedByName = request.UpdatedByName;
                backendUser.UpdatedById = request.UpdatedById;
                backendUser.UpdatedDate = DateTime.Now;

                return await _backendUserRepository.UpdateAsync(backendUser);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SoftDeleteBackendUserAsync(SoftDeleteBackendUserRequestDTO request)
        {
            try
            {
                var backendUser = await _backendUserRepository.GetByIdAsync(request.UserId);
                if (backendUser == null)
                {
                    return false;
                }

                backendUser.IsDeleted = true;
                backendUser.UpdatedById = request.UpdatedById;
                backendUser.UpdatedByName = request.UpdatedByName;
                backendUser.UpdatedDate = DateTime.Now;

                return await _backendUserRepository.UpdateAsync(backendUser);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return false;
            }
        }

        public async Task<List<GetBackendUsersRolesVM>?> GetBackendUsersRolesAsync(Guid userId)
        {
            var backendUsersRoles = await _backendUserRepository.GetBackendUsersRolesAsync(userId);
            return _mapper.Map<List<GetBackendUsersRolesVM>>(backendUsersRoles);
        }
    }
}
