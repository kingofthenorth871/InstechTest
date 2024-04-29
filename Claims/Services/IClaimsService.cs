using Claims.Models;

namespace Claims.Services
{
    public interface IClaimsService
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim> GetClaimAsync(string id);    
        Task AddItemAsync(Claim item);
        Task DeleteItemAsync(string id);

    }
}
