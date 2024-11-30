using bet_blocker.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace bet_blocker.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class APIManagerController : ControllerBase
    {
        private readonly IBetBusiness _betBusiness;

        public APIManagerController(IBetBusiness betBusiness, IWebHostEnvironment env)
        {
            _betBusiness = betBusiness;
        }

        [HttpPost("start")]
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

        [HttpGet("dns")]
        public async Task<IActionResult> GetDnsResolution([FromQuery] string? date = null)
        {
            try
            {
                var currentDate = date ?? DateTime.UtcNow.ToString("dd-MM-yyyy");
                var documents = _betBusiness.GetResolutionStatus();

                if (documents is MongoDB.Bson.BsonDocument bsonDocument)
                {
                    var jsonResult = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<object>(bsonDocument.ToJson());
                    return Ok(jsonResult);
                }

                return NotFound(new { message = "Nenhum dado encontrado para a data solicitada." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao processar a solicitação.", details = ex.Message });
            }
        }

    }
}
