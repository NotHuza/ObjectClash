namespace CR.Servers.CoC.Core.Database.Models.Mongo
{
    using MongoDB.Bson;

    public class Battles
    {
        public ObjectId Id;

        public int HighId;
        public int LowId;
        public string Replay;
    }
}