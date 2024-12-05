namespace Application.Business.Interfaces
{
    public interface IBetBusiness
    {
        Task StartResolutionProcess(CancellationToken cancellationToken);
        Task<List<string>> GetBlocklistGithub();
        double GetStatus();

        bool IsResolutionInProgress { get; }
    }
}

