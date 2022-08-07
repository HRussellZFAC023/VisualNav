using CefSharp;
using System.IO;
using System.Threading.Tasks;
using VisualNav.Schema;

namespace VisualNav.Utilities;

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

    private bool _blockBeingAdded; // mutex
    private bool _clear = true;

    public async Task<JavascriptResponse> AddNewBlockToAreaAsync(Command c, bool preview, bool custom)
    {
        var method = custom ? "addCustomBlockToArea" // does not actually add the block? Only adds to radial menu
            : "addNewBlockToArea";

        while (_blockBeingAdded)
        {
            await Task.Delay(1);
            if (!_clear)
            {
                return null;
            }
        }

        _clear = false;
        _blockBeingAdded = true;
        if (_preview)
        {
            await ClearAsync();
        }

        JavascriptResponse ret;
        if (custom)
            ret = await _b.EvaluateScriptAsync(method, c.Text, c.Type, c.Color);
        else
            ret = await _b.EvaluateScriptAsync(method, c.Text, c.Type);

        if (!preview)
        {
            await CenterAsync();
        }
        _blockBeingAdded = false;
        return ret;
    }

    public async Task<JavascriptResponse> ClearAsync()
    {
        var ret = await _b.EvaluateScriptAsync("Blockly.mainWorkspace.clear()");
        _clear = true; // accept new blocks
        return ret;
    }

    public async Task CenterAsync()
    {
        await Task.Delay(100);
        await _b.EvaluateScriptAsync("Blockly.mainWorkspace.zoomControls_.resetZoom_()");
        await Task.Delay(500);
        // restore zoom from settings
        var settingSize = Options.Settings.Instance.BlockSize;
        await _b.EvaluateScriptAsync("Blockly.mainWorkspace.zoomControls_.zoom_(" + settingSize + ")");
        await _b.EvaluateScriptAsync("Blockly.mainWorkspace.cleanUp()");
    }

    public async Task ResetZoomAsync()
    {
        await Task.Delay(100);
        await _b.EvaluateScriptAsync("Blockly.mainWorkspace.zoomControls_.resetZoom_()");
        Options.Settings.Instance.BlockSize = 0;
        await Options.Settings.Instance.SaveAsync();
    }

    public async Task ZoomOutAsync()
    {
        await _b.EvaluateScriptAsync("Blockly.mainWorkspace.zoomControls_.zoom_(-1)");
        if (Options.Settings.Instance.BlockSize > -7)
        {
            Options.Settings.Instance.BlockSize--;
        }

        await Options.Settings.Instance.SaveAsync();
        // update settings
        // - 1
    }

    public async Task ZoomInAsync()
    {
        await _b.EvaluateScriptAsync("Blockly.mainWorkspace.zoomControls_.zoom_(1)");
        if (Options.Settings.Instance.BlockSize < 7)
        {
            Options.Settings.Instance.BlockSize++;
        }

        await Options.Settings.Instance.SaveAsync();
        // update settings
        // + 1
    }
}