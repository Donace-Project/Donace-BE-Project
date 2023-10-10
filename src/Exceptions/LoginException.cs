namespace Donace_BE_Project.Exceptions;

public class LoginException : FriendlyException
{
    public LoginException() : base(string.Empty, $"Sai tài khoản hoặc mật khẩu")
    {
    }
}
