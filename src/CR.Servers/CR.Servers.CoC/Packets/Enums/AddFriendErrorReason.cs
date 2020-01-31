namespace CR.Servers.CoC.Packets.Enums
{
    public enum AddFriendErrorReason
    {
        Generic = 0,
        TooManyRequestsYou = 1,
        TooManyRequestsOther = 2,

        OwnAvatar = 4,
        DoesNotExist = 5,

        TooManyFriendsYou = 7,
        TooManyFriendsOther = 8
    }
}