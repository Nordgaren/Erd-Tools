using Erd_Tools.Models.System.Dlc;
using System;
using System.Collections.Generic;
using Erd_Tools.Utils;

namespace Erd_Tools.Models
{
    public class Grace
    {
        public string Name { get; init; }
        public string Continent { get; init; }
        public string Hub { get; init; }
        public int EntityID { get; init; }
        public int EventFlagID { get; init; }
        public DlcName Dlc { get; init; }

        public Grace(string name, string hub, string continent, int eventFlagId, int entityId, DlcName dlc) {
            Name = name;
            Hub = hub;
            Continent = continent;
            EventFlagID = eventFlagId;
            EntityID = entityId;
            Dlc = dlc;
        }
    }
    public class Hub
    {
        public string Name { get; init; }
        public int RowId { get; init; }
        public ushort TabId { get; init; }
        public DlcName Dlc { get; init; }
        public List<Grace> Graces { get; init; }

        public Hub(string name, int rowId, ushort tabId, DlcName dlc, List<Grace> graces)
        {
            Name = name;
            RowId = rowId;
            TabId = tabId;
            Dlc = dlc;
            Graces = graces;
        }
    }

    public class Continent
    {
        [Obsolete("This property is deprecated and will become private, soon. Use ErdHook.GetContinents(), instead.")]
        public static List<Continent>? All { get; set; }
        public string Name { get; init; }
        public List<Hub> Hubs { get; init; }
        public int RowId { get; init; }
        public int TextId { get; init; }
        public DlcName Dlc { get; init; }
        public Continent(string name, int rowId, List<Hub> hubs, int textId, DlcName dlc) {
            Name = name;
            RowId = rowId;
            Hubs = hubs;
            TextId = textId;
            Dlc = dlc;
        }

        /// <summary>
        /// A function that clears caches continent list, so that ErdTools repopulates it through params. 
        /// </summary>
        public static void ClearContinents()
        {
            All = null;
        }

        [Obsolete("This function is deprecated. Use ErdHook.GetContinents(), instead.")]
        public static void GetContinents()
        {
            List<Continent>? c = Util.DeserializeXml<List<Continent>>(@"Resources\Systems\SitesOfGrace.xml");
            All = c ?? throw new NullReferenceException("Continent list is null."); 
        }
        
    }

}
