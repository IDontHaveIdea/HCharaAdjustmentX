using System;

using BepInEx.Logging;
using HSceneUtility;
using KKAPI.Utilities;

using UnityEngine;

using IDHIUtils;

using CharacterType = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController.CharacterType;
using MoveType = IDHIPlugins.MoveEvent.MoveType;
using static FaceScreenShot;


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
            /// Check for configured key shortcuts and execute the type of movement desired
            ///
            /// TODO:
            ///     Clasify positions for correct relative left/right and forward/backwards
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
                    throw new NullReferenceException($"SHCA0027: HProc instance invalid.");
                }
                _controller = GetController(_chaControl);
                _guideObject = _controller._guideObject;
                _animationID = _hprocInstance.flags.nowAnimationInfo.id;
                _animationGUID = _hprocInstance.flags.nowAnimationInfo.nameAnimation;
                _pathFemaleBase = _hprocInstance.flags.nowAnimationInfo.pathFemaleBase.assetpath;
                if (_animationGUID == null)
                {
                    _Log.Level(LogLevel.Warning, $"SHCA0028: No animationGUID.");
                }
                if (_animationID < 0)
                {
                    _Log.Level(LogLevel.Warning, $"SHCA0029: No animationID.");
                }
#if DEBUG
                if (_animationGUID != null)
                {
                    _Log.Info($"SHCA0030: Move {moveType} requested for ID: {_animationID} - name" +
                        $" {Translate(_animationGUID)} ({_animationGUID}).");
                    _Log.Info($"SHCA0031: asset {_pathFemaleBase}");
                }
                else
                {
                    _Log.Info($"SHCA0032: Move {moveType} requested for ID: {_animationID}.");
                }
#endif
                // Normal button press
                var originalPosition = _controller._originalPosition;
                var movement = _controller._movement;
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

                    default:
                        _doShortcutMove = false;
                        break;

                }
                if (_doShortcutMove)
                {
#if DEBUG
                    var tmp = _chaControl.transform.position;
                    newPosition = RecalcPosition(_chaControl, originalPosition, movement);
                    _Log.Info($"SHCA0033: Move {chaType}\n" +
                        $"from position {tmp.ToString("F7")} " +
                        $" to position {newPosition.ToString("F7")}\n" +
                        $" for movement {movement.ToString("F7")} " +
                        $" to recalc {newPosition.ToString("F7")}");
#endif
                    _doShortcutMove = false;
                    _chaControl.transform.position = newPosition;
                    _guideObject.amount.position = _chaControl.transform.position;
                    _controller._movement = movement;
                    _controller._lastMovePosition = tmp;
                }
                return _doShortcutMove;
            }

            internal static Vector3 RecalcPosition(ChaControl chaControl, Vector3 original, Vector3 move)
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
                    _Log.Info($"SHCA0033: Move {chaControl.name}\n" +
                        $"original position {original.ToString("F7")}\n" +
                        $"      move vector {move.ToString("F7")}\n" +
                        $"          right x {rightXAxis.ToString("F7")}\n" +
                        $"             up y {upYAxis.ToString("F7")}\n" +
                        $"        forward z {forwardZAxis.ToString("F7")}\n" +
                        $"        to recalc {newPosition.ToString("F7")}");
#endif
                    return newPosition;
                }
                catch (Exception e)
                {
                    _Log.Error($"0010: Cannot adjust positoin {chaControl.name} - {e}.");
                }
                return Vector3.zero;
            }

            internal static string Translate(string name)
            {
                // if (!TranslationHelper.TryTranslate(name, out string tmp))
                if (!TranslationHelper.TryTranslate(name, out var tmp))
                {
                    return name;
                }

                return tmp;
            }

            internal static readonly Func<bool, Vector3, Vector3, Vector3> setDirection =
                (sign, trans, adjustment) =>
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
