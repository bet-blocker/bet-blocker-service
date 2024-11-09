using bet_blocker.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bet_blocker.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class APIManagerController : ControllerBase
    {
        private readonly IBetBusiness _betBusiness;

        public APIManagerController(IBetBusiness betBusiness)
        {
            _betBusiness = betBusiness;
        }

        [HttpGet("start")]
        public IActionResult StartResolution(CancellationToken cancellationToken)
        {
            try
            {
                _betBusiness.StartResolutionProcess(cancellationToken);
                return Ok("A resolução de DNS foi iniciada. Verifique o status com /status.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("status")]
        public IActionResult GetResolutionStatus()
        {
            var status = _betBusiness.GetResolutionStatus();

            if (status is string statusText && statusText == "Processing")
            {
                return Ok("Processando... A resolução de DNS ainda está em andamento. Por favor, verifique novamente em breve.");
            }

            return Ok(status ?? "Nenhum processo iniciado");
        }
    }
}
