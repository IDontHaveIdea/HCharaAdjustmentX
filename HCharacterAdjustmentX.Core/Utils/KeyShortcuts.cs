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
        /// <summary>
        /// Shortcut to toggle Guide on/off
        /// </summary>
        public ConfigEntry<KeyboardShortcut> GuideObject { get; set; }
        /// <summary>
        /// Shortcut to move Guide to original position
        /// </summary>
        public ConfigEntry<KeyboardShortcut> GuideObjectReset { get; set; }
    }
}
