using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VisualNav.Options
{
    internal class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.Options), "VisualNav", "Settings", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class Options : BaseOptionPage<Settings>
        { }
    }

    public class Settings : BaseOptionModel<Settings>
    {
        [Category("Options")]
        [DisplayName("Child Friendly Mode")]
        [Description("An informative description.")]
        [DefaultValue(true)]
        public bool ChildMode { get; set; } = true;

        [Category("Options")]
        [DisplayName("Custom blocks")]
        [Description("True for custom blocks, False to restore.")]
        [DefaultValue(false)]
        public bool CustomBlock { get; set; } = false;
    }
}