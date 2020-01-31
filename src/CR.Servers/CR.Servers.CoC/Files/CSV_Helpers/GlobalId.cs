namespace CR.Servers.CoC.Files.CSV_Helpers
{
    using System;

    internal static class GlobalId
    {
        internal static int GetType(int GlobalID)
        {
            return GlobalID / 1000000;
        }

        internal static int GetID(int GlobalID)
        {
            return GlobalID % 1000000;
        }

        internal static int Create(int Type, int ID)
        {
            return Type * 1000000 + ID;
        }


        [Obsolete]
        internal static int GetTypeOld(int GlobalID)
        {
            GlobalID = (int) ((1125899907 * (long) GlobalID) >> 32);
            return (GlobalID >> 18) + (GlobalID >> 31);
        }

        [Obsolete]
        internal static int GetIDOld(int GlobalID)
        {
            int ReferenceT = 0;
            ReferenceT = (int) ((1125899907 * (long) GlobalID) >> 32);
            return GlobalID - 1000000 * ((ReferenceT >> 18) + (ReferenceT >> 31));
        }
    }
}