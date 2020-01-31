namespace CR.Servers.CoC.Logic.Mode
{
    using CR.Servers.CoC.Logic.Manager;

    internal class GameMode
    {
        internal CommandManager CommandManager;
        internal Device Device;
        internal Level Level;

        internal Time Time;

        public GameMode(Device Device)
        {
            this.Device = Device;
            this.Time = new Time();
            this.CommandManager = new CommandManager();
            /*this.GameLogManager = new GameLogManager(this);*/
        }
        //internal State State;

        /*
        internal GameLogManager GameLogManager;*/

        internal bool Connected
        {
            get
            {
                return this.Device != null && this.Device.Connected;
            }
        }

        internal void LoadLevel(Level Level)
        {
            this.Level = Level;
            this.Level.SetGameMode(this);
            this.CommandManager.SetLevel(Level);
        }
    }
}