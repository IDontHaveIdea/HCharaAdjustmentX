//
// MoveEvent
//
using System;
using System.Collections.Generic;

using static IDHIPlugins.HCharaAdjustmentX;

namespace IDHIPlugins
{
    public enum MoveKind
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

    public enum CharType
    {
        Heroine,
        Heroine3P,
        Player,
        Janitor,
        Group,
        Unknown
    }

    public partial class MoveEvent2
    {
        public static event EventHandler<MoveEventArgs> OnMoveEvent;

        public class MoveEventArgs : EventArgs
        {
            internal MoveKind Move { get; }
            //internal CharacterType ChaType { get; }

            internal MoveEventArgs(
                //CharacterType chaType,
                MoveKind move)
            {
                //ChaType = chaType;
                Move = move;
            }
        }

        /// <summary>
        /// Trigger OnMoveEvent request event
        /// </summary>
        /// <param name="_sender">caller object</param>
        /// <param name="_args">event arguments</param>
        internal static void InvokeOnMoveRequest(object _sender, MoveEventArgs _args)
        {
            OnMoveEvent?.Invoke(_sender, _args);
        }

        // <summary>
        /// Add action to OnMoveEvent
        /// </summary>
        internal static void RegisterMovementEvents()
        {
            OnMoveEvent += (_sender, _args) =>
            {
#if DEBUG
                _Log.Info($"[RegisterMovementEvents] Call to action " +
                    $"{_args.Move}");
#endif
                //CharMovement.Move(_args.ChaType, _args.Move);
            };
        }
    }
}
