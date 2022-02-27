// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
    "Style",
    "IDE0044:Add readonly modifier",
    Justification = "Cannot update _buttons in OnGui otherwise",
    Scope = "member",
    Target = "~F:IDHIPlugins.HCharaAdjustmentX.ButtonsGUI2._buttons")]
[assembly: SuppressMessage(
    "Style",
    "IDE0044:Add readonly modifier",
    Justification = "Cannot update _buttons in OnGui otherwise",
    Scope = "member",
    Target = "~F:IDHIPlugins.HCharaAdjustmentX.ButtonsGUI._buttons")]
[assembly: SuppressMessage(
    "Style",
    "IDE0060:Remove unused parameter",
    Justification = "Needed for patch to work",
    Scope = "member",
    Target = "~M:IDHIPlugins.HCharaAdjustmentX.Hooks.ChangeCategoryPostfix(H.HPointData,System.Int32)")]
[assembly: SuppressMessage(
    "Style",
    "IDE0060:Remove unused parameter",
    Justification = "Part of API",
    Scope = "member",
    Target = "~M:IDHIPlugins.Utils.SceneChanged(UnityEngine.SceneManagement.Scene,UnityEngine.SceneManagement.Scene)")]
