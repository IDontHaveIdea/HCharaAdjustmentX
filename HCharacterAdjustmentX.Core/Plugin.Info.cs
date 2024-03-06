﻿using System.Reflection;

using IDHIPlugins;
using IDHIUtils;

#region Assembly attributes

/*
 * These attributes define various meta-information of the generated DLL.
 * In general, you don't need to touch these. Instead, edit the values in Info.
 */
[assembly: AssemblyTitle(Constants.Prefix + "_" +   HCharaAdjustmentX.PlugInName + " (" + HCharaAdjustmentX.GUID + ")")]
[assembly: AssemblyProduct(Constants.Prefix + "_" + HCharaAdjustmentX.PlugInName)]
[assembly: AssemblyVersion(HCharaAdjustmentX.Version)]
[assembly: AssemblyFileVersion(HCharaAdjustmentX.Version)]

#endregion Assembly attributes


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        public const string GUID = "com.ihavenoidea.hcharaadjustmentx";
#if DEBUG
        public const string PlugInDisplayName = "HScene Character AdjustmentX (Debug)";
#else
        public const string PluginDisplayName = "HScene Character AdjustmentX";
#endif
        public const string PlugInName = "HCharaAdjustmentX";
        public const string Version = "0.6.0.0";
        public const int DataVersion = 1;
    }
}
