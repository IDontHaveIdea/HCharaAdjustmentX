using System;
using System.Collections.Generic;

using HSceneUtility;
using KKAPI.Utilities;

using UnityEngine;

using MoveType = IDHIPlugins.MoveEvent.MoveType;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        /// <summary>
        /// Take care of movement requests
        /// </summary>
        internal class CharMovement
        {
            #region private fields
            internal static HSceneGuideObject _guideObject;
            internal static ChaControl _chaControl;
            internal static HCharaAdjusmentXController _controller;
            internal static bool _doShortcutMove;
            internal static bool _positiveMove;
            internal static string _animationGUID = "";
            internal static int _animationID = 0;
            internal static string _pathFemaleBase = "";
            #endregion

            /// <summary>
            /// Check for configured key shortcuts and execute the type of movement
            /// desired
            ///
            /// TODO:
            ///     Classify positions for correct relative left/right
            ///     and forward/backwards
            /// </summary>
            /// <param name="chaType">Character type</param>
            /// <param name="moveType">Move triggered</param>
            /// <returns></returns>
            public static bool Move(CharacterType chaType, MoveType moveType)
            {
                _doShortcutMove = false;

                if (chaType == CharacterType.Player)
                {
                    _chaControl = _hprocInstance.flags.player?.chaCtrl;
                    _positiveMove = false;
                }
                else
                {
                    _chaControl = _hprocInstance.flags.lstHeroine[(int)chaType]?.chaCtrl;
                    _positiveMove = true;
                }
                if (_chaControl == null)
                {
                    throw new NullReferenceException($"HCAX0040: HProc instance " +
                        $"invalid.");
                }

                _controller = GetControllerByType(chaType);
                _guideObject = _controller.GuideObject;
                _animationID = _hprocInstance.flags.nowAnimationInfo.id;
                _animationGUID = _hprocInstance.flags.nowAnimationInfo.nameAnimation;
                _pathFemaleBase = _hprocInstance.flags.nowAnimationInfo
                    .pathFemaleBase.assetpath;

                // Normal button press
                var originalPosition = _controller.OriginalPosition;
                var movement = _controller.Movement;
                Vector3 newPosition = new(0, 0, 0);
                switch (moveType)
                {
                    case MoveType.RESET:
                        _controller.ResetPosition();
                        break;

                    case MoveType.UP:
                        movement.y += _fAdjustStep;
                        _doShortcutMove = true;
                        break;

                    case MoveType.DOWN:
                        movement.y -= _fAdjustStep;
                        _doShortcutMove = true;
                        break;

                    case MoveType.RIGHT:
                        if (chaType == CharacterType.Player)
                        {
                            movement.x -= _fAdjustStep;
                        }
                        else
                        {
                            movement.x += _fAdjustStep;
                        }
                        _doShortcutMove = true;
                        break;

                    case MoveType.LEFT:
                        if (chaType == CharacterType.Player)
                        {
                            movement.x += _fAdjustStep;
                        }
                        else
                        {
                            movement.x -= _fAdjustStep;
                        }
                        _doShortcutMove = true;
                        break;

                    case MoveType.CLOSER:
                        if (chaType == CharacterType.Player)
                        {
                            movement.z -= _fAdjustStep;
                        }
                        else
                        {
                            movement.z += _fAdjustStep;
                        }
                        _doShortcutMove = true;
                        break;

                    case MoveType.APART:
                        if (chaType == CharacterType.Player)
                        {
                            movement.z += _fAdjustStep;
                        }
                        else
                        {
                            movement.z -= _fAdjustStep;
                        }
                        _doShortcutMove = true;
                        break;
                    case MoveType.SAVE:
                        if (!_animationKey.IsNullOrEmpty())
                        {
                            var Position =
                                new Dictionary<CharacterType, PositionData> {
                                    [chaType] = new(_controller.Movement,
                                    Vector3.zero)
                                };
                            _controller.MoveData[_animationKey] = Position;
                        }
                        _doShortcutMove = false;
                        _controller.SaveData();
                        break;
                    case MoveType.LOAD:
                        if (!_animationKey.IsNullOrEmpty())
                        {
                            _controller.MoveData.Data.TryGetValue(_animationKey,
                                out var position);
                            if (position != null)
                            {
                                // Use TryGetValue
                                movement = position[chaType]
                                   .Position;
                            }
                        }
                        _doShortcutMove = true;
                        break;
#if DEBUG
                    // Buttons for testing
                    case MoveType.TEST1:
                        var tmp = _chaControl.transform.position;
                        var rot = _chaControl.transform.rotation;

                        var strTmp = $"{chaType} position={tmp.ToString("F7")} " +
                            $"rotation={rot.ToString("F7")}\n";
                                              
                        _Log.Info($"[Move.TEST1]\n\n{strTmp}\n");
                        _doShortcutMove = false;
                        break;
                    case MoveType.TEST2:
                        ShowGroupGuide = !ShowGroupGuide;
                        break;
#endif
                    // Execute a move event with current parameters used
                    // for automatic position adjustment
                    case MoveType.MOVE:
                        _doShortcutMove = true;
                        break;
                    default:
                        _doShortcutMove = false;
                        break;
                }
                if (_doShortcutMove)
                {
                    var tmp = _chaControl.transform.position;
                    if (!_controller.Moved)
                    {
                        _controller.Moved = true;
                    }
                    newPosition = RecalcPosition(
                        _chaControl, originalPosition, movement);
#if DEBUG
                    _Log.Info($"HCAX0046: Move {chaType}\n" +
                        $"from position {tmp.ToString("F7")} " +
                        $" to position {newPosition.ToString("F7")}\n" +
                        $" for movement {movement.ToString("F7")} " +
                        $" to recalc {newPosition.ToString("F7")}");
#endif
                    _doShortcutMove = false;
                    _chaControl.transform.position = newPosition;
                    _guideObject.amount.position = _chaControl.transform.position;
                    _controller.Movement = movement;
                    _controller.LastMovePosition = newPosition;
                }
                return _doShortcutMove;
            }

            internal static Vector3 RecalcPosition(
                ChaControl chaControl,
                Vector3 original,
                Vector3 move)
            {
                try
                {
                    var rightXAxis = chaControl.transform.right * move.x;
                    var upYAxis = chaControl.transform.up * move.y;
                    var forwardZAxis = chaControl.transform.forward * move.z;

                    var newPosition = original;

                    newPosition += rightXAxis;
                    newPosition += upYAxis;
                    newPosition += forwardZAxis;
#if DEBUG
                    _Log.Info($"[RecalcPosition] Move {chaControl.name}\n" +
                        $"original position {original.ToString("F7")}\n" +
                        $"      move vector {move.ToString("F7")}\n" +
                        $"          right x {rightXAxis.ToString("F7")}\n" +
                        $"             up y {upYAxis.ToString("F7")}\n" +
                        $"        forward z {forwardZAxis.ToString("F7")}\n" +
                        $"       to re-calc {newPosition.ToString("F7")}");
#endif
                    return newPosition;
                }
                catch (Exception e)
                {
                    _Log.Error($"HCAX0048: Cannot adjust position {chaControl.name} " +
                        $"- {e}.");
                }
                return Vector3.zero;
            }

            internal static string Translate(string name)
            {
                if (!TranslationHelper.TryTranslate(name, out var tmp))
                {
                    return name;
                }

                return tmp;
            }

            internal static readonly Func<bool, Vector3, Vector3, Vector3> setDirection
                = (sign, trans, adjustment) =>
                {
                    var tmp = new Vector3(0, 0, 0);
                    if (sign)
                    {
                        tmp = (trans + adjustment);
                        return tmp;
                    }
                    tmp = trans - adjustment;
                    return tmp;
                };
        }
    }
}
