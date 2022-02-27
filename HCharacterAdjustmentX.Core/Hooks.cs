//
// Hooks for HCharaAdjustmentX
//
using System;

using H;

using BepInEx.Logging;
using HarmonyLib;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
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
            }

            /// <summary>
            /// Set the new original position when changing positions not using the H point picker
            /// </summary>
            /// <param name="_nextAinmInfo"></param>
            [HarmonyPrefix]
            [HarmonyPatch(typeof(HSceneProc), nameof(HSceneProc.ChangeAnimator))]
            static private void ChangeAnimatorPrefix(
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
                }
                catch (Exception e)
                {
                    _Log.Level(LogLevel.Error, $"SHCA0023: Error - {e.Message}");
                }
            }
        }
    }
}
