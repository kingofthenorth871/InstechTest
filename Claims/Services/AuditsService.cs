using Claims.Data;

namespace Claims.Services
{
    public class AuditsService : IAuditsService
    {
        private readonly AuditContext _context;

        public AuditsService(AuditContext context)
        {
            _context = context;
        }
        public async Task AuditClaim(string id, string httpRequestType)
        {
            await _context.AuditClaim(id, httpRequestType);
        }

        public async Task AuditCover(string id, string httpRequestType)
        {
            await _context.AuditCover(id, httpRequestType);
        }
    }
}
