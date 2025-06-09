using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Api.Management.Controllers;

namespace Umbraco.Cms.Management.Api.Controllers.Backoffice
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Backoffice")]
    [Route("umbraco/management/api/v1/backoffice")]
    public class BackofficeController : ManagementApiControllerBase
    {
        [HttpPost("check")]
        public IActionResult CheckStatus([FromQuery] bool isOk)
        {
            if (isOk)
                return Ok("Status is OK.");
            else
                return BadRequest("Status is not OK.");
        }
    }
}