using static bet_blocker.DTOs.ResponseHostDto;

namespace bet_blocker.Business.Interfaces
{
    public interface IBetBusiness
    {
        void StartResolutionProcess(CancellationToken cancellationToken);
        Task<List<ResponseHostsDTO>> GetList(CancellationToken cancellationToken);
        object GetResolutionStatus();
    }
}

