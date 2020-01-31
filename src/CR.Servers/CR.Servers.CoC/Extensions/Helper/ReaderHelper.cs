namespace CR.Servers.CoC.Extensions.Helper
{
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.Extensions.Binary;

    public static class ReaderHelper
    {
        internal static Data ReadData(this Reader Reader)
        {
            int GlobalID = Reader.ReadInt32();
            return CSV.Tables.GetWithGlobalId(GlobalID);
        }

        internal static T ReadData<T>(this Reader Reader) where T : Data
        {
            int GlobalID = Reader.ReadInt32();
            return CSV.Tables.GetWithGlobalId(GlobalID) as T;
        }
    }
}