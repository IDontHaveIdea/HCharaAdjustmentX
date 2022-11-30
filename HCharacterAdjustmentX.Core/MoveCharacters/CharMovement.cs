using System;
using System.Collections.Generic;

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
            internal static ChaControl _chaControl;
            internal static HCharaAdjusmentXController _controller;
            internal static HCharaAdjusmentXController _controllerPlayer;
            internal static HCharaAdjusmentXController _controllerHeroine;
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
                _animationID = _hprocInstance.flags.nowAnimationInfo.id;
                _animationGUID = _hprocInstance.flags.nowAnimationInfo.nameAnimation;
                _pathFemaleBase = _hprocInstance.flags.nowAnimationInfo
                    .pathFemaleBase.assetpath;

                // Normal button press
                var originalPosition = _controller.OriginalPosition;
                var movement = _controller.Movement;
                var fullMovement = _controller.Movement;
                if (!_controller.Moved)
                {
                    if ((_controller.Movement == Vector3.zero)
                        && (_controller.ALMovement != Vector3.zero))
                    {
                        fullMovement = _controller.ALMovement;
                        movement = _controller.ALMovement;
                    }
                }
#if DEBUG
                _Log.Warning($"START wit Moved={_controller.Moved} for " +
                    $"KEY={_animationKey} MOVEMENT={movement.ToString("F7")} " +
                    $"ALMove={_controller.ALMovement.ToString("F7")} " +
                    $"Movement={_controller.Movement.ToString("F7")}");
#endif
                Vector3 newPosition = new(0, 0, 0);
                switch (moveType)
                {
                    case MoveType.RESET:
                        _controller.ResetPosition();
                        break;

                    case MoveType.UP:
                        movement.y += _fAdjustStep;
                        fullMovement += (Vector3.up * _fAdjustStep);
                        _doShortcutMove = true;
                        break;

                    case MoveType.DOWN:
                        movement.y -= _fAdjustStep;
                        fullMovement += (Vector3.down * _fAdjustStep);
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
                        fullMovement += (Vector3.right * _fAdjustStep);
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
                        fullMovement += (Vector3.left * _fAdjustStep);
                        _doShortcutMove = true;
                        break;

                    case MoveType.FORWARD:
                        if (chaType == CharacterType.Player)
                        {
                            movement.z -= _fAdjustStep;
                        }
                        else
                        {
                            movement.z += _fAdjustStep;
                        }
                        fullMovement += (Vector3.forward * _fAdjustStep);
                        _doShortcutMove = true;
                        break;

                    case MoveType.BACK:
                        if (chaType == CharacterType.Player)
                        {
                            movement.z += _fAdjustStep;
                        }
                        else
                        {
                            movement.z -= _fAdjustStep;
                        }
                        fullMovement += (Vector3.back * _fAdjustStep);
                        _doShortcutMove = true;
                        break;
                    case MoveType.SAVE:
                        if (!_animationKey.IsNullOrEmpty())
                        {
                            _controllerPlayer =
                                GetControllerByType(CharacterType.Player);
                            _controllerHeroine =
                                GetControllerByType(CharacterType.Heroine);

                            var positions = new Dictionary<CharacterType, PositionData>
                            {
                                [chaType] = new(_controllerHeroine.Movement,
                                    Vector3.zero)
                            };

                            positions[CharacterType.Player] =
                                new(_controllerPlayer.Movement, Vector3.zero);
                            positions[CharacterType.Heroine] =
                                new(_controllerHeroine.Movement, Vector3.zero);
                            _controller.MoveData[_animationKey] = positions;
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
                        //ShowGroupGuide = !ShowGroupGuide;
#region Camera
                        var cameraObject = Camera.main.gameObject;
                        var camCtrl = cameraObject?.GetComponent<CameraControl_Ver2>();
                        if (camCtrl != null)
                        if (camCtrl != null)
                        {
                            _Log.Warning($"\n[CAMERA]\n" +
                                $" Position: {camCtrl.TargetPos}\n" +
                                $"    Angle: {camCtrl.CameraAngle}\n" +
                                $"Direction: {camCtrl.CameraDir}\n" +
                                $"      FOV: {camCtrl.CameraFov}");
                        }
                        cameraObject = GameObject.Find("HProc/CamBase/Camera");
                        if (cameraObject != null)
                        {
                            _Log.Warning("HPROC HAS A CAMERA");
                        }
#endregion
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
                        _chaControl, originalPosition, movement, fullMovement);
#if DEBUG
                    _Log.Info($"HCAX0046: Move {chaType} by {movement.ToString("F7")}\n"
                        + $"from position {tmp.ToString("F7")} "
                        + $"to position {newPosition.ToString("F7")}");
#endif
                    _doShortcutMove = false;
                    _chaControl.transform.position = newPosition;
                    _controller.Movement = fullMovement;
                    _controller.LastMovePosition = newPosition;
                }
                return _doShortcutMove;
            }

            internal static Vector3 RecalcPosition(
                ChaControl chaControl,
                Vector3 original,
                Vector3 move,
                Vector3 fullMove)
            {
                try
                {
                    var rightXAxis = chaControl.transform.right * move.x;
                    var upYAxis = chaControl.transform.up * move.y;
                    var forwardZAxis = chaControl.transform.forward * move.z;

                    var newPosition = original;
                    var fullNewPosition = original + fullMove;

                    newPosition += rightXAxis;
                    newPosition += upYAxis;
                    newPosition += forwardZAxis;
#if DEBUG
                    _Log.Info($"[RecalcPosition] Move {chaControl.name}\n" +
                        $"original position {original.ToString("F7")}\n" +
                        $"      move vector {move.ToString("F7")}\n" +
                        $" full move vector {fullMove.ToString("F7")}\n" +
                        $"          right x {rightXAxis.ToString("F7")}\n" +
                        $"             up y {upYAxis.ToString("F7")}\n" +
                        $"        forward z {forwardZAxis.ToString("F7")}\n" +
                        $"       to re-calc {newPosition.ToString("F7")}\n" +
                        $"       to fullNew {fullNewPosition.ToString("F7")}");
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
