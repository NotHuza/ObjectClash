namespace CR.Servers.CoC.Packets.Enums
{
    internal enum LoginFailedReason
    {
        Default = 0,
        Reset = 1,
        Patch = 7,
        Update = 8,
        Redirection = 9,
        Maintenance = 10,
        Banned = 11,
        Pause = 12,
        Locked = 13,
        PurchaseFailed = 14,
        UpdateInProgress = 16,
        ExtendedBreak = 21
    }
}