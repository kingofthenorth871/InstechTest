using Claims.Models;

namespace Claims.Services
{
    public interface ICoversService
    {
        Task AddItemAsync(Cover item);
        Task<Cover> GetCoverAsync(string id);
        Task DeleteItemAsync(string id);
        Task<IEnumerable<Cover>> GetCoversAsync();
    }
}
