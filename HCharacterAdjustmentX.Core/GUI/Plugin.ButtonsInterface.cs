// 
// Buttons interface handling
//
using KKAPI;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        #region private classes
        internal class ButtonsInterface
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Style",
                "IDE0044:Add readonly modifier",
                Justification = "Field is dependent on the class instance.")]
            private CharacterType _chaType;
            private bool _showInterface;
            private ChaControl _chaControl;

            public ButtonsInterface(CharacterType chaType)
            {
                _chaType = chaType;
                _showInterface = false;
            }

            public bool ShowInterface
            {
                get
                {
                    if (_showInterface)
                    {
                        return CanShow();
                    }
                    return _showInterface;
                }
                set
                {
                    _showInterface = value;
                    if (_botones[_chaType] != null)
                    {
                        _botones[_chaType]?.Clear();
                    }
                    if (!value)
                    {
                        return;
                    }
                    SetupInterface(_chaType);
                }
            }

            private bool CanShow()
            {
                //if (!HProcScene.Nakadashi)
                if (!HProcMonitor.Nakadashi)
                {
                    return false;
                }

                switch (_chaType)
                {
                    case CharacterType.Heroine:
                        _chaControl = Heroines[0];
                        break;
                    case CharacterType.Player:
                        _chaControl = Player;
                        break;
                    case CharacterType.Heroine3P:
                        _chaControl = Heroines[1];
                        break;
                    case CharacterType.Janitor:
                        break;
                    case CharacterType.Group:
                        break;
                    case CharacterType.Unknown:
                        break;
                    default:
                        _chaControl = null;
                        break;
                }

                if (_chaControl == null)
                {
                    return false;
                }
                if (!_chaControl.visibleAll)
                {
                    return false;
                }
                if (SceneApi.GetAddSceneName() == "Config")
                {
                    return false;
                }
                if (SceneApi.GetIsOverlap())
                {
                    return false;
                }
                if (SceneApi.GetIsNowLoadingFade())
                {
                    return false;
                }
                return true;
            }
        }
        #endregion
    }
}
