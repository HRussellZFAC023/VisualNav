﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace VisualThreading.Options
{
    internal class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.General1Options), "VisualThreading", "General1", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class General1Options : BaseOptionPage<General1>
        { }
    }

    public class General1 : BaseOptionModel<General1>
    {
        [Category("My category")]
        [DisplayName("My Option")]
        [Description("An informative description.")]
        [DefaultValue(true)]
        public bool MyOption { get; set; } = true;

        [Category("My category")]
        [DisplayName("Radial Size")]
        [Description("The default size of radial menu")]
        [DefaultValue("12,150,82.5,135,150,142.5,60.0,25")] 
        // 60 is the size of back, 25 is the size of icon 
        public String RadialSize { get; set; } = "12,150,82.5,135,150,142.5,60.0,25";

        [Category("Child Mode")]
        [DisplayName("My Option")]
        [Description("An informative description.")]
        [DefaultValue(true)]
        public bool ChildMode { get; set; } = true;

    }
}