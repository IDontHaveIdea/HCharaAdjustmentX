//
// Hooks for HCharaAdjustmentX
//
using System;

using UnityEngine;

using H;

using HarmonyLib;

using KKAPI.MainGame;

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

            #region AnalStuff
            [HarmonyPrefix]
            [HarmonyPatch(typeof(HSprite), nameof(HSprite.OnInsertAnalClick), new Type[] { })]
            private static void OnInsertAnalClickPre(HSprite __instance)
            {
                _Log.Warning($"Movement Entering Hook.");

                /*var heroine = Heroines[0].GetHeroine();

                if (heroine == null)
                {
                    _Log.Warning($"Heroine null.");
                    return;
                }

                if (!__instance.flags.isAnalInsertOK)
                {
                    if (__instance.flags.count.sonyuOrg >= 2)
                    {
                        var b = (int)(heroine.hAreaExps[3] + heroine.countAnalH);
                        b = Mathf.Min(100, b);
                        var ratioRand = new GlobalMethod.RatioRand();
                        ratioRand.Add(0, b);
                        if (100 - b != 0)
                        {
                            ratioRand.Add(1, 100 - b);
                        }
                        _hprocInstance.flags.isAnalInsertOK = ratioRand.Random() == 0;
                        _Log.Warning($"Anal OK={_hprocInstance.flags.isAnalInsertOK}.");
                    }
                }*/
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(HSprite), nameof(HSprite.OnInsertAnalClick), new Type[] { })]
            private static void OnInsertAnalNoVoiceClickPost(HSprite __instance, bool __state)
            {
                _Log.Warning($"Movement Postfix");
            }

            /*[HarmonyPrefix]
            [HarmonyPatch(typeof(HSprite), nameof(HSprite.OnInsertAnalNoVoiceClick), new Type[] { })]
            private static void OnInsertAnalNoVoiceClickPre(HSprite __instance, out bool __state)
            {
                __state = __instance.flags.isAnalInsertOK;

                // Check if player can circumvent the anal deny
                if (__instance.flags.count.sonyuAnalOrg >= 1)
                {
                    __instance.flags.isAnalInsertOK = true;
                    __instance.flags.isDenialvoiceWait = false;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(typeof(HSprite), nameof(HSprite.OnInsertAnalClick), new Type[] { })]
            [HarmonyPatch(typeof(HSprite), nameof(HSprite.OnInsertAnalNoVoiceClick), new Type[] { })]
            private static void OnInsertAnalNoVoiceClickPost(HSprite __instance, bool __state)
            {
                __instance.flags.isAnalInsertOK = __state;
            }*/
            #endregion



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
#if DEBUG
                // Get calling method name
                var callingMethod = Utilities.CallingMethod();
                _Log.Warning($"[{callingMethod}] Calling ChangeCategoryPostfix");
#endif
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
#if DEBUG
                // Get calling method name
                var callingMethod = Utilities.CallingMethod();
                _Log.Warning($"[{callingMethod}] Calling ChangeAnimatorPrefix");
#endif
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
#if DEBUG
                // Get calling method name
                var callingMethod = Utilities.CallingMethod();
                _Log.Warning($"[{callingMethod}] Calling ChangeAnimatorPostfix");
#endif
                _animationKey = Utils.GetAnimationKey(_nextAinmInfo);
                Utils.SetMode(_nextAinmInfo.mode);
                Utils.SetOriginalPositionAll(_nextAinmInfo);
                Utils.RecalcAdjustmentAll();
                Utils.InitialPosition();
            }
        }
    }
}
