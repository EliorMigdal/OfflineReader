using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("offlineReader/articlesHistory")]
public class ArticlesHistoryController : ControllerBase
{
    private readonly DBService m_DBService = DBService.Instance;
    
    [HttpGet("getArticles")]
    public IActionResult GetArticlesList([FromQuery] string website, [FromQuery] string date)
    {
        try
        {
            if (string.IsNullOrEmpty(website) || string.IsNullOrEmpty(date))
            {
                return BadRequest("Missing required query parameters: 'name' and 'url'.");
            }
            
            return Ok(m_DBService.GetArticlesList(website, date));
        }
        
        catch (Exception e)
        {
            return BadRequest(e);
        }
    }
}