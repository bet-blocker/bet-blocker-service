using Api.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class APIManagerController : Controller
    {
        [HttpGet(Name = "bets")]
        public IEnumerable<ResponseHostsDTO> Get()
        {
            IEnumerable<ResponseHostsDTO> response = new List<ResponseHostsDTO>();
            return response;
        }
    }
}
