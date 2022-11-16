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
            public HSceneGuideObject GuideObject { get; set; } = null;

            /// <summary>
            /// Show the guide object associated with this character
            /// </summary>
            public void ShowGuideObject()
            {
                if ((GuideObject == null) || !IsSupportedScene) 
                { 
                    return; 
                }
                if (OriginalPosition == Vector3.zero)
                {
                    SetOriginalPosition();
                }
                //GuideObject.gameObject.SetActive(true);
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
            }

            /// <summary>
            /// Hide the guide object associated with this character
            /// </summary>
            public void HideGuideObject()
            {
                if ((GuideObject == null) || !IsSupportedScene)
                { 
                    return; 
                }
                //GuideObject.gameObject.SetActive(false);
            }

            /// <summary>
            /// Toggle the guide object on/off
            /// </summary>
            public void ToggleGuideObject()
            {
                if ((GuideObject == null) || !IsSupportedScene)
                {
                    return;
                }
                if (GuideObject.gameObject.activeInHierarchy)
                {
                    HideGuideObject();
                }
                else
                {
                    ShowGuideObject();
                }
            }

            /// <summary>
            /// Create a copy of the H scene guide object for each character
            /// </summary>
            internal void CreateGuideObject(HSceneProc hSceneProc, CharacterType characterType)
            {
                ChaType = characterType;
                //var female = hSceneProc.lstFemale[0];
                if (GuideObject == null)
                {
                    GuideObject = Instantiate(hSceneProc.guideObject);

                    //GuideObject = _hprocInstance.guideObject;
                    //GuideObject = Instantiate(hSceneProc.guideObject,
                    //    female.transform.position,
                    //    female.transform.rotation,
                    //    female.transform);
                    //GuideObject.transform
                    //    .SetPositionAndRotation(female.transform.position, female.transform.rotation);
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
                    _Log.Info($"HCAX0033: Creating character guide object for {characterType}");
#endif
                }
            }
        }
    }
}

