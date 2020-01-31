namespace CR.Servers.CoC.Core.Database.Models.Mongo
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class Players
    {
        [BsonId] internal ObjectId Id;

        public int HighId { get; set; }

        public int LowId { get; set; }

        public string GameCenterId { get; set; } = string.Empty;

        public string GoogleId { get; set; } = string.Empty;

        public string FacebookId { get; set; } = string.Empty;

        public BsonDocument Player { get; set; }

        public BsonDocument Home { get; set; }
    }
}