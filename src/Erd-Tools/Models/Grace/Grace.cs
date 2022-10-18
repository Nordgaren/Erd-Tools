using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Erd_Tools.Utils;

namespace Erd_Tools.Models
{
    public class Grace
    {
        public string Name { get; set; }
        public string Continent { get; init; }
        public string Hub { get; init; }
        public int EntityID { get; set; }
        public int EventFlagID { get; set; }
        public int PtrOffset { get; set; }
        public int DataOffset { get; set; }
        public int BitStart { get; set; }
    }
    public class Hub
    {
        public string Name { get; set; } = "";
        public List<Grace> Graces { get; set; } = new();
    }

    public class Continent
    {
        public static List<Continent> All { get; set; }
        public string Name { get; set; } = "";
        public List<Hub> Hubs { get; set; } = new();

        public static void GetContinents()
        {
            List<Continent>? c = Util.DeserializeXml<List<Continent>>(@"Resources\Systems\SitesOfGrace.xml");
            All = c ?? throw new NullReferenceException("Continent list is null."); 
        }
    }

}
