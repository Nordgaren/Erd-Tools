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
        }
        private PHPointer? _targetEnemyIns { get; set; }
        private PHPointer? TargetEnemyIns
        {
            get => _targetEnemyIns;
            set
            {
                _targetEnemyIns = value;
                TargetEnemyModuleBase = _targetEnemyIns != null ? _hook.CreateChildPointer(_targetEnemyIns, (int)EROffsets.EnemyIns.ModuleBase) : null;
                TargetEnemyData = _targetEnemyIns != null ? _hook.CreateChildPointer(TargetEnemyModuleBase, (int)EROffsets.ModuleBase.EnemyData) : null;
                TargetEnemyResistance = _targetEnemyIns != null ? _hook.CreateChildPointer(TargetEnemyModuleBase, (int)EROffsets.ModuleBase.ResistenceData) : null;
                TargetEnemyStagger = _targetEnemyIns != null ? _hook.CreateChildPointer(TargetEnemyModuleBase, (int)EROffsets.ModuleBase.StaggerData) : null;
            }
        }
        public string TargetEnemyInsPtr => _targetEnemyIns?.Resolve().ToString("X2") ?? "";

        private PHPointer? TargetEnemyModuleBase;
        private PHPointer? TargetEnemyData;
        private PHPointer? TargetEnemyResistance;
        private PHPointer? TargetEnemyStagger;

        public int TargetHandle => _targetEnemyIns?.ReadInt32((int)EROffsets.EnemyIns.EnemyHandle) ?? 0;
        public string TargetModel => TargetEnemyData?.ReadString((int)EROffsets.EnemyData.Model, Encoding.Unicode, 0x10) ?? "No Target";
        public string TargetName => TargetEnemyData?.ReadString((int)EROffsets.EnemyData.Name, Encoding.Unicode, 0x28) ?? "No Target";

        public int GetTargetChrType() => _targetEnemyIns?.ReadInt32((int)EROffsets.EnemyIns.ChrType) ?? 0;
        public void SetTargetChrType(int val) => _targetEnemyIns?.WriteInt32((int)EROffsets.EnemyIns.ChrType, val);

        public int GetTargetHp() => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.Hp) ?? 0;
        public void SetTargetHp(int val) => TargetEnemyData?.WriteInt32((int)EROffsets.EnemyData.Hp, val);
        public int GetTargetHpMax() => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.HpMax) ?? 0;
        public void SetTargetHpMax(int val) => TargetEnemyData?.WriteInt32((int)EROffsets.EnemyData.HpMax, val);

        public int GetTargetFp() => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.Fp) ?? 0;
        public void SetTargetFp(int val) => TargetEnemyData?.WriteInt32((int)EROffsets.EnemyData.Fp, val);

        public int GetTargetFpMax() => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.FpMax) ?? 0;
        public void SetTargetFpMax(int val) => TargetEnemyData?.WriteInt32((int)EROffsets.EnemyData.FpMax, val);

        public int GetTargetStam() => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.Stam) ?? 0;
        public void SetTargetStam(int val) => TargetEnemyData?.WriteInt32((int)EROffsets.EnemyData.Stam, val);

        public int GetTargetStamMax() => TargetEnemyData?.ReadInt32((int)EROffsets.EnemyData.StamMax) ?? 0;
        public void SetTargetStamMax(int val) => TargetEnemyData?.WriteInt32((int)EROffsets.EnemyData.StamMax, val);

        public int GetTargetPoison() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Poison) ?? 0;
        public void SetTargetPoison(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.Poison, val);

        public int GetTargetPoisonMax() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.PoisonMax) ?? 0;
        public void SetTargetPoisonMax(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.PoisonMax, val);

        public int GetTargetRot() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Rot) ?? 0;
        public void SetTargetRot(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.Rot, val);

        public int GetTargetRotMax() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.RotMax) ?? 0;
        public void SetTargetRotMax(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.RotMax, val);

        public int GetTargetBleed() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Bleed) ?? 0;
        public void SetTargetBleed(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.Bleed, val);

        public int GetTargetBleedMax() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.BleedMax) ?? 0;
        public void SetTargetBleedMax(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.BleedMax, val);

        public int GetTargetFrost() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Frost) ?? 0;
        public void SetTargetFrost(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.Frost, val);

        public int GetTargetFrostMax() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.FrostMax) ?? 0;
        public void SetTargetFrostMax(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.FrostMax, val);

        public int GetTargetBlight() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Blight) ?? 0;
        public void SetTargetBlight(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.Blight, val);

        public int GetTargetBlightMax() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.BlightMax) ?? 0;
        public void SetTargetBlightMax(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.BlightMax, val);

        public int GetTargetSleep() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Sleep) ?? 0;
        public void SetTargetSleep(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.Sleep, val);

        public int GetTargetSleepMax() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.SleepMax) ?? 0;
        public void SetTargetSleepMax(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.SleepMax, val);

        public int GetTargetMadness() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.Madness) ?? 0;
        public void SetTargetMadness(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.Madness, val);

        public int GetTargetMadnessMax() => TargetEnemyResistance?.ReadInt32((int)EROffsets.ResistenceData.MadnessMax) ?? 0;
        public void SetTargetMadnessMax(int val) => TargetEnemyResistance?.WriteInt32((int)EROffsets.ResistenceData.MadnessMax, val);

        public float GetTargetStagger() => TargetEnemyStagger?.ReadSingle((int)EROffsets.StaggerData.Stagger) ?? 0;
        public void SetTargetStagger(float val) => TargetEnemyStagger?.WriteSingle((int)EROffsets.StaggerData.Stagger, val);

        public float GetTargetStaggerMax() => TargetEnemyStagger?.ReadSingle((int)EROffsets.StaggerData.StaggerMax) ?? 0;
        public void SetTargetStaggerMax(float val) => TargetEnemyStagger?.WriteSingle((int)EROffsets.StaggerData.StaggerMax, val);

        public float GetTargetResetTime() => TargetEnemyStagger?.ReadSingle((int)EROffsets.StaggerData.ResetTime) ?? 0;
        public void SetTargetResetTime(float val) => TargetEnemyStagger?.WriteSingle((int)EROffsets.StaggerData.ResetTime, val);

    }
}
