namespace CR.Servers.CoC.Core.Database.Models.Mongo
{
    using MongoDB.Bson;

    public class Clans
    {
        public ObjectId Id;

        public int HighId { get; set; }

        public int LowId { get; set; }

        public BsonDocument Data { get; set; }
    }
}