using Erd_Tools.Utils;
using Org.BouncyCastle.Math.Field;
using PropertyHook;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SoulsFormats.PARAMDEF;

namespace Erd_Tools.Models
{
    public class Param : IComparable<Param>
    {
        private readonly object _loadLock = new();
        private bool _isLoaded;

        public PHPointer Pointer { get; private set; }
        public int Offset { get; private set; }
        public PARAMDEF ParamDef { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public int Length { get; private set; }
        public byte[] Bytes { get; private set; }
        public List<Row> _rows { get; private set; }
        public List<Field> _fields { get; private set; }
        public Dictionary<int, string> _nameDictionary { get; private set; }
        public Dictionary<int, int> _offsetDict { get; private set; }
        public List<Row> Rows 
        { 
            get 
            { 
                EnsureLoaded(); 
                return _rows; 
            } 
        }

        public List<Field> Fields 
        { 
            get 
            { 
                EnsureLoaded(); 
                return _fields; 
            } 
        }

        public Dictionary<int, string> NameDictionary 
        { 
            get 
            { 
                EnsureLoaded(); 
                return _nameDictionary; 
            } 
        }

        public Dictionary<int, int> OffsetDict 
        { 
            get 
            { 
                EnsureLoaded(); 
                return _offsetDict; 
            } 
        }
        private static Regex _paramEntryRx { get; } = new(@"^\s*(?<id>\S+)\s+(?<name>.*)$", RegexOptions.CultureInvariant);
        public int RowLength { get; private set; }
        public Row? this[int rowId] => Rows.Find((r) => r.ID == rowId);
        private void EnsureLoaded()
        {
            if (_isLoaded) return;

            lock (_loadLock)
            {
                if (_isLoaded) return; 
        
                // Note: Since you do Length and Bytes in the constructor now, 
                // make sure you removed them from BuildOffsetDictionary!
                BuildNameDictionary(); 
                BuildOffsetDictionary();
                BuildCells();
        
                _isLoaded = true;
            }
        }
        public async Task LoadAsync()
        {
            if (_isLoaded) return;

            // We push the synchronous lock onto a background thread
            await Task.Run(() => 
            {
                EnsureLoaded(); 
            });
        }

        public Param(PHPointer pointer, int offset, PARAMDEF Paramdef, string name)
        {
            Pointer = pointer;
            Offset = offset;
            ParamDef = Paramdef;
            Name = name;
            Type = Paramdef.ParamType;
            RowLength = ParamDef.GetRowSize();
            Length = (Pointer.ReadInt32((int)Offsets.Param.ParamFileSize) + 0xF) & -0x10;
            Bytes = Pointer.ReadBytes(0x0, (uint)Length);
        }
        private void BuildOffsetDictionary()
        {
            _rows = new();
            _offsetDict = new();
            int typeNameOffset = Pointer.ReadInt32((int)Offsets.Param.NameOffset);
            
            string paramType = Pointer.ReadString(typeNameOffset, Encoding.UTF8, (uint)Type.Length);
            if (paramType != Type)
                throw new InvalidOperationException($"Incorrect Param Pointer: {paramType} should be {Type}");

            int tableLength = BitConverter.ToInt32(Bytes ,(int)Offsets.Param.TableLength);
            int param = 0x40;
            int paramID = 0x0;
            int paramOffset = 0x8;
            int nameOffset = 0x10;
            int paramSize = 0x18;

            while (param < tableLength)
            {
                int itemID = BitConverter.ToInt32(Bytes, param + paramID);
                int itemParamOffset = BitConverter.ToInt32(Bytes, param + paramOffset);
                string name = string.Empty;
             
                if (!_offsetDict.ContainsKey(itemID))
                    _offsetDict.Add(itemID, itemParamOffset);

                int runtimeOffset = BitConverter.ToInt32(Bytes, param + nameOffset);
                uint maxLen = (uint) (Length - runtimeOffset);
                name = Pointer.ReadString(runtimeOffset, Encoding.Unicode, maxLen);
                
                if (string.IsNullOrWhiteSpace(name) && _nameDictionary.ContainsKey(itemID))
                    name = _nameDictionary[itemID];

                _rows.Add(new(this ,name, itemID, itemParamOffset));

                param += paramSize;
            }
        }
        private void BuildNameDictionary()
        {
            _nameDictionary = new();
            string[] result = Util.GetListResource(@$"Resources/Params/Names/{Name}.txt");
            if (result.Length == 0)
                return;

            foreach (string line in result)
            {
                if (!Util.IsValidTxtResource(line)) //determine if line is a valid resource or not
                    continue;

                Match itemEntry = _paramEntryRx.Match(line);
                string name = itemEntry.Groups["name"].Value;//.Replace("\r", "");
                int id = Convert.ToInt32(itemEntry.Groups["id"].Value);
                if (_nameDictionary.ContainsKey(id))
                    continue;

                _nameDictionary.Add(id, name);
            };
        }
        public override string ToString()
        {
            return Name;
        }
        public int CompareTo(Param? otherParam)
        {
            if (otherParam == null)
                throw new ArgumentNullException(nameof(otherParam));

            return string.Compare(Name, otherParam.Name, StringComparison.Ordinal);
        }
        public void RestoreParam()
        {
            Pointer.WriteBytes(0 ,Bytes);
        }
        public class Row
        {
            public Param Param { get; private set; }
            public string Name { get; private set; }
            public int ID { get; private set; }
            public int DataOffset { get; private set; }
            public Field? this[string fieldName] => Param.Fields.Find((f) => f.InternalName == fieldName);

            public Row(Param param, string name, int id, int offset)
            {
                Param = param;
                Name = $"{id} - {name}";
                ID = id;
                DataOffset = offset;
            }
            public override string ToString()
            {
                return Name;
            }
        }

        private void BuildCells()
        {
            _fields = new();
            int totalSize = 0;
            for (int i = 0; i < ParamDef.Fields.Count; i++)
            {
                PARAMDEF.Field field = ParamDef.Fields[i];
                DefType type = field.DisplayType;
                int size = ParamUtil.IsArrayType(type) ? ParamUtil.GetValueSize(type) * field.ArrayLength : ParamUtil.GetValueSize(type);
                if (ParamUtil.IsArrayType(type))
                    totalSize += ParamUtil.GetValueSize(type) * field.ArrayLength;
                else
                    totalSize += ParamUtil.GetValueSize(type);

                if (ParamUtil.IsBitType(type) && field.BitSize != -1)
                {
                    int bitOffset = field.BitSize;
                    DefType bitType = type == DefType.dummy8 ? DefType.u8 : type;
                    int bitLimit = ParamUtil.GetBitLimit(bitType);
                    _fields.Add(GetBitField(field, totalSize, size, bitOffset));


                    for (; i < ParamDef.Fields.Count - 1; i++)
                    {
                        PARAMDEF.Field nextField = ParamDef.Fields[i + 1];
                        DefType nextType = nextField.DisplayType;
                        if (!ParamUtil.IsBitType(nextType) || nextField.BitSize == -1 || bitOffset + nextField.BitSize > bitLimit
                            || (nextType == DefType.dummy8 ? DefType.u8 : nextType) != bitType)
                            break;
                        bitOffset += nextField.BitSize;
                        _fields.Add(GetBitField(nextField, totalSize, size, bitOffset));
                    }
                    continue;
                }

                switch (field.DisplayType)
                {
                    case DefType.s8:
                    case DefType.s16:
                    case DefType.s32:
                        _fields.Add(new NumericField(field, totalSize - size, true));
                        break;
                    case DefType.u8:
                    case DefType.dummy8:
                    case DefType.u16:
                    case DefType.u32:
                        _fields.Add(new NumericField(field, totalSize - size, false));
                        break;
                    case DefType.f32:
                        _fields.Add(new SingleField(field, totalSize - size));
                        break;
                    case DefType.fixstr:
                        _fields.Add(new FixedStr(field, totalSize - size, Encoding.ASCII));
                        break;
                    case DefType.fixstrW:
                        _fields.Add(new FixedStr(field, totalSize - size, Encoding.Unicode));
                        break;
                    default:
                        throw new($"Unknown type: {field.DisplayType}");
                }
            }
        }
        private static BitField GetBitField(PARAMDEF.Field field, int totalSize, int size, int bitOffset)
        {

            switch (field.DisplayType)
            {
                case DefType.u8:
                case DefType.dummy8:
                    return field.BitSize > 1
                        ? new PartialByteField(field, totalSize - size, bitOffset - field.BitSize, field.BitSize)
                        : new BitField(field, totalSize - size, bitOffset - 1);
                case DefType.u16:
                    return field.BitSize > 1 
                        ? new PartialUShortField(field, totalSize - size, bitOffset - field.BitSize, field.BitSize) 
                        : new BitField(field, totalSize - size, bitOffset - 1);
                case DefType.u32:
                    return field.BitSize > 1
                        ? new PartialUIntField(field, totalSize - size, bitOffset - field.BitSize, field.BitSize)
                        : new BitField(field, totalSize - size, bitOffset - 1);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public abstract class Field
        {
            private PARAMDEF.Field _paramdefField { get; }
            public DefType Type => _paramdefField.DisplayType;
            public string InternalName => _paramdefField.InternalName;
            public string DisplayName => _paramdefField.DisplayName;
            public string Description => _paramdefField.Description;
            public int ArrayLength => _paramdefField.ArrayLength;
            public object Increment => _paramdefField.Increment;
            public int FieldOffset { get; }

            public Field(PARAMDEF.Field field, int fieldOffset)
            {
                _paramdefField = field;
                FieldOffset = fieldOffset;
            }

            public override string ToString()
            {
                return InternalName;
            }
            
        }

        public class FixedStr : Field
        {
            public Encoding Encoding;
            public FixedStr(PARAMDEF.Field field, int fieldOffset, Encoding encoding) : base(field, fieldOffset)
            {
                Encoding = encoding;
            }
        }

        public class NumericField : Field
        {
            public bool IsSigned;
            public NumericField(PARAMDEF.Field field, int fieldOffset, bool isSigned) : base(field, fieldOffset)
            {
                IsSigned = isSigned;
            }
        }

        public class BitField : Field
        {
            public int BitPosition;
            public BitField(PARAMDEF.Field field, int fieldOffset, int bitPosition) : base(field, fieldOffset)
            {
                BitPosition = bitPosition;
            }
        }

        public class PartialByteField : BitField
        {
            public int Width;
            public PartialByteField(PARAMDEF.Field field, int fieldOffset, int bitPosition, int width) : base(field, fieldOffset, bitPosition)
            {
                Width = width;
            }
        }

        public class PartialUShortField : BitField
        {
            public int Width;
            public PartialUShortField(PARAMDEF.Field field, int fieldOffset, int bitPosition, int width) : base(field, fieldOffset, bitPosition)
            {
                Width = width;
            }
        }

        public class PartialUIntField : BitField
        {
            public int Width;
            public PartialUIntField(PARAMDEF.Field field, int fieldOffset, int bitPosition, int width) : base(field, fieldOffset, bitPosition)
            {
                Width = width;
            }
        }

        public class SingleField : Field
        {
            public SingleField(PARAMDEF.Field field, int fieldOffset) : base(field, fieldOffset)
            {
            }
        }
    }
}
