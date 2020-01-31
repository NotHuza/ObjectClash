namespace CR.Servers.CoC.Logic
{
    internal class ModSlot : DataSlots
    {
        internal bool AIAttack;
        internal Level AILevel;

        internal bool SelfAttack
        {
            get
            {
                return this.GetCountByGlobalId(0) == 1;
            }
            set
            {
                this.Set(0, value ? 1 : 0);
            }
        }

        internal void Initialize()
        {
            this.Set(0, 0);
        }
    }
}