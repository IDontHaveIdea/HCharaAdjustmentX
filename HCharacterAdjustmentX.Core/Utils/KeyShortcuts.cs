using BepInEx.Configuration;

namespace IDHIPlugIns
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
