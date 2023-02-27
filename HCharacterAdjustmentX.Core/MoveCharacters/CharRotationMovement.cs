using System;
using System.Collections.Generic;

using UnityEngine;

using BepInEx.Logging;

using KKAPI.MainGame;
using KKAPI.Utilities;

using IDHIUtils;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        /// <summary>
        /// Take care of movement requests
        /// </summary>
        internal class CharRotationMovement
        {
            #region private fields
            internal static ChaControl _chaControl;
            internal static HCharaAdjusmentXController _controller;
            internal static bool _doAngleMove;
            internal static bool _positiveMove;
            #endregion

            #region Properties
            internal static Axis CurrentAxis { get; set; }
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
                _doAngleMove = false;

                _controller = GetControllerByType(chaType);

                // Normal button press
                var rotationPosition = _controller.OriginalRotation;
                var angleMovement = _controller.Rotation;

                Vector3 newRotation = new(0, 0, 0);

                switch (moveType)
                {
                    case MoveType.RESETROTATION:
                        _controller.ResetRotation();
                        break;
                }

                if (_doAngleMove)
                {
                }
                return _doAngleMove;
            }

            internal static Vector3 RecalcPosition(
                ChaControl chaControl,
                Vector3 original,
                Vector3 move,
                Vector3 fullMove)
            {
                try
                {
                    var right = chaControl.transform.right * move.x;
                    var up = chaControl.transform.up * move.y;
                    var forward = chaControl.transform.forward * move.z;

                    var currentPosition = chaControl.transform.position;

                    var newPosition = original;
                    var fullNewPosition = original + fullMove
                        .MovementTransform(chaControl.transform);

                    newPosition += up;
                    newPosition += right;
                    newPosition += forward;

                    if (DebugInfo.Value)
                    {
                        _Log.Debug($"[RecalcPosition] Move {chaControl.name}\n" +
                            $" original position {original.Format()}\n" +
                            $"  current position {currentPosition.Format()}\n" +
                            $"      move by axis {move.Format()}\n" +
                            $"    move by vector {fullMove.Format()}\n" +
                            $"           right x {right.Format()}\n" +
                            $"              up y {up.Format()}\n" +
                            $"         forward z {forward.Format()}\n" +
                            $"  position by axis {newPosition.Format()}\n" +
                            $"position by vector {fullNewPosition.Format()}");
                    }
                    return newPosition;
                }
                catch (Exception e)
                {
                    _Log.Level(LogLevel.Error, $"HCAX0048: Cannot adjust position " +
                        $"{chaControl.name} - {e}.");
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
