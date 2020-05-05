namespace CR.Servers.CoC.Packets.Messages.Server.Leaderboard
{
    using CR.Servers.CoC.Core;
    using CR.Servers.Extensions;
    using CR.Servers.Extensions.List;
    using CR.Servers.CoC.Logic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Logic.Clan;

    internal class Global_Clans : Message
    {
        internal List<Alliance> Alliances;

        public Global_Clans(Device Device) : base(Device)
        {

        }


        internal override short Type
        {
            get
            {
                return 24401;
            }
        }

        internal override void Encode()
        {
            var _Packet = new List<byte>();
            var i = 0;

            this.Data.AddInt(this.Alliances.Count);

            foreach (Alliance T in this.Alliances)
            {
                this.Data.AddLong(T.AllianceId);
                this.Data.AddString(T.Header.Name);
                this.Data.AddInt(i++);
                this.Data.AddInt(T.Header.RequiredScore); //should be trophies, not required score.. trophies need to be established
                this.Data.AddInt(Resources.Random.Next(0, 10));
                this.Data.AddInt(T.Header.Badge);
                this.Data.AddInt(T.Header.NumberOfMembers);
                this.Data.AddInt(0);
                this.Data.AddInt(T.Header.ExpLevel);
            }

            this.Data.AddInt((int)(DateTime.UtcNow.LastDayOfMonth() - DateTime.UtcNow).TotalSeconds);

            this.Data.AddInt(3);
            this.Data.AddInt(50000);
            this.Data.AddInt(30000);
            this.Data.AddInt(15000);
        }
    }
}