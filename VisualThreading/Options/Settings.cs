using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VisualThreading.Options
{
    internal class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.General1Options), "VisualThreading", "Settings", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class General1Options : BaseOptionPage<Settings>
        { }
    }

    public class Settings : BaseOptionModel<Settings>
    {
        [Category("Options")]
        [DisplayName("Child Friendly Mode")]
        [Description("An informative description.")]
        [DefaultValue(true)]
        public bool ChildMode { get; set; } = true;
    }
}