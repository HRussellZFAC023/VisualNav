using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace VisualNav.Schema;

public class Schema
{
    public Radialmenu[] RadialMenu { get; set; }

    public static async Task<Schema> LoadAsync()
    {
        var dir = Path.GetDirectoryName(typeof(Schema).Assembly.Location);

        StreamReader reader = null;

        bool custom = Options.Settings.Instance.CustomBlock;
        if (custom) // use the custom setting
        {
            var file = Path.Combine(dir!, "Schema", "Modified.json");
            reader = new StreamReader(file);

            var json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<Schema>(json);
        }
        else // copy the json val of the original to Modified.json to restore setting and read the unmodified version
        {
            var file = Path.Combine(dir!, "Schema", "Schema.json");
            reader = new StreamReader(file);

            var original_json = await reader.ReadToEndAsync();
            var json_obj = JsonConvert.DeserializeObject<Schema>(original_json); 
            File.WriteAllText(Path.Combine(dir!, "Schema", "Modified.json"), JsonConvert.SerializeObject(json_obj));

            return json_obj;
        }
    }
}

public class Radialmenu
{
    public string[] FileExt { get; set; }
    public string Text { get; set; }
    public bool allow_insertion_from_menu { get; set; }
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