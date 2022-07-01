using System.IO;
using System.Threading.Tasks;

namespace VisualThreading.Utilities;

public class FileReaderAdapter
{
    public async Task<string> ReadFileAsync(string file)
    {
        using var reader = new StreamReader(file);
        var content = await reader.ReadToEndAsync();
        return content;
    }
}