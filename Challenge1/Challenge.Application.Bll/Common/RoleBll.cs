using AutoMapper;
using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Repositories.Interfaces;

namespace challenge1.Application.Bll.Common
{
    public class RoleBll : IRoleBll
    {
        private readonly ILogging _logging;
        private readonly IMapper _mapper;
        private readonly IRoleRepository _roleRepository;

        public RoleBll(ILogging logging, IMapper mapper, IRoleRepository roleRepository)
        {
            _logging = logging;
            _mapper = mapper;
            _roleRepository = roleRepository;
        }

        public async Task<List<GetRolesResponseDTO>?> GetRolesAsync(GetRolesFilter request)
        {
            return _mapper.Map<List<GetRolesResponseDTO>?>(await _roleRepository.GetRolesAsync(request));
        }

        public async Task<GetRoleByIdResponseDTO?> GetRoleByIdAsync(Guid roleId)
        {
            return _mapper.Map<GetRoleByIdResponseDTO?>(await _roleRepository.GetRoleByIdAsync(roleId));
        }

        public async Task<Guid?> CreateRoleAsync(CreateRoleRequestDTO request)
        {
            try
            {
                var role = _mapper.Map<Role>(request);

                role.RoleId = Guid.NewGuid();
                role.IsDeleted = false;
                role.CreatedDate = DateTime.Now;

                var result = await _roleRepository.CreateAsync(role);
                if (result == null)
                {
                    return null;
                }

                return role.RoleId;
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return null;
            }
        }

        public async Task<bool> UpdateRoleAsync(UpdateRoleRequestDTO request)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(request.RoleId);
                if (role == null)
                {
                    return false;
                }

                role.RoleNumber = request.RoleNumber;
                role.RoleName = request.RoleName;
                role.UpdatedById = request.UpdatedById;
                role.UpdatedByName = request.UpdatedByName;
                role.UpdatedDate = DateTime.Now;

                return await _roleRepository.UpdateAsync(role);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return false;
            }
        }

        public async Task<bool> SoftDeleteRoleAsync(SoftDeleteRoleRequestDTO request)
        {
            try
            {
                var role = await _roleRepository.GetByIdAsync(request.RoleId);
                if (role == null)
                {
                    return false;
                }

                role.UpdatedById = request.UpdatedById;
                role.UpdatedByName = request.UpdatedByName;

                return await _roleRepository.SoftDeleteRoleAsync(role);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
                return false;
            }
        }
    }
}
