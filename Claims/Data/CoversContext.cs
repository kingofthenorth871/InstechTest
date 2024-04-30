using Claims.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Data
{
    public class CoversContext : DbContext
    {
        public DbSet<Cover> Covers { get; init; }

        private readonly MongoDatabaseDAO _mongoDBService;      

        public CoversContext(MongoDatabaseDAO mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);          
            modelBuilder.Entity<Cover>().ToCollection("covers");
        }

        public async Task<IEnumerable<Cover>> GetCoversAsync()
        {
            var CoversFromMongo = _mongoDBService.GetCoverCollection();

            var filter = Builders<Cover>.Filter.Empty; // Match all documents

            var allCoversFromMongo = CoversFromMongo.Find(filter).ToListAsync();

            return await allCoversFromMongo;

        }

        public async Task AddItemAsync(Cover item)
        {
            var CoversFromMongo = _mongoDBService.GetCoverCollection();
            await CoversFromMongo.InsertOneAsync(item);
        
        }

        public async Task<Cover> GetCoverAsync(string id)
        {

            var CoverFromMongo = _mongoDBService.GetCoverCollection();

            var filter = Builders<Cover>.Filter.Eq(cover => cover.Id, id); // Match document with id            

            var claim = await CoverFromMongo.Find(filter).FirstOrDefaultAsync();
            return claim;
        
        }

        public async Task DeleteCoverItemAsync(string id)
        {          

            var cover = await GetCoverAsync(id);
            if (cover is not null)
            {
                var filter = Builders<Models.Cover>.Filter.Eq(coverDB => coverDB.Id, id);

                var CoversFromMongo = _mongoDBService.GetCoverCollection();
                await CoversFromMongo.DeleteOneAsync(filter);

            }
        }

        public async Task DeleteItemAsync(string id)
        {
            var cover = await GetCoverAsync(id);
            if (cover is not null)
            {
                Covers.Remove(cover);
                await SaveChangesAsync();
            }
        }        

    }
}
