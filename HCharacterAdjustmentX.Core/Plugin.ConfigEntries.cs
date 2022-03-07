//
// Configuration entries
//
using UnityEngine;

using BepInEx.Configuration;

using KKAPI.Utilities;

//using IDHIUtils;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        static internal KeyShortcuts KeyHeroine = new();
        static internal KeyShortcuts KeyHeroine3P = new();
        static internal KeyShortcuts KeyPlayer = new();
        static internal ConfigEntry<KeyboardShortcut> GroupGuide { get; set; }
        static internal ConfigEntry<float> cfgAdjustmentStep;
        
        internal void ConfigEntries(bool bHCAInstalled, bool bheroine3P = false)
        {
            // Definition of configuration items
            var sectionKeys = "Keyboard Shortcuts for Guide";

            if (!bHCAInstalled)
            {
                #region HCharaAdjustment Functionality
#if DEBUG
                _Log.Info($"SHCA0036: Creating Shortcuts for Guide");
#endif
                KeyHeroine.GuideObject = Config.Bind(
                    section: sectionKeys,
                    key: "Show Female 1 Guide Object",
                    defaultValue: new KeyboardShortcut(KeyCode.O),
                    configDescription: new ConfigDescription(
                        description: "Show the guide object for adjusting girl 1 position",
                        acceptableValues: null,
                        tags: new ConfigurationManagerAttributes { Order = 35 }));
                KeyHeroine.GuideObjectReset = Config.Bind(
                    section: sectionKeys,
                    key: "Reset Female 1 Position",
                    defaultValue: new KeyboardShortcut(KeyCode.O, KeyCode.RightControl),
                    configDescription: new ConfigDescription(
                        description: "Reset adjustments for girl 1 position",
                        acceptableValues: null,
                        tags: new ConfigurationManagerAttributes { Order = 34 }));
                if (bheroine3P)
                {
                    KeyHeroine3P.GuideObject = Config.Bind(
                        section: sectionKeys,
                        key: "Show Female 2 Guide Object",
                        defaultValue: new KeyboardShortcut(KeyCode.P),
                        configDescription: new ConfigDescription(
                            description: "Show the guide object for adjusting girl 2 position",
                            acceptableValues: null,
                            tags: new ConfigurationManagerAttributes { Order = 33 }));
                    KeyHeroine3P.GuideObjectReset = Config.Bind(
                        section: sectionKeys,
                        key: "Reset Female 2 Position",
                        defaultValue: new KeyboardShortcut(KeyCode.P, KeyCode.RightControl),
                        configDescription: new ConfigDescription(
                            description: "Reset adjustments for girl 2 position",
                            acceptableValues: null,
                            tags: new ConfigurationManagerAttributes { Order = 32, Browsable = false }));
                }
                KeyPlayer.GuideObject = Config.Bind(
                    section: sectionKeys,
                    key: "Show Player Guide Object",
                    defaultValue: new KeyboardShortcut(KeyCode.G),
                    configDescription: new ConfigDescription(
                        description: "Show the guide object for adjusting the boy's position",
                        acceptableValues: null,
                        tags: new ConfigurationManagerAttributes { Order = 31 }));
                KeyPlayer.GuideObjectReset = Config.Bind(
                    section: sectionKeys,
                    key: "Reset Player Position",
                    defaultValue: new KeyboardShortcut(KeyCode.I, KeyCode.RightControl),
                    configDescription: new ConfigDescription(
                        description: "Reset adjustments for girl 2 position",
                        acceptableValues: null,
                        tags: new ConfigurationManagerAttributes { Order = 30 }));

                GroupGuide = Config.Bind(
                    section: sectionKeys,
                    key: "Show Group Guide",
                    defaultValue: new KeyboardShortcut(KeyCode.I),
                    configDescription: new ConfigDescription(
                        description: "Show the guide object for adjusting the group position",
                        acceptableValues: null,
                        tags: new ConfigurationManagerAttributes { Order = 29 }));
                #endregion HCharaAdjustment Functionality
            }
            else
            {
                #region HCharaAdjustment Functionality Off
                // if HCharaAdjustment is detected eliminate conflicting shortcuts from configuration file
                KeyHeroine.GuideObject = Config.Bind(
                    sectionKeys,
                    "Show Female 1 Guide Object",
                    new KeyboardShortcut());
                KeyHeroine.GuideObject.ConfigFile.Remove(
                    new ConfigDefinition(sectionKeys,
                    "Show Female 1 Guide Object"));
                KeyHeroine.GuideObjectReset = Config.Bind(
                    sectionKeys,
                    "Reset Female 1 Position",
                    new KeyboardShortcut());
                KeyHeroine.GuideObjectReset.ConfigFile.Remove(
                    new ConfigDefinition(
                        sectionKeys,
                        "Reset Female 1 Position"));

                KeyHeroine3P.GuideObjectReset = Config.Bind(
                    sectionKeys,
                    "Reset Female 2 Position",
                    new KeyboardShortcut());
                KeyHeroine3P.GuideObjectReset.ConfigFile.Remove(
                    new ConfigDefinition(
                        sectionKeys,
                        "Reset Female 2 Position"));
                KeyHeroine3P.GuideObject = Config.Bind(
                    sectionKeys,
                    "Show Female 2 Guide Object",
                    new KeyboardShortcut());
                KeyHeroine3P.GuideObject.ConfigFile.Remove(
                    new ConfigDefinition(
                        sectionKeys,
                        "Show Female 2 Guide Object"));

                KeyPlayer.GuideObject = Config.Bind(
                    sectionKeys,
                    "Show Player Guide Object",
                    new KeyboardShortcut());
                KeyPlayer.GuideObject.ConfigFile.Remove(
                    new ConfigDefinition(
                        sectionKeys,
                        "Show Player Guide Object"));
                KeyPlayer.GuideObjectReset = Config.Bind(
                    sectionKeys,
                    "Reset Player Position",
                    new KeyboardShortcut());
                KeyPlayer.GuideObjectReset.ConfigFile.Remove(
                    new ConfigDefinition(
                        sectionKeys,
                        "Reset Player Position"));
#if DEBUG
                _Log.Info($"SHCA0037: Removing Shortcuts for Guide");
#endif
                #endregion
            }

#if DEBUG
            _Log.Info($"SHCA0038: Creating Shortcuts for Characters");
#endif
            #region Heroine
            KeyHeroine.Menu = Config.Bind(
                section: sectionKeys,
                key: "Toggle button interface for Heroine.",
                defaultValue: new KeyboardShortcut(KeyCode.L),
                configDescription: new ConfigDescription(
                    description: "Show movement buttons",
                    acceptableValues: null,
                    tags: new ConfigurationManagerAttributes { Order = 27 }));
            #endregion Female

            #region Player
            KeyPlayer.Menu = Config.Bind(
                section: sectionKeys,
                key: "Toggle button interface for Player.",
                defaultValue: new KeyboardShortcut(KeyCode.L, KeyCode.RightAlt, KeyCode.AltGr),
                configDescription: new ConfigDescription(
                    description: "Show movement buttons",
                    acceptableValues: null,
                    tags: new ConfigurationManagerAttributes { Order = 26 }));
            #endregion Player

            #region Heroine3P
            if (bheroine3P)
            {
                KeyHeroine.Menu = Config.Bind(
                    section: sectionKeys,
                    key: "Toggle button interface Heroine 2.",
                    defaultValue: new KeyboardShortcut(KeyCode.L),
                    configDescription: new ConfigDescription(
                        description: "Show movements buttons",
                        acceptableValues: null,
                        tags: new ConfigurationManagerAttributes { Order = 25 }));
            }
            #endregion Female 2

            #region Steps
#if DEBUG
            _Log.Info($"SHCA0039: Creating Shortcuts for Steps");
#endif

            sectionKeys = "Movement Step";
            cfgAdjustmentStep = Config.Bind(
                section: sectionKeys,
                key: "Move step amount",
                defaultValue: 0.01f,
                configDescription: new ConfigDescription(
                    description: "Set the step by with to move",
                    acceptableValues: null,
                    tags: new ConfigurationManagerAttributes { Order = 14 }));
            cfgAdjustmentStep.SettingChanged += (_sender, _args) =>
            {
                if (_fAdjustStep != cfgAdjustmentStep.Value)
                {
                    _fAdjustStep = cfgAdjustmentStep.Value;
#if DEBUG
                    _Log.Info($"SHCA0040: Movement step read in configuration - {cfgAdjustmentStep.Value}");
#endif
                }
            };
            #endregion Steps
        }
    }
}
