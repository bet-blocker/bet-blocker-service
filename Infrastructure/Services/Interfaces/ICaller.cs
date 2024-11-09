namespace Infrastructure.Services.Interfaces
{
    public interface ICaller
    {
        Task<string> Call(string endpoint, HttpMethod method, object? body = null);
    }
}

