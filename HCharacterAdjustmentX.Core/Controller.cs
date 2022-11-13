﻿using System.Collections.Generic;

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
using KKAPI.Maker;

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

        public partial class HCharaAdjusmentXController : CharaCustomFunctionController
        {
            #region private fields
            internal MoveData MoveData;
            internal List<MoveActionButton> buttons;
            #endregion

            #region public fields
            public enum CharacterType { Heroine, Heroine3P, Player, Janitor, Group, Unknown }
            #endregion

            #region properties
            public bool DoRecalc { get; set; } = true;
            public bool Moved { get; set; } = false;
            public Vector3 OriginalPosition { get; set; } = new(0, 0, 0);
            public Vector3 LastMovePosition { get; set; } = new(0, 0, 0);
            public Vector3 Movement { get; set; } = new(0, 0, 0);
            public CharacterType ChaType { get; set; } = CharacterType.Unknown;
            #endregion

            #region private methods
            internal void Init(HSceneProc hSceneProc, CharacterType characterType)
            {
                _Log.Info($"SHCA0002: Initialization for {characterType}");
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
                OriginalPosition = ChaControl.transform.position;
                LastMovePosition = ChaControl.transform.position;
                Movement = Vector3.zero;
                Moved = false;
            }
            #endregion

            #region public methods
            /// <summary>
            /// Restore original position
            /// </summary>
            public void ResetPosition()
            {
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
                    Moved = false;
#if DEBUG
                    _Log.Info($"SHCA0003: Reset position for {ChaType} " +
                        $"[{OriginalPosition}]");
#endif
                }
            }

            public void ReadData()
            {
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
                    _Log.Warning($"[ReadData] [{name}] Data is null.");
                }
            }

            public void SaveData(bool clear = false)
            {
                if (MoveData != null)
                {
                    if (MoveData.Count == 0)
                    {
#if DEBUG
                        _Log.Error($"[SaveData] [{name}] MoveData total is 0 setting " +
                            $"ExtendedData to null.");
#endif
                        SetExtendedData(null);
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
                    _Log.Error($"[SaveData] [{name}] MoveData is null.");
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
#if DEBUG
                //var calllingMethod = Utilities.CallingMethod();
                //_Log.Warning($"[OnCardBeingSaved] Calling Method {calllingMethod}.");
#endif
                SaveData();
            }

            protected override void OnReload(GameMode currentGameMode, bool maintainState)
            {
                if (maintainState)
                {
                    _Log.Warning($"[OnReload] Maintain state out.");
                    return;
                }
#if DEBUG
                //var calllingMethod = Utilities.CallingMethod();
                //_Log.Warning($"[OnReload] Calling Method {calllingMethod}.");
#endif
                ReadData();
            }

            protected override void Update()
            {
                if (HProcScene.Nakadashi && IsSupportedScene && (ChaType != CharacterType.Unknown))
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
#if DEBUG
                        _Log.Info($"[Update] Calculation for {ChaType} " +
                            $"with Step {_fAdjustStep}:\n" +
                            $"forward (z,  blue) {_forwardZAxisAdjustUnit.ToString("F7")}\n" +
                            $"  right (x,   red) {_rightXAxisAdjustUnit.ToString("F7")}\n" +
                            $"     up (y, green) {_upYAxisAdjustUnit.ToString("F7")})");
#endif
                    }
                    if (ChaType == CharacterType.Heroine)
                    {
                        if (KeyHeroine.GuideObject.Value.IsDown())
                        {
                            ToggleGuideObject();
                        }

                        if (KeyHeroine.Menu.Value.IsDown())
                        {
#if DEBUG
                            _Log.Info($"[Update] Toggle interface for {ChaType} " +
                                $"current {_buttonsInterface[ChaType].ShowInterface}");
#endif
                            _buttonsInterface[ChaType].ShowInterface =
                                !_buttonsInterface[ChaType].ShowInterface;
                        }
                    }
                    if (ChaType == CharacterType.Player)
                    {
                        if (KeyPlayer.Menu.Value.IsDown())
                        {
#if DEBUG
                            _Log.Info($"[Update] Toggle interface for {ChaType} " +
                                $"current {_buttonsInterface[ChaType].ShowInterface}");
#endif
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
