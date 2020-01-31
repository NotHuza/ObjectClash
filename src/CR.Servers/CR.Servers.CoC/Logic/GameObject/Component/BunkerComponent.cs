namespace CR.Servers.CoC.Logic
{
    using CR.Servers.CoC.Extensions;
    using CR.Servers.CoC.Extensions.Game;
    using CR.Servers.CoC.Extensions.Helper;
    using Newtonsoft.Json.Linq;

    internal class BunkerComponent : Component
    {
        internal Timer ArrangeWarTimer;
        internal Timer ChallengeTimer;
        internal Timer ClanMailTimer;
        internal Timer ElderKickTimer;
        internal Timer ShareReplayTimer;

        internal Timer UnitRequestTimer;

        public BunkerComponent(GameObject GameObject) : base(GameObject)
        {
        }

        internal override int Type
        {
            get
            {
                return 7;
            }
        }

        internal bool CanSendUnitRequest
        {
            get
            {
                if (this.UnitRequestTimer != null)
                {
                    return this.UnitRequestTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0;
                }

                return true;
            }
        }

        internal bool CanSendClanMail
        {
            get
            {
                if (this.ClanMailTimer != null)
                {
                    return this.ClanMailTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0;
                }

                return true;
            }
        }

        internal bool CanShareReplay
        {
            get
            {
                if (this.ShareReplayTimer != null)
                {
                    return this.ShareReplayTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0;
                }

                return true;
            }
        }

        internal bool CanElderKick
        {
            get
            {
                if (this.ElderKickTimer != null)
                {
                    return this.ElderKickTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0;
                }

                return true;
            }
        }

        internal bool CanCreateChallenge
        {
            get
            {
                if (this.ChallengeTimer != null)
                {
                    return this.ChallengeTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0;
                }

                return true;
            }
        }

        internal bool CanArrangeWar
        {
            get
            {
                if (this.ArrangeWarTimer != null)
                {
                    return this.ArrangeWarTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0;
                }

                return true;
            }
        }

        internal override void FastForwardTime(int Secs)
        {
            this.UnitRequestTimer?.FastForward(Secs);
            this.ClanMailTimer?.FastForward(Secs);
            this.ShareReplayTimer?.FastForward(Secs);
            this.ElderKickTimer?.FastForward(Secs);
            this.ChallengeTimer?.FastForward(Secs);
            this.ArrangeWarTimer?.FastForward(Secs);
        }

        internal override void Load(JToken Json)
        {
            //TODO: Check if calculated duration is longer then time
            int UnitRequestTime;
            int UnitRequestTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "unit_req_time", out UnitRequestTime) && JsonHelper.GetJsonNumber(Json, "unit_req_time_end", out UnitRequestTimeEnd))
            {
                int UnitRequestTimeDuration = 0;
                if (UnitRequestTime >= 0)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Parent.Level.Player.LastTick);
                    UnitRequestTimeDuration = UnitRequestTimeEnd - startTime;

                    if (UnitRequestTimeDuration < 0)
                    {
                        UnitRequestTimeDuration = 0;
                    }
                    else if (UnitRequestTimeDuration > Globals.AllianceTroopRequestCooldown)
                    {
                        UnitRequestTimeDuration = Globals.AllianceTroopRequestCooldown;
                    }
                }

                this.UnitRequestTimer = new Timer();
                this.UnitRequestTimer.StartTimer(this.Parent.Level.Player.LastTick, UnitRequestTimeDuration);
            }

            int ClanMailTime;
            int ClanMailTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "clan_mail_time", out ClanMailTime) && JsonHelper.GetJsonNumber(Json, "clan_mail_time_end", out ClanMailTimeEnd))
            {
                int ClanMailTimeDuration = 0;
                if (ClanMailTime >= 0)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Parent.Level.Player.LastTick);
                    ClanMailTimeDuration = ClanMailTimeEnd - startTime;

                    if (ClanMailTimeDuration < 0)
                    {
                        ClanMailTimeDuration = 0;
                    }
                    else if (ClanMailTimeDuration > Globals.ClanMailCooldown)
                    {
                        ClanMailTimeDuration = Globals.ClanMailCooldown;
                    }
                }

                this.ClanMailTimer = new Timer();
                this.ClanMailTimer.StartTimer(this.Parent.Level.Player.LastTick, ClanMailTimeDuration);
            }

            int ShareReplayTime;
            int ShareReplayTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "share_replay_time", out ShareReplayTime) && JsonHelper.GetJsonNumber(Json, "share_replay_time_end", out ShareReplayTimeEnd))
            {
                int ShareReplayTimeDuration = 0;
                if (ShareReplayTime >= 0)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Parent.Level.Player.LastTick);
                    ShareReplayTimeDuration = ShareReplayTimeEnd - startTime;

                    if (ShareReplayTimeDuration < 0)
                    {
                        ShareReplayTimeDuration = 0;
                    }
                    else if (ShareReplayTimeDuration > Globals.ReplayShareCooldown)
                    {
                        ShareReplayTimeDuration = Globals.ReplayShareCooldown;
                    }
                }

                this.ShareReplayTimer = new Timer();
                this.ShareReplayTimer.StartTimer(this.Parent.Level.Player.LastTick, ShareReplayTimeDuration);
            }

            int ElderKickTime;
            int ElderKickTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "elder_kick_time", out ElderKickTime) && JsonHelper.GetJsonNumber(Json, "elder_kick_time_end", out ElderKickTimeEnd))
            {
                int ElderKickTimeDuration = 0;
                if (ElderKickTime >= 0)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Parent.Level.Player.LastTick);
                    ElderKickTimeDuration = ElderKickTimeEnd - startTime;

                    if (ElderKickTimeDuration < 0)
                    {
                        ElderKickTimeDuration = 0;
                    }
                    else if (ElderKickTimeDuration > Globals.ElderKickCooldown)
                    {
                        ElderKickTimeDuration = Globals.ElderKickCooldown;
                    }
                }

                this.ElderKickTimer = new Timer();
                this.ElderKickTimer.StartTimer(this.Parent.Level.Player.LastTick, ElderKickTimeDuration);
            }

            int ChallengeTime;
            int ChallengeTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "challenge_time", out ChallengeTime) && JsonHelper.GetJsonNumber(Json, "challenge_time_end", out ChallengeTimeEnd))
            {
                int ChallengeTimeDuration = 0;
                if (ChallengeTime >= 0)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Parent.Level.Player.LastTick);
                    ChallengeTimeDuration = ChallengeTimeEnd - startTime;

                    if (ChallengeTimeDuration < 0)
                    {
                        ChallengeTimeDuration = 0;
                    }
                    else if (ChallengeTimeDuration > Globals.ChallengeCooldown)
                    {
                        ChallengeTimeDuration = Globals.ChallengeCooldown;
                    }
                }

                this.ChallengeTimer = new Timer();
                this.ChallengeTimer.StartTimer(this.Parent.Level.Player.LastTick, ChallengeTimeDuration);
            }

            int ArrangeWarTime;
            int ArrangeWarTimeEnd;
            if (JsonHelper.GetJsonNumber(Json, "arrwar_time", out ArrangeWarTime) && JsonHelper.GetJsonNumber(Json, "arrwar_time_end", out ArrangeWarTimeEnd))
            {
                int ArrangeWarTimeDuration = 0;
                if (ArrangeWarTime >= 0)
                {
                    int startTime = (int) TimeUtils.ToUnixTimestamp(this.Parent.Level.Player.LastTick);
                    ArrangeWarTimeDuration = ArrangeWarTimeEnd - startTime;

                    if (ArrangeWarTimeDuration < 0)
                    {
                        ArrangeWarTimeDuration = 0;
                    }
                    else if (ArrangeWarTimeDuration > Globals.ArrangeWarCooldown)
                    {
                        ArrangeWarTimeDuration = Globals.ArrangeWarCooldown;
                    }
                }

                this.ArrangeWarTimer = new Timer();
                this.ArrangeWarTimer.StartTimer(this.Parent.Level.Player.LastTick, ArrangeWarTimeDuration);
            }

            base.Load(Json);
        }

        internal override void Save(JObject Json)
        {
            if (this.UnitRequestTimer != null)
            {
                Json.Add("unit_req_time", this.UnitRequestTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick));
                Json.Add("unit_req_time_end", this.UnitRequestTimer.EndTime);
            }

            if (this.ClanMailTimer != null)
            {
                Json.Add("clan_mail_time", this.ClanMailTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick));
                Json.Add("clan_mail_time_end", this.ClanMailTimer.EndTime);
            }

            if (this.ShareReplayTimer != null)
            {
                Json.Add("share_replay_time", this.ShareReplayTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick));
                Json.Add("share_replay_time_end", this.ShareReplayTimer.EndTime);
            }

            if (this.ElderKickTimer != null)
            {
                Json.Add("elder_kick_time", this.ElderKickTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick));
                Json.Add("elder_kick_time_end", this.ElderKickTimer.EndTime);
            }

            if (this.ChallengeTimer != null)
            {
                Json.Add("challenge_time", this.ChallengeTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick));
                Json.Add("challenge_time_end", this.ChallengeTimer.EndTime);
            }

            if (this.ArrangeWarTimer != null)
            {
                Json.Add("arrwar_time", this.ArrangeWarTimer.GetRemainingSeconds(this.Parent.Level.Player.LastTick));
                Json.Add("arrwar_time_end", this.ArrangeWarTimer.EndTime);
            }

            base.Save(Json);
        }

        internal override void Tick()
        {
            if (this.UnitRequestTimer?.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0)
            {
                this.UnitRequestTimer = null;
            }

            if (this.ClanMailTimer?.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0)
            {
                this.ClanMailTimer = null;
            }

            if (this.ShareReplayTimer?.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0)
            {
                this.ShareReplayTimer = null;
            }

            if (this.ElderKickTimer?.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0)
            {
                this.ElderKickTimer = null;
            }

            if (this.ChallengeTimer?.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0)
            {
                this.ChallengeTimer = null;
            }

            if (this.ArrangeWarTimer?.GetRemainingSeconds(this.Parent.Level.Player.LastTick) <= 0)
            {
                this.ArrangeWarTimer = null;
            }
        }
    }
}