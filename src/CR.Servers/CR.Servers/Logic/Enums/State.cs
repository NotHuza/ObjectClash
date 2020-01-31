namespace CR.Servers.Logic.Enums
{
    public enum State
    {
        DISCONNECTED = 0,
        SESSION = 1,
        SESSION_OK = 3,
        UNLOCK_ACC = 4,
        LOGIN = 5,
        LOGGED = 6,
        SEARCH_BATTLE = 7,
        IN_PC_BATTLE = 8,
        IN_NPC_BATTLE = 9,
        IN_AMICAL_BATTLE = 10,
        IN_1VS1_BATTLE = 11,
        VISIT = 12,
        WAR_EMODE = 13,
        REPLAY = 14,
    }
}