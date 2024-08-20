using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("offlineReader/supportedWebsites")]
public class SupportedWebsitesController : ControllerBase
{
    private readonly DBService m_DBService = DBService.Instance;
    
    [HttpGet("getWebsites")]
    public IActionResult GetSupportedWebsites()
    {
        return Ok(m_DBService.GetSupportedWebsites());
    }

    [HttpPost("postWebsite")]
    public IActionResult PostSupportedWebsite([FromQuery] string name, [FromQuery] string url)
    {
        try
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url))
            {
                return BadRequest("Missing required query parameters: 'name' and 'url'.");
            }
            
            m_DBService.AddSupportedWebsite(name, url);
            
            return Ok();
        }
        
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}