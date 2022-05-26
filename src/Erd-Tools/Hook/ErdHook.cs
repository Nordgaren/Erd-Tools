using Keystone;
using PropertyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Category = Erd_Tools.Models.Item.Category;
using static SoulsFormats.PARAMDEF;
using System.Collections;
using System.Text.RegularExpressions;
using SoulsFormats;
using System.Threading.Tasks;
using System.Threading;
using Erd_Tools.Models;
using Erd_Tools.Utils;

namespace Erd_Tools
{
    public class ErdHook : PHook, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public event EventHandler<PHEventArgs>? OnSetup;
        private void RaiseOnSetup()
        {
            OnSetup?.Invoke(this, new PHEventArgs(this));
        }

        private PHPointer GameDataMan { get; set; }
        private PHPointer PlayerGameData { get; set; }
        private PHPointer PlayerInventory { get; set; }
        private PHPointer SoloParamRepository { get; set; }
        private PHPointer CapParamCall { get; set; }
        public PHPointer ItemGive { get; set; }
        public PHPointer MapItemMan { get; set; }
        public PHPointer EventFlagMan { get; set; }
        public PHPointer SetEventFlagFunction { get; set; }
        public PHPointer WorldChrMan { get; set; }
        public PHPointer PlayerIns { get; set; }
        public PHPointer DisableOpenMap { get; set; }
        public PHPointer CombatCloseMap { get; set; }
        public PHPointer WorldAreaWeather { get; set; }
        public static bool Reading { get; set; }
        public string ID => Process?.Id.ToString() ?? "Not Hooked";
        public List<PHPointer>? ParamPointers { get; set; }
        //private PHPointer DurabilityAddr { get; set; }
        //private PHPointer DurabilitySpecialAddr { get; set; }
        public bool Loaded => PlayerIns != null ? PlayerIns.Resolve() != IntPtr.Zero : false;
        public bool Setup = false;
        public bool Focused => Hooked && User32.GetForegroundProcessID() == Process.Id;

        public ErdHook(int refreshInterval, int minLifetime, Func<Process, bool> processSelector)
            : base(refreshInterval, minLifetime, processSelector)
        {
            OnHooked += ErdHook_OnHooked;
            OnUnhooked += ErdHook_OnUnhooked;

            GameDataMan = RegisterRelativeAOB(Offsets.GameDataManAoB, Offsets.RelativePtrAddressOffset, Offsets.RelativePtrInstructionSize, 0x0);
            PlayerGameData = CreateChildPointer(GameDataMan, Offsets.PlayerGameData);
            PlayerInventory = CreateChildPointer(PlayerGameData, Offsets.EquipInventoryDataOffset, Offsets.PlayerInventoryOffset);

            SoloParamRepository = RegisterRelativeAOB(Offsets.SoloParamRepositoryAoB, Offsets.RelativePtrAddressOffset, Offsets.RelativePtrInstructionSize, 0x0);

            ItemGive = RegisterAbsoluteAOB(Offsets.ItemGiveAoB);
            MapItemMan = RegisterRelativeAOB(Offsets.MapItemManAoB, Offsets.RelativePtrAddressOffset, Offsets.RelativePtrInstructionSize);
            EventFlagMan = RegisterRelativeAOB(Offsets.EventFlagManAoB, Offsets.RelativePtrAddressOffset, Offsets.RelativePtrInstructionSize, 0x0);
            SetEventFlagFunction = RegisterAbsoluteAOB(Offsets.EventCallAoB);

            CapParamCall = RegisterAbsoluteAOB(Offsets.CapParamCallAoB);

            WorldChrMan = RegisterRelativeAOB(Offsets.WorldChrManAoB, Offsets.RelativePtrAddressOffset, Offsets.RelativePtrInstructionSize, 0x0);
            PlayerIns = CreateChildPointer(WorldChrMan, Offsets.PlayerInsOffset);

            DisableOpenMap = RegisterAbsoluteAOB(Offsets.DisableOpenMapAoB);
            CombatCloseMap = RegisterAbsoluteAOB(Offsets.CombatCloseMapAoB);
            WorldAreaWeather = RegisterRelativeAOB(Offsets.WorldAreaWeatherAoB, Offsets.RelativePtrAddressOffset, Offsets.RelativePtrInstructionSize, 0x0);

            ItemEventDictionary = BuildItemEventDictionary();
            ItemCategory.GetItemCategories();

        }

        private void ErdHook_OnUnhooked(object? sender, PHEventArgs e)
        {
            Setup = false;
        }

        private void ErdHook_OnHooked(object? sender, PHEventArgs e)
        {
            //IntPtr gameDataMan = GameDataMan.Resolve();
            //IntPtr paramss = SoloParamRepository.Resolve();
            //IntPtr itemGive = ItemGive.Resolve();
            //IntPtr mapItemMan = MapItemMan.Resolve();
            //IntPtr eventFlagMan = EventFlagMan.Resolve();
            //IntPtr capParamCall = CapParamCall.Resolve();
            //IntPtr worldChrMan = WorldChrMan.Resolve();

            //IntPtr disableOpenMap = DisableOpenMap.Resolve();
            //IntPtr combatCloseMap = CombatCloseMap.Resolve();

            Task t = Task.Run(() => AsyncSetup());
            t.GetAwaiter().GetResult();
            GetInventoryList();
            //LogABunchOfStuff();
        }

        private async Task AsyncSetup()
        {
            Params = GetParams();
            await ReadParams();
            Setup = true;
            RaiseOnSetup();
        }

        private void LogABunchOfStuff()
        {
            List<string> list = new List<string>();
            list.Add($"WorldChrMan {WorldChrMan.Resolve():X2}");
            list.Add($"ItemGib {ItemGive.Resolve():X2}");
            list.Add($"GameDataMan {GameDataMan.Resolve():X2}");
            list.Add($"SoloParamRepository {SoloParamRepository.Resolve():X2}");
            File.WriteAllLines(Environment.CurrentDirectory + @"\HookLog.txt", list);
        }

        public Param? EquipParamAccessory;
        public Param? EquipParamGem;
        public Param? EquipParamGoods;
        public Param? EquipParamProtector;
        public Param? EquipParamWeapon;
        public Param? MagicParam;
        public Param? NpcParam;

        private Engine Engine = new Engine(Architecture.X86, Mode.X64);
        //TKCode
        private void AsmExecute(string asm)
        {
            //Assemble once to get the size
            EncodedData? bytes = Engine.Assemble(asm, (ulong)Process.MainModule.BaseAddress);
            //DebugPrintArray(bytes.Buffer);
            KeystoneError error = Engine.GetLastKeystoneError();
            if (error != KeystoneError.KS_ERR_OK)
                throw new Exception("Something went wrong during assembly. Code could not be assembled.");

            IntPtr insertPtr = GetPrefferedIntPtr(bytes.Buffer.Length, Kernel32.PAGE_EXECUTE_READWRITE);

            //Reassemble with the location of the isertPtr to support relative instructions
            bytes = Engine.Assemble(asm, (ulong)insertPtr);
            error = Engine.GetLastKeystoneError();

            Kernel32.WriteBytes(Handle, insertPtr, bytes.Buffer);
#if DEBUG
            DebugPrintArray(bytes.Buffer);
#endif

            Execute(insertPtr);
            Free(insertPtr);
        }

#if DEBUG
        private static void DebugPrintArray(byte[] bytes)
        {
            Debug.WriteLine("");
            foreach (byte b in bytes)
            {
                Debug.Write($"{b.ToString("X2")} ");
            }
            Debug.WriteLine("");
        }
#endif

        #region Params

        public List<Param> Params;

        private List<Param> GetParams()
        {
            List<Param> paramList = new List<Param>();
            string paramPath = $"{Util.ExeDir}/Resources/Params/";

            string pointerPath = $"{paramPath}/Pointers/";
            string[] paramPointers = Directory.GetFiles(pointerPath, "*.txt");
            foreach (string path in paramPointers)
            {
                string[] pointers = File.ReadAllLines(path);
                AddParam(paramList, paramPath, path, pointers);
            }

            return paramList;
        }

        public void AddParam(List<Param> paramList, string paramPath, string path, string[] pointers)
        {
            foreach (string entry in pointers)
            {
                if (!Util.IsValidTxtResource(entry))
                    continue;

                string[] info = entry.TrimComment().Split(':');
                string name = info[1];
                string defName = info.Length > 2 ? info[2] : name;

                string defPath = $"{paramPath}/Defs/{defName}.xml";
                if (!File.Exists(defPath))
                    throw new Exception($"The PARAMDEF {defName} does not exist for {entry}. If the PARAMDEF is named differently than the param name, add another \":\" and append the PARAMDEF name" +
                        $"Example: 3130:WwiseValueToStrParam_BgmBossChrIdConv:WwiseValueToStrConvertParamFormat");

                int offset = int.Parse(info[0], System.Globalization.NumberStyles.HexNumber);

                PHPointer pointer = GetParamPointer(offset);

                PARAMDEF paramDef = XmlDeserialize(defPath);

                Param param = new Param(pointer, offset, paramDef, name);

                SetParamPtrs(param);

                paramList.Add(param);
            }
            paramList.Sort();
        }

        private void SetParamPtrs(Param param)
        {
            switch (param.Name)
            {
                case "EquipParamAccessory":
                    EquipParamAccessory = param;
                    break;
                case "EquipParamGem":
                    EquipParamGem = param;
                    break;
                case "EquipParamGoods":
                    EquipParamGoods = param;
                    break;
                case "EquipParamProtector":
                    EquipParamProtector = param;
                    break;
                case "EquipParamWeapon":
                    EquipParamWeapon = param;
                    break;
                case "Magic":
                    MagicParam = param;
                    break;
                case "NpcParam":
                    NpcParam = param;
                    break;
                default:
                    break;
            }
        }

        internal PHPointer GetParamPointer(int offset)
        {
            return CreateChildPointer(SoloParamRepository, new int[] { offset, 0x80, 0x80 });
        }
        public void SaveParam(Param param)
        {
            string asmString = Util.GetEmbededResource("Assembly.SaveParams.asm");
            string asm = string.Format(asmString, SoloParamRepository.Resolve(), param.Offset, CapParamCall.Resolve());
            AsmExecute(asm);
        }
        public void RestoreParams()
        {
            if (!Setup)
                return;

            EquipParamWeapon.RestoreParam();
            EquipParamGem.RestoreParam();
        }
        private async Task ReadParams()
        {
            List<Task> tasks = new List<Task>();

            foreach (ItemCategory category in ItemCategory.All)
            {
                tasks.Add(Task.Run(() => SetupItems(category)));
            }

            Task setupItems = Task.WhenAll(tasks);
            await setupItems;
            if (setupItems.Exception != null)
                throw setupItems.Exception;

            tasks.Clear();

            foreach (ItemCategory category in ItemCategory.All)
            {
                if (category.Category == Category.Weapons)
                    tasks.Add(Task.Run(() => SetupGems(category)));
            }

            Task setupGems = Task.WhenAll(tasks);
            await setupGems;
            if (setupGems.Exception != null)
                throw setupGems.Exception;

            tasks.Clear();
        }

        #endregion
        /// <summary>
        /// Sets the event flag in game by calling the "Set Event Flag" function.
        /// </summary>
        /// <param name="flag"></param>
        public void SetEventFlag(int flag)
        {
            IntPtr idPointer = GetPrefferedIntPtr(sizeof(int));
            Kernel32.WriteInt32(Handle, idPointer, flag);

            string asmString = Util.GetEmbededResource("Assembly.SetEventFlag.asm");
            string asm = string.Format(asmString, EventFlagMan.Resolve(), idPointer.ToString("X2"), SetEventFlagFunction.Resolve());
            AsmExecute(asm);
            Free(idPointer);
        }

        #region Inventory

        private static Regex ItemEventEntryRx = new Regex(@"^(?<event>\S+) (?<item>\S+)$", RegexOptions.CultureInvariant);

        private static Dictionary<int, int> ItemEventDictionary;

        private Dictionary<int, int> BuildItemEventDictionary()
        {
            Dictionary<int, int> itemEventDictionary = new Dictionary<int, int>();
            string[] goodsEvents = Util.GetListResource("Resources/Events/GoodsEvents.txt");
            foreach (string line in goodsEvents)
            {
                if (!Util.IsValidTxtResource(line))
                    continue;

                Match itemEntry = ItemEventEntryRx.Match(line.TrimComment());
                int eventID = Convert.ToInt32(itemEntry.Groups["event"].Value);
                int itemID = Convert.ToInt32(itemEntry.Groups["item"].Value);
                itemEventDictionary.Add(itemID + (int)Category.Goods, eventID);
            }

            return itemEventDictionary;
        }

        private void SetupGems(ItemCategory category)
        {
            foreach (Weapon weapon in category.Items)
            {
                Gem? gem = Gem.All.FirstOrDefault(gem => gem.SwordArtID == weapon.SwordArtId);
                if (gem != null)
                    weapon.DefaultGem = gem;
            }
        }

        private void SetupItems(ItemCategory category)
        {
            foreach (Item item in category.Items)
            {
                SetupItem(item);
                int fullID = item.ID + (int)Category.Goods;
                item.EventID = ItemEventDictionary.ContainsKey(fullID) ? ItemEventDictionary[fullID] : -1;
            }
        }

        private void SetupItem(Item item)
        {
            switch (item.ItemCategory)
            {
                case Category.Weapons:
                    item.SetupItem(EquipParamWeapon);
                    break;
                case Category.Protector:
                    item.SetupItem(EquipParamProtector);
                    break;
                case Category.Accessory:
                    item.SetupItem(EquipParamAccessory);
                    break;
                case Category.Goods:
                    item.SetupItem(EquipParamGoods);
                    break;
                case Category.Gem:
                    item.SetupItem(EquipParamGem);
                    break;
                default:
                    break;
            }
        }

        public void GetItem(int id, int quantity, int infusion, int upgrade, int gem)
        {
            byte[]   itemInfobytes = new byte[0x34];
            IntPtr itemInfo = GetPrefferedIntPtr(0x34);

            byte[] bytes = BitConverter.GetBytes(0x1);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)Offsets.ItemGiveStruct.Count, bytes.Length);

            bytes = BitConverter.GetBytes(id + infusion + upgrade);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)Offsets.ItemGiveStruct.ID, bytes.Length);

            bytes = BitConverter.GetBytes(quantity);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)Offsets.ItemGiveStruct.Quantity, bytes.Length);

            bytes = BitConverter.GetBytes(gem);
            Array.Copy(bytes, 0x0, itemInfobytes, (int)Offsets.ItemGiveStruct.Gem, bytes.Length);

            Kernel32.WriteBytes(Handle, itemInfo, itemInfobytes);

            string asmString = Util.GetEmbededResource("Assembly.ItemGib.asm");
            string asm = string.Format(asmString, itemInfo.ToString("X2"), MapItemMan.Resolve(), ItemGive.Resolve() + Offsets.ItemGiveOffset);
            AsmExecute(asm);
            Free(itemInfo);
        }

        List<InventoryEntry>? Inventory;
        public int InventoryEntries => PlayerGameData.ReadInt32((int)Offsets.PlayerGameDataStruct.InventoryCount);
        //public int LastInventoryCount => GetInventoryCount();

        public IEnumerable GetInventory()
        {
            Inventory = GetInventoryList();
            return Inventory;
        }
        private List<InventoryEntry> GetInventoryList()
        {
            List<InventoryEntry> inventory = new List<InventoryEntry>();

            byte[] bytes = PlayerInventory.ReadBytes(0x0, (uint)InventoryEntries * Offsets.PlayInventoryEntrySize);

            for (int i = 0; i < InventoryEntries; i++)
            {
                byte[] entry = new byte[Offsets.PlayInventoryEntrySize];
                Array.Copy(bytes, i * Offsets.PlayInventoryEntrySize, entry, 0, entry.Length);

                if (BitConverter.ToInt32(entry, (int)Offsets.InventoryEntry.ItemID) == -1)
                    continue;

                inventory.Add(new InventoryEntry(entry, this));
            }

            return inventory;
        }

        //private int GetInventoryCount()
        //{
        //    int count = 0;

        //    byte[] bytes = PlayerInventory.ReadBytes(0x0, (uint)InventoryEntries * Offsets.PlayInventoryEntrySize);

        //    for (int i = 0; i < InventoryEntries; i++)
        //    {
        //        byte[] entry = new byte[Offsets.PlayInventoryEntrySize];
        //        Array.Copy(bytes, i * Offsets.PlayInventoryEntrySize, entry, 0, entry.Length);

        //        if (BitConverter.ToInt32(entry, (int)Offsets.InventoryEntry.ItemID) == -1)
        //            continue;

        //        count++;
        //    }
        //    return count;
        //}

        public void ResetInventory()
        {
            Inventory = new List<InventoryEntry>();
        }
        #endregion

        #region Target  

        public enum PhantomParam
        {
            Normal = 0x00,
            Friendly = 0x01,
            Invader = 0x02,
            Phantom = 0x03,
            Original = 0x05,
            HostOfFingers = 0x08,
            InvaderNoText = 0x10,
            BrightWhitePhantomNoText = 0x14,
            BloodyFingerRedText = 0x15,
            RecusantText = 0x16,
            BlueHunterText = 0x17,
            BloodyFingerRedText2 = 0x18,
            OrangeGlowNoText = 0x19,
            InvalidInvader = 0x20,
            InvalidInvader1 = 0x21,
            InvalidInvader2 = 0x22,
        }

        private Enemy? LastTargetEnemy { get; set; }

        public int CurrentTargetHandle => PlayerIns?.ReadInt32((int)Offsets.PlayerIns.TargetHandle) ?? 0;
        public int CurrentTargetArea => PlayerIns?.ReadInt32((int)Offsets.PlayerIns.TargetArea) ?? 0;
        public void UpdateLastEnemy()
        {
            if (CurrentTargetHandle == -1 || CurrentTargetHandle == LastTargetEnemy?.Handle)
                return;

            GetTarget();
        }

        public Enemy GetTarget()
        {
            PHPointer worldBlockChr = CreateBasePointer(WorldChrMan.Resolve() + (int)Offsets.WorldChrMan.WorldBlockChr);
            int targetHandle = CurrentTargetHandle; //Only read from memory once
            int targetArea = CurrentTargetArea;

            while (true)
            {
                int numChrs = worldBlockChr.ReadInt32((int)Offsets.WorldBlockChr.NumChr);
                PHPointer chrSet = CreateChildPointer(worldBlockChr, (int)Offsets.WorldBlockChr.ChrSet);

                for (int j = 0; j <= numChrs; j++)
                {
                    PHPointer enemyIns = CreateChildPointer(chrSet, (j * (int)Offsets.ChrSet.EnemyIns));
                    int enemyHandle = enemyIns.ReadInt32((int)Offsets.EnemyIns.EnemyHandle);
                    int enemyArea = enemyIns.ReadInt32((int)Offsets.EnemyIns.EnemyArea);

                    if (targetHandle == enemyHandle && targetArea == enemyArea)
                        return new Enemy(enemyIns, this);

                }

                long assertVal = worldBlockChr.ReadInt64(0x80);
                if (assertVal == -1)
                    worldBlockChr = CreateBasePointer(worldBlockChr.Resolve() + 0x160);
                else
                    break;
            }

            Enemy lastTargetEnemy = TryGetEnemy(targetHandle, targetArea, (int)Offsets.WorldChrMan.ChrSet1);

            if (lastTargetEnemy != null)
                return lastTargetEnemy;

            return TryGetEnemy(targetHandle, targetArea, (int)Offsets.WorldChrMan.ChrSet2);

        }

        public Enemy TryGetEnemy(int targetHandle, int targetArea, int offset)
        {
            PHPointer chrSet1 = CreateChildPointer(WorldChrMan, offset);
            int numEntries1 = chrSet1.ReadInt32((int)Offsets.ChrSet.NumEntries);

            for (int i = 0; i <= numEntries1; i++)
            {
                int enemyHandle = chrSet1.ReadInt32(0x78 + (i * 0x10));
                int enemyArea = chrSet1.ReadInt32(0x78 + 4 + (i * 0x10));
                if (targetHandle == enemyHandle && targetArea == enemyArea)
                    return new Enemy(CreateChildPointer(chrSet1, 0x78 + 8 + (i * 0x10)), this);
            }

            return null;
        }

        #endregion

        public int Level => PlayerGameData.ReadInt32((int)Offsets.Player.Level);
        public string LevelString => PlayerGameData?.ReadInt32((int)Offsets.Player.Level).ToString() ?? "";

        #region Cheats

        byte[]? OriginalCombatCloseMap;

        public bool CombatMapEnabled { get; set; }

        public void ToggleMapCombat(bool enable)
        {
            if (enable)
                EnableMapInCombat();
            else
                DisableMapInCombat();
        }

        private void EnableMapInCombat()
        {
            OriginalCombatCloseMap = CombatCloseMap.ReadBytes(0x0, 0x5);
            byte[]? assembly = new byte[] { 0x48, 0x31, 0xC0, 0x90, 0x90 };

            DisableOpenMap.WriteByte(0x0, 0xEB); //Write Jump
            CombatCloseMap.WriteBytes(0x0, assembly);
            CombatMapEnabled = true;
        }

        private void DisableMapInCombat()
        {
            DisableOpenMap.WriteByte(0x0, 0x74); //Write Jump Equals
            CombatCloseMap.WriteBytes(0x0, OriginalCombatCloseMap); //Place original bytes back for combat close map
            CombatMapEnabled = false;
        }
        private short WeatherParamID => WorldAreaWeather?.ReadInt16((int)Offsets.WorldAreaWeather.WeatherParamID) ?? 0;
        private short ForceWeatherParamID 
        {
            set => WorldAreaWeather?.WriteInt16((int)Offsets.WorldAreaWeather.ForceWeatherParamID, value); 
        }

        public enum WeatherTypes
        {
            [Description("Slightly Cloudy")]
            SlightlyCloudy = 0,
            Sunny = 1,
            Overcast = 10,
            [Description("Storm Clouds")]
            StormClouds = 11,
            Rain = 20,
            [Description("Heavy Rain")]
            HeavyRain = 21,
            Downpour = 30,
            Fog = 31,
            [Description("Light Snow")]
            LightSnow = 40,
            Snow = 41,
            [Description("Freezing Fog")]
            FreezingFog = 50,
            [Description("Deep Freezing Fog")]
            DeepFreezingFog = 51,
            [Description("Deep Freezing Rainy Fog")]
            DeepFreezingRainyFog = 52,
            Windy = 60,
            Blizzard = 81,
            [Description("Rain and Snow")]
            RainSnow = 82,
            Moonlight = 83,
            [Description("Light Fog")]
            ClearLightFog = 99,
            Weather1001 = 1001,
            Weather1010 = 1010,
            Weather1011 = 1011,
            Weather1020 = 1020,
            Weather1021 = 1021,
            Weather1040 = 1040,
            Weather1050 = 1050,
            Weather1051 = 1051,
            Weather1052 = 1052,
            Weather2010 = 2010,
            Weather2011 = 2011,
            Weather2020 = 2020,
            Weather2021 = 2021,
            Weather2110 = 2110,
            Weather2111 = 2111,
            Weather3010 = 3010,
            Weather3011 = 3011,
            Weather3101 = 3101,
            Weather3110 = 3110,
            Weather3111 = 3111,
            Weather3120 = 3120,
            Weather4000 = 4000,
            Weather4010 = 4010,
            Weather4011 = 4011,
            Weather4040 = 4040,
            Weather4110 = 4110,
            Weather4111 = 4111,
            Weather4140 = 4140,
            Weather4201 = 4201,
            Weather4210 = 4210,
            Weather4211 = 4211,
            Weather4220 = 4220,
            Weather4221 = 4221,
            Weather4230 = 4230,
            Weather4231 = 4231,
            Weather4240 = 4240,
            Weather4241 = 4241,
            Weather4250 = 4250,
            Weather4251 = 4251,
            Weather4252 = 4252,
            Weather4260 = 4260,
        }
        private WeatherTypes _selectedWeather;
        //public WeatherTypes SelectedWeather {
        //    get => _selectedWeather;
        //    set 
        //    { 
        //        _selectedWeather = value;
        //        if (_forceWeather)
        //            ForceWeatherParamID = (short)_selectedWeather;
        //    }
        //}
        private WeatherTypes _lastSelectedWeather;

        private bool _forceWeather { get; set; }
        //public bool ForceWeather
        //{
        //    get { return _forceWeather; }
        //    set 
        //    {
        //        ForceWeatherParamID = (short)_selectedWeather;
        //        _forceWeather = value;

        //        if (_forceWeather)
        //            _lastSelectedWeather = (WeatherTypes)WeatherParamID;
        //        else
        //            ForceWeatherParamID = (short)_lastSelectedWeather;

        //    }
        //}

        public void SetForcedWeatherValue(WeatherTypes weather)
        {
            _selectedWeather = weather;
        }

        public void ToggleForceWeather(bool enable)
        {
            if (enable)
                _lastSelectedWeather = (WeatherTypes)WeatherParamID;
            else
                ForceWeatherParamID = (short)_lastSelectedWeather;
        }

        public void ForceWeather()
        {
            ForceWeatherParamID = (short)_selectedWeather;
        }

        public void ForceSetWeather()
        {
            ForceWeatherParamID = (short)_selectedWeather;
        }

        #endregion

        #region ChrAsm
        public byte ArmStyle
        {
            get => PlayerGameData.ReadByte((int)Offsets.ChrIns.ArmStyle);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteByte((int)Offsets.ChrIns.ArmStyle, value);
            }
        }
        public int CurrWepSlotOffsetLeft
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.CurrWepSlotOffsetLeft);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.CurrWepSlotOffsetLeft, value);
            }
        }
        public int CurrWepSlotOffsetRight
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.CurrWepSlotOffsetRight);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.CurrWepSlotOffsetRight, value);
            }
        }
        public int RHandWeapon1
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.RHandWeapon1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.RHandWeapon1, value);
            }
        }
        public int RHandWeapon2
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.RHandWeapon2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.RHandWeapon2, value);
            }
        }
        public int RHandWeapon3
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.RHandWeapon3);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.RHandWeapon3, value);
            }
        }
        public int LHandWeapon1
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.LHandWeapon1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.LHandWeapon1, value);
            }
        }
        public int LHandWeapon2
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.LHandWeapon2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.LHandWeapon2, value);
            }
        }
        public int LHandWeapon3
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.LHandWeapon3);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.LHandWeapon3, value);
            }
        }
        public int Arrow1
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.Arrow1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.Arrow1, value);
            }
        }
        public int Arrow2
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.Arrow2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.Arrow2, value);
            }
        }
        public int Bolt1
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.Bolt1);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.Bolt1, value);
            }
        }
        public int Bolt2
        {
            get => PlayerGameData.ReadInt32((int)Offsets.ChrIns.Bolt2);
            set
            {
                if (!Loaded)
                    return;
                PlayerGameData.WriteInt32((int)Offsets.ChrIns.Bolt2, value);
            }
        }
        #endregion

        private int OGRHandWeapon1 { get; set; }
        private PHPointer OGRHandWeapon1Param
        {
            get => CreateBasePointer(EquipParamWeapon.Pointer.Resolve() + EquipParamWeapon.OffsetDict[OGRHandWeapon1]);
        }
        private int OGRHandWeapon1SwordArtID
        {
            get => OGRHandWeapon1Param?.ReadInt32((int)Offsets.EquipParamWeapon.SwordArtsParamId) ?? 0;
            set => OGRHandWeapon1Param?.WriteInt32((int)Offsets.EquipParamWeapon.SwordArtsParamId, value);
        }
        private int OGLHandWeapon1 { get; set; }
        private PHPointer OGLHandWeapon1Param
        {
            get => CreateBasePointer(EquipParamWeapon.Pointer.Resolve() + EquipParamWeapon.OffsetDict[OGLHandWeapon1]);
        }
        private int OGLHandWeapon1SwordArtID
        {
            get => OGLHandWeapon1Param?.ReadInt32((int)Offsets.EquipParamWeapon.SwordArtsParamId) ?? 0;
            set => OGLHandWeapon1Param?.WriteInt32((int)Offsets.EquipParamWeapon.SwordArtsParamId, value);
        }

    }
}
