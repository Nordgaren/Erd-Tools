using PropertyHook;
using System;

namespace Erd_Tools.Models.System.Dlc
{
    /// <summary>
    /// A class that has data regarding wether or not the player owns a specific dlc or not.  
    /// </summary>
    public class CSDlcImp
    {
        private const int DLC_ARRAY_START = 0x10;
        private const int DLC_ARRAY_SIZE = 0x31;
        private PHPointer _csDlcImp { get; }
        /// <summary>
        /// pointer to the CSDlcImp instance.  
        /// </summary>
        /// <param name="csDlcImp"></param>
        public CSDlcImp(PHPointer csDlcImp)
        {
            _csDlcImp = csDlcImp;
        }

        /// <summary>
        /// Resolve the underlying pointer to `CSDlcImp`
        /// </summary>
        /// <returns></returns>
        internal IntPtr Resolve()
        {
            return _csDlcImp.Resolve();
        }

        /// <summary>
        /// Returns whether the main player owns the specific dlc.
        /// </summary>
        /// <param name="dlc">The DLC to check</param>
        /// <returns></returns>
        public bool DlcAvailable(DlcName dlc)
        {
            if (dlc == DlcName.None)
            {
                return true;
            }
            return _csDlcImp.ReadBoolean(DLC_ARRAY_START + (int)dlc);
        }
    }

    /// <summary>
    /// The name of the downloadable content.
    /// </summary>
    public enum DlcName
    {
        None = -1,
        PreOrderGesture = 0x0,
        ShadowOfTheErdtree = 0x1,
        ShadowOfTheErdtreePreOrderGesture = 0x2,
    }
}