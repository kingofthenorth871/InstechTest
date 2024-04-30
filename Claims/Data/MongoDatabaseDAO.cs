using Claims.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using static System.Net.Mime.MediaTypeNames;

namespace Claims.Data
{
    public class MongoDatabaseDAO
    {

        private readonly IMongoCollection<Claim> _claimCollection;
        private readonly IMongoCollection<Cover> _coverCollection;

        private readonly IConfiguration _configuration;

        
        public MongoDatabaseDAO(IConfiguration configuration)
        {

            // Retrieve MongoDB connection string from appsettings.json
            string mongoDbConnectionString = configuration.GetConnectionString("MongoDb");

            // Create MongoClientSettings from the connection string
            MongoClientSettings settings = MongoClientSettings.FromConnectionString(mongoDbConnectionString);

            // Create a MongoClient instance
            MongoClient client = new MongoClient(settings);

            // Get a reference to the database
            IMongoDatabase database = client.GetDatabase(configuration["MongoDb:DatabaseName"]); // Use the database name from appsettings.json

            _claimCollection = database.GetCollection<Claim>("claims");

            _coverCollection = database.GetCollection<Cover>("covers");

        }

        public IMongoCollection<Cover> GetCoverCollection()
        {
            return _coverCollection;
        }

        public IMongoCollection<Claim> GetClaimCollection()
        {
            return _claimCollection;
        }


    }
}
