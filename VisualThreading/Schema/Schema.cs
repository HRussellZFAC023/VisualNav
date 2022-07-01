using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

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
        public string FileExt { get; set; }
        public string Text { get; set; }
        public Menuitem[] MenuItems { get; set; }
        public Command[] Commands { get; set; }
    }

    public class Menuitem
    {
        public string Name { get; set; }
        public string Parent { get; set; }
        public string[] Submenu { get; set; }
        public string[] Children { get; set; }
        public string Icon { get; set; }
    }

    public class Command
    {
        public string Text { get; set; }
        public string Parent { get; set; }
        public string Preview { get; set; }
        public string Color { get; set; }
        public string Type { get; set; }
    }
}
