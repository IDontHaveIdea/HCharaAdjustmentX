// 
// Buttons interface handling
//
using System.Collections.Generic;
using System.Linq;

using KKAPI;

using UnityEngine;

using IDHIUtils;
using static IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        #region private fields
        // TODO: Use a properties class to have one dictionary
        internal static Dictionary<CharacterType, ButtonsInterface> _buttonsInterface =
            new()
            {
                { CharacterType.Heroine, new ButtonsInterface(CharacterType.Heroine) },
                { CharacterType.Player, new ButtonsInterface(CharacterType.Player) },
                { CharacterType.Heroine3P, new ButtonsInterface(CharacterType.Heroine3P) }
            };
        internal static Dictionary<CharacterType, List<MoveActionButton>> _botones =
            new()
            {
                { CharacterType.Heroine, null },
                { CharacterType.Heroine3P, null },
                { CharacterType.Player, null }
            };
        #endregion

        #region Properties
        internal static bool ShowGroupGuide
        {
            get
            {
                return _showGroupGuide;
            }

            set
            {
                _showGroupGuide = value;
                if (value)
                {
                    _showGroupGuide = CanShow();
                }
                ToggleGroupGuideObject(_showGroupGuide);
            }
        }
        #endregion

        #region Unity methods
        private void OnGUI()
        {
            //
            // Rect(x, y, xMax, yMax)
            //
            if (_buttonsInterface[CharacterType.Heroine].ShowInterface)
            {
                // Have _buttons to show
                // TODO: check how not to execute every frame
                if (_botones[CharacterType.Heroine].Count > 0)
                {
                    foreach (var moveButton in _botones[CharacterType.Heroine])
                    {
                        GUI.contentColor = moveButton.ForegroundColor;
                        GUI.backgroundColor = _SvgColor.mediumorchid;
                        GUI.backgroundColor.AlphaMultiplied(1f);
                        if (GUI.Button(moveButton.Position, moveButton.Text))
                        {
                            moveButton.TriggerEvent();
                        }
                    }
                }
            }

            if (_buttonsInterface[CharacterType.Player].ShowInterface)
            {
                if (_botones[CharacterType.Player]?.Count > 0)
                {
                    foreach (var moveButton in _botones[CharacterType.Player])
                    {
                        GUI.contentColor = moveButton.ForegroundColor;
                        GUI.backgroundColor = _SvgColor.blue;
                        GUI.backgroundColor.AlphaMultiplied(1f);
                        if (GUI.Button(moveButton.Position, moveButton.Text))
                        {
                            moveButton.TriggerEvent();
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (GroupGuide.Value.IsDown())
            {
                ShowGroupGuide = !ShowGroupGuide;
            }
        }
        #endregion

        #region private methods
        private static void SetupInterface(CharacterType chaType)
        {
            var _chaControl = chaType switch
            {
                CharacterType.Heroine => Heroines[0],
                CharacterType.Player => Player,
                CharacterType.Heroine3P => Heroines[1],
                _ => null,
            };

            if (_chaControl != null)
            {
                if (_botones[chaType]?.Count > 0)
                {
                    return;
                }
            }

            _botones[chaType] = GetControllerByType(chaType).buttons.ToList();
        }

        /// <summary>
        /// Toogle the group guide object on/off
        /// </summary>
        public static void ToggleGroupGuideObject(bool state)
        {
            _Log.Warning($"[ToggleGroupGuideObject] Group Guide set to={state}");
            _hprocInstance.sprite.axis.tglDraw.isOn = state;
            _hprocInstance.sprite.MoveAxisDraw(_hprocInstance.sprite.axis.tglDraw.isOn);
            _hprocInstance.guideObject.gameObject.SetActive(state);
        }

        /// <summary>
        /// Conditions when the group guide should be hidden
        /// </summary>
        /// <returns></returns>
        private static bool CanShow()
        {
            // character is not showing
            if (!Player.visibleAll)
            {
                return false;
            }
            // Some pop up dialog on
            if (SceneApi.GetIsOverlap())
            {
                return false;
            }
            // scene still loading or start to exit
            if (SceneApi.GetIsNowLoadingFade())
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}
