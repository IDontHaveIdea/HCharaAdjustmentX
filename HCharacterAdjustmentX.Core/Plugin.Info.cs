using System.Reflection;

using IDHIUtils;

#region Assembly attributes

/*
 * These attributes define various meta-information of the generated DLL.
 * In general, you don't need to touch these. Instead, edit the values in Info.
 */
[assembly: AssemblyTitle(Constants.Prefix + "_" + IDHIPlugins.PInfo.PluginName + " (" + IDHIPlugins.PInfo.GUID + ")")]
[assembly: AssemblyProduct(Constants.Prefix + "_" + IDHIPlugins.PInfo.PluginName)]
[assembly: AssemblyVersion(IDHIPlugins.PInfo.Version)]
[assembly: AssemblyFileVersion(IDHIPlugins.PInfo.Version)]

#endregion Assembly attributes

//
// Login ID: 0007, 0008, 0042
//


namespace IDHIPlugins
{
    internal struct PInfo
    {
        internal const string GUID = "com.ihavenoidea.simplehcharaadjust";
        internal const string PluginDisplayName = "HScene Character AdjustmentX";
        internal const string PluginName = "HCharacterAjustX";
        internal const string Version = "0.3.8.0";
    }
}
