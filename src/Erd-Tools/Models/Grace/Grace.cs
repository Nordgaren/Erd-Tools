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
        public int ParamRowID { get; set; }
        public List<string> Offsets { get; set; } = new();
        public int BitStart { get; set; }
    }
    public class Hub
    {
        public string Name { get; set; } = "";
        public List<Grace> Graces { get; set; } = new();
    }

    public class Continent
    {
        public static List<Continent> Continents { get; set; }
        public string Name { get; set; } = "";
        public List<Hub> Hubs { get; set; } = new();

        public static void GetContinents()
        {
            XmlSerializer xmlSerializer = new(typeof(List<Continent>));
            List<Continent>? c = Util.DeserializeXml<List<Continent>>(@"Resources\Systems\SitesOfGrace.xml");
            Continents = c ?? throw new NullReferenceException("Continent list is null."); 
        }
    }

}
