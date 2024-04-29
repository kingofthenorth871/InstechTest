using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Claims.Models
{
    public class Claim
    {
        [BsonId]
        public string Id { get; set; }

        //[BsonElement("coverId")]
        [BsonElement("CoverId")]
        public string CoverId { get; set; }

        [BsonElement("Created")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Created { get; set; } = DateTime.Today; // Initialize with today's date

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Type")]
        public ClaimType Type { get; set; }

        [BsonElement("DamageCost")]      
        public decimal DamageCost { get; set; }
    }

    public enum ClaimType
    {
        Collision = 0,
        Grounding = 1,
        BadWeather = 2,
        Fire = 3
    }
}
