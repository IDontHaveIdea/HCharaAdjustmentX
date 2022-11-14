using System.Collections.Generic;

using UnityEngine;

using KKAPI;
using KKAPI.Chara;

using IDHIUtils;
using System.Text;

namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        // Controller
        internal static Vector3 _forwardZAxisAdjustUnit = Vector3.zero;
        internal static Vector3 _rightXAxisAdjustUnit = Vector3.zero;
        internal static Vector3 _upYAxisAdjustUnit = Vector3.zero;
        internal static float _fAdjustStep = 0.01f;
        internal const string MoveDataID = "MoveData";
        public enum CharacterType { Heroine, Heroine3P, Player, Janitor, Group, Unknown }

        public partial class HCharaAdjusmentXController : CharaCustomFunctionController
        {
            #region private fields
            internal MoveData MoveData;
            internal List<MoveActionButton> buttons;
            #endregion

            #region properties
            public bool DoRecalc { get; set; } = true;
            public bool Moved { get; set; } = false;
            public Vector3 OriginalPosition { get; set; } = new(0, 0, 0);
            public Vector3 LastMovePosition { get; set; } = new(0, 0, 0);
            public Vector3 Movement { get; set; } = new(0, 0, 0);
            internal CharacterType ChaType { get; set; } = CharacterType.Unknown;
            #endregion

            #region private methods
            internal void Init(HSceneProc hSceneProc, CharacterType characterType)
            {
#if DEBUG
                _Log.Info($"HCAX0025: Initialization for {characterType}");
#endif
                ChaType = characterType;
                MoveData ??= new(ChaControl);
                CreateGuideObject(hSceneProc, characterType);
                SetOriginalPosition();
                if (characterType == CharacterType.Heroine)
                {
                    buttons = new ButtonsGUI(characterType, xMargin: 0f, yMargin: 0.08f,
                        width: 57f, height: 25f, xOffset: (-124f)).Buttons;
                }
                else if (characterType == CharacterType.Player)
                {
                    buttons = new ButtonsGUI(characterType, xMargin: 0f, yMargin: 0.08f,
                        width: 57f, height: 25f, xOffset: (-240f)).Buttons;
                }
                // Start disabled
                enabled = false;
            }

            /// <summary>
            /// Save original position
            /// </summary>
            internal void SetOriginalPosition()
            {
                var lines = new StringBuilder();
                // Get calling method name
                var callingMethod = Utilities.CallingMethod();

                lines.AppendLine($"Original Position={OriginalPosition} Set={ChaControl.transform.position}");
                lines.AppendLine($"Last Move Position={LastMovePosition} Set={ChaControl.transform.position}");
                lines.AppendLine($"Movement={Movement} Moved={Moved}");

                OriginalPosition = ChaControl.transform.position;
                LastMovePosition = Vector3.zero;
                Movement = Vector3.zero;
                Moved = false;
                _Log.Warning($"[{callingMethod}] ChaType={ChaType}\n{lines}");
            }
            #endregion

            #region public methods
            /// <summary>
            /// Restore original position
            /// </summary>
            public void ResetPosition()
            {
                var Original = OriginalPosition;
                // Get calling method name
                var callingMethod = Utilities.CallingMethod();
                _Log.Warning($"[{callingMethod}] ResetPosition");

                if (OriginalPosition != Vector3.zero)
                {
                    if (GuideObject.gameObject.activeInHierarchy)
                    {
                        GuideObject.amount.position = OriginalPosition;
                    }
                    else
                    {
                        ChaControl.transform.position = OriginalPosition;
                    }
                    Movement = Vector3.zero;
                    LastMovePosition= Vector3.zero;
                    Moved = false;
#if DEBUG
                    _Log.Info($"HCAX0026: Reset position for {ChaType} " +
                        $"from Original={Original} to=[{OriginalPosition}]");
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
                    var name = ChaControl.chaFile?.parameter.fullname.Trim()
                        ?? string.Empty;
#if DEBUG
                    _Log.Warning($"[SaveData] [{name}] MoveData is null.");
#endif
                }
            }
            #endregion

            #region unity methods
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

            protected override void OnReload(GameMode currentGameMode, bool maintainState)
            {
                if (currentGameMode == GameMode.Maker)
                {
                    //return;
                }
                if (maintainState)
                {
                    return;
                }
                ReadData();
            }

            protected override void Update()
            {
                if (HProcMonitor.Nakadashi && IsSupportedScene && (ChaType != CharacterType.Unknown))
                {
                    if (GuideObject)
                    {
                        if (GuideObject.gameObject.activeInHierarchy)
                        {
                            ChaControl.transform.position = GuideObject.amount.position;
                        }
                        else
                        {
                            GuideObject.amount.position = ChaControl.transform.position;
                        }
                    }
                    if (DoRecalc)
                    {
                        _fAdjustStep = cfgAdjustmentStep.Value;
                        _forwardZAxisAdjustUnit = ChaControl.transform.forward * _fAdjustStep;
                        _rightXAxisAdjustUnit = ChaControl.transform.right * _fAdjustStep;
                        _upYAxisAdjustUnit = ChaControl.transform.up * _fAdjustStep;
                        DoRecalc = false;
                    }
                    if (ChaType == CharacterType.Heroine)
                    {
                        if (KeyHeroine.GuideObject.Value.IsDown())
                        {
                            ToggleGuideObject();
                        }

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
        }
    }
}
