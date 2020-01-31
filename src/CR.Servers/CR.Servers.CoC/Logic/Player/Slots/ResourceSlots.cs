namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Logic.Enums;

    internal class ResourceSlots : DataSlots
    {
        internal ResourceSlots(int Capacity = 10) : base(Capacity)
        {
            // ResourceSlots.
        }

        internal void Initialize()
        {
            if (Extension.ParseConfigBoolean("Game:StartingResources:FetchFromCSV"))
            {
                this.Set(Resource.Gold, Globals.StartingGold);
                this.Set(Resource.Elixir, Globals.StartingElixir);
                this.Set(Resource.Builder_Gold, Globals.StartingGold2);
                this.Set(Resource.Builder_Elixir, Globals.StartingElixir2);
            }
            else
            {
                this.Set(Resource.Gold, Extension.ParseConfigInt("Game:StartingResources:Gold"));
                this.Set(Resource.Elixir, Extension.ParseConfigInt("Game:StartingResources:Elixir"));
                this.Set(Resource.DarkElixir, Extension.ParseConfigInt("Game:StartingResources:DarkElixir"));
                this.Set(Resource.Builder_Gold, Extension.ParseConfigInt("Game:StartingResources:GoldV2"));
                this.Set(Resource.Builder_Elixir, Extension.ParseConfigInt("Game:StartingResources:ElixirV2"));
            }
        }

        internal void Set(Resource Resource, int Count)
        {
            this.Set(3000000 + (int) Resource, Count);
        }

        internal void Plus(int Global, int Count)
        {
            int i = this.FindIndex(R => R.Data == Global);

            if (i > -1)
            {
                this[i].Count += Count;
            }
            else
            {
                this.Add(new Item(Global, Count));
            }
        }

        internal void Plus(Resource Resource, int Count)
        {
            this.Plus(3000000 + (int) Resource, Count);
        }

        internal bool Minus(int Global, int Count)
        {
            int i = this.FindIndex(R => R.Data == Global);

            if (i > -1)
            {
                if (this[i].Count >= Count)
                {
                    this[i].Count -= Count;
                    return true;
                }
            }

            return false;
        }

        internal void Minus(Resource _Resource, int _Value)
        {
            int Index = this.FindIndex(T => T.Data == 3000000 + (int) _Resource);

            if (Index > -1)
            {
                this[Index].Count -= _Value;
            }
        }
    }
}