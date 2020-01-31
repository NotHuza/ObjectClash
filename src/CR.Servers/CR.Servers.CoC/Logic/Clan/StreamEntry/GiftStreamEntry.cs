namespace CR.Servers.CoC.Logic.Clan
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class GiftStreamEntry : StreamEntry
    {
        internal List<long> Claimers = new List<long>();

        internal int ClaimLeft = 50;
        internal int DiamondCount;

        public GiftStreamEntry()
        {
        }

        public GiftStreamEntry(Member Member) : base(Member)
        {
        }

        internal override AllianceStream Type
        {
            get
            {
                return AllianceStream.Gift;
            }
        }

        internal override void Encode(List<byte> Packet)
        {
            base.Encode(Packet);
            Packet.AddVInt(this.ClaimLeft);
            Packet.AddVInt(this.DiamondCount);
            Packet.AddInt(this.Claimers.Count);

            foreach (long Claimer in this.Claimers)
            {
                Packet.AddLong(Claimer);
            }
        }

        internal override void Load(JToken Json)
        {
            base.Load(Json);

            JsonHelper.GetJsonNumber(Json, "claim_left", out this.ClaimLeft);
            JsonHelper.GetJsonNumber(Json, "diamond_count", out this.DiamondCount);

            JArray claimers = (JArray) Json["claimers"];

            if (claimers != null)
            {
                foreach (JToken claimer in claimers)
                {
                    int ID = claimer.ToObject<int>();

                    if (ID > 0)
                    {
                        this.Claimers.Add(ID);
                    }
                }
            }
        }

        internal override JObject Save()
        {
            JObject Json = base.Save();

            JArray claimers = new JArray();

            Json.Add("claim_left", this.ClaimLeft);
            Json.Add("diamond_count", this.DiamondCount);

            foreach (long claimer in this.Claimers)
            {
                claimers.Add(claimer);
            }

            Json.Add("claimers", claimers);

            return Json;
        }
    }
}