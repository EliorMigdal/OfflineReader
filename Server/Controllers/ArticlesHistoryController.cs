using BusinessLogic.Article.Partials;
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
            return Ok(m_DBService.GetArticlesList(website, date));
        }
        
        catch (Exception e)
        {
            return BadRequest(new {error = e.Message});
        }
    }

    [HttpGet("getDates")]
    public IActionResult GetArticlesDates([FromQuery] string website)
    {
        try
        {
            return Ok(m_DBService.GetAvailableDates(website));
        }
        
        catch (Exception e)
        {
            return BadRequest(new {error = e.Message});
        }
    }

    [HttpPost("postArticle")]
    public IActionResult PostArticleHistoryEntry([FromBody] OuterArticle article)
    {
        try
        {
            m_DBService.InsertArticleToTable(article);
            
            return Ok();
        }
        
        catch (Exception e)
        {
            return BadRequest(new {error = e.Message});
        }
    }
}