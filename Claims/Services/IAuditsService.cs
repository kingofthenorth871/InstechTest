namespace Claims.Services
{
    public interface IAuditsService
    {
        Task AuditClaim(string id, string httpRequestType);
        Task AuditCover(string id, string httpRequestType);
    }
}
