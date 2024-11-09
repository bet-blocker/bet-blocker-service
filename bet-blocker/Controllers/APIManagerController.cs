using bet_blocker.DTOs;
using Microsoft.AspNetCore.Mvc;
namespace bet_blocker.Controllers;

[ApiController]
[Route("api/v1")]
public class APIManagerController : ControllerBase
{
    [HttpGet(Name = "bets")]
    public IEnumerable<ResponseHostDto> Get()
    {
        List<ResponseHostDto> response = new();

        return response;
    }
}
