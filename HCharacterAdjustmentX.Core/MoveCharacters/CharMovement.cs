using System;

using BepInEx.Logging;
using HSceneUtility;
using KKAPI.Utilities;

using UnityEngine;

using IDHIUtils;

using CharacterType = IDHIPlugins.HCharaterAdjustX.HCharacterAdjustXController.CharacterType;
using MoveType = IDHIPlugins.MoveEvent.MoveType;


namespace IDHIPlugins
{
    public partial class HCharaterAdjustX
    {
        /// <summary>
        /// Take care of movement requests
        /// </summary>
        internal class CharMovement
        {
            #region private fields
            static internal HSceneGuideObject _guideObject;
            static internal ChaControl _chaControl;
            static internal HCharacterAdjustXController _controller;
            static internal bool _doShortcutMove;
            static internal bool _positiveMove;
            static internal string _animationGUID = "";
            static internal int _animationID = 0;
            static internal string _pathFemaleBase = "";
            #endregion

            /// <summary>
            /// Check for configured key shortcuts and execute the type of movement desired       
            /// </summary>
            /// <param name="chaType">Character type</param>
            /// <param name="move">Move triggered</param>
            /// <returns></returns>
            static public bool Move(CharacterType chaType, MoveType move)
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
                    Log.Level(LogLevel.Warning, $"SHCA0028: No animationGUID.");
                }
                if (_animationID < 0)
                {
                    Log.Level(LogLevel.Warning, $"SHCA0029: No animationID.");
                }
#if DEBUG
                if (_animationGUID != null)
                {
                    Log.Info($"SHCA0030: Move {move} requested for ID: {_animationID} - name" +
                        $" {Translate(_animationGUID)} ({_animationGUID}).");
                    Log.Info($"SHCA0031: asset {_pathFemaleBase}");
                }
                else
                {
                    Log.Info($"SHCA0032: Move {move} requested for ID: {_animationID}.");
                }
#endif
                // Normal button press
                switch (move)
                {
                    case MoveType.RESET:
                        _newPosition = _controller._originalPosition;
                        _controller.ResetPosition();
                        break;

                    case MoveType.UP:
                        _newPosition = _chaControl.transform.position + _udAdjustUnit;
                        _doShortcutMove = true;
                        break;

                    case MoveType.DOWN:
                        _newPosition = _chaControl.transform.position - _udAdjustUnit;
                        _doShortcutMove = true;
                        break;

                    case MoveType.RIGHT:
                        if (chaType == CharacterType.Player)
                        {
                            _newPosition = _chaControl.transform.position - _lrAdjustUnit;
                        }
                        else
                        {
                            _newPosition = _chaControl.transform.position + _lrAdjustUnit;
                        }
                        _doShortcutMove = true;
                        break;

                    case MoveType.LEFT:
                        if (chaType == CharacterType.Player)
                        {
                            _newPosition = _chaControl.transform.position + _lrAdjustUnit;
                        }
                        else
                        {
                            _newPosition = _chaControl.transform.position - _lrAdjustUnit;
                        }
                        _doShortcutMove = true;
                        break;

                    case MoveType.CLOSER:
                        if (chaType == CharacterType.Player)
                        {
                            _newPosition = _chaControl.transform.position - _clAdjustUnit;
                        }
                        else
                        {
                            _newPosition = _chaControl.transform.position + _clAdjustUnit;
                        }
                        _doShortcutMove = true;
                        break;

                    case MoveType.APART:
                        if (chaType == CharacterType.Player)
                        {
                            _newPosition = _chaControl.transform.position + _clAdjustUnit;
                        }
                        else
                        {
                            _newPosition = _chaControl.transform.position - _clAdjustUnit;
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
                    Log.Info($"SHCA0033: Move from position" +
                        $" ({tmp.x }, {tmp.y}, {tmp.z}) {chaType}" +
                             $" to position" +
                        $" ({_newPosition.x }, {_newPosition.y}, {_newPosition.z}) {chaType}");
#endif
                    _doShortcutMove = false;
                    _chaControl.transform.position = _newPosition;
                    _guideObject.amount.position = _chaControl.transform.position;
                }
                _controller._lastMovePosition = _newPosition;
                return _doShortcutMove;
            }

            static internal string Translate(string name)
            {
                // if (!TranslationHelper.TryTranslate(name, out string tmp))
                if (!TranslationHelper.TryTranslate(name, out var tmp))
                {
                    return name;
                }

                return tmp;
            }

            static readonly internal Func<bool, Vector3, Vector3, Vector3> setDirection =
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
