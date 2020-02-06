using CR.Servers.CoC.Files.CSV_Helpers;
using CR.Servers.Files.CSV_Reader;

namespace CR.Servers.CoC.Files.CSV_Logic.Logic
{
    internal class EffectData : Data
    {
        public EffectData(Row rw, DataTable dt) : base(rw, dt)
        {
        }

        public override string Name
        {
          get; set;
        }

        public string SWF 
        {
          get; protected set;
        }

        public string ExportName 
        {
          get; protected set;
        }

        protected string[] ParticleEmitter 
        {
          get; set;
        }

        public string EmitterDelayMs 
        {
          get; protected set;
        }

        public int CameraShake 
        {
          get; protected set;
        }

        public int CameraShakeTimeMS 
        {
          get; protected set;
        }

        public bool CameraShakeInReplay 
        {
          get; protected set;
        }

        protected bool[] AttachToParent 
        {
          get; set;
        }

        protected bool[] DetachAfterStart 
        {
          get; set;
        }

        protected bool[] DestroyWhenParentDies 
        {
          get; set;
        }

        public bool Looping 
        {
          get; protected set;
        }

        protected string[] IsoLayer 
        {
          get; protected set;
        }

        public bool Targeted 
        {
          get; protected set;
        }

        public int MaxCount 
        {
          get; protected set;
        }

        protected string[] Sound 
        {
          get; set;
        }

        protected int[] Volume 
        {
          get; set;
        }

        protected int[] MinPitch 
        {
          get; set;
        }

        protected int[] MaxPitch 
        {
          get; set;
        }

        public string LowEndSound 
        {
          get; protected set;
        }

        public int LowEndVolume 
        {
          get; protected set;
        }

        public int LowEndMinPitch 
        {
          get; protected set;
        }

        public int LowEndMaxPitch 
        {
          get; protected set;
        }

        public bool StopSound 
        {
          get; protected set;
        }

        public bool Beam 
        {
          get; protected set;
        }

        public bool SortInFrontOfParent 
        {
          get; protected set;
        }
    }
}
