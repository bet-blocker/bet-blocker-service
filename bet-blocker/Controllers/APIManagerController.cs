using bet_blocker.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace bet_blocker.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class APIManagerController : ControllerBase
    {
        private readonly IBetBusiness _betBusiness;
        private readonly string _storagePath;

        public APIManagerController(IBetBusiness betBusiness, IWebHostEnvironment env)
        {
            _betBusiness = betBusiness;
            _storagePath = Path.Combine(env.ContentRootPath, "json");
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
            try
            {
                var status = _betBusiness.GetResolutionStatus();

                if (status is string statusText && statusText == "Processing")
                {
                    return Ok("Processando... A resolução de DNS ainda está em andamento. Por favor, verifique novamente em breve.");
                }

                return Ok(status ?? "Nenhum processo iniciado");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao processar a solicitação.", details = ex.Message });
            }
        }

        [HttpGet("dns-resolution/{date}.json")]
        public IActionResult GetDnsResolution(string date)
        {
            var filePath = Path.Combine(_storagePath, $"{date}.json");

            if (System.IO.File.Exists(filePath))
            {
                var json = System.IO.File.ReadAllText(filePath);
                return Ok(json);
            }

            return NotFound("Arquivo não encontrado.");
        }
    }
}
