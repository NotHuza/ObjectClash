namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Extensions.Helper;
    using CR.Servers.CoC.Files;
    using CR.Servers.CoC.Files.CSV_Helpers;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using Newtonsoft.Json.Linq;

    internal class CombatComponent : Component
    {
        internal int AimAngle = 360;
        internal int AimAngleDraft = 360;
        internal bool AimRotateStep;
        internal bool AirMode;
        internal bool AirModeDraft;
        internal bool AltAttackMode;
        internal bool AltDirectionMode;
        internal bool AltTrapAttackMode;

        internal int Ammo = -1;
        internal bool AttackMode;
        internal bool AttackModeDraft;
        internal int GearUp = -1;
        internal bool NeedsRepair = true;
        internal int TrapDirection;
        internal int TrapDirectionDraft;
        internal int WallI = -1;
        internal int WallP = -1;
        internal int WallX = -1;

        public CombatComponent(GameObject GameObject) : base(GameObject)
        {
            if (GameObject is Building)
            {
                Building Building = (Building)GameObject;
                BuildingData bd = Building.BuildingData;

                if (bd.AmmoCount > 0)
                {
                    this.Ammo = bd.AmmoCount;
                }

                if (bd.AltAttackMode)
                {
                    this.AltAttackMode = true;
                }

                if (bd.AimRotateStep > 0)
                {
                    this.AimRotateStep = true;
                }
            }
            else if (GameObject is Trap)
            {
                Trap Trap = (Trap)GameObject;
                TrapData td = Trap.TrapData;
                if (td.HasAltMode)
                {
                    this.AltTrapAttackMode = true;
                }

                if (td.DirectionCount > 0)
                {
                    this.AltDirectionMode = true;
                }
            }
        }

        internal override int Type
        {
            get
            {
                return 1;
            }
        }

        internal void FillAmmo()
        {
            Player ca = this.Parent.Level.Player;
            BuildingData bd = (BuildingData)this.Parent.Data;
            Data rd = CSV.Tables.Get(Gamefile.Resources).GetData(bd.AmmoResource);
            if (ca.Resources.GetCountByData(rd) >= bd.AmmoCost[0])
            {
                ca.Resources.Minus(rd.GlobalId, bd.AmmoCost[0]);
                this.Ammo = bd.AmmoCount;
            }
        }

        internal override void Load(JToken Json)
        {
            int GearUp;
            if (JsonHelper.GetJsonNumber(Json, "gear", out GearUp))
            {
                this.GearUp = GearUp;
            }

            int Ammo;
            if (JsonHelper.GetJsonNumber(Json, "ammo", out Ammo))
            {
                this.Ammo = Ammo;
            }

            int WallX;
            if (JsonHelper.GetJsonNumber(Json, "wX", out WallX))
            {
                this.WallX = WallX;
            }

            int WallP;
            if (JsonHelper.GetJsonNumber(Json, "wP", out WallP))
            {
                this.WallP = WallP;
            }

            int WallI;
            if (JsonHelper.GetJsonNumber(Json, "wI", out WallI))
            {
                this.WallI = WallI;

                if (WallI > this.Parent.Level.Player.WallGroupId)
                {
                    this.Parent.Level.Player.WallGroupId = WallI;
                }
            }


            bool AttackMode;
            bool AttackModeDraft;
            if (JsonHelper.GetJsonBoolean(Json, "attack_mode", out AttackMode) && JsonHelper.GetJsonBoolean(Json, "attack_mode_draft", out AttackModeDraft))
            {
                this.AltAttackMode = true;
                this.AttackMode = AttackMode;
                this.AttackModeDraft = AttackModeDraft;
            }

            bool AirMode;
            bool AirModeDraft;
            if (JsonHelper.GetJsonBoolean(Json, "air_mode", out AirMode) && JsonHelper.GetJsonBoolean(Json, "air_mode", out AirModeDraft))
            {
                this.AltTrapAttackMode = true;
                this.AirMode = AirMode;
                this.AirModeDraft = AirModeDraft;
            }

            int AimAngle;
            int AimAngleDraft;
            if (JsonHelper.GetJsonNumber(Json, "aim_angle", out AimAngle) && JsonHelper.GetJsonNumber(Json, "aim_angle_draft", out AimAngleDraft))
            {
                this.AimRotateStep = true;
                this.AimAngle = AimAngle;
                this.AimAngleDraft = AimAngleDraft;
            }


            int TrapDirection;
            int TrapDirectionDraft;
            if (JsonHelper.GetJsonNumber(Json, "trapd", out TrapDirection) && JsonHelper.GetJsonNumber(Json, "trapd_draft", out TrapDirectionDraft))
            {
                this.AltDirectionMode = true;
                this.TrapDirection = TrapDirection;
                this.TrapDirectionDraft = TrapDirectionDraft;
            }

            base.Load(Json);
        }

        internal override void Save(JObject jsonObject)
        {
            if (this.GearUp >= 0)
            {
                jsonObject.Add("gear", this.GearUp);
            }

            if (this.Ammo >= 0)
            {
                jsonObject.Add("ammo", this.Ammo);
            }

            if (this.WallX >= 0)
            {
                jsonObject.Add("wX", this.WallX);
            }

            if (this.WallP >= 0)
            {
                jsonObject.Add("wP", this.WallP);
            }

            if (this.WallI >= 0)
            {
                jsonObject.Add("wI", this.WallI);
            }

            if (this.AltAttackMode)
            {
                jsonObject.Add("attack_mode", this.AttackMode);
                jsonObject.Add("attack_mode_draft", this.AttackModeDraft);
            }

            if (this.AltTrapAttackMode)
            {
                jsonObject.Add("air_mode", this.AirMode);
                jsonObject.Add("air_mode_draft", this.AirModeDraft);
            }

            if (this.AimRotateStep)
            {
                jsonObject.Add("aim_angle", this.AimAngle);
                jsonObject.Add("aim_angle_draft", this.AimAngle);
            }

            if (this.AltDirectionMode)
            {
                jsonObject.Add("trapd", this.TrapDirection);
                jsonObject.Add("trapd_draft", this.TrapDirectionDraft);
            }

            base.Save(jsonObject);
        }
    }
}