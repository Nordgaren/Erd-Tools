using PropertyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erd_Tools.Models
{
    public class Enemy
    {
        private ErdHook _hook;
        public Enemy(PHPointer enemyIns, ErdHook hook)
        {
            _hook = hook;
            _targetEnemyIns = enemyIns;
            _targetEnemyModuleBase = _hook.CreateChildPointer(_targetEnemyIns, (int)Offsets.EnemyIns.ModuleBase);
            _targetEnemyData = _hook.CreateChildPointer(_targetEnemyModuleBase, (int)Offsets.ModuleBase.EnemyData);
            _targetEnemyResistance = _hook.CreateChildPointer(_targetEnemyModuleBase, (int)Offsets.ModuleBase.ResistenceData);
            _targetEnemyStagger =_hook.CreateChildPointer(_targetEnemyModuleBase, (int)Offsets.ModuleBase.StaggerData);
        }
        private PHPointer _targetEnemyIns { get; set; }

        public string TargetEnemyInsPtr => $"0x{_targetEnemyIns?.Resolve():X2}";

        private PHPointer _enemyCtrl;
        private PHPointer _targetEnemyModuleBase;
        private PHPointer _targetEnemyData;
        private PHPointer _targetEnemyResistance;
        private PHPointer _targetEnemyStagger;

        public int Handle => _targetEnemyIns.ReadInt32((int)Offsets.EnemyIns.EnemyHandle);

        public int ChrType
        {
            get => _targetEnemyIns.ReadInt32((int)Offsets.EnemyIns.ChrType);
            set => _targetEnemyIns.WriteInt32((int)Offsets.EnemyIns.ChrType, value);

        }

        public byte TeamType
        {
            get => _targetEnemyIns.ReadByte((int)Offsets.EnemyIns.TeamType);
            set => _targetEnemyIns.WriteByte((int)Offsets.EnemyIns.TeamType, value);

        }

        #region Data
        public string Model => _targetEnemyData.ReadString((int)Offsets.EnemyData.Model, Encoding.Unicode, 0x10);
        public string Name => _targetEnemyData.ReadString((int)Offsets.EnemyData.Name, Encoding.Unicode, 0x14);
        public int Hp
        {
            get => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.Hp);
            set => _targetEnemyData.WriteInt32((int)Offsets.EnemyData.Hp, value);

        }
        public int HpMax => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.HpMax);
        public int HpBase => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.HpBase);
        public int Fp
        {
            get => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.Fp);
            set => _targetEnemyData.WriteInt32((int)Offsets.EnemyData.Fp, value);
        }
        public int FpMax => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.FpMax);
        public int FpBase => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.FpBase);
        public int Stamina
        {
            get => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.Stam);
            set => _targetEnemyData.WriteInt32((int)Offsets.EnemyData.Stam, value);

        }
        public int StaminaMax => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.StamMax);
        public int StaminaBase => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.StamBase); 
        #endregion

        #region Resistence
        public int Poison => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Poison);
        public int PoisonMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.PoisonMax);
        public int Rot => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Rot);
        public int RotMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.RotMax);
        public int Bleed => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Bleed);
        public int BleedMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.BleedMax);
        public int Frost => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Frost);
        public int FrostMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.FrostMax);
        public int Blight => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Blight);
        public int BlightMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.BlightMax);
        public int Sleep => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Sleep);
        public int SleepMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.SleepMax);
        public int Madness => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Madness);
        public int MadnessMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.MadnessMax);
        #endregion

        #region Stagger
        public float Stagger => _targetEnemyStagger.ReadSingle((int)Offsets.StaggerData.Stagger);
        public float StaggerMax => _targetEnemyStagger.ReadSingle((int)Offsets.StaggerData.StaggerMax);
        public float ResetTime => _targetEnemyStagger.ReadSingle((int)Offsets.StaggerData.ResetTime);
        #endregion

    }
}
