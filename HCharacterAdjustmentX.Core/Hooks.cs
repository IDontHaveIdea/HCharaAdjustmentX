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
        internal static bool _MovePerformed = false;

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
                object __instance,
                HSceneProc.AnimationListInfo _nextAinmInfo)
            {
                if (_nextAinmInfo == null)
                {
                    return;
                }
#if DEBUG
                // This hook is executed before HProcMonitor HSceneProc finish loading event
                _hprocTraverse ??= new(__instance);

                // Get calling method name
                try
                {
                    var nowHPointDataPos = _hprocTraverse.nowHpointDataPos;
                    var nowHPointData = _hprocTraverse.nowHpointData;
                    var callingMethod = Utilities.CallingMethod();
                    _Log.Warning($"[{callingMethod}] Calling ChangeAnimatorPrefix Position " +
                        $"Name={nowHPointData} Position Point={nowHPointDataPos}");
                }
                catch
                {
                    var callingMethod = Utilities.CallingMethod();
                    _Log.Warning($"[{callingMethod}] Calling ChangeAnimatorPrefix");
                }
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
                try
                {
                    var nowHPointDataPos = _hprocTraverse.nowHpointDataPos;
                    var nowHPointData = _hprocTraverse.nowHpointData;
                    nowHPointData ??= "null";
                    if (nowHPointDataPos == null)
                    {
                        nowHPointDataPos = new Vector3(-1, -1, -1);
                    }

                    var callingMethod = Utilities.CallingMethod();
                    _Log.Warning($"[{callingMethod}] Calling ChangeAnimatorPostfix Position " +
                        $"Name={nowHPointData} Position Point={nowHPointDataPos}");
                }
                catch
                {
#if KKS
                    var nowHPointDataPos = HPointInfo.InitialPositon;
                    var nowHPointData = HPointInfo.InitialPositionName;
                    // Get calling method name
                    var callingMethod = Utilities.CallingMethod();
                    _Log.Warning($"[{callingMethod}] Error: Calling ChangeAnimatorPostfix Position " +
                        $"Name={nowHPointData} Position Point={nowHPointDataPos}");
#else
                    var callingMethod = Utilities.CallingMethod();
                    _Log.Warning($"[{callingMethod}] Calling ChangeAnimatorPrefix");

#endif
                }
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
