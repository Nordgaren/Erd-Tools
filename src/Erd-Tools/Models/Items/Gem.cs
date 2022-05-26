using System;
using System.Collections.Generic;
using System.Linq;
using Infusion = Erd_Tools.Models.Weapon.Infusion;
using WeaponType = Erd_Tools.Models.Weapon.WeaponType;

namespace Erd_Tools.Models
{
    public class Gem : Item
    {
        public static List<Gem> All = new List<Gem>();

        public long CanMountBitfield;
        public int SwordArtID;
        public short WeaponAttr;
        public List<Infusion> Infusions;
        public List<WeaponType> WeaponTypes = new List<WeaponType>();

        private void GetInfusions()
        {
            Infusions = new List<Infusion>();

            if (WeaponAttr == 0)
                return;

            for (int i = 0; i < AllInfusions.Count; i++)
            {
                if ((WeaponAttr & (1 << i)) != 0)
                    Infusions.Add(AllInfusions[i]);
            }
        }

        private void GetWeapons()
        {
            WeaponTypes = new List<WeaponType>();

            if (CanMountBitfield == 0)
                return;

            for (int i = 0; i < Weapons.Count; i++)
            {
                if ((CanMountBitfield & (1L << i)) != 0)
                    WeaponTypes.Add(Weapons[i]);
            }
        }

        public override void SetupItem(Param param)
        {
            MaxQuantity = 1;

            if (ID == -1)
            {
                WeaponTypes = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList();
                Infusions = new List<Infusion>() { Infusion.Standard };
                return;
            }

            byte bitfield = param.Bytes[param.OffsetDict[ID] + (int)Offsets.EquipParamGem.IsDiscard];
            IsDrop = (bitfield & (1 << 1)) != 0;
            IsMultiplayerShare = (bitfield & (1 << 3)) == 0;

            SwordArtID = BitConverter.ToInt32(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamGem.SwordArtsParamId);
            CanMountBitfield = BitConverter.ToInt64(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamGem.CanMountWep_Dagger);
            WeaponAttr = BitConverter.ToInt16(param.Bytes, param.OffsetDict[ID] + (int)Offsets.EquipParamGem.ConfigurableWepAttr);
            GetWeapons();
            GetInfusions();
        }

        public Gem(string config, Category category, bool showIDs) : base(config, category, showIDs)
        {
            All.Add(this);
        }

        public static List<WeaponType> Weapons = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList();

        public static List<Infusion> AllInfusions = Enum.GetValues(typeof(Infusion)).Cast<Infusion>().ToList();

    }
}
