using AutoMapper;
using challenge1.Application.Bll.Common.Interfaces;
using challenge1.Application.DTO.Common;
using challenge1.Application.Filter.Common;
using challenge1.Common.Logging;
using challenge1.Database.Repositories.Repositories.Interfaces;

namespace challenge1.Application.Bll.Common
{
    public class LoginUserBll : ILoginUserBll
    {
        private readonly IMapper _mapper;
        private readonly ILogging _logging;
        private readonly ILoginUserRepository _loginUserRepository;

        public LoginUserBll(ILogging logging, ILoginUserRepository loginUserRepository, IMapper mapper)
        {
            _mapper = mapper;
            _logging = logging;
            _loginUserRepository = loginUserRepository;
        }

        public async Task<GetLoginUserByLoginIdResponseDTO?> GetLoginUserByLoginIdAsync(string loginId)
        {
            var loginUser = await _loginUserRepository.GetLoginUserByLoginIdAsync(loginId);
            return _mapper.Map<GetLoginUserByLoginIdResponseDTO>(loginUser);
        }

        public async Task<List<GetLoginUsersResponseDTO>?> GetLoginUsersAsync(GetLoginUsersFilter request)
        {
            try
            {
                var loginUsers = await _loginUserRepository.GetLoginUsersAsync(request);
                return _mapper.Map<List<GetLoginUsersResponseDTO>>(loginUsers);
            }
            catch (Exception ex)
            {
                _logging.LogBllError(ex.ToString());
            }

            return null;
        }
    }
}
