using Erd_Tools.Utils;
using System;
using System.Linq;

namespace Erd_Tools.Models
{
    public class Weapon : Item
    {
        public enum Infusion : short
        {
            Standard = 000,
            Heavy = 100,
            Keen = 200,
            Quality = 300,
            Fire = 400,
            FlameArt = 500,
            Lightning = 600,
            Sacred = 700,
            Magic = 800,
            Cold = 900,
            Posion = 1000,
            Blood = 1100,
            Occult = 1200,
        };

        public enum WeaponType
        {
            Dagger = 1, //Dagger
            StraightSword = 3, //SwordNormal
            Greatsword = 5, //SwordLarge
            ColossalSword = 7, //SwordGigantic
            CurvedSword = 9, //SabreNormal
            CurvedGreatsword = 11, //SabreLarge
            Katana = 13, //katana
            Twinblade = 14, //SwordDoulbeEdge
            ThrustingSword = 15, //SwordPierce
            HeavyThrustingSword = 16, //RapierHeavy
            Axe = 17, //AxeNormal
            Greataxe = 19, //AxeLarge
            Hammer = 21, //HammerNormal
            GreatHammer = 23, //HammerLarge
            Flail = 24, //Flail
            Spear = 25, //SpearNormal
            SpearLarge = 26, //SpearLarge UNUSED
            GreatSpear = 28, //SpearHeavy
            Halberd = 29, //SpearAxe
            Reaper = 31, //Sickle
            Fist = 35, //Knuckle
            Claws = 37, //Claw
            Whip = 39, //Whip
            ColossalWeapon = 41, //Axhammerlarge
            LightBow = 50, //BowSmall
            Bow = 51, //BowNormal
            Greatbow = 53, //BowLarge
            Crossbow = 55, //Clossbow
            Ballista = 56, //Ballista
            GlintstoneStaff = 57, //Staff
            Sorcery = 58, //Sorcery UNUSED
            FingerSeal = 61, //Talisman
            SmallShield = 65, //ShieldSmall
            MediumShield = 67, //ShieldNormal
            Greatshield = 69, //SheildLarge
            Torch = 87 //Torch
        }

        public enum AmmoType
        {
            Unarmed = 33,
            Arrow = 81,
            GreatArrow = 83,
            Bolt = 85,
            BallistaBolt = 86
        }
        public int RealID { get; set; }
        public short IconID { get; set; }
        public bool Unique { get; set; }
        public int SwordArtId { get; set; }
        public bool Infusible { get; set; }
        public int MaxUpgrade { get; set; }
        public WeaponType Type { get; set; }
        public AmmoType TypeAmmo { get; set; }
        public Gem DefaultGem { get; set; }

        public Weapon(Weapon source) : base(source) {
            RealID = source.RealID;
            Infusible = source.Infusible;
            IconID = source.IconID;
            Unique = source.Unique;
            SwordArtId = source.SwordArtId;
            Type = source.Type;
        }
        public Weapon(string config, Category category, bool showIDs) : base(config, category, showIDs)
        {
            RealID = Util.DeleteFromEnd(ID, 4);
            DefaultGem = Gem.All.FirstOrDefault(g => g.ID == -1);
        }

        public override void SetupItem(Param param)
        {
            MaxQuantity = param.Bytes[param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.MaxArrowQuantity];

            byte bitfield = param.Bytes[param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.DisableMultiDropShare];
            IsMultiplayerShare = (bitfield & (1 << 0)) == 0;
            IsDrop = (bitfield & (1 << 2)) != 0;
            Infusible = (bitfield & (1 << 7)) == 0;

            if (!param.OffsetDict.ContainsKey(ID))
                return;
                //throw new Exception($"No offset present for {Name}");

            Unique = BitConverter.ToInt32(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.MaterialSetID) == 2200;

            SwordArtId = BitConverter.ToInt32(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.SwordArtsParamId);
            

            Type = (WeaponType)BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.WepType);
            TypeAmmo = (AmmoType)BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.WepType);


            int val = 0;
            for (int i = 1; i < 16 && val != -1; i++)
            {
                val = BitConverter.ToInt32(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.OriginEquipWep + (i * 4));
                if (val != -1)
                    MaxUpgrade++;
            }

            for (int i = 0; i < 10 && val != -1; i++)
            {
                val = BitConverter.ToInt32(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.OriginEquipWep16 + (i * 4));
                if (val != -1)
                    MaxUpgrade++;
            }
            
            IconID = BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamWeapon.IconID);
        }
        
    }
}
