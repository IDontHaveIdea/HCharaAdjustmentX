// 
// Buttons interface handling
//
using System;
using System.Collections.Generic;

using UnityEngine.SceneManagement;

using BepInEx.Logging;

//using static IDHIPlugins.HProcScene;
using static IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        /// <summary>
        /// HSceneProc instance
        /// </summary>
        static public HSceneProc Instance { get; internal set; }

        /// <summary>
        /// Female list
        /// </summary>
        static public List<ChaControl> Heroines { get; internal set; }

        /// <summary>
        /// Player
        /// </summary>
        static public ChaControl Player { get; internal set; }


        /// <summary>
        /// Wait for screen with name HProc this is a H scene loading.
        /// This will enable HProcScene and SHCAdjustController
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        private void MonitorHProc(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.name == "HProc")
            {
                _sceneName = scene.name;
                Hooks.Init();
                HProcScene.HSHooks.Init();
                HProcScene.Nakadashi = true;
                HProcScene.OnHSceneExiting += OnHProcExit;
                HProcScene.OnHSceneFinishedLoading += OnHProcFinishedLoading;
                HProcScene.InvokeOnHSceneStartLoading(null, null);
            }
        }

        private void OnHProcExit(object s, EventArgs e)
        {
#if DEBUG
            _Log.Info($"SHCA0024: Removing patches and disabling SHCA.");
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
                _Log.Level(LogLevel.Error, $"SHCA0034: {ex}");
            }
#if DEBUG
            _Log.Info($"SHCA0025: Removing patches and disabling SHCA OK.");
#endif
            HProcScene.OnHSceneExiting -= OnHProcExit;
        }

        private void OnHProcFinishedLoading(object s, HProcScene.HSceneFinishedLoadingEventArgs e)
        {
#if DEBUG
            _Log.Info($"SHCA0041: Enabling SHCA..");
#endif
            SetupController(e.Instance);
            Heroines = e.Heroines;
            Player = e.Male;
            enabled = true;
            SetControllerEnabled(true);
            HProcScene.OnHSceneFinishedLoading -= OnHProcFinishedLoading;
        }

        static private void SetupController(HSceneProc instance)
        {
            CharacterType chType;
            _hprocInstance = instance;  // HSceneProc instance will be used later

            // set various flags
            Utils.SetMode(_hprocInstance.flags.mode);

            // verify if is a scene we support
            if (!IsSupportedScene)
            {
                _Log.Warning($"SHCA0006: The _mode {_mode}" +
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
            _hprocInstance.sprite.axis.tglDraw.isOn = false;
            _hprocInstance.guideObject.gameObject.SetActive(false);
        }

        /// <summary>
        /// Method to enable/disabled the characters controllers
        /// </summary>
        /// <param name="setState"></param>
        static internal void SetControllerEnabled(bool setState = true)
        {
            try
            {
                for (var i = 0; i < Heroines.Count; i++)
                {
                    try
                    {
                        GetController(Heroines[i]).enabled = setState;
#if DEBUG
                        _Log.Info($"SHCA0014: Controller {setEnabled(setState)} for {(CharacterType)i}");
#endif
                    }
                    catch (Exception e)
                    {
                        _Log.Level(LogLevel.Warning, $"SHCA0016: Error trying to " +
                            $"{setEnabled(setState)} the Controller for {(CharacterType)i} - {e}");
                    }
                }

                try
                {
                    GetController(Player).enabled = setState;
#if DEBUG
                    _Log.Info($"SHCA0015: Controller {setEnabled(setState)} for Player");
#endif
                }
                catch (Exception e)
                {
                    _Log.Level(LogLevel.Warning, $"SHCA0017: Error trying to " +
                        $"{setEnabled(setState)} the Controller for Player - \n{e}");
                }
            }
            catch { _Log.Level(LogLevel.Error, $"No Heroines found."); }
        }

        static readonly internal Func<bool, string> setEnabled = state =>
            state ? "set to enabled" : "set to disabled";
    }
}
