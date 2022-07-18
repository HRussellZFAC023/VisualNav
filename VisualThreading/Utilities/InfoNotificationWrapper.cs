using System.Diagnostics;
using Microsoft.VisualStudio.Imaging.Interop;

// TODO (wip)
namespace VisualThreading.Utilities;

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
    /// <param name="icon"> knownmokier icon </param>
    /// <param name="guid"> a string identifier of a toolwindow @example ToolWindowGuids80.SolutionExplorer </param>
    /// <param name="timeout"> close the infobox automatically after a delay </param>
    /// 
    public static async void ShowSimple(string message, ImageMoniker icon, string guid, int timeout)
    {
        var model = new InfoBarModel(
            new[]
            {
                new InfoBarTextSpan(message),
            },
            icon,
            false);

        var infoBar = await VS.InfoBar.CreateAsync(guid, model);
            
        //await Task.Delay(timeout).ContinueWith(t =>
        //{
        //    Debug.Assert(infoBar != null, nameof(infoBar) + " != null");
        //    infoBar.Close();
        //});
            

    }
}