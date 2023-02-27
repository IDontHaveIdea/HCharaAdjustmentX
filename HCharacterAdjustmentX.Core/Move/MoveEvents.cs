//
// MoveEvent
//
using System;
using System.Collections.Generic;

using static IDHIPlugins.HCharaAdjustmentX;

namespace IDHIPlugins
{
    public enum MoveType
    {
        UNKNOWN = -1,
        UP,
        DOWN,
        LEFT,
        RIGHT,
        FORWARD,
        BACK,
        SAVE,
        LOAD,
        ROTP,
        ROTN,
        AXIS,
        RESETMOVE,
        RESETROTATION,
        MOVE
    }

    public partial class MoveEvent
    {
        public static event EventHandler<MoveEventArgs> OnPositionMoveEvent;
        public static event EventHandler<MoveEventArgs> OnRotationEvent;

        public class MoveEventArgs : EventArgs
        {
            internal MoveType Move { get; }
            internal CharacterType ChaType { get; }

            internal MoveEventArgs(
                CharacterType chaType,
                MoveType move)
            {
                ChaType = chaType;
                Move = move;
            }
        }

        /// <summary>
        /// Trigger OnPositionMoveEvent request event
        /// </summary>
        /// <param name="_sender">caller object</param>
        /// <param name="_args">event arguments</param>
        internal static void InvokeOnPositionMoveEvent(object _sender, MoveEventArgs _args)
        {
            OnPositionMoveEvent?.Invoke(_sender, _args);
        }

        /// <summary>
        /// Trigger OnRotationEvent request event
        /// </summary>
        /// <param name="_sender">caller object</param>
        /// <param name="_args">event arguments</param>
        internal static void InvokeOnRotationEvent(object _sender, MoveEventArgs _args)
        {
            OnPositionMoveEvent?.Invoke(_sender, _args);
        }

        // <summary>
        /// Add action to OnMoveEvent
        /// </summary>
        internal static void RegisterMoveEvents()
        {
            OnPositionMoveEvent += (_sender, _args) =>
            {
#if DEBUG
                _Log.Info($"[RegisterMoveEvents] Call to position move action " +
                    $"{_args.Move}");
#endif
                CharPositionMovement.Move(_args.ChaType, _args.Move);
            };

            OnRotationEvent += (_sender, _args) =>
            {
#if DEBUG
                _Log.Info($"[RegisterMoveEvents] Call to rotation action " +
                    $"{_args.Move}");
#endif
                CharRotationMovement.Move(_args.ChaType, _args.Move);
            };
        }
    }
}
