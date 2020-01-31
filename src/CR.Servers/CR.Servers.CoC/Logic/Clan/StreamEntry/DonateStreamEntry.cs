namespace CR.Servers.CoC.Logic.Clan
{
    using System.Collections.Generic;
    using System.Reflection;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Logic.Clan.Items;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Extensions;
    using CR.Servers.Extensions.List;
    using Newtonsoft.Json.Linq;

    internal class DonateStreamEntry : StreamEntry
    {
        internal bool HaveMessage;
        internal int MaxSpell;

        internal int MaxTroop;
        internal string Message;
        internal bool New = true;
        internal AllianceUnitSlots Units;
        internal int UsedSpell;
        internal int UsedTroop;

        internal int UsedTroopSend; //Do not save this

        public DonateStreamEntry()
        {
            this.Units = new AllianceUnitSlots();
        }

        public DonateStreamEntry(Member Member) : base(Member)
        {
            this.Units = new AllianceUnitSlots();
        }

        internal override AllianceStream Type
        {
            get
            {
                return AllianceStream.Donate;
            }
        }

        internal override void Encode(List<byte> Packet)
        {
            base.Encode(Packet);

            Packet.AddInt(this.LowId);
            Packet.AddInt(this.MaxTroop);
            Packet.AddInt(this.MaxSpell);
            Packet.AddInt(this.UsedTroop);
            Packet.AddInt(this.UsedSpell);

            this.New = true;


            Packet.AddInt(0);

            /*var requestorDonatedUnit = this.Units.FindAll(T => T.DonatorId == this.RequesterId);

            if (requestorDonatedUnit.Count > 0)
            {
                Packet.AddInt(1);
                Packet.AddLong(this.RequesterId);

                Packet.AddInt(requestorDonatedUnit.Sum(unit => unit.Count));

                foreach (UnitItem Unit in requestorDonatedUnit)
                {
                    for (int i = 0; i < Unit.Count; i++)
                    {
                        Packet.AddInt(Unit.Data);
                        Packet.AddInt(Unit.Level);
                    }
                }
            }
            else
            {
                Packet.AddInt(0);
            }*/

            Packet.AddBool(this.HaveMessage);
            if (this.HaveMessage)
            {
                Packet.AddString(this.Message);
            }

            if (this.Units == null)
            {
                this.Units = new AllianceUnitSlots();
            }

            UnitItem[] donatedUnits = this.Units.ToArray();

            Packet.AddInt(donatedUnits.Length);

            foreach (UnitItem donatedUnit in donatedUnits)
            {
                Packet.AddInt(donatedUnit.Data);
                Packet.AddInt(donatedUnit.Count);
                Packet.AddInt(donatedUnit.Level);
            }
        }

        internal override void Load(JToken Json)
        {
            base.Load(Json);

            string Message;
            if (JsonHelper.GetJsonString(Json, "message", out Message))
            {
                this.Message = Message;
                this.HaveMessage = true;
            }
            else
            {
                this.Message = string.Empty;
                this.HaveMessage = false;
            }

            JsonHelper.GetJsonNumber(Json, "max_troop", out this.MaxTroop);
            JsonHelper.GetJsonNumber(Json, "max_spell", out this.MaxSpell);
            JsonHelper.GetJsonNumber(Json, "used_troop", out this.UsedTroop);
            JsonHelper.GetJsonNumber(Json, "used_spell", out this.UsedSpell);

            JArray units = (JArray) Json["units"];

            if (units != null)
            {
                if (this.Units != null)
                {
                    this.Units.Load(units);
                }
                else
                {
                    this.Units = new AllianceUnitSlots();
                    this.Units.Load(units);
                }
            }
        }

        internal override JObject Save()
        {
            JObject Json = base.Save();

            Json.Add("message", this.Message);
            Json.Add("max_troop", this.MaxTroop);
            Json.Add("max_spell", this.MaxSpell);
            Json.Add("used_troop", this.UsedTroop);
            Json.Add("used_spell", this.UsedSpell);

            if (this.Units != null)
            {
                Json.Add("units", this.Units.Save());
            }
            else
            {
                Json.Add("units", new JArray());
                ;
            }

            return Json;
        }

        internal void ShowValues()
        {
            foreach (FieldInfo Field in this.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (Field != null)
                {
                    Logging.Info(this.GetType(), ConsolePad.Padding(Field.Name) + " : " + ConsolePad.Padding(!string.IsNullOrEmpty(Field.Name) ? (Field.GetValue(this) != null ? Field.GetValue(this).ToString() : "(null)") : "(null)", 40));
                }
            }
        }
    }
}