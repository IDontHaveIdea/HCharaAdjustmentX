//
// Utils.cs
//
using System;
using System.Collections.Generic;

using UnityEngine;

using BepInEx.Logging;

using IDHIUtils;

using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;
using static IDHIPlugins.HCharaAdjustmentX;
using static HandCtrl;


namespace IDHIPlugins
{
    public class Utils
    {
        /// <summary>
        /// Return categories in the string form "{ cat 1, cat 2, ,,,}"
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="names"></param>
        /// <param name="quotes"></param>
        /// <returns></returns>
        internal static string CategoryList(
            List<HSceneProc.Category> categories, bool names = false, bool quotes = true)
        {
            var tmp = "";
            var first = true;

            foreach (var c in categories)
            {
                if (first)
                {
                    if (names)
                    {
                        tmp += (PositionCategory)c.category;
                    }
                    else
                    {
                        tmp += c.category.ToString();
                    }
                    first = false;
                }
                else
                {
                    if (names)
                    {
                        tmp += ", " + (PositionCategory)c.category;
                    }
                    else
                    {
                        tmp += ", " + c.category.ToString();
                    }
                }
            }
            return quotes ? "\" { " + tmp + " }\"" : "{ " + tmp + " }";
        }

        /// <summary>
        /// Ditto
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="names"></param>
        /// <param name="quotes"></param>
        /// <returns></returns>
        internal static string CategoryList(
            List<int> categories, bool names = false, bool quotes = true)
        {
            var tmp = "";
            var first = true;

            foreach (var c in categories)
            {
                if (first)
                {
                    if (names)
                    {
                        tmp += (PositionCategory)c;
                    }
                    else
                    {
                        tmp += c.ToString();
                    }
                    first = false;
                }
                else
                {
                    if (names)
                    {
                        tmp += ", " + (PositionCategory)c;
                    }
                    else
                    {
                        tmp += ", " + c.ToString();
                    }
                }
            }
            return quotes ? "\" { " + tmp + " }\"" : "{ " + tmp + " }";
        }

        internal static void CheckAnimationLoader()
        {
            if (_animationLoader.Installed)
            {
                if (_animationLoader.VersionAtLeast("1.1.2.2"))
                {
                    _animationKeyOk = true;
                }
                else
                {
                    _animationKeyOk = false;
                }
                if (_animationLoader.VersionAtLeast("1.1.3.0"))
                {
                    _animationMovementOk = true;
                }
                else
                {
                    _animationMovementOk = false;
                }
            }
        }

        /// <summary>
        /// Get animation key
        /// </summary>
        /// <param name="animation"></param>
        /// <returns></returns>
        internal static string GetAnimationKey(HSceneProc.AnimationListInfo animation)
        {
            var result = "";
            if (_animationKeyOk)
            {
                try
                {
                    result = _animationLoader
                            .GetAnimationKey(animation);
                }
                catch (Exception e)
                {
                    _animationKeyOk = false;
                    _Log.Level(LogLevel.Error, $"HCAX0024A: Error - {e.Message}");
                }
            }
            return result;
        }


        internal static List<Vector3> GetAnimationMovement(
            HSceneProc.AnimationListInfo animation)
        {
            List<Vector3> result = new() {
                new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

            if (animation != null)
            {
                if (_animationMovementOk)
                {
                    try
                    {
                        result = _animationLoader
                            .GetAnimationMovement(animation);
                    }
                    catch(Exception e)
                    {
                        _animationMovementOk = false;
                        _Log.Level(LogLevel.Error, $"HCAX0024B: Error - {e.Message}");
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Determine if there is a change in original position
        /// </summary>
        /// <param name="chaControl"></param>
        /// <returns></returns>
        internal static bool IsNewPosition(ChaControl chaControl)
        {
            var controller = GetController(chaControl);
            var currentPosition = _hprocTraverse.nowHpointDataPos;
            var originalPosition = controller.OriginalPosition;
            if (currentPosition != originalPosition)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if there is a change in original position
        /// </summary>
        /// <param name="chaControl"></param>
        /// <returns></returns>
        internal static bool IsSamePosition(ChaControl chaControl)
        {
            return !IsNewPosition(chaControl);
        }

        /// <summary>
        /// Show some information for Heroine 1
        /// </summary>
        /// <param name="instance"></param>
        internal static void InitialPositionInfo(HSceneProc instance)
        {
            //if (!HProcScene.Nakadashi || (instance == null))
            if (!HProcMonitor.Nakadashi || (instance == null))
            {
                return;
            }
            var tmp = instance.flags.lstHeroine[0].chaCtrl.transform;
        }

        /// <summary>
        /// Show some information for Heroine 1
        /// </summary>
        /// <param name="instance"></param>
        internal static void InitialPosition()
        {
            if (_hprocInstance == null)
            {
                return;
            }
            if (_animationKey.IsNullOrEmpty())
            {
                return;
            }

            var heroines = _hprocInstance.flags.lstHeroine;
            CTRL ctrl;

            for (var i = 0; i < heroines.Count; i++)
            {
                ctrl = GetController(heroines[i].chaCtrl);
                if (ctrl.MoveData.Data.Count > 0)
                {
                    ctrl.MoveData.Data.TryGetValue(_animationKey,
                        out var position);
                    if (position != null)
                    {
                        // Use TryGetValue
                        position.TryGetValue(ctrl.ChaType, out var data);
                        if (data != null)
                        {
                            var movement = data.Position;
                            ctrl.Movement = movement;
                            CTRL.InvokeOnMoveRequest(null,
                                new CTRL.MoveRequestEventArgs(
                                    ctrl.ChaType, MoveEvent.MoveType.MOVE));
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Move characters to saved original position
        /// </summary>
        /// <param name="message"></param>
        internal static void ResetPositionAll()
        {
            if (_hprocInstance == null)
            {
                return;
            }
            // Get calling method name
            var callingMethod = Utilities.CallingMethod();
            _Log.Warning($"[{callingMethod}] ResetAllPositions");
            var heroines = _hprocInstance.flags.lstHeroine;
            CTRL ctrl;
            for (var i = 0; i < heroines.Count; i++)
            {
                ctrl = GetController(heroines[i].chaCtrl);
                if (IsSamePosition(heroines[i].chaCtrl))
                {
                    ctrl.ResetPosition();
                }
            }
            ctrl = GetController(_hprocInstance.flags.player.chaCtrl);
            if (IsSamePosition(_hprocInstance.flags.player.chaCtrl))
            {
                ctrl.ResetPosition();
            }
        }

        /// <summary>
        /// Set new original position for characters if there is a move 
        /// from original position saved
        /// </summary>
        /// <param name="message"></param>
        internal static void SetOriginalPositionAll(
            HSceneProc.AnimationListInfo _nextAinmInfo = null)
        {
            if (_hprocInstance == null)
            {
                return;
            }

            List<Vector3> movement = new() {
                new Vector3(0, 0, 0), new Vector3(0, 0, 0) };

            if (_nextAinmInfo != null)
            {
                if (_animationMovementOk)
                {
                    movement = Utils
                        .GetAnimationMovement(_nextAinmInfo);
                }
            }

            var heroines = _hprocInstance.flags.lstHeroine;
            for (var i = 0; i < heroines.Count; i++)
            {
                if (IsNewPosition(heroines[i].chaCtrl))
                {
                    var ctrl = GetController(heroines[i].chaCtrl);
                    if ((_nextAinmInfo is not null) && (i == 0))
                    {
                        if (_animationMovementOk)
                        {
                            ctrl.ALMovement = movement[(int)Sex.Female];
                        }
                    }
                    ctrl.SetOriginalPosition();
                }
            }
            if (IsNewPosition(_hprocInstance.flags.player.chaCtrl))
            {
                var ctrl = GetController(_hprocInstance.flags.player.chaCtrl);
                if (_nextAinmInfo is not null)
                {
                    if (_animationMovementOk)
                    {
                        ctrl.ALMovement = movement[(int)Sex.Male];
                    }
                }
                ctrl.SetOriginalPosition();
            }
        }

        /// <summary>
        /// Set new original position for characters if there is a move 
        /// from original position saved
        /// </summary>
        /// <param name="message"></param>
        internal static void SetOriginalPositionAllBad(string message = null)
        {
            if (_hprocInstance == null)
            {
                return;
            }
            var heroines = _hprocInstance.flags.lstHeroine;
            for (var i = 0; i < heroines.Count; i++)
            {
                GetController(heroines[i].chaCtrl).SetOriginalPosition();
                if (i == 0)
                {
                    Utils.InitialPositionInfo(_hprocInstance);
                }                
            }
            GetController(_hprocInstance.flags.player.chaCtrl).SetOriginalPosition();
        }

        /// <summary>
        /// Save H _mode flag
        /// </summary>
        /// <param name="emode"></param>
        internal static void SetMode(HFlag.EMode emode)
        {
            // set various flags
            _mode = emode;
            IsAibu = (emode == HFlag.EMode.aibu);
            IsHoushi = (emode == HFlag.EMode.houshi);
            IsSonyu = (emode == HFlag.EMode.sonyu);
            IsSupportedScene = (IsHoushi || IsSonyu);
        }

        /// <summary>
        /// Turn on recalculation flag
        /// </summary>
        /// <param name="message"></param>
        internal static void RecalcAdjustmentAll(string message = null)
        {
            if (_hprocInstance == null)
            {
                return;
            }
            var heroines = _hprocInstance.flags.lstHeroine;
            for (var i = 0; i < heroines.Count; i++)
            {
                GetController(heroines[i].chaCtrl).DoRecalc = true;
            }
            GetController(_hprocInstance.flags.player.chaCtrl).DoRecalc = true;
        }
    }
}
