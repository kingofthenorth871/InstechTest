using Claims.Models.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Claims.Data
{
    public class AuditContext : DbContext
    {
        
        public DbSet<ClaimAudit> ClaimAudits { get; set; }
        public DbSet<CoverAudit> CoverAudits { get; set; }

        public AuditContext(DbContextOptions<AuditContext> options) 
            : base(options)
        {
        }

        public async Task AuditClaim(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            ClaimAudits.Add(claimAudit);
            await SaveChangesAsync();
        }

        public async Task AuditCover(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.Now,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            CoverAudits.Add(coverAudit);
            await SaveChangesAsync();
        }

    }
}
