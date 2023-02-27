﻿//
// Configuration entries
//
using UnityEngine;

using BepInEx.Configuration;
using BepInEx.Logging;

using KKAPI.Utilities;

//using IDHIUtils;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        internal static KeyShortcuts KeyHeroine = new();
        internal static KeyShortcuts KeyHeroine3P = new();
        internal static KeyShortcuts KeyPlayer = new();

        internal static ConfigEntry<bool> DebugInfo;
        internal static ConfigEntry<bool> DebugToConsole;
        internal static ConfigEntry<KeyboardShortcut> GroupGuide { get; set; }
        internal static ConfigEntry<float> cfgAdjustmentStep;
        internal static ConfigEntry<float> cfgRotationStep;

        internal void ConfigEntries(bool bheroine3P = false)
        {
            // Definition of configuration items
            var sectionKeys = "Keyboard Shortcuts for Guide";
#if DEBUG
            _Log.Info($"HCAX0016: Creating Shortcuts for Characters");
            GroupGuide = Config.Bind(
                section: sectionKeys,
                key: "Show Group Guide",
                defaultValue: new KeyboardShortcut(KeyCode.I),
                configDescription: new ConfigDescription(
                    description: "Show the guide object for adjusting the group position",
                    acceptableValues: null,
                    tags: new ConfigurationManagerAttributes { Order = 29 }));
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
                defaultValue: new KeyboardShortcut(
                    KeyCode.L, KeyCode.RightAlt, KeyCode.AltGr),
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
            _Log.Info($"HCAX0017: Creating Shortcuts for Steps");
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
                    _Log.Info($"HCAX0018: Movement step read in configuration - " +
                        $"{cfgAdjustmentStep.Value}");
#endif
                }
            };

            cfgRotationStep = Config.Bind(
                section: sectionKeys,
                key: "Rotation step amount",
                defaultValue: 5f,
                configDescription: new ConfigDescription(
                    description: "Set the rotation step by with to move",
                    acceptableValues: null,
                    tags: new ConfigurationManagerAttributes { Order = 14 }));
            cfgAdjustmentStep.SettingChanged += (_sender, _args) =>
            {
                if (_fAdjustStep != cfgAdjustmentStep.Value)
                {
                    _fAdjustStep = cfgAdjustmentStep.Value;
#if DEBUG
                    _Log.Info($"HCAX0018: Movement step read in configuration - " +
                        $"{cfgAdjustmentStep.Value}");
#endif
                }
            };

            #endregion Steps
        }

        internal void ConfigDebugEnntry()
        {
            DebugInfo = Config.Bind(
                section: "Debug",
                key: "Debug Information",
                defaultValue: false,
                configDescription: new ConfigDescription(
                    description: "Log debug information",
                    acceptableValues: null,
                    tags: new ConfigurationManagerAttributes {
                        Order = 1,
                        IsAdvanced = true }));
            DebugInfo.SettingChanged += (_sender, _args) =>
            {
                _Log.Enabled = DebugInfo.Value;
#if DEBUG
                _Log.Level(LogLevel.Info, $"HCAX0019A: Log.Enabled set to {_Log.Enabled}");
#endif
            };

            DebugToConsole = Config.Bind(
                section: "Debug",
                key: "Debug information to Console",
                defaultValue: false,
                configDescription: new ConfigDescription(
                    description: "Show debug information in Console",
                    acceptableValues: null,
                    tags: new ConfigurationManagerAttributes {
                        Order = 1,
                        IsAdvanced = true
                    }));
            DebugToConsole.SettingChanged += (_sender, _args) =>
            {
                _Log.DebugToConsole = DebugToConsole.Value;
#if DEBUG
                _Log.Level(LogLevel.Info, $"HCAX0019B: Log.DebugToConsole set to {_Log.DebugToConsole}");
#endif
            };

        }
    }
}
