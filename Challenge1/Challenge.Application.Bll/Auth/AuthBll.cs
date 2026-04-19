using challenge1.Application.Bll.Auth.Interfaces;
using challenge1.Database.Models;
using challenge1.Database.Repositories.Repositories.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace challenge1.Application.Bll.Auth
{
    public class AuthBll : IAuthBll
    {
        private readonly IBackendUserRepository _backendUserRepository;

        public AuthBll(IBackendUserRepository backendUserRepository)
        {
            _backendUserRepository = backendUserRepository;
        }

        public async Task<BackendUser?> ValidateCredentialsAsync(string loginId, string password)
        {
            var user = await _backendUserRepository.GetBackendUserByLoginIdAsync(loginId);
            if (user == null || user.IsDeleted || user.Status != 1 || user.PasswordHash == null)
                return null;

            return user.PasswordHash == HashPassword(password) ? user : null;
        }

        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
