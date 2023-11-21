namespace Donace_BE_Project.Interfaces.Services
{
    public interface IRabbitMQService
    {
        Task SubReplyAsync(string message);
    }
}
