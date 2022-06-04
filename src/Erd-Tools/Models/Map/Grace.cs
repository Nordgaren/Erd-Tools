using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Erd_Tools.Models.Old;
using Erd_Tools.Utils;

namespace Erd_Tools.Models.Old
{
    public class Grace
    {
        public string Name { get; init; }
        public string Continent { get; init; }
        public string Hub { get; init; }
        public List<string> Offsets { get; init; }
        public int BitStart { get; init; }

        public static List<Grace> All { get; set; }

        static Grace()
        {
            XmlSerializer xmlSerializer = new(typeof(List<Grace>));
            List<Grace>? c = Util.DeserializeXml<List<Grace>>(@"Resources\Systems\SitesOfGrace.xml");
            All = c ?? throw new NullReferenceException("Continent list is null.");
        }
        public Grace()
        {
            
        }
    }
}
