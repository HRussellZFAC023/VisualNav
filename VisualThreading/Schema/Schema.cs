using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace VisualThreading.Schema
{
    public class Schema
    {
        /// <summary>
        /// Private constructor singleton
        /// </summary>
        private Schema()
        { }

        public Radialmenu[] RadialMenu { get; set; }

        public static async Task<Schema> LoadAsync()
        {
            var dir = Path.GetDirectoryName(typeof(Schema).Assembly.Location);
            var file = Path.Combine(dir!, "Schema", "schema.json");

            using var reader = new StreamReader(file);
            var json = await reader.ReadToEndAsync();
            return JsonConvert.DeserializeObject<Schema>(json);
        }
    }

    public class Radialmenu
    {
        public string FileExt { get; set; }
        public string Text { get; set; }
        public Command[] Commands { get; set; }
        public object[] Ui { get; set; }
        public object[] Symbols { get; set; }
    }

    public class Command
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }
}