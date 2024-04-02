using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codedberries.Models.DTOs
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectIdDTO : ControllerBase
    {
        public int ProjectId { get; set; }
    }
}
