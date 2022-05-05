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
            TargetEnemyIns = enemyIns;
            _targetEnemyModuleBase = _hook.CreateChildPointer(_targetEnemyIns, (int)Offsets.EnemyIns.ModuleBase);
            _targetEnemyData = _hook.CreateChildPointer(_targetEnemyModuleBase, (int)Offsets.ModuleBase.EnemyData);
            _targetEnemyResistance = _hook.CreateChildPointer(_targetEnemyModuleBase, (int)Offsets.ModuleBase.ResistenceData);
            _targetEnemyStagger =_hook.CreateChildPointer(_targetEnemyModuleBase, (int)Offsets.ModuleBase.StaggerData);
        }
        private PHPointer _targetEnemyIns { get; set; }
        private PHPointer TargetEnemyIns
        {
            get => _targetEnemyIns;
            set => _targetEnemyIns = value;
        }
        public string TargetEnemyInsPtr => $"0x{_targetEnemyIns?.Resolve():X2}";

        private PHPointer _targetEnemyModuleBase;
        private PHPointer _targetEnemyData;
        private PHPointer _targetEnemyResistance;
        private PHPointer _targetEnemyStagger;

        public int EnemyHandle => _targetEnemyIns.ReadInt32((int)Offsets.EnemyIns.EnemyHandle);
        public string EnemyModel => _targetEnemyData.ReadString((int)Offsets.EnemyData.Model, Encoding.Unicode, 0x10);
        public string EnemyName => _targetEnemyData.ReadString((int)Offsets.EnemyData.Name, Encoding.Unicode, 0x28);

        public int TargetChrType
        {
            get => _targetEnemyIns.ReadInt32((int)Offsets.EnemyIns.ChrType);
            set => _targetEnemyIns.WriteInt32((int)Offsets.EnemyIns.ChrType, value);

        }
        public int TargetHp
        {
            get => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.Hp);
            set => _targetEnemyData.WriteInt32((int)Offsets.EnemyData.Hp, value);

        }
        public int TargetHpMax => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.HpMax);
        public int TargetFp
        {
            get => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.Fp);
            set => _targetEnemyData.WriteInt32((int)Offsets.EnemyData.Fp, value);
        }
        public int TargetFpMax => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.FpMax);
        public int TargetStam
        {
            get => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.Stam);
            set => _targetEnemyData.WriteInt32((int)Offsets.EnemyData.Stam, value);

        }
        public int TargetStamMax => _targetEnemyData.ReadInt32((int)Offsets.EnemyData.StamMax);
        public int TargetPoison => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Poison);
        public int TargetPoisonMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.PoisonMax);
        public int TargetRot => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Rot);
        public int TargetRotMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.RotMax);
        public int TargetBleed => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Bleed);
        public int TargetBleedMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.BleedMax);
        public int TargetFrost => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Frost);
        public int TargetFrostMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.FrostMax);
        public int TargetBlight => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Blight);
        public int TargetBlightMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.BlightMax);
        public int TargetSleep => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Sleep);
        public int TargetSleepMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.SleepMax);
        public int TargetMadness => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.Madness);
        public int TargetMadnessMax => _targetEnemyResistance.ReadInt32((int)Offsets.ResistenceData.MadnessMax);
        public float TargetStagger => _targetEnemyStagger.ReadSingle((int)Offsets.StaggerData.Stagger);
        public float TargetStaggerMax => _targetEnemyStagger.ReadSingle((int)Offsets.StaggerData.StaggerMax);
        public float TargetResetTime => _targetEnemyStagger.ReadSingle((int)Offsets.StaggerData.ResetTime);

    }
}
