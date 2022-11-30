// 
// Buttons interface handling
//
using System;
using System.Collections.Generic;

using BepInEx.Logging;

using IDHIUtils;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        public static HSceneProc Instance { get; internal set; }
        public static List<ChaControl> Heroines { get; internal set; }
        public static ChaControl Player { get; internal set; }

        private void OnHStart(object s, EventArgs e)
        {
            Hooks.Init();
            HProcMonitor.OnHSceneExiting += OnHProcExit;
            HProcMonitor.OnHSceneFinishedLoading += OnHProcFinishedLoading;

        }

        private void OnHProcExit(object s, EventArgs e)
        {
#if DEBUG
            _Log.Info($"HCAX0004: Removing patches and disabling HCAX.");
#endif
            SetControllerEnabled(false);
            if (_hprocInstance != null)
            {
                _hprocInstance = null;
            }
            enabled = false;
            try
            {
                _hookInstance.UnpatchSelf();
                _hookInstance = null;
            }
            catch (Exception ex) {
                _Log.Level(LogLevel.Error, $"HCAX0005: {ex}");
            }
#if DEBUG
            _Log.Info($"HCAX0006: Removing patches and disabling HCAX OK.");
#endif
            HProcMonitor.OnHSceneExiting -= OnHProcExit;
        }

        private void OnHProcFinishedLoading(
            object s, HProcMonitor.HSceneFinishedLoadingEventArgs e)
        {
#if DEBUG
            _Log.Info($"HCAX0007: Enabling HCAX.");
#endif
            SetupController(e.ObjectInstance);
            Heroines = e.Females;
            Player = e.Male;
            enabled = true;
            SetControllerEnabled(true);
            HProcMonitor.OnHSceneFinishedLoading -= OnHProcFinishedLoading;
        }

        private static void SetupController(object instance)
        {
            CharacterType chType;
            // HSceneProc instance will be used later
            _hprocInstance = (HSceneProc)instance;
            _hprocObject = instance;
            _hprocTraverse = new HSceneProcTraverse(instance);

            // set various flags
            Utils.SetMode(_hprocInstance.flags.mode);

            // verify if is a scene we support
            if (!IsSupportedScene)
            {
                _Log.Warning($"HCAX0008: The _mode {_mode}" +
                    $" is not supported.");
                return;
            }

            // Creates guides and disables the controllers
            for (var i = 0; i < _hprocInstance.flags.lstHeroine.Count; i++)
            {
                chType = (CharacterType)i;

                GetController(_hprocInstance.flags.lstHeroine[i].chaCtrl).Init(
                    _hprocInstance, chType);
            }

            GetController(_hprocInstance.flags.player.chaCtrl).Init(
                _hprocInstance, CharacterType.Player);
            // Group move guide off
            _hprocInstance.sprite.axis.tglDraw.isOn = false;
            _hprocInstance.guideObject.gameObject.SetActive(false);
        }

        /// <summary>
        /// Method to enable/disabled the characters controllers
        /// </summary>
        /// <param name="setState"></param>
        internal static void SetControllerEnabled(bool setState = true)
        {
            try
            {
                for (var i = 0; i < Heroines.Count; i++)
                {
                    try
                    {
                        GetController(Heroines[i]).enabled = setState;
#if DEBUG
                        _Log.Info($"HCAX0009: Controller {setEnabled(setState)} for {(CharacterType)i}");
#endif
                    }
                    catch (Exception e)
                    {
                        _Log.Level(LogLevel.Warning, $"HCAX0010: Error trying to " +
                            $"{setEnabled(setState)} the Controller for " +
                            $"{(CharacterType)i} - {e}");
                    }
                }

                try
                {
                    GetController(Player).enabled = setState;
#if DEBUG
                    _Log.Info($"HCAX0011: Controller {setEnabled(setState)} for Player");
#endif
                }
                catch (Exception e)
                {
                    _Log.Level(LogLevel.Warning, $"HCAX0012: Error trying to " +
                        $"{setEnabled(setState)} the Controller for Player - \n{e}");
                }
            }
            catch { _Log.Level(LogLevel.Error, $"HCAX0013: No Heroines found."); }
        }

        internal static readonly Func<bool, string> setEnabled = state =>
            state ? "set to enabled" : "set to disabled";
    }
}
