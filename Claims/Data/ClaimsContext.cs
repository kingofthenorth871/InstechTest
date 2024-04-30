using Claims.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using System;


namespace Claims.Data
{
    public class ClaimsContext : DbContext
    {
        private DbSet<Claim> Claims { get; init; }       

        private readonly MongoDatabaseDAO _mongoDatabaseDAO;

        public ClaimsContext(MongoDatabaseDAO mongoDBService)
        {
            _mongoDatabaseDAO = mongoDBService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("claims");          
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            var ClaimsFromMongo = _mongoDatabaseDAO.GetClaimCollection();         

            var filter = Builders<Claim>.Filter.Empty; // Match all documents
           
            var allClaimsFromMongo = ClaimsFromMongo.Find(filter).ToListAsync();

            return await allClaimsFromMongo;
        }

        public async Task<Claim> GetClaimAsync(string id)
        {
            var ClaimsFromMongo = _mongoDatabaseDAO.GetClaimCollection();

            var filter = Builders<Claim>.Filter.Eq(claim =>claim.Id, id); // Match document with id           

            var claim = await ClaimsFromMongo.Find(filter).FirstOrDefaultAsync();
            return claim;           
        }

        public async Task AddItemAsync(Claim item)
        {
            var ClaimsFromMongo = _mongoDatabaseDAO.GetClaimCollection();
            await ClaimsFromMongo.InsertOneAsync(item);
        }

        public async Task DeleteItemAsync(string id)
        {
            var claim = await GetClaimAsync(id);
            if (claim is not null)
            {
                var filter = Builders<Models.Claim>.Filter.Eq(claimDB => claimDB.Id, id);

                var ClaimsFromMongo = _mongoDatabaseDAO.GetClaimCollection();
                await ClaimsFromMongo.DeleteOneAsync(filter);
            }
        }
    }
}
