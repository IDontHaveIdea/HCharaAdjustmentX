//
// Hooks for HCharaAdjustmentX
//
using System;

using UnityEngine;

using H;

using BepInEx.Logging;
using HarmonyLib;


using IDHIUtils;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        // Hooks
        internal static Harmony _hookInstance;
        internal static HSceneProc _hprocInstance;
        internal static object _hprocObject;
        internal static HFlag.EMode _mode;
        internal static string _animationKey = "";
        internal static HSceneProcTraverse _hprocTraverse;

        internal partial class Hooks
        {
            /// <summary>
            /// Patch system and save patch instance
            /// </summary>
            internal static void Init()
            {
                _hookInstance = Harmony.CreateAndPatchAll(typeof(Hooks));
            }

            /// <summary>
            /// Set the new original position when changing positions via the
            /// H point picker scene
            /// </summary>
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.ChangeCategory))]
            private static void ChangeCategoryPostfix(HPointData _data, int _category)
            {
                if (!IsSupportedScene)
                {
                    return;
                }
                // Get calling method name
                var callingMethod = Utilities.CallingMethod();
                _Log.Warning($"[{callingMethod}] ChangeCategoryPostfix");
                Utils.SetOriginalPositionAll();
                Utils.RecalcAdjustmentAll("from [ChangeCategory]");
            }

            [HarmonyPrefix]
            [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.ChangeAnimator))]
            private static void ChangeAnimatorPrefix(
                HSceneProc.AnimationListInfo _nextAinmInfo)
            {
                if (_nextAinmInfo == null)
                {
                    return;
                }

                _animationKey = "";
                _animationKey = Utils.GetAnimationKey(_nextAinmInfo);
                Utils.ResetPositionAll();
            }

            /// <summary>
            /// Set the new original position when changing positions not using
            /// the H point picker
            /// </summary>
            /// <param name="_nextAinmInfo"></param>
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.ChangeAnimator))]
            private static void ChangeAnimatorPostfix(
                HSceneProc.AnimationListInfo _nextAinmInfo)
            {
                if (_nextAinmInfo == null)
                {
                    return;
                }
                _animationKey = Utils.GetAnimationKey(_nextAinmInfo);
                Utils.SetMode(_nextAinmInfo.mode);
                Utils.SetOriginalPositionAll(_nextAinmInfo);
                Utils.RecalcAdjustmentAll();
                Utils.InitialPosition();
            }
        }
    }
}
