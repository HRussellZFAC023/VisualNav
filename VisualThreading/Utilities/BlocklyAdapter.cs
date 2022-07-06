using CefSharp;
using System.IO;
using System.Threading.Tasks;
using VisualThreading.Schema;

namespace VisualThreading.Utilities;

/// <summary>
/// A narrow interface for talking with blockly - all such javascripts should go here
/// to decouple blockly from our implementation and avoid duplications.
///
/// If blockly updates in the future, we only need to change this class rather than the whole project
///
/// </summary>
public class BlocklyAdapter
{
    private readonly bool _preview;
    private readonly IChromiumWebBrowserBase _b;
    private readonly string _blocklyJs;
    private readonly string _blocklyHtml;

    /// <summary>
    /// BlocklyAdapter constructor method.
    /// </summary>
    /// <param name="browser"> chromium web-browser context</param>
    /// <param name="preview"> if true, trashcan and modifiers will be hidden</param>
    public BlocklyAdapter(IChromiumWebBrowserBase browser, bool preview)
    {
        _b = browser;
        _preview = preview;
        var root = Path.GetDirectoryName(typeof(VisualStudioServices).Assembly.Location);
        _blocklyJs = Path.Combine(root!, "Resources", "js", "blockly");
        _blocklyHtml = Path.Combine(root!, "Resources", "html", "blocklyHTML.html");
    }

    public async Task LoadHtmlAsync()
    {
        // import HTML
        var fr = new FileReaderAdapter();
        _b.LoadHtml(await fr.ReadFileAsync(_blocklyHtml));
    }

    public async Task InitAsync()
    {
        var fr = new FileReaderAdapter();

        // import blockly core
        _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "blockly_compressed.js")));
        _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "blocks_compressed.js")));
        _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "msg", "js", "en.js")));

        if (!_preview)
        {
            // import code generator
            _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "csharp_compressed.js")));
            _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "javascript_compressed.js")));
            _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "python_compressed.js")));
            _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "php_compressed.js")));
            _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "dart_compressed.js")));
            _b.ExecuteScriptAsync(await fr.ReadFileAsync(Path.Combine(_blocklyJs, "lua_compressed.js")));
        }

        await _b.EvaluateScriptAsync("init", _preview);
    }

    public async Task<JavascriptResponse> ShowCodeAsync()
    {
        var lang = LanguageMediator.GetCurrentActiveFileExtension();
        return await _b.EvaluateScriptAsync(
            "showCode", lang);
    }

    public async Task<JavascriptResponse> AddNewBlockToAreaAsync(Command c)
    {
        return await _b.EvaluateScriptAsync("addNewBlockToArea", c.Type);
    }

    public async Task ClearAsync()
    {
        await _b.GetMainFrame().EvaluateScriptAsync("Blockly.mainWorkspace.clear()");
    }
}