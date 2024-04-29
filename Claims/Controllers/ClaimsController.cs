using Claims.Data;
using Claims.Models;
using Claims.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;


namespace Claims.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;          
        private readonly IAuditsService _auditsService;
        private readonly IClaimsService _claimsService;
        
        public ClaimsController(ILogger<ClaimsController> logger, IClaimsService claimsService, IAuditsService AuditsService)
        {
            _logger = logger;                     
            _claimsService = claimsService;
            _auditsService = AuditsService;
        }
       
        [HttpGet]
        public async Task<IEnumerable<Claim>> GetAsync()
        {
            return await _claimsService.GetClaimsAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(Claim claim)
        {
            try {
                claim.Id = Guid.NewGuid().ToString();
                await _claimsService.AddItemAsync(claim);
                await _auditsService.AuditClaim(claim.Id, "POST");
                return Ok(claim);
            }

            catch (Exception ex)
            {              
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteAsync(string id)
        {           
            await _auditsService.AuditClaim(id, "DELETE");
            await _claimsService.DeleteItemAsync(id);
        }

        [HttpGet("{id}")]
        public async Task<Claim> GetAsync(string id)
        {
            return await _claimsService.GetClaimAsync(id);
        }
    }
   
}
