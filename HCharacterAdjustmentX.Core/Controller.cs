using System.Collections.Generic;

using UnityEngine;
using MessagePack;

using HSceneUtility;

using ExtensibleSaveFormat;
using KKAPI;
using KKAPI.Chara;

using IDHIUtils;
using System;
using KKAPI.MainGame;
using ADV.Commands.Camera;

namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        // Controller
        internal static Vector3 _forwardZAxisAdjustUnit = Vector3.zero;
        internal static Vector3 _rightXAxisAdjustUnit = Vector3.zero;
        internal static Vector3 _upYAxisAdjustUnit = Vector3.zero;
        internal static float _fAdjustStep = 0.01f;
        internal static bool _moved = false;
        //internal static Dictionary<string, PositionData> MoveData = new();
        //internal static CMoveData CMoveDataCMoveData = new();

        public partial class HCharaAdjusmentXController : CharaCustomFunctionController
        {
            #region private fields
            internal CharacterType _chaType = CharacterType.Unknown;
            internal Vector3 _lastMovePosition = new(0, 0, 0);
            internal Vector3 _originalPosition = new(0, 0, 0);
            internal Vector3 _movement = new(0, 0, 0);
            internal Dictionary<string,
                Dictionary<CharacterType, PositionData>> MoveData = new();
            #endregion

            #region public fields
            public List<MoveActionButton> buttons;
            public enum CharacterType { Heroine, Heroine3P, Player, Janitor, Group, Unknown }
            #endregion

            #region properties
            public bool DoRecalc { get; set; } = true;
            public bool Moved
            {
                get
                {
                    return _moved;
                }
                set
                {
                    _moved = value;
                }
            }
            #endregion

            #region private methods
            internal void Init(HSceneProc hSceneProc, CharacterType characterType)
            {
                _Log.Info($"SHCA0002: Initialization for {characterType}");
                _chaType = characterType;
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
                _originalPosition = ChaControl.transform.position;
                _lastMovePosition = ChaControl.transform.position;
                _movement = Vector3.zero;
                _moved = false;
            }
            #endregion

            #region public methods
            /// <summary>
            /// Restore original position
            /// </summary>
            public void ResetPosition()
            {
                if (_originalPosition != Vector3.zero)
                {
                    if (_guideObject.gameObject.activeInHierarchy)
                    {
                        _guideObject.amount.position = _originalPosition;
                    }
                    else
                    {
                        ChaControl.transform.position = _originalPosition;
                    }
                    _movement = Vector3.zero;
                    _moved = false;
#if DEBUG
                    _Log.Info($"SHCA0003: Reset position for {_chaType} " +
                        $"[{_originalPosition}]");
#endif
                }
            }
            #endregion

            #region unity methods
            /// <summary>
            /// TODO: Verify if data is saved to the card in Maker when called from room.
            /// Need to identify 3P and Darkness scene.  
            /// For now it won't be supported. Message to remember.
            /// This must be defined.
            /// </summary>
            /// <param name="currentGameMode"></param>
            protected override void OnCardBeingSaved(GameMode currentGameMode)
            {
                if (currentGameMode == GameMode.Maker)
                {
                    _Log.Warning($"[OnCardBeingSaved] Maker out.");
                    return;
                }
#if DEBUG
                var calllingMethod = Utilities.CallingMethod();
                _Log.Warning($"[OnCardBeingSaved] Calling Method {calllingMethod}.");
#endif
                SaveData();
            }

            protected override void OnReload(GameMode currentGameMode, bool maintainState)
            {
                if (currentGameMode == GameMode.Maker)
                {
                    _Log.Warning($"[OnReload] Maker out.");
                    return;
                }

                if (maintainState)
                {
                    _Log.Warning($"[OnReload] Maintain state out.");
                    return;
                }
#if DEBUG
                var calllingMethod = Utilities.CallingMethod();
                _Log.Warning($"[OnReload] Calling Method {calllingMethod}.");
#endif
                ReadData();
            }

            public void ReadData()
            {
                var calllingMethod = Utilities.CallingMethod();
                var heroine = ChaControl.GetHeroine();
                var chaName = $"{ChaControl.name.Trim()} " +
                    $"({ChaControl.chaFile.parameter.fullname.Trim()})";
                var name = chaName;

                _Log.Warning($"[ReadData] Calling Method {calllingMethod}.");

                if (heroine != null)
                {                    
                    name = $"{heroine.Name.Trim()} ({chaName})";
                }

                _Log.Warning($"[ReadData] Load data for {name}.");

                var data = GetExtendedData();
                // MoveData.Load(data)
                if (data != null)
                {
                    Dictionary<string, PositionDataPair> MoveDataSerialize;

                    if (data.data.TryGetValue(nameof(MoveData),
                        out var loadedMoveData)
                        && loadedMoveData != null)
                    {
                        MoveDataSerialize = MessagePackSerializer
                            .Deserialize<Dictionary<string, PositionDataPair>>
                            ((byte[])loadedMoveData);
                        if (MoveDataSerialize != null)
                        {
                            //MoveData.Clear();
                            MoveData = RestoreMoveData(MoveDataSerialize);
#if DEBUG
                            PrintData(MoveData);
#endif
                        }
                        else
                        {
                            _Log.Error($"[ReadData] Can't unpack data for {name}.");
                        }
                    }
                }
            }

            public void SaveData(bool clear = false)
            {
                var calllingMethod = Utilities.CallingMethod();
                var heroine = ChaControl.GetHeroine();
                var chaName = $"{ChaControl.name.Trim()}";
                var name = chaName;

                if (heroine != null)
                {
                    name = $"{heroine.Name.Trim()} ({chaName})";
                }
#if DEBUG
                _Log.Warning($"[SaveData] Calling Method={calllingMethod} name={name}.");
#endif
                if (MoveData.Count == 0)
                {
                    _Log.Error($"[SaveData] No Data name={name}.");
                    if (clear)
                    {
                        SetExtendedData(null);
                    }
                }
                else
                {
                    var MoveDataSerialize = PrepareSerialize(MoveData);

                    var data = new PluginData {
                        version = 1
                    };
                    data.data.Add(nameof(MoveData),
                        MessagePackSerializer.Serialize(MoveDataSerialize));
                    SetExtendedData(data);
#if DEBUG
                    var Moving = RestoreMoveData(MoveDataSerialize);
                    PrintData(Moving);
#endif
                }
            }

            /// <summary>
            /// 
            /// </summary>
            protected override void Update()
            {
                if (HProcScene.Nakadashi && IsSupportedScene && (_chaType != CharacterType.Unknown))
                {
                    if (_guideObject)
                    {
                        if (_guideObject.gameObject.activeInHierarchy)
                        {
                            ChaControl.transform.position = _guideObject.amount.position;
                        }
                        else
                        {
                            _guideObject.amount.position = ChaControl.transform.position;
                        }
                    }
                    if (DoRecalc)
                    {
                        _fAdjustStep = cfgAdjustmentStep.Value;
                        _forwardZAxisAdjustUnit = ChaControl.transform.forward * _fAdjustStep;
                        _rightXAxisAdjustUnit = ChaControl.transform.right * _fAdjustStep;
                        _upYAxisAdjustUnit = ChaControl.transform.up * _fAdjustStep;
                        DoRecalc = false;
#if DEBUG
                        _Log.Info($"SHCA0036: Calculation for {_chaType} " +
                            $"with Step {_fAdjustStep}:\n" +
                            $"forward (z,  blue) {_forwardZAxisAdjustUnit.ToString("F7")}\n" +
                            $"  right (x,   red) {_rightXAxisAdjustUnit.ToString("F7")}\n" +
                            $"     up (y, green) {_upYAxisAdjustUnit.ToString("F7")})");
#endif
                    }
                    if (_chaType == CharacterType.Heroine)
                    {
                        if (KeyHeroine.GuideObject.Value.IsDown())
                        {
                            ToggleGuideObject();
                        }

                        if (KeyHeroine.Menu.Value.IsDown())
                        {
#if DEBUG
                            _Log.Info($"[SHCAdjustController] Toggle interface for {_chaType} " +
                                $"current {_buttonsInterface[_chaType].ShowInterface}");
#endif
                            _buttonsInterface[_chaType].ShowInterface =
                                !_buttonsInterface[_chaType].ShowInterface;
                        }
                    }
                    if (_chaType == CharacterType.Player)
                    {
                        if (KeyPlayer.Menu.Value.IsDown())
                        {
#if DEBUG
                            _Log.Info($"[SHCAdjustController] Toggle interface for {_chaType} " +
                                $"current {_buttonsInterface[_chaType].ShowInterface}");
#endif
                            _buttonsInterface[_chaType].ShowInterface =
                                !_buttonsInterface[_chaType].ShowInterface;
                        }
                    }
                }
                base.Update();
            }
            #endregion
        }
    }
}
