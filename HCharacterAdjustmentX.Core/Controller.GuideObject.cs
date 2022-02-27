//
// _guideObject related methods
//
using HSceneUtility;

using UnityEngine;

using KKAPI.Chara;

using IDHIUtils;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        public partial class HCharaAdjusmentXController : CharaCustomFunctionController
        {
            public HSceneGuideObject _guideObject;

            /// <summary>
            /// Create a copy of the H scene guide object for each character
            /// </summary>
            internal void CreateGuideObject(HSceneProc hSceneProc, CharacterType characterType)
            {
                _chaType = characterType;
                if (_guideObject == null)
                {
                    _guideObject = Instantiate(hSceneProc.guideObject);
#if KKSWish
                    var rot = ChaControl.transform.rotation;
                    Vector3 pos = ChaControl.transform.position;
                    Vector3 transPos = ChaControl.transform.TransformVector(pos);
                    _guideObject.amountOffset.position = transPos;
                    _guideObject.amountOffset.rotation = rot.eulerAngles;
                    _guideObject.amountTotal.position = _guideObject.amount.position
                        + _guideObject.amountOffset.position;
                    _guideObject.amountTotal.rotation = _guideObject.amount.rotation
                        + _guideObject.amountOffset.rotation;
#endif
#if DEBUG
                    _Log.Info($"SHCA0026: Creating character guide object for {characterType}");
#endif
                }
            }

            /// <summary>
            /// Show the guide object associated with this character
            /// </summary>
            public void ShowGuideObject()
            {
                if ((_guideObject == null) || !IsSupportedScene) 
                { 
                    return; 
                }
                if (_originalPosition == Vector3.zero)
                {
                    SetOriginalPosition();
                }
                _guideObject.gameObject.SetActive(true);
#if KKSWish
                var rot = ChaControl.transform.rotation;
                Vector3 pos = ChaControl.transform.position;
                Vector3 transPos = ChaControl.transform.TransformVector(pos);
                _guideObject.amountOffset.position = transPos;
                _guideObject.amountOffset.rotation = rot.eulerAngles;
                _guideObject.amountTotal.position = _guideObject.amount.position
                    + _guideObject.amountOffset.position;
                _guideObject.amountTotal.rotation = _guideObject.amount.rotation
                    + _guideObject.amountOffset.rotation;
#endif

#if DEBUG
                _Log.Info($"Guide should show.");
#endif
            }

            /// <summary>
            /// Hide the guide object associated with this character
            /// </summary>
            public void HideGuideObject()
            {
                if ((_guideObject == null) || !IsSupportedScene)
                { 
                    return; 
                }
                _guideObject.gameObject.SetActive(false);
#if DEBUG
                _Log.Info($"Guide should hide");
#endif
            }

            /// <summary>
            /// Toggle the guide object on/off
            /// </summary>
            public void ToggleGuideObject()
            {
                if ((_guideObject == null) || !IsSupportedScene)
                {
                    return;
                }
                if (_guideObject.gameObject.activeInHierarchy)
                {
                    HideGuideObject();
                }
                else
                {
                    ShowGuideObject();
                }
            }
        }
    }
}

