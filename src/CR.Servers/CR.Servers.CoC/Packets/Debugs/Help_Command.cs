namespace CR.Servers.CoC.Packets.Debugs
{
    using CR.Servers.CoC.Logic;
    using CR.Servers.Logic.Enums;

    internal class Help_Command : Debug
    {
        public Help_Command(Device Device, params string[] Parameters) : base(Device, Parameters)
        {
            // Help_Command
        }

        internal override Rank RequiredRank
        {
            get
            {
                return Rank.Player;
            }
        }

        internal override void Process()
        {
            SendChatMessage("~ Available Server Commands ~ \n" +
                "/help (lists available server commands) \n" +
                "/status (prints the server's status) \n" +
                "/id (prints your Account ID) \n" +
                "/addunits (adds 500 of all units) \n" +
                "/clearunits (clears all of your units) \n" +
                "/addspells (adds 500 of all spells) \n" +
                "/clearspells (clears all of your spells) \n" +
                "/ownbaseattack (attack yourself) \n" +
                "/clearobstacles (clears all of your existing obstacles) \n" +
                "/resetbase (clears all of your existing buildings and resets your main base) \n" +
                "/maxlevels <village_id> (maximize your existing buildings' level) \n" +
                "/setlevel <level> (sets your avatar XP level (example: '/setlevel 500') ) \n" +
                "/setscore <amount> (sets your trophy count (example: '/setscore 5000') ) \n" +
                "/maxresources (refills your storages) \n" +
                "/setbases (set your bases maxed - by inputting the desired TH level. Example: '/setbases 11' - will give you TH11 & BH8 maxed base. You can use TH levels from 1 to 11, as much times as you want.)");
        }
    }
}