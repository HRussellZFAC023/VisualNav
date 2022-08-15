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

        [Category("Options")]
        [DisplayName("Block size for building window")]
        [Description("Positive Integer for zoom in, negative integer for zoom out, 0 for default size")]
        [DefaultValue(0)]
        public int BlockSize { get; set; } = 0;

        [Category("Options")]
        [DisplayName("Block size for preview window")]
        [Description("Positive Integer for zoom in, negative integer for zoom out, 0 for default size")]
        [DefaultValue(0)]
        public int BlockSize_Preview { get; set; } = 0;
    }
}