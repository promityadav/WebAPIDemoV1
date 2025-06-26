namespace WebAPIDemo.Authority
{
    public interface IAppRepository
    {
        Task<Application?> GetApplicationByClientIdAsync(string clientId);
    }
}
