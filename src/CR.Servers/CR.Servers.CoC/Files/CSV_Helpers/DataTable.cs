namespace CR.Servers.CoC.Files.CSV_Helpers
{
    using System.Collections.Generic;
    using CR.Servers.CoC.Files.CSV_Logic.Logic;
    using CR.Servers.CoC.Logic.Enums;
    using CR.Servers.Files.CSV_Reader;

    internal class DataTable
    {
        internal List<Data> Datas;
        internal int Index;

        internal DataTable()
        {
            this.Datas = new List<Data>();
        }

        internal DataTable(Table Table, int Index)
        {
            this.Index = Index;
            this.Datas = new List<Data>();

            for (int i = 0; i < Table.GetRowCount(); i++)
            {
                Row Row = Table.GetRowAt(i);
                Data Data = this.Create(Row);

                this.Datas.Add(Data);
            }
        }

        internal Data Create(Row _rw)
        {
            Data _dt;

            switch ((Gamefile) this.Index)
            {
                case Gamefile.Buildings:
                    _dt = new BuildingData(_rw, this);
                    break;
                case Gamefile.Locales:
                    _dt = new LocaleData(_rw, this);
                    break;
                case Gamefile.Resources:
                    _dt = new ResourceData(_rw, this);
                    break;
                case Gamefile.Characters:
                    _dt = new CharacterData(_rw, this);
                    break;
                case Gamefile.Building_Classes:
                    _dt = new BuildingClassData(_rw, this);
                    break;
                case Gamefile.Obstacles:
                    _dt = new ObstacleData(_rw, this);
                    break;
                case Gamefile.Effect:
                    _dt = new EffectData(_rw, this);
                    break;
                case Gamefile.Traps:
                    _dt = new TrapData(_rw, this);
                    break;
                case Gamefile.Globals:
                    _dt = new GlobalData(_rw, this);
                    break;
                case Gamefile.Experience_Levels:
                    _dt = new ExperienceLevelData(_rw, this);
                    break;
                case Gamefile.Townhall_Levels:
                    _dt = new TownhallLevelData(_rw, this);
                    break;
                case Gamefile.Npcs:
                    _dt = new NpcData(_rw, this);
                    break;
                case Gamefile.Decos:
                    _dt = new DecoData(_rw, this);
                    break;
                case Gamefile.Shields:
                    _dt = new ShieldData(_rw, this);
                    break;
                case Gamefile.Missions:
                    _dt = new MissionData(_rw, this);
                    break;
                case Gamefile.Achievements:
                    _dt = new AchievementData(_rw, this);
                    break;
                case Gamefile.Spells:
                    _dt = new SpellData(_rw, this);
                    break;
                case Gamefile.Heroes:
                    _dt = new HeroData(_rw, this);
                    break;
                case Gamefile.Leagues:
                    _dt = new LeagueData(_rw, this);
                    break;
                case Gamefile.Regions:
                    _dt = new RegionData(_rw, this);
                    break;
                case Gamefile.AllianceBadgeLayer:
                    _dt = new AllianceBadgeLayerData(_rw, this);
                    break;
                case Gamefile.Variables:
                    _dt = new VariableData(_rw, this);
                    break;
                case Gamefile.Village_Objects:
                    _dt = new VillageObjectData(_rw, this);
                    break;
                default:
                    _dt = new Data(_rw, this);
                    break;
            }

            return _dt;
        }

        internal Data GetDataWithID(int ID)
        {
            int InstanceID = GlobalId.GetID(ID);
            return this.Datas[InstanceID];
        }

        internal Data GetDataWithInstanceID(int ID)
        {
            return this.Datas[ID];
        }

        internal Data GetData(string _Name)
        {
            for (int i = 0; i < Datas.Count; i++)
            {
                var data = Datas[i];
                if (data.Row.Name == _Name)
                    return data;
            }
            return null;
        }
    }
}
