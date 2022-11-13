//
// Utils.cs
//
using System.Collections.Generic;

using BepInEx.Logging;

using IDHIUtils;

using UnityEngine.SceneManagement;

using static FaceScreenShot;
using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;
using static IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public class Utils
    {
        public enum PositionCategory
        {
            LieDown = 0,
            Stand = 1,
            SitChair = 2,
            Stool = 3,
            SofaBench = 4,
            BacklessBench = 5,
            SchoolDesk = 6,
            Desk = 7,
            Wall = 8,
            StandPool = 9,
            SitDesk = 10,
            SquadDesk = 11,
            Pool = 1004,
            Ground3P = 1100,
            AquariumCrowded = 1304,
        }

        /// <summary>
        /// Return categories in the string form "{ cat 1, cat 2, ,,,}"
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="names"></param>
        /// <param name="quotes"></param>
        /// <returns></returns>
        internal static string CategoryList(List<HSceneProc.Category> categories, bool names = false, bool quotes = true)
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
        internal static string CategoryList(List<int> categories, bool names = false, bool quotes = true)
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

        /// <summary>
        /// Determine if there is a change in original position
        /// </summary>
        /// <param name="chaControl"></param>
        /// <returns></returns>
        internal static bool IsNewPosition(ChaControl chaControl)
        {
            var controller = GetController(chaControl);
            var currentPosition = chaControl.transform.position;
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
            var controller = GetController(chaControl);
            var currentPosition = chaControl.transform.position;
            var lastMovePosition = controller.LastMovePosition;
            if (currentPosition == lastMovePosition)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Show some information for Heroine 1
        /// </summary>
        /// <param name="instance"></param>
        internal static void InitialPositionInfo(HSceneProc instance)
        {
            if (!HProcScene.Nakadashi || (instance == null))
            {
                return;
            }
            var tmp = instance.flags.lstHeroine[0].chaCtrl.transform;
            _Log.Info($"SHCA0016: Scene -  {_sceneName} Female transform position ("
                + $"{tmp.position.x}, {tmp.position.y}, {tmp.position.z})");
        }

        /// <summary>
        /// Show some information for Heroine 1
        /// </summary>
        /// <param name="instance"></param>
/*        internal static void InitialPosition()
        {
            if (_hprocInstance == null)
            {
                return;
            }
#if DEBUG
            var calllingMethod = Utilities.CallingMethod();
            _Log.Info($"[InitialPosition] Called by - [{calllingMethod}]");
#endif
            var heroines = _hprocInstance.flags.lstHeroine;
            HCharaAdjusmentXController ctrl;
            for (var i = 0; i < heroines.Count; i++)
            {
                ctrl = GetController(heroines[i].chaCtrl);
                if (ctrl.MoveData.Count > 0)
                {
                    ctrl.MoveData.Data.TryGetValue(_animationKey,
                        out var position);
                    if (position != null)
                    {
                        // Use TryGetValue
                        position.TryGetValue(ctrl.ChaType, out var data);
                        if (data != null)
                        {
                            var movement = data.PositionAdjustment;
                            ctrl.Movement = movement;
                            CTRL.InvokeOnMoveRequest(null,
                                new CTRL.MoveRequestEventArgs(
                                    ctrl.ChaType, MoveEvent.MoveType.MOVE));
                            _Log.Info($"[InitialPosition] InvokeOnMoveRequest - [{movement}]");
                        }
                    }
                }
            }
            / * ctrl = GetController(_hprocInstance.flags.player.chaCtrl);
            if (ctrl.Moved
                && IsSamePosition(_hprocInstance.flags.player.chaCtrl))
            {
                ctrl.ResetPosition();
            } * /
        }*/

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
#if DEBUG
            var calllingMethod = Utilities.CallingMethod();
            _Log.Info($"[CInitialPosition] Called by - [{calllingMethod}]");
#endif
            var heroines = _hprocInstance.flags.lstHeroine;
            HCharaAdjusmentXController ctrl;
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
                            _Log.Info($"[InitialPosition] InvokeOnMoveRequest - [{movement}]");
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
#if DEBUG
            var calllingMethod = Utilities.CallingMethod();
            _Log.Info($"SHCA0018: [ResetPositionAll] Called by - [{calllingMethod}]");
#endif
            var heroines = _hprocInstance.flags.lstHeroine;
            HCharaAdjusmentXController ctrl;
            for (var i = 0; i < heroines.Count; i++)
            {
                ctrl = GetController(heroines[i].chaCtrl);
                _Log.Error($"Origial Position {ctrl.OriginalPosition} " +
                    $"current {heroines[i].chaCtrl.transform.position} " +
                    $"_lastMovedPostion={ctrl.LastMovePosition}" +
                    $"IsNewPositoin={IsNewPosition(heroines[i].chaCtrl)} " +
                    $"IsSamePosition={IsSamePosition(heroines[i].chaCtrl)}" +
                    $"Moved={ctrl.Moved}");
                if (ctrl.Moved && IsSamePosition(heroines[i].chaCtrl))
                {
                    ctrl.ResetPosition();
                }
            }
            ctrl = GetController(_hprocInstance.flags.player.chaCtrl);
            if (ctrl.Moved
                && IsSamePosition(_hprocInstance.flags.player.chaCtrl))
            {
                ctrl.ResetPosition();
            }
        }

        /// <summary>
        /// Set new original position for characters if there is a move 
        /// from original position saved
        /// </summary>
        /// <param name="message"></param>
        internal static void SetOriginalPositionAll(string message = null)
        {
            if (_hprocInstance == null)
            {
                return;
            }
//#if DEBUG
//            var calllingMethod = Utilities.CallingMethod();
//            _Log.Info($"SHCA0019: [SetOrigianalPositionAll] Called by - [{calllingMethod}]");
//#endif
            var heroines = _hprocInstance.flags.lstHeroine;
            for (var i = 0; i < heroines.Count; i++)
            {
                if (IsNewPosition(heroines[i].chaCtrl))
                {
                    GetController(heroines[i].chaCtrl).SetOriginalPosition();
#if DEBUG
                    if (i == 0)
                    {
                        Utils.InitialPositionInfo(_hprocInstance);
                    }
#endif
                }
            }
            if (IsNewPosition(_hprocInstance.flags.player.chaCtrl))
            {
                GetController(_hprocInstance.flags.player.chaCtrl).SetOriginalPosition();
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
#if DEBUG
            var calllingMethod = Utilities.CallingMethod();
            _Log.Info($"SHCA0019: [SetOrigianalPositionAll] Called by - [{calllingMethod}]");
#endif
            var heroines = _hprocInstance.flags.lstHeroine;
            for (var i = 0; i < heroines.Count; i++)
            {
/*                if (IsPositionMoved(heroines[i].chaCtrl))
                {
                    GetController(heroines[i].chaCtrl).ResetPosition();
#if DEBUG
                    _Log.Info($"[SetOrigianalPositionAll] Called by - [{calllingMethod}] Found is heroine moved position.");
#endif
                }*/
                GetController(heroines[i].chaCtrl).SetOriginalPosition();
                if (i == 0)
                {
                    Utils.InitialPositionInfo(_hprocInstance);
                }                
            }
            /*if (IsPositionMoved(_hprocInstance.flags.player.chaCtrl))
            {
                GetController(_hprocInstance.flags.player.chaCtrl).ResetPosition();
#if DEBUG
                _Log.Info($"SHCA0019: [SetOrigianalPositionAll] Called by - [{calllingMethod}] Found is player move position.");

#endif
            }*/

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
            IsSupportedScene = (IsAibu || IsHoushi || IsSonyu);
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
#if DEBUG
            var calllingMethod = Utilities.CallingMethod();
            _Log.Info($"SHCA0020: [RecalcAdjustmentAll] Called by - [{calllingMethod}]");
#endif
            var heroines = _hprocInstance.flags.lstHeroine;
            for (var i = 0; i < heroines.Count; i++)
            {
                GetController(heroines[i].chaCtrl).DoRecalc = true;
            }
            GetController(_hprocInstance.flags.player.chaCtrl).DoRecalc = true;
        }

#region public methods
        /// <summary>
        /// Save active scene on scene change
        /// </summary>
        /// <param name="currentScene"></param>
        /// <param name="newScene"></param>
        public static void SceneChanged(Scene previousScene, Scene newScene)
        {
            _activeScene = newScene.name;
        }
#endregion
    }
}
