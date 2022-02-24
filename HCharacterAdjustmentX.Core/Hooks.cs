//
// Hooks for SimpleHCharaAdjust
//
using System;
using System.Collections.Generic;

using UnityEngine;
using H;

using BepInEx.Logging;
using HarmonyLib;

using AnimationLoader;
using IDHIUtils;
using CTRL = IDHIPlugins.HCharaterAdjustX.HCharacterAdjustXController;


namespace IDHIPlugins
{
    public partial class HCharaterAdjustX
    {
        // Hooks
        static internal Harmony _hookInstance;
        static internal HSceneProc _hprocInstance;
        static internal HFlag.EMode _mode;

        internal partial class Hooks
        {
            /// <summary>
            /// Patch system and save patch instance
            /// </summary>
            static internal void Init()
            {
                _hookInstance = Harmony.CreateAndPatchAll(typeof(Hooks));
            }

            /// <summary>
            /// Set the new original position when changing positions via the H point picker scene
            /// </summary>
            [HarmonyPostfix]
            [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.ChangeCategory))]
            static private void ChangeCategoryPostfix(HPointData _data, int _category)
            {
                if (!IsSupportedScene)
                {
                    return;
                }
                Utils.SetOriginalPositionAll("from [ChangeCategory]");
                Utils.RecalcAdjustmentAll("from [ChangeCategory]");
                Log.Info($"HPoint name={_data.name} Experience={_data.Experience} position {_data.transform.position}");
            }

            /// <summary>
            /// Set the new original position when changing positions not using the H point picker
            /// </summary>
            /// <param name="_nextAinmInfo"></param>
            [HarmonyPrefix]
            [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.ChangeAnimator))]
            static private void ChangeAnimatorPrefix(
                HSceneProc __instance,
                HSceneProc.AnimationListInfo _nextAinmInfo)
            {
                if (_nextAinmInfo == null)
                {
                    return;
                }

                try
                {
                    Utils.SetMode(_nextAinmInfo.mode);
                    Utils.RecalcAdjustmentAll(" from [ChangeAnimator]");
                    Utils.SetOriginalPositionAll(" from [ChangeAnimator]");
#if DEBUG
                    // var animationKey = SwapAnim.AnimationInfo.GetKey(_nextAinmInfo);
                    var _animInfo = new SwapAnim.AnimationInfo(_nextAinmInfo);
                    Log.Error("-------");
                    Log.Warning($"SHCA0021: Animator change ID: {_nextAinmInfo.mode} - "
                        + $"{_animInfo.Id} - {_animInfo.Key} - "
                        + $"name {SwapAnim.AnimationInfo.TranslateName(_nextAinmInfo)}.");
                    Log.Warning($"SHCA0022: Asset {_nextAinmInfo.pathFemaleBase.assetpath}\n"
                        + $"instance category "
                        + $"{Utils.CategoryList(__instance.categorys, quotes: false)}\n"
                        + $"now categories "
                        + $"{Utils.CategoryList(__instance.flags.nowAnimationInfo.lstCategory, quotes: false)}\n"
                        + $"next categories "
                        + $"{Utils.CategoryList(_nextAinmInfo.lstCategory, quotes: false)}\n");
                    Log.Error("-------");
#endif
                }
                catch (Exception e)
                {
                    Log.Level(LogLevel.Error, $"SHCA0023: Error - {e.Message}");
                }
            }

            static readonly internal Func<string, string> translateName = (animationName) =>
            {
                var tmp = Utilities.Translate(animationName);
                if (tmp == animationName)
                {
                    return tmp;
                }
                return $"{tmp} ({animationName})";
            };
        }
    }
}
