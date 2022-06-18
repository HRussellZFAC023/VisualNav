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
        public Thread[] Thread { get; set; }
        public Test[] Test { get; set; }
        public Code[] Code { get; set; }
        public UI[] UI { get; set; }
    }

    public class Thread
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class Test
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class Code
    {
        public IO[] IO { get; set; }
        public MethodKeyword[] MethodKeyword { get; set; }
        public Condition[] Condition { get; set; }
        public Loop[] Loop { get; set; }
        public Variable[] Variables { get; set; }
        public Operator[] Operator { get; set; }
        public Comparetor[] Comparetor { get; set; }
    }

    public class IO
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class MethodKeyword
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class Condition
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class Loop
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class Variable
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class Operator
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class Comparetor
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }

    public class UI
    {
        public string text { get; set; }
        public string preview { get; set; }
        public string color { get; set; }
    }
}
