using Claims.Data;
using Claims.Models;
using Claims.Services;
using Claims.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Claims.Controllers;

[ApiController]
[Route("[controller]")]
public class CoversController : ControllerBase
{
    
    private readonly ICoversService _coversService;
    private readonly ILogger<CoversController> _logger;  
    private readonly IAuditsService _auditsService;

    public CoversController(ILogger<CoversController> logger, IAuditsService AuditsService, ICoversService coversService)
    {
        
        _logger = logger;      
        _auditsService = AuditsService;    
        _coversService = coversService;
    }

    [HttpPost("compute")]
    public Task<ActionResult> ComputePremiumAsync(DateTime startDate, DateTime endDate, CoverType coverType)
    {
        return Task.FromResult<ActionResult>(Ok(ComputePremium(startDate, endDate, coverType)));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
    {
       
        var results = await _coversService.GetCoversAsync();

        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cover>> GetAsync(string id)
    {
        
        var results = await _coversService.GetCoverAsync(id);

        return Ok(results);
    }

    [HttpPost]
    public async Task<ActionResult> CreateAsync(Cover cover)
    {
        try
        {           
            cover.Id = Guid.NewGuid().ToString();           
            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);   
            await _coversService.AddItemAsync(cover);   
            await _auditsService.AuditCover(cover.Id, "POST");          
            return Ok(cover);
        }
        catch (Exception ex)
        {           
            return StatusCode(500, ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task DeleteAsync(string id)
    {
        await _auditsService.AuditCover(id, "DELETE");   

        var cover = await _coversService.GetCoverAsync(id);

        if (cover is not null)
        {
            await _coversService.DeleteItemAsync(cover.Id);          
        }
    }

    private decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
    {       
        return ComputePremiumCalculator.compute(startDate, endDate, coverType);
    }
}
