using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;

namespace Erd_Tools.Models
{
    public abstract class Chr
    {
        private ErdHook _hook;
        public Chr(PHPointer chrIns, ErdHook hook)
        {
            _hook = hook;
            _chrIns = chrIns;
            _chrModuleBase = _hook.CreateChildPointer(_chrIns, (int)Offsets.EnemyIns.ModuleBase);
            _chrData = _hook.CreateChildPointer(_chrModuleBase, (int)Offsets.ModuleBase.EnemyData);
            _chrResistance = _hook.CreateChildPointer(_chrModuleBase, (int)Offsets.ModuleBase.ResistenceData);
            _chrStagger = _hook.CreateChildPointer(_chrModuleBase, (int)Offsets.ModuleBase.StaggerData);
            _chrActionRequest = _hook.CreateChildPointer(_chrModuleBase, (int)Offsets.ModuleBase.ActionRequest);
        }
        private PHPointer _chrIns { get; set; }

        public string TargetChrInsPtr => $"0x{_chrIns?.Resolve():X2}";

        private PHPointer _chrCtrl;
        private PHPointer _chrModuleBase;
        private PHPointer _chrData;
        private PHPointer _chrResistance;
        private PHPointer _chrStagger;
        private PHPointer _chrActionRequest;

        public int Handle => _chrIns.ReadInt32((int)Offsets.EnemyIns.EnemyHandle);

        public int ChrType
        {
            get => _chrIns.ReadInt32((int)Offsets.EnemyIns.ChrType);
            set => _chrIns.WriteInt32((int)Offsets.EnemyIns.ChrType, value);

        }

        public byte TeamType
        {
            get => _chrIns.ReadByte((int)Offsets.EnemyIns.TeamType);
            set => _chrIns.WriteByte((int)Offsets.EnemyIns.TeamType, value);

        }

        #region Data
        public string Model => _chrData.ReadString((int)Offsets.EnemyData.Model, Encoding.Unicode, 0x10);
        public string Name => _chrData.ReadString((int)Offsets.EnemyData.Name, Encoding.Unicode, 0x14);
        public int Hp
        {
            get => _chrData.ReadInt32((int)Offsets.EnemyData.Hp);
            set => _chrData.WriteInt32((int)Offsets.EnemyData.Hp, value);

        }
        public int HpMax => _chrData.ReadInt32((int)Offsets.EnemyData.HpMax);
        public int HpBase => _chrData.ReadInt32((int)Offsets.EnemyData.HpBase);
        public int Fp
        {
            get => _chrData.ReadInt32((int)Offsets.EnemyData.Fp);
            set => _chrData.WriteInt32((int)Offsets.EnemyData.Fp, value);
        }
        public int FpMax => _chrData.ReadInt32((int)Offsets.EnemyData.FpMax);
        public int FpBase => _chrData.ReadInt32((int)Offsets.EnemyData.FpBase);
        public int Stamina
        {
            get => _chrData.ReadInt32((int)Offsets.EnemyData.Stam);
            set => _chrData.WriteInt32((int)Offsets.EnemyData.Stam, value);

        }
        public int StaminaMax => _chrData.ReadInt32((int)Offsets.EnemyData.StamMax);
        public int StaminaBase => _chrData.ReadInt32((int)Offsets.EnemyData.StamBase);
        #endregion

        #region Resistence
        public int Poison => _chrResistance.ReadInt32((int)Offsets.ResistenceData.Poison);
        public int PoisonMax => _chrResistance.ReadInt32((int)Offsets.ResistenceData.PoisonMax);
        public int Rot => _chrResistance.ReadInt32((int)Offsets.ResistenceData.Rot);
        public int RotMax => _chrResistance.ReadInt32((int)Offsets.ResistenceData.RotMax);
        public int Bleed => _chrResistance.ReadInt32((int)Offsets.ResistenceData.Bleed);
        public int BleedMax => _chrResistance.ReadInt32((int)Offsets.ResistenceData.BleedMax);
        public int Frost => _chrResistance.ReadInt32((int)Offsets.ResistenceData.Frost);
        public int FrostMax => _chrResistance.ReadInt32((int)Offsets.ResistenceData.FrostMax);
        public int Blight => _chrResistance.ReadInt32((int)Offsets.ResistenceData.Blight);
        public int BlightMax => _chrResistance.ReadInt32((int)Offsets.ResistenceData.BlightMax);
        public int Sleep => _chrResistance.ReadInt32((int)Offsets.ResistenceData.Sleep);
        public int SleepMax => _chrResistance.ReadInt32((int)Offsets.ResistenceData.SleepMax);
        public int Madness => _chrResistance.ReadInt32((int)Offsets.ResistenceData.Madness);
        public int MadnessMax => _chrResistance.ReadInt32((int)Offsets.ResistenceData.MadnessMax);
        #endregion

        #region Stagger
        public float Stagger => _chrStagger.ReadSingle((int)Offsets.StaggerData.Stagger);
        public float StaggerMax => _chrStagger.ReadSingle((int)Offsets.StaggerData.StaggerMax);
        public float ResetTime => _chrStagger.ReadSingle((int)Offsets.StaggerData.ResetTime);
        #endregion

        #region ActionRequest
        public int CurrentAnimation => _chrActionRequest.ReadInt32((int)Offsets.ActionRequest.CurrentAnimation);
        #endregion
    }
}
