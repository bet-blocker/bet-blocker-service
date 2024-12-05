namespace Application.Business.Interfaces
{
    public interface IBetBusiness
    {
        double GetStatus();
        Task StartResolutionProcess(CancellationToken cancellationToken);
        bool IsResolutionInProgress { get; }
    }
}

