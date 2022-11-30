using BepInEx.Configuration;

namespace IDHIPlugins
{
    /// <summary>
    /// Class for common keyboard shortcuts used.
    /// </summary>
    public partial class KeyShortcuts
    {
        /// <summary>
        /// Character movement buttons menu
        /// </summary>
        public ConfigEntry<KeyboardShortcut> Menu { get; set; }
    }
}
