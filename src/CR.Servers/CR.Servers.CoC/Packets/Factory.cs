namespace CR.Servers.CoC.Packets
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CR.Servers.CoC.Core;
    using CR.Servers.CoC.Logic;
    using CR.Servers.CoC.Packets.Commands.Client;
    using CR.Servers.CoC.Packets.Commands.Client.Battle;
    using CR.Servers.CoC.Packets.Commands.Client.Unknown;
    using CR.Servers.CoC.Packets.Commands.Server;
    using CR.Servers.CoC.Packets.Debugs;
    using CR.Servers.CoC.Packets.Debugs.Elite;
    using CR.Servers.CoC.Packets.Messages.Client.Account;
    using CR.Servers.CoC.Packets.Messages.Client.Alliances;
    using CR.Servers.CoC.Packets.Messages.Client.API;
    using CR.Servers.CoC.Packets.Messages.Client.Avatar;
    using CR.Servers.CoC.Packets.Messages.Client.Battle;
    using CR.Servers.CoC.Packets.Messages.Client.Friend;
    using CR.Servers.CoC.Packets.Messages.Client.Home;
    using CR.Servers.CoC.Packets.Messages.Client.Leaderboard;
    using CR.Servers.Extensions.Binary;

    internal static class Factory
    {
        internal const string RC4Key = "fhsd6f86f67rt8fw78fw789we78r9789wer6re";
        internal const char Delimiter = '/';

        internal static readonly Dictionary<short, Type> Messages = new Dictionary<short, Type>();
        internal static readonly Dictionary<int, Type> Commands = new Dictionary<int, Type>();
        internal static readonly Dictionary<string, Type> Debugs = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        internal static void Initialize()
        {
            Factory.LoadMessages();
            Factory.LoadCommands();
            Factory.LoadDebugs();
        }

        private static void LoadMessages()
        {
            Factory.Messages.Add(10100, typeof(ClientHelloMessage));
            Factory.Messages.Add(10101, typeof(AuthenticationMessage));
            Factory.Messages.Add(10108, typeof(KeepAliveMessage));
            Factory.Messages.Add(10113, typeof(SetDeviceTokenMessage));
            Factory.Messages.Add(10121, typeof(UnlockAccountMessage));
            Factory.Messages.Add(10212, typeof(ChangeAvatarMessage));
            Factory.Messages.Add(10501, typeof(AcceptFriendRequestMessage));
            Factory.Messages.Add(10502, typeof(FriendRequestMessage));
            Factory.Messages.Add(10504, typeof(AskForAvatarFriendListMessage));
            Factory.Messages.Add(10506, typeof(RemoveFriendMessage));
            Factory.Messages.Add(10513, typeof(AskForPlayingFacebookFriends));
            Factory.Messages.Add(10905, typeof(InboxOpenedMessage));
            Factory.Messages.Add(14101, typeof(GoHomeMessage));
            Factory.Messages.Add(14102, typeof(EndClientTurnMessage));
            Factory.Messages.Add(14103, typeof(CancelDuelMatchmakeMessage));
            Factory.Messages.Add(14113, typeof(VisitHomeMessage));
            Factory.Messages.Add(14123, typeof(AttackMatchedHomeMessage));
            Factory.Messages.Add(14134, typeof(AskForAttackNpcMessage));
            Factory.Messages.Add(14135, typeof(Village2AttackNpcMessage));
            Factory.Messages.Add(14201, typeof(BindFacebookAccountMessage));
            Factory.Messages.Add(14212, typeof(BindGamecenterAccountMessage));
            Factory.Messages.Add(14301, typeof(CreateAllianceMessage));
            Factory.Messages.Add(14302, typeof(AskForAllianceDataMessage));
            Factory.Messages.Add(14303, typeof(AskForJoinableAllianceListMessage));
            Factory.Messages.Add(14305, typeof(JoinAllianceMessage));
            Factory.Messages.Add(14306, typeof(ChangeAllianceMemberRoleMessage));
            Factory.Messages.Add(14308, typeof(LeaveAllianceMessage));
            Factory.Messages.Add(14310, typeof(DonateAllianceMessage));
            Factory.Messages.Add(14315, typeof(ChatToAllianceStreamMessage));
            Factory.Messages.Add(14316, typeof(ChangeAllianceSettingsMessage));
            Factory.Messages.Add(14317, typeof(RequestJoinAllianceMessage));
            Factory.Messages.Add(14321, typeof(RespondToAllianceJoinRequestMessage));
            Factory.Messages.Add(14324, typeof(SearchAlliancesMessage));
            Factory.Messages.Add(14325, typeof(AskForAvatarProfileMessage));
            Factory.Messages.Add(14334, typeof(ClaimAllianceGiftMessage));
            Factory.Messages.Add(14401, typeof(AskForAllianceRankingListMessage));
            Factory.Messages.Add(14403, typeof(AskForAvatarRankingListMessage));
            Factory.Messages.Add(14404, typeof(AskForAvatarLocalRankingListMessage));
            Factory.Messages.Add(14510, typeof(BattleEndClientTurnMessage));
            Factory.Messages.Add(14715, typeof(SendGlobalChatLineMessage));
            Factory.Messages.Add(15110, typeof(DuelLiveReplayMessage));
        }

        private static void LoadCommands()
        {
            Factory.Commands.Add(1, typeof(Joined_Alliance));
            Factory.Commands.Add(2, typeof(Leaved_Alliance));
            Factory.Commands.Add(3, typeof(Name_Change_Callback));
            Factory.Commands.Add(4, typeof(Donate_Unit_Callback));
            Factory.Commands.Add(5, typeof(Alliance_Unit_Received));
            Factory.Commands.Add(6, typeof(Changed_Alliance_Settings));
            Factory.Commands.Add(7, typeof(Diamonds_Added));
            Factory.Commands.Add(8, typeof(Changed_Alliance_Role));

            Factory.Commands.Add(500, typeof(Buy_Building)); // ?
            Factory.Commands.Add(501, typeof(Move_Building));
            Factory.Commands.Add(502, typeof(Upgrade_Building));
            Factory.Commands.Add(503, typeof(Sell_Building));
            Factory.Commands.Add(504, typeof(Speed_Up_Construction));
            Factory.Commands.Add(505, typeof(Cancel_Consturction));
            Factory.Commands.Add(506, typeof(Collect_Resource));
            Factory.Commands.Add(507, typeof(Clear_Obstacle));
            Factory.Commands.Add(508, typeof(Train_Unit));
            Factory.Commands.Add(509, typeof(Cancel_Troop_Training));
            Factory.Commands.Add(510, typeof(Buy_Trap));
            Factory.Commands.Add(511, typeof(Troop_Request));
            Factory.Commands.Add(512, typeof(Buy_Deco));
            Factory.Commands.Add(513, typeof(Speed_Up_Troop_Training));
            Factory.Commands.Add(516, typeof(Upgrade_Unit));
            Factory.Commands.Add(517, typeof(Speed_Up_Upgrade_Unit));
            Factory.Commands.Add(518, typeof(Buy_Resources));
            Factory.Commands.Add(519, typeof(Mission_Progress));
            Factory.Commands.Add(520, typeof(Unlock_Building));
            Factory.Commands.Add(521, typeof(Free_Worker));
            Factory.Commands.Add(522, typeof(Buy_Shield));
            Factory.Commands.Add(523, typeof(Claim_Achievement_Reward));
            Factory.Commands.Add(524, typeof(Change_Weapon_Mode));
            Factory.Commands.Add(526, typeof(Boost_Building));
            Factory.Commands.Add(527, typeof(Upgrade_Hero));
            Factory.Commands.Add(528, typeof(Speed_Up_Hero_Upgrade));
            Factory.Commands.Add(529, typeof(Change_Hero_State));
            Factory.Commands.Add(530, typeof(Unknown_530));
            Factory.Commands.Add(531, typeof(Cancel_Hero_Upgrade));
            Factory.Commands.Add(532, typeof(New_Shop_Seen));
            Factory.Commands.Add(533, typeof(Move_Multiple_Buildings));
            Factory.Commands.Add(534, typeof(Unknown_534));
            Factory.Commands.Add(537, typeof(Send_Alliance_Mail));
            Factory.Commands.Add(538, typeof(League_Notifications_Seen));
            Factory.Commands.Add(539, typeof(News_Seen));
            Factory.Commands.Add(540, typeof(Save_Alliance_Troop_Request_Message));
            Factory.Commands.Add(541, typeof(Speed_Up_Troop_Request));
            Factory.Commands.Add(543, typeof(Elder_Kick));
            Factory.Commands.Add(544, typeof(Edit_Mode_Shown));
            Factory.Commands.Add(546, typeof(Move_Building_In_Layout));
            Factory.Commands.Add(548, typeof(Finish_Later_Layout));
            Factory.Commands.Add(549, typeof(Upgrade_Multiple_Building));
            Factory.Commands.Add(550, typeof(Remove_Units));
            Factory.Commands.Add(552, typeof(Open_Close_Layout));
            Factory.Commands.Add(553, typeof(Unknown_553));
            Factory.Commands.Add(554, typeof(Change_Weapon_Heading));
            Factory.Commands.Add(556, typeof(Unknown_556));
            Factory.Commands.Add(558, typeof(Add_Quick_Train));
            Factory.Commands.Add(559, typeof(Train_Quick_Units));
            Factory.Commands.Add(560, typeof(Unknown_560));
            Factory.Commands.Add(566, typeof(Unknown_566));
            Factory.Commands.Add(567, typeof(Set_Active_Layout));
            Factory.Commands.Add(568, typeof(Copy_Village_Layout));
            Factory.Commands.Add(569, typeof(Remove_All_Building_In_Layout));
            Factory.Commands.Add(570, typeof(Unknown_570));
            Factory.Commands.Add(571, typeof(Toggle_Clan_Filter));
            Factory.Commands.Add(572, typeof(Change_Hero_Mode));
            Factory.Commands.Add(573, typeof(Unknown_573));
            Factory.Commands.Add(574, typeof(Send_Alliance_Challenge));
            Factory.Commands.Add(576, typeof(Unknown_576));
            Factory.Commands.Add(577, typeof(Swap_GameObject));
            Factory.Commands.Add(579, typeof(Friend_List_Last_Opened));
            Factory.Commands.Add(581, typeof(Unknown_581));
            Factory.Commands.Add(584, typeof(Boost_Buildings_Of_Type));
            Factory.Commands.Add(585, typeof(Unknown_585));
            Factory.Commands.Add(586, typeof(Rename_Quick_Train));
            Factory.Commands.Add(590, typeof(Buy_Walls));
            Factory.Commands.Add(591, typeof(Change_Village_Mode));
            Factory.Commands.Add(592, typeof(Train_Unit_V2));
            Factory.Commands.Add(593, typeof(Speed_Up_All_Training_V2));
            Factory.Commands.Add(595, typeof(Clock_Tower_Boost));
            Factory.Commands.Add(596, typeof(Remove_Units_V2));
            Factory.Commands.Add(597, typeof(Unknown_597));
            Factory.Commands.Add(598, typeof(Layout_Building_Position));
            Factory.Commands.Add(599, typeof(Unknown_599));
            Factory.Commands.Add(600, typeof(Gear_Up));
            Factory.Commands.Add(601, typeof(Search_Opponent_V2));
            Factory.Commands.Add(603, typeof(Account_Bound));
            Factory.Commands.Add(604, typeof(Seen_Builder_Menu));
            Factory.Commands.Add(605, typeof(Unknown_605));
            Factory.Commands.Add(700, typeof(Place_Attacker));
            Factory.Commands.Add(701, typeof(Place_Alliance_Portal));
            Factory.Commands.Add(702, typeof(End_Attack_Preparation));
            Factory.Commands.Add(703, typeof(Surrender_Attack));
            Factory.Commands.Add(704, typeof(Place_Spell));
            Factory.Commands.Add(705, typeof(Place_Hero));
            Factory.Commands.Add(706, typeof(Hero_Rage));
            Factory.Commands.Add(711, typeof(Change_Battle_Troop));
            Factory.Commands.Add(800, typeof(Search_Opponent));
        }

        private static void LoadDebugs()
        {
            Factory.Debugs.Add("AddCastleUnit", typeof(Add_Castle_Unit));
            Factory.Debugs.Add("AddSpells", typeof(Add_Spells));
            Factory.Debugs.Add("AddUnits", typeof(Add_Units));
            Factory.Debugs.Add("AiBase", typeof(AI_Generate_Base)); // Elite Command.
            Factory.Debugs.Add("AiLowBase", typeof(AI_Generate_Low_Base)); // Elite Command.
            Factory.Debugs.Add("ClearObstacles", typeof(Clear_All_Obstacles));
            Factory.Debugs.Add("ClearUnits", typeof(Clear_All_Units));
            Factory.Debugs.Add("ClearSpells", typeof(Clear_All_Spells));
            Factory.Debugs.Add("FastForward", typeof(Fast_Forward));
            Factory.Debugs.Add("Help", typeof(Help_Command));
            Factory.Debugs.Add("Id", typeof(Print_Account_ID));
            Factory.Debugs.Add("MaxLevels", typeof(Max_Levels));
            Factory.Debugs.Add("MaxResources", typeof(Max_Resources));
            Factory.Debugs.Add("ResetBase", typeof(Remove_All_Buildings));
            Factory.Debugs.Add("SetBases", typeof(Set_Bases));
            Factory.Debugs.Add("SetLevel", typeof(Set_Exp_Level));
            Factory.Debugs.Add("SetScore", typeof(Set_Score));
            Factory.Debugs.Add("Status", typeof(Server_Status));
            Factory.Debugs.Add("OwnBaseAttack", typeof(Own_Base_Attack));
        }


        internal static Message CreateMessage(short Type, Device Device, Reader Reader)
        {
            Type MType;
            if (Factory.Messages.TryGetValue(Type, out MType))
            {
                return (Message) Activator.CreateInstance(MType, Device, Reader);
            }

            Logging.Error(typeof(Factory), "Can't handle the following message : ID " + Type + ".");

            return null;
        }

        internal static Command CreateCommand(int Type, Device Device, Reader Reader)
        {
            Type CType;
            if (Factory.Commands.TryGetValue(Type, out CType))
            {
                return (Command) Activator.CreateInstance(CType, Device, Reader);
            }

            Logging.Error(typeof(Factory), "Command " + Type + " not exist.");

            return null;
        }

        internal static Debug CreateDebug(string Message, Device Device, out string CommandName)
        {
            string[] Parameters = Message.Remove(0, 1).Split(' ');
            CommandName = Parameters[0];

            Type DType;
            if (Factory.Debugs.TryGetValue(Parameters[0], out DType))
            {
                string[] args = Parameters.Skip(1).ToArray();
                Debug Debug = (Debug) Activator.CreateInstance(DType, Device, args);

                return Debug;
            }

            return null;
        }
    }
}