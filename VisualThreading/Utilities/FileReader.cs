using System.IO;
using System.Threading.Tasks;

namespace VisualThreading;

public class FileReader
{
    public async Task<string> ReadFileAsync(string file)
    {
        using var reader = new StreamReader(file);
        var content = await reader.ReadToEndAsync();
        return content;
    }
}