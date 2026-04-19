namespace challenge1.Application.DTO.Auth
{
    public class LoginRequestDTO
    {
        public string LoginId { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResponseDTO
    {
        public string Token { get; set; } = null!;
        public string LoginId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
    }
}
