using Erd_Tools.Utils;
using PropertyHook;
using System;

namespace Erd_Tools.Models.CSFD4
{
    /// <summary>
    /// A class that deals with the games `CSFD4VirtualMemoryFlag` class. Can lookup and set flags.
    /// </summary>
    public class CSFD4VirtualMemoryFlag
    {
        private PHPointer _csfd4VirtualMemoryFlag;
        private ErdHook _hook;
        private PHPointer _flagHolder;
        private int?[] _cache;

        public CSFD4VirtualMemoryFlag(PHPointer csfd4VirtualMemoryFlag, ErdHook hook)
        {
            _csfd4VirtualMemoryFlag = csfd4VirtualMemoryFlag;
            _hook = hook;
            int entryCount =
                _csfd4VirtualMemoryFlag.ReadInt32((int)Offsets.CSFD4VirtualMemoryFlag.FlagGroupEntryCount);
            _flagHolder =
                _hook.CreateBasePointer(
                    _csfd4VirtualMemoryFlag.ReadIntPtr((int)Offsets.CSFD4VirtualMemoryFlag.FlagHolder));
            _cache = new int?[entryCount];
            ClearCache();
        }
        /// <summary>
        /// Re-exposes the underlying PHPointers Resolve function
        /// </summary>
        /// <returns>IntPtr to `CSFD4VirtualMemoryFlag`</returns>
        public IntPtr Resolve()
        {
            return _csfd4VirtualMemoryFlag.Resolve();
        }
        /// <summary>
        /// Clears the CSFD4VirtualMemoryFlag cache, which consists of the position of even flag groups in the flag holder.
        /// </summary>
        public void ClearCache()
        {
           Array.Clear(_cache);
        }
        /// <summary>
        /// Sets the event flag in game by calling the "Set Event Flag" function.
        /// </summary>
        /// <param name="flag">Flag Id</param>
        /// <param name="state">On or off</param>
        public void SetEventFlag(int flag, bool state)
        {
            IntPtr idPointer = _hook.GetPrefferedIntPtr(sizeof(int));
            PropertyHook.Kernel32.WriteInt32(_hook.Handle, idPointer, flag);

            string asmString = Util.GetEmbededResource("Assembly.SetEventFlag.asm");
            string asm = string.Format(asmString, _csfd4VirtualMemoryFlag.Resolve(), (state ? 1 : 0), idPointer.ToString("X2"),
                _hook.SetEventFlagFunction.Resolve());
            _hook.AsmExecute(asm);
            _hook.Free(idPointer);
        }
        /// <summary>
        /// Returns the state of the flag in game by calling the "Is Event Flag" function.
        /// </summary>
        /// <param name="flag">Flag Id</param>
        /// <returns>Flag state</returns>
        public bool IsEventFlag(int flag)
        {
            IntPtr returnPtr = _hook.GetPrefferedIntPtr(sizeof(bool));
            IntPtr idPointer = _hook.GetPrefferedIntPtr(sizeof(int));
            PropertyHook.Kernel32.WriteInt32(_hook.Handle, idPointer, flag);

            string asmString = Util.GetEmbededResource("Assembly.IsEventFlag.asm");
            string asm = string.Format(asmString, _csfd4VirtualMemoryFlag.Resolve(), idPointer.ToString("X2"),
                _hook.IsEventFlagFunction.Resolve(), returnPtr.ToString("X2"));

            _hook.AsmExecute(asm);
            bool state = PropertyHook.Kernel32.ReadBoolean(_hook.Handle, returnPtr);
            _hook.Free(returnPtr);
            _hook.Free(idPointer);

            return state;
        }
        /// <summary>
        /// Returns the state of the flag in game by checking the flag bit manually. Is faster than `IsEventFlag`
        /// </summary>
        /// <param name="flagId">Flag Id</param>
        /// <returns>Flag state</returns>
        public bool IsEventFlagFast(int flagId)
        {
            int group = flagId / 1000;
            int bitPos = flagId % 1000;

            int? cached = _cache[group];
            if (cached != null)
            {
                return _readEventFlag(cached.Value, bitPos);
            }

            IntPtr root = _csfd4VirtualMemoryFlag.ReadIntPtr((int)Offsets.CSFD4VirtualMemoryFlag.FlagGroupRootNode);
            PHPointer rootPtr = _hook.CreateBasePointer(root);
            IntPtr parent = rootPtr.ReadIntPtr((int)Offsets.EventFlagGroupNode.Parent);
            PHPointer currentPtr = _hook.CreateBasePointer(parent);
            bool isLeaf = currentPtr.ReadBoolean((int)Offsets.EventFlagGroupNode.IsLeaf);

            PHPointer foundPtr = rootPtr;

            while (!isLeaf)
            {
                int currentGroup = currentPtr.ReadInt32((int)Offsets.EventFlagGroupNode.Group);
                PHPointer nextPtr;
                if (currentGroup < group)
                {
                    IntPtr next = currentPtr.ReadIntPtr((int)Offsets.EventFlagGroupNode.Right);
                    nextPtr = _hook.CreateBasePointer(next);
                    currentPtr = foundPtr;
                }
                else
                {
                    IntPtr next = currentPtr.ReadIntPtr((int)Offsets.EventFlagGroupNode.Left);
                    nextPtr = _hook.CreateBasePointer(next);
                }

                foundPtr = currentPtr;
                currentPtr = nextPtr;
                isLeaf = nextPtr.ReadBoolean((int)Offsets.EventFlagGroupNode.IsLeaf);
            }

            if (foundPtr.Resolve() == rootPtr.Resolve() || group < foundPtr.ReadInt32((int)Offsets.EventFlagGroupNode.Group))
            {
                return false;
            }

            int entrySize =
                _csfd4VirtualMemoryFlag.ReadInt32((int)Offsets.CSFD4VirtualMemoryFlag.FlagHolderEntrySize);
            int groupIndex = foundPtr.ReadInt32((int)Offsets.EventFlagGroupNode.Location) * entrySize;
            _cache[group] = groupIndex;
            return _readEventFlag(groupIndex, bitPos);

        }
        /// <summary>
        /// Helper method that reads the event flag with the specified group index and bit position information.
        /// </summary>
        /// <param name="groupIndex">Group index in bytes. Location * EntrySize</param>
        /// <param name="bitPos">The bit position information of the flag. FlagId % Divisor</param>
        /// <returns>Flag State</returns>
        private bool _readEventFlag(int groupIndex, int bitPos)
        {
            int bitIndex = bitPos >> 3;
            byte bitfield = _flagHolder.ReadByte(groupIndex + bitIndex);
            
            int bitStart = 7 - (bitPos & 0b00000111);
            return (bitfield & (1 << bitStart)) != 0;
        }
    }
}