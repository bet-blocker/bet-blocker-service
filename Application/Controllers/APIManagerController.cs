using System.Text.Json;
using Application.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Application.Controllers
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

        [HttpPost("start")]
        public IActionResult StartResolution(CancellationToken cancellationToken)
        {
            try
            {
                var filePath = $"Json/{DateTime.UtcNow:dd-MM-yyyy}.json";

                if (_betBusiness.IsResolutionInProgress && System.IO.File.Exists(filePath))
                {
                    return Ok(new { message = $"The file is already in progress", progress = $"{_betBusiness.GetStatus():F2}%" });
                }
                else if (!_betBusiness.IsResolutionInProgress && System.IO.File.Exists(filePath))
                {
                    return Ok(new { message = $"The file {filePath} already exists. Data was not recreated." });
                }

                if (_betBusiness.IsResolutionInProgress)
                {
                    return Ok(new { message = $"A resolution is already in progress.", progress = $"{_betBusiness.GetStatus():F2}%" });
                }

                _betBusiness.StartResolutionProcess(cancellationToken);

                return Ok(new { message = "DNS resolution has started." });
            }
            catch (Exception ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("dns")]
        public async Task<IActionResult> GetDnsResolution([FromQuery] string? date = null)
        {
            try
            {
                var currentDate = date ?? DateTime.UtcNow.ToString("dd-MM-yyyy");

                var filePath = $"Json/{currentDate}.json";

                if (System.IO.File.Exists(filePath))
                {
                    var fileContent = await System.IO.File.ReadAllTextAsync(filePath);
                    var data = JsonSerializer.Deserialize<object>(fileContent);

                    return Ok(data);
                }
                else
                {
                    return BadRequest("The referenced file does not exist in our database.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal error while processing the request.", details = ex.Message });
            }
        }

        [HttpGet("domains")]
        public async Task<IActionResult> GetDomains()
        {
            try
            {
                var domains = await _betBusiness.GetBlocklistGithub();

                return Ok(new { Date = DateTime.UtcNow.ToString("dd-MM-yyyy"), Domains = domains });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal error while processing the request.", details = ex.Message });
            }
        }
    }
}
