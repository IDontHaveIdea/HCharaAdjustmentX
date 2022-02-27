//
// HProcScene handle events for start and stop H Scene
// Enables SHCAdjustController operation is disable
// upon start and disabled on H Scene exit
//
using System;
using System.Collections.Generic;

using BepInEx.Logging;
using HarmonyLib;

using IDHIUtils;

using SHCA = IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public partial class HProcScene
    {
        static internal Harmony _hsHookInstance;
        static internal Type HSceneProcType;
        //static internal SHCA mother;

        #region public properties
        /// <summary>
        /// HProcScene Hooks loaded if true
        /// </summary>
        static public bool Kuuhou { get; internal set; }

        /// <summary>
        /// True if we are inside an HScene
        /// </summary>
        static public bool Nakadashi { get; internal set; }

        static public HSceneProc Instance { get; internal set; }
        /// <summary>
        /// Sprites
        /// </summary>
        static public List<HSprite> Sprites { get; internal set; } = new List<HSprite>();

        /// <summary>
        /// Female list
        /// </summary>
        static public List<ChaControl> Heroines { get; internal set; }

        /// <summary>
        /// Player
        /// </summary>
        static public ChaControl Player { get; internal set; }
        #endregion

        #region events
        static public event EventHandler OnHSceneStartLoading;
        static public event EventHandler OnHSceneExiting;
        static public event EventHandler<HSceneFinishedLoadingEventArgs> OnHSceneFinishedLoading;
        public class HSceneFinishedLoadingEventArgs : EventArgs
        {
            public HSceneProc Instance { get; }
            public List<ChaControl> Heroines { get; }
            public ChaControl Male { get; }
            public HSceneFinishedLoadingEventArgs(HSceneProc instance,
                List<ChaControl> lstFemale, ChaControl male)
            {
                Instance = instance;
                Heroines = lstFemale;
                Male = male;
            }
        }

        static internal void InvokeOnHSceneStartLoading(object _sender, EventArgs _args)
        {
            OnHSceneStartLoading?.Invoke(_sender, _args);
        }
        #endregion

        #region private methods
        static internal void Init()
        {
            //mother = obj;

            OnHSceneStartLoading += (_sender, _args) =>
            {
                SHCA._Log.Info($"SHCA0009: [OnHSceneStartLoading]");
            };

            OnHSceneFinishedLoading += (_sender, _args) =>
            {
                SHCA._Log.Info($"SHCA0010: [OnHSceneFinishedLoading]");
            };

            OnHSceneExiting += (_sender, _args) =>
            {
                SHCA._Log.Info($"SHCA0011: [OnHSceneExiting]");
            };
        }
        #endregion

        internal class HSHooks
        {
            static internal void Init()
            {
                _hsHookInstance = Harmony.CreateAndPatchAll(typeof(HSHooks));
                if (_hsHookInstance == null)
                {
                    SHCA._Log.Level(LogLevel.Error, $"SHCA0012: [CharHScene] Cannot patch the " +
                        $"system.");
                    throw new ApplicationException($"SHCA0012: [CharHScene] Cannot patch the " +
                        $"system.");
                }

                // Patch through reflection
                HSceneProcType = Type.GetType("HSceneProc, Assembly-CSharp");
                _hsHookInstance.Patch(HSceneProcType.GetMethod("OnDestroy", AccessTools.all),
                    prefix: new HarmonyMethod(typeof(HSHooks), nameof(HSHooks.OnDestroyPrefix)));
                _hsHookInstance.Patch(HSceneProcType.GetMethod("SetShortcutKey", AccessTools.all),
                    postfix: new HarmonyMethod(typeof(HSHooks), nameof(HSHooks.SetShortcutKeyPostfix)));
#if DEBUG
                SHCA._Log.Info($"SHCA0036: Patch seams OK.");
#endif
            }

            static private void OnDestroyPrefix()
            {
                OnHSceneExiting?.Invoke(null, null);
                Nakadashi = false;
                Kuuhou = false;
                Heroines = null;
                Sprites.Clear();
                _hsHookInstance.UnpatchSelf();
                _hsHookInstance = null;
            }

            static private void SetShortcutKeyPostfix(HSceneProc __instance,
                List<ChaControl> ___lstFemale, ChaControl ___male, HSprite ___sprite)
            {
                if (Kuuhou)
                {
                    SHCA._Log.Level(LogLevel.Warning, $"SHCA0013: [SetShortcutKey] Already loaded.");
                    return;
                }
                Kuuhou = true;
                Instance = __instance;
                Heroines = ___lstFemale;
                Player = ___male;
                Sprites.Add(___sprite);
                OnHSceneFinishedLoading?.Invoke(null, 
                    new HSceneFinishedLoadingEventArgs(__instance, ___lstFemale, ___male));
            }
        }
    }
}
