//
// HProcScene handle events for start and stop H Scene
// Enables HCAXdjustController operation is disable
// upon start and disabled on H Scene exit
//

/*
using System;
using System.Collections.Generic;

using BepInEx.Logging;
using HarmonyLib;

using IDHIUtils;

using HCAX = IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public partial class HProcScene
    {
        internal static Harmony _hsHookInstance;
        internal static Type HSceneProcType;
        //static internal HCAX mother;

        #region public properties
        /// <summary>
        /// HProcScene Hooks loaded if true
        /// </summary>
        public static bool Kuuhou { get; internal set; }

        /// <summary>
        /// True if we are inside an HScene
        /// </summary>
        public static bool Nakadashi { get; internal set; }
        #endregion

        #region events
        public static event EventHandler OnHSceneStartLoading;
        public static event EventHandler OnHSceneExiting;
        public static event EventHandler<HSceneFinishedLoadingEventArgs> OnHSceneFinishedLoading;
        public class HSceneFinishedLoadingEventArgs : EventArgs
        {
            public object Instance { get; }
            public List<ChaControl> Heroines { get; }
            public ChaControl Male { get; }
            public HSceneFinishedLoadingEventArgs(object instance,
                List<ChaControl> lstFemale, ChaControl male)
            {
                Instance = instance;
                Heroines = lstFemale;
                Male = male;
            }
        }

        internal static void InvokeOnHSceneStartLoading(object _sender, EventArgs _args)
        {
            OnHSceneStartLoading?.Invoke(_sender, _args);
        }
        #endregion

        #region private methods
        internal static void Init()
        {
            //mother = obj;

            OnHSceneStartLoading += (_sender, _args) =>
            {
                HCAX._Log.Info($"HCAX0009: [OnHSceneStartLoading]");
            };

            OnHSceneFinishedLoading += (_sender, _args) =>
            {
                HCAX._Log.Info($"HCAX0010: [OnHSceneFinishedLoading]");
            };

            OnHSceneExiting += (_sender, _args) =>
            {
                HCAX._Log.Info($"HCAX0011: [OnHSceneExiting]");
            };
        }
        #endregion

        internal class HSHooks
        {
            internal static void Init()
            {
                _hsHookInstance = Harmony.CreateAndPatchAll(typeof(HSHooks));
                if (_hsHookInstance == null)
                {
                    HCAX._Log.Level(LogLevel.Error, $"HCAX0012: [CharHScene] Cannot patch the " +
                        $"system.");
                    throw new ApplicationException($"HCAX0012: [CharHScene] Cannot patch the " +
                        $"system.");
                }

                // Patch through reflection
                HSceneProcType = Type.GetType("HSceneProc, Assembly-CSharp");
                _hsHookInstance.Patch(HSceneProcType.GetMethod("OnDestroy", AccessTools.all),
                    prefix: new HarmonyMethod(typeof(HSHooks), nameof(HSHooks.OnDestroyPrefix)));
                _hsHookInstance.Patch(HSceneProcType.GetMethod("SetShortcutKey", AccessTools.all),
                    postfix: new HarmonyMethod(typeof(HSHooks), nameof(HSHooks.SetShortcutKeyPostfix)));
#if DEBUG
                HCAX._Log.Info($"HCAX0036: Patch seams OK.");
#endif
            }

            private static void OnDestroyPrefix()
            {
                OnHSceneExiting?.Invoke(null, null);
                Nakadashi = false;
                Kuuhou = false;
                _hsHookInstance.UnpatchSelf();
                _hsHookInstance = null;
            }

            private static void SetShortcutKeyPostfix(object __instance,
                List<ChaControl> ___lstFemale, ChaControl ___male)
            {
                if (Kuuhou)
                {
                    HCAX._Log.Level(LogLevel.Warning, $"HCAX0013: [SetShortcutKey] Already loaded.");
                    return;
                }
                Kuuhou = true;
                Nakadashi = true;
                OnHSceneFinishedLoading?.Invoke(null, 
                    new HSceneFinishedLoadingEventArgs(__instance, ___lstFemale, ___male));
            }
        }
    }
}
*/
