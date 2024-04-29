using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Claims.Models;

public class Cover
{
    [BsonId]
    public string Id { get; set; }

    private DateTime startDate;
    [BsonElement("StartDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime StartDate
    {
        get => startDate;
        set
        {
           
            startDate = value;
        } 
    } 

    private DateTime endDate;
    [BsonElement("EndDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
    public DateTime EndDate
    {
        get => endDate;
        set
        {
            
            endDate = value;
        }
    }

    [BsonElement("Type")]
    public CoverType Type { get; set; }

    [BsonElement("Premium")]
    public decimal Premium { get; set; }
}


