using Codedberries.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriorityController : ControllerBase
    {
        private readonly PriorityService _priorityService;

        public PriorityController(PriorityService priorityService)
        {
            _priorityService = priorityService;
        }
    }
}
