using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CR.Servers.CoC.Logic;

namespace CR.Servers.CoC.Packets.Messages.Server.Battle
{
    internal class Battle_Fail : Message
    {
        internal override short Type => 24103;

        public Battle_Fail(Device Device) : base(Device)
        {
            
        }
    }
}
