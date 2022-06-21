using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace VisualThreading.Schema
{
    public class Schema
    {
        public Radialmenu[] RadialMenu { get; set; }
        public static async Task<Schema> LoadAsync()
        {
            var dir = Path.GetDirectoryName(typeof(Schema).Assembly.Location);
            var file = Path.Combine(dir!, "Schema", "Schema.json");

            using var reader = new StreamReader(file);
            var json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<Schema>(json);
        }
    }


    public class Rootobject
    {
        public Radialmenu[] RadialMenu { get; set; }
    }

    public class Radialmenu
    {
        public string fileExt { get; set; }
        public string text { get; set; }
        public Menuitem[] MenuItems { get; set; }
        public Command[] commands { get; set; }
    }

    public class Menuitem
    {
        public string name { get; set; }
        public string parent { get; set; }
        public string[] submenu { get; set; }
        public string[] chidren { get; set; }
    }

    public class Command
    {
        public string text { get; set; }
        public string parent { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

}
