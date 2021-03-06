using System;
using System.Collections.Generic;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using KKAPI;
using KKAPI.Chara;
using KKAPI.Maker;
using KKAPI.Maker.UI.Sidebar;
using KKAPI.Utilities;

using UnityEngine;
using UnityEngine.SceneManagement;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX;

namespace IDHIPlugins
{
    public class Utils
    {
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
        /// Move characters to saved original position
        /// </summary>
        /// <param name="message"></param>
        internal static void ResetPositionAll(string message = null)
        {
            if (_hprocInstance == null)
            {
                return;
            }
#if DEBUG
            if (message != null)
            {
                _Log.Info($"SHCA0018: [ResetPositionAll]  - {message}");
            }
#endif
            var heroines = _hprocInstance.flags.lstHeroine;
            for (var i = 0; i < heroines.Count; i++)
            {
                GetController(heroines[i].chaCtrl).ResetPosition();
            }
            GetController(_hprocInstance.flags.player.chaCtrl).ResetPosition();
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
#if DEBUG
            if (message != null)
            {
                _Log.Info($"SHCA0019: [SetOrigianalPositionAll]  - {message}");
            }
#endif
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
            if (message != null)
            {
                _Log.Info($"SHCA0020: [RecalcAdjustmentAll]  - {message}");
            }
#endif
            var heroines = _hprocInstance.flags.lstHeroine;
            for (var i = 0; i < heroines.Count; i++)
            {
                GetController(heroines[i].chaCtrl).DoRecalc = true;
            }
            GetController(_hprocInstance.flags.player.chaCtrl).DoRecalc = true;
        }

        /// <summary>
        /// Determine if there is a change in original position
        /// </summary>
        /// <param name="chaControl"></param>
        /// <returns></returns>
        internal static bool IsNewPosition(ChaControl chaControl)
        {
            var controller = GetController(chaControl);
            var newPosition = chaControl.transform.position;
            var originalPosition = controller._originalPosition;
            var lastMovePosition = controller._lastMovePosition;
            if (newPosition != originalPosition && newPosition != lastMovePosition)
            {
                return true;
            }
            return false;
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
