using Erd_Tools.Models.Msg;
using PropertyHook;
using System;
using System.Collections.Generic;

namespace Erd_Tools.Models.Game
{
    public record class Gesture(int Id, string Name);
    
    public class GestureGameData
    {
        private const int GESTURE_LIMIT = 0x40;
        private PHPointer _gameGestureData { get; }
        private Param _gestureParam { get; }
        private MsgRepositoryImp _msgRepository { get; }
        private List<Gesture>? _gestures;
        public GestureGameData(PHPointer gameGestureData, Param gestureParam, MsgRepositoryImp msgRepository)
        {
            _gameGestureData = gameGestureData;
            _gestureParam = gestureParam;
            _msgRepository = msgRepository;
        }

        public List<Gesture> GetGestures()
        {
            if (_gestures != null)
            {
                return _gestures;
            }

            _gestures = new List<Gesture>();
            foreach (Param.Row row in _gestureParam.Rows)
            {
                int itemId = _gestureParam.Pointer.ReadInt32(row.DataOffset + row["itemId"].FieldOffset);
                string? name = _msgRepository.GetEntry(FmgId.GoodsName, itemId);
                if (name == null)
                {
                    name = _msgRepository.GetEntry(FmgId.GoodsName_dlc01, itemId);
                }
                
                if (name != null)
                {
                    _gestures.Add(new Gesture(row.ID, name));
                }
            }

            return _gestures;
        }

        public bool CheckGesture(int id)
        {
            for (int i = 0; i < GESTURE_LIMIT; i++)
            {
                int entry = _gameGestureData.ReadInt32(0x8 + (i * 4));
                if (entry >> 1 == id)
                {
                    return Convert.ToBoolean(entry & 1);
                }
            }
            return false;
        }

        public bool SetGesture(int id, bool state)
        {
            for (int i = 0; i < GESTURE_LIMIT; i++)
            {
                int offset = 0x8 + (i * 4); 
                uint entry = _gameGestureData.ReadUInt32(offset);
                if (entry >> 1 == id)
                {
                    entry &= 0xFFFFFFFE;
                    entry |= Convert.ToUInt32(state);
                    _gameGestureData.WriteUInt32(offset, entry);
                    return true;
                }
            }
            return false;
        }
    }
}