//
// Character Controller
//
using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

using KKAPI;
using KKAPI.Chara;

using IDHIUtils;

namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        // Controller
        #region  Fields
        internal const string MoveDataID = "MoveData";

        internal static Vector3 _forwardZAxisAdjustUnit = Vector3.zero;
        internal static Vector3 _rightXAxisAdjustUnit = Vector3.zero;
        internal static Vector3 _upYAxisAdjustUnit = Vector3.zero;
        internal static float _fAdjustStep = 0.01f;
        #endregion

        #region enums
        public enum CharacterType { Heroine, Heroine3P, Player, Janitor, Group, Unknown }
        #endregion

        public partial class HCharaAdjusmentXController : CharaCustomFunctionController
        {
            #region Fields
            internal MoveData MoveData;
            internal List<MoveActionButton> buttons;
            #endregion

            #region Properties
            public bool DoRecalc { get; set; } = true;
            public bool Moved { get; set; } = false;
            public Vector3 OriginalPosition { get; set; } = new(0, 0, 0);
            public Vector3 LastMovePosition { get; set; } = new(0, 0, 0);
            public Vector3 FoundPosition { get; set; } = new(0, 0, 0);
            public Vector3 Movement { get; set; } = new(0, 0, 0);
            public Vector3 ALMovement { get; set; } = new(0, 0, 0);
            internal CharacterType ChaType { get; set; } = CharacterType.Unknown;
            #endregion

            #region public Methods
            /// <summary>
            /// Restore original position
            /// </summary>
            public void ResetPosition()
            {
                var original = ChaControl.transform.position;
                // Get calling method name
                var callingMethod = Utilities.CallingMethod();
                _Log.Warning($"[{callingMethod}] ResetPosition");

                if (OriginalPosition != Vector3.zero)
                {
#if DEBUG
                    /*if (GuideObject.gameObject.activeInHierarchy)
                    {
                        original = GuideObject.transform.position;
                        GuideObject.transform.position = OriginalPosition;
                    }
                    else
                    {

                        ChaControl.transform.position = OriginalPosition;
                    }*/
                    ChaControl.transform.position = OriginalPosition;
#else
                    ChaControl.transform.position = OriginalPosition;

#endif
                    Movement = Vector3.zero;
                    LastMovePosition = Vector3.zero;
                    Moved = false;
                    // Move in case ALMovement is not zero
                    if (ALMovement != Vector3.zero)
                    {
                        InvokeOnMoveRequest(null,
                            new MoveRequestEventArgs(
                                ChaType, MoveEvent.MoveType.MOVE));
                    }
#if DEBUG
                    _Log.Info($"HCAX0026: Reset position for {ChaType} " +
                        $"from Original={original} to=[{OriginalPosition}]");
#endif
                }
            }

            public void ReadData()
            {
                // Information for Player is save in Heroine card
                if (ChaControl.sex == (byte)Sex.Male)
                {
                    return;
                }
                // Initialize MoveData if null
                MoveData ??= new(ChaControl);

                var data = GetExtendedData();
                if (data != null)
                {
                    MoveData.Load(data);
                }
                else
                {
                    var name = ChaControl.chaFile.parameter.fullname.Trim()
                            ?? string.Empty;
#if DEBUG
                    _Log.Warning($"[ReadData] [{name}] Data is null.");
#endif
                }
            }

            public void SaveData(bool clear = false)
            {
                // Information for Player is save in Heroine card
                if (ChaControl.sex == (byte)Sex.Male)
                {
                    return;
                }
                if (MoveData != null)
                {
                    if (MoveData.Count == 0)
                    {
#if DEBUG
                        var name = ChaControl.chaFile.parameter.fullname.Trim()
                            ?? string.Empty;
                        _Log.Warning($"[SaveData] [{name}] MoveData total is 0 setting " +
                            $"ExtendedData to null(Not Really!).");
#endif
                        //SetExtendedData(null);
                    }
                    else
                    {
                        SetExtendedData(MoveData.Save());
                    }
                }
                else
                {
#if DEBUG
                    var name = ChaControl.chaFile?.parameter.fullname.Trim()
                        ?? string.Empty;
                    _Log.Warning($"[SaveData] [{name}] MoveData is null.");
#endif
                }
            }
            #endregion

            #region protected override Methods
            /// <summary>
            /// TODO: Verify if data is saved to the card in Maker when called from room.
            /// Need to identify 3P and Darkness scene. For now it won't be supported.
            /// Message to remember. This must be defined.
            /// </summary>
            /// <param name="currentGameMode"></param>
            protected override void OnCardBeingSaved(GameMode currentGameMode)
            {
                if (currentGameMode == GameMode.Maker)
                {
                    //return;
                }
                SaveData();
            }

            protected override void OnReload(
                GameMode currentGameMode, bool maintainState)
            {
                if (currentGameMode == GameMode.Maker)
                {
                    if (MakerInfo.InRoomMaker)
                    {
                        _Log.Message($"[{PluginName}] If you load a card with Card " +
                            $"Info selected you may lose move information.");
                    }
                }
                if (maintainState)
                {
                    return;
                }
                ReadData();
            }

            protected override void Update()
            {
                if (HProcMonitor.Nakadashi && IsSupportedScene
                    && (ChaType != CharacterType.Unknown))
                {
#if DEBUG
                    if (GuideObject)
                    {
                        if (GuideObject.gameObject.activeInHierarchy)
                        {
                            ChaControl.transform.position =
                                GuideObject.transform.position;
                        }
                        else
                        {
                            GuideObject.transform.position =
                                ChaControl.transform.position;
                        }
                    }
#endif
                    if (DoRecalc)
                    {
                        _fAdjustStep = cfgAdjustmentStep.Value;
                        _forwardZAxisAdjustUnit =
                            ChaControl.transform.forward * _fAdjustStep;
                        _rightXAxisAdjustUnit =
                            ChaControl.transform.right * _fAdjustStep;
                        _upYAxisAdjustUnit = ChaControl.transform.up * _fAdjustStep;
                        DoRecalc = false;
                    }
                    if (ChaType == CharacterType.Heroine)
                    {
#if DEBUG
                        if (KeyHeroine.GuideObject.Value.IsDown())
                        {
                            ToggleGuideObject();
                        }
#endif
                        if (KeyHeroine.Menu.Value.IsDown())
                        {
                            _buttonsInterface[ChaType].ShowInterface =
                                !_buttonsInterface[ChaType].ShowInterface;
                        }
                    }
                    if (ChaType == CharacterType.Player)
                    {
                        if (KeyPlayer.Menu.Value.IsDown())
                        {
                            _buttonsInterface[ChaType].ShowInterface =
                                !_buttonsInterface[ChaType].ShowInterface;
                        }
                    }
                }
                base.Update();
            }
#endregion

#region private Methods
            internal void Init(HSceneProc hSceneProc, CharacterType characterType)
            {
#if DEBUG
                _Log.Info($"HCAX0025: Initialization for {characterType}");
#endif
                ChaType = characterType;
                MoveData ??= new(ChaControl);
#if DEBUG
                // CreateGuideObject(hSceneProc, characterType);
#endif
                SetOriginalPosition();
                if (characterType == CharacterType.Heroine)
                {
                    buttons = new ButtonsGUI(characterType, xMargin: 0f, yMargin: 0.08f,
                        width: 62f, height: 25f, xOffset: (-126f)).Buttons;
                }
                else if (characterType == CharacterType.Player)
                {
                    buttons = new ButtonsGUI(characterType, xMargin: 0f, yMargin: 0.08f,
                        width: 62f, height: 25f, xOffset: (-248f)).Buttons;
                }
                // Start disabled
                enabled = false;
            }

            /// <summary>
            /// Save original position
            /// </summary>
            internal void SetOriginalPosition()
            {
                var nowHPointDataPos = _hprocTraverse.nowHpointDataPos;
#if DEBUG
                var original = OriginalPosition;
#endif
                OriginalPosition = nowHPointDataPos;
                FoundPosition = ChaControl.transform.position;
                LastMovePosition = Vector3.zero;
                Movement = Vector3.zero;
                Moved = false;
#if DEBUG
                var lines = new StringBuilder();
                // Get calling method name
                var callingMethod = Utilities.CallingMethod();

                // Real original position AnimationLoader can change them when we get
                // here
                lines.AppendLine($"Original={original} Set={nowHPointDataPos}");
                lines.AppendLine($"Last Move={LastMovePosition} Set={Vector3.zero}");
                lines.AppendLine($"Current Position={FoundPosition.ToString("F7")} " +
                    $"ALMove={ALMovement.ToString("F7")} Moved={Moved} " +
                    $"POS={nowHPointDataPos.ToString("F7")}");
                _Log.Warning($"[{callingMethod}] [SetOriginalPosition] " +
                    $"ChaType={ChaType}\n{lines}");
#endif
            }
#endregion
        }
    }
}
