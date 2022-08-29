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
        var file = Path.Combine(dir!, "Schema", "Modified.json");
        StreamReader reader;
        
        if (!Options.Settings.Instance.CustomBlock)
        {
            var original = Path.Combine(dir!, "Schema", "Schema.json");
            var target = Path.Combine(dir!, "Schema", "Modified.json");

            StreamReader tempReader = new StreamReader(original);
            var originalJson = await tempReader.ReadToEndAsync();
            tempReader.Close();

            File.WriteAllText(target, originalJson);
        }

        try
        {
            reader = new StreamReader(file);
        }
        catch (Exception)
        {
            file = Path.Combine(dir!, "Schema", "Schema.json");
            reader = new StreamReader(file);
        }

        var json = await reader.ReadToEndAsync();
        reader.Close();
        return JsonConvert.DeserializeObject<Schema>(json);
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
    public string Description { get; set; }
}

public class Command
{
    public string Text { get; set; }
    public string Parent { get; set; }
    public string Preview { get; set; }
    public string Color { get; set; }
    public string Type { get; set; }
    public string Description { get; set; }
}


