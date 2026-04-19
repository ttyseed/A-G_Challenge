using challenge1.Database.Models;

namespace challenge1.Application.Bll.Auth.Interfaces
{
    public interface IAuthBll
    {
        Task<BackendUser?> ValidateCredentialsAsync(string loginId, string password);
    }
}
