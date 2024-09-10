using System;
using BusinessLogic.SupportedWebsite;
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
        try
        {
            return Ok(m_DBService.GetSupportedWebsites());
        }
        
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }

    [HttpPost("postWebsite")]
    public IActionResult PostSupportedWebsite([FromBody] SupportedWebsite website)
    {
        try
        {
            m_DBService.AddSupportedWebsite(website.Name, website.URL, website.ImageURL);

            return Ok();
        }
        
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }
    
    [HttpDelete("deleteWebsite")]
    public IActionResult DeleteSupportedWebsite([FromQuery] string name, [FromQuery] string code)
    {
        try
        {
            string? connectionString = Environment.GetEnvironmentVariable("ID");

            if (!code.Equals(connectionString))
            {
                return BadRequest("Wrong code for website deletion.");
            }
            
            m_DBService.RemoveSupportedWebsite(name);
            
            return Ok();
        }
        
        catch (Exception e)
        {
            return BadRequest(new { error = e.Message });
        }
    }
}