using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;

namespace VisualNav.Utilities;

/// <summary>
/// Update the user with a message using the info mechanism.
/// Any types of infobox's we need should be defined here.
///
/// @see https://www.vsixcookbook.com/recipes/notifications.html
/// </summary>
internal class InfoNotificationWrapper
{
    /// <summary>
    /// A simple info message, without link or button that closes automatically
    /// </summary>
    /// <param name="message"> message to display</param>
    /// <param name="i"> knownmokier icon name</param>
    /// <param name="guid"> a string identifier of a toolwindow @example ToolWindowGuids80.SolutionExplorer </param>
    /// <param name="timeout"> close the infobox automatically after a delay </param>
    ///
    public static async Task ShowSimpleAsync(string message, string i, string guid, int timeout)
    {
        var propertyInfo = typeof(KnownMonikers).GetProperty(i);
        var icon = (ImageMoniker)propertyInfo?.GetValue(null, null)!;

        var model = new InfoBarModel(
            new[]
            {
                new InfoBarTextSpan(message),
            },
            icon,
            false);

        var infoBar = await VS.InfoBar.CreateAsync(guid, model);
        if (infoBar == null) return;
        await infoBar.TryShowInfoBarUIAsync();

        await Task.Delay(timeout);
        infoBar.Close();
    }
}