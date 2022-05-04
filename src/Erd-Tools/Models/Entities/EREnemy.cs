using PropertyHook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Erd_Tools
{
    public class EREnemy
    {
        private ERHook _hook;
        public EREnemy(PHPointer enemyIns, ERHook hook)
        {
            _hook = hook;
            TargetEnemyIns = enemyIns;
            _targetEnemyModuleBase = _hook.CreateChildPointer(_targetEnemyIns, (int)EROffsets.EnemyIns.ModuleBase);
            _targetEnemyData = _hook.CreateChildPointer(_targetEnemyModuleBase, (int)EROffsets.ModuleBase.EnemyData);
            _targetEnemyResistance = _hook.CreateChildPointer(_targetEnemyModuleBase, (int)EROffsets.ModuleBase.ResistenceData);
            _targetEnemyStagger =_hook.CreateChildPointer(_targetEnemyModuleBase, (int)EROffsets.ModuleBase.StaggerData);
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

        public int TargetHandle => _targetEnemyIns.ReadInt32((int)EROffsets.EnemyIns.EnemyHandle);
        public string TargetModel => _targetEnemyData.ReadString((int)EROffsets.EnemyData.Model, Encoding.Unicode, 0x10);
        public string TargetName => _targetEnemyData.ReadString((int)EROffsets.EnemyData.Name, Encoding.Unicode, 0x28);

        public int TargetChrType
        {
            get => _targetEnemyIns.ReadInt32((int)EROffsets.EnemyIns.ChrType);
            set => _targetEnemyIns.WriteInt32((int)EROffsets.EnemyIns.ChrType, value);

        }
        public int TargetHp
        {
            get => _targetEnemyData.ReadInt32((int)EROffsets.EnemyData.Hp);
            set => _targetEnemyData.WriteInt32((int)EROffsets.EnemyData.Hp, value);

        }
        public int TargetHpMax => _targetEnemyData.ReadInt32((int)EROffsets.EnemyData.HpMax);
        public int TargetFp
        {
            get => _targetEnemyData.ReadInt32((int)EROffsets.EnemyData.Fp);
            set => _targetEnemyData.WriteInt32((int)EROffsets.EnemyData.Fp, value);
        }
        public int TargetFpMax => _targetEnemyData.ReadInt32((int)EROffsets.EnemyData.FpMax);
        public int TargetStam
        {
            get => _targetEnemyData.ReadInt32((int)EROffsets.EnemyData.Stam);
            set => _targetEnemyData.WriteInt32((int)EROffsets.EnemyData.Stam, value);

        }
        public int TargetStamMax => _targetEnemyData.ReadInt32((int)EROffsets.EnemyData.StamMax);
        public int TargetPoison => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.Poison);
        public int TargetPoisonMax => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.PoisonMax);
        public int TargetRot => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.Rot);
        public int TargetRotMax => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.RotMax);
        public int TargetBleed => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.Bleed);
        public int TargetBleedMax => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.BleedMax);
        public int TargetFrost => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.Frost);
        public int TargetFrostMax => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.FrostMax);
        public int TargetBlight => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.Blight);
        public int TargetBlightMax => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.BlightMax);
        public int TargetSleep => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.Sleep);
        public int TargetSleepMax => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.SleepMax);
        public int TargetMadness => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.Madness);
        public int TargetMadnessMax => _targetEnemyResistance.ReadInt32((int)EROffsets.ResistenceData.MadnessMax);
        public float TargetStagger => _targetEnemyStagger.ReadSingle((int)EROffsets.StaggerData.Stagger);
        public float TargetStaggerMax => _targetEnemyStagger.ReadSingle((int)EROffsets.StaggerData.StaggerMax);
        public float TargetResetTime => _targetEnemyStagger.ReadSingle((int)EROffsets.StaggerData.ResetTime);

    }
}
