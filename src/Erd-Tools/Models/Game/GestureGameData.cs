using Erd_Tools.Models.Msg;
using PropertyHook;
using System;
using System.Collections.Generic;

namespace Erd_Tools.Models.Game
{
    /// <summary>
    /// Class that represents relevant info for gestures.
    /// </summary>
    /// <param name="Id">The gesture ID, used for unlocking the gesture</param>
    /// <param name="Name">Name of the gesture</param>
    /// <param name="ItemId">Item id of the gesture, used for displaying aquisition</param>
    public record class Gesture(int Id, string Name, int ItemId);
    
    public class GestureGameData
    {
        private const int GESTURE_LIMIT = 0x40;
        private PHPointer _gameGestureData { get; }
        private Param _gestureParam { get; }
        private MsgRepositoryImp _msgRepository { get; }
        private List<Gesture>? _gestures;
        /// <summary>
        /// GestureGameData structure responsible for interacting with the games `GestureGameData`.  
        /// </summary>
        /// <param name="gameGestureData">Pointer to the games GameGestureData class</param>
        /// <param name="gestureParam">The GestureParam</param>
        /// <param name="msgRepository">MsgRespositoryImp class for checking string names</param>
        public GestureGameData(PHPointer gameGestureData, Param gestureParam, MsgRepositoryImp msgRepository)
        {
            _gameGestureData = gameGestureData;
            _gestureParam = gestureParam;
            _msgRepository = msgRepository;
        }

        /// <summary>
        /// Returns a list of all the gestures in the hooked game. Gesture class includes id, name and item id.  
        /// </summary>
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
                    _gestures.Add(new Gesture(row.ID, name, itemId));
                }
            }

            return _gestures;
        }

        /// <summary>
        /// Checks if the gesture is enabled or not.
        /// </summary>
        /// <param name="id">gesture id</param>
        /// <returns>state</returns>
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

        /// <summary>
        /// Enables or diables the gesture. Returns a bool indicating success. Returns false if the gesture is not found.
        /// Note: Might change to `bool?` where null indicates gesture not found.  
        /// </summary>
        /// <param name="id">gesture id</param>
        /// <param name="state">enable or disable</param>
        /// <returns>success</returns>
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