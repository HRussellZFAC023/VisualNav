global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using System.Runtime.InteropServices;
using System.Threading;
using VisualThreading.Options;
using VisualThreading.ToolWindows;
using VisualThreading.Utilities;

namespace VisualThreading
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.VisualThreadingString)]
    [ProvideOptionPage(typeof(OptionsProvider.General1Options), "Visual Threading", "General1", 0, 0, true, SupportsProfiles = true)]
    [ProvideToolWindow(typeof(BuildingWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.SolutionExplorer)]
    [ProvideToolWindow(typeof(RadialWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.Toolbox)]
    [ProvideToolWindow(typeof(PreviewWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.Properties)]
    public sealed class VisualThreadingPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await this.RegisterCommandsAsync();
            this.RegisterToolWindows();
            await LanguageMediator.InitializeAsync();
        }
    }
}