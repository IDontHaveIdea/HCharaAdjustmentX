//
// Move events
//
using System;

using KKAPI.Chara;

using IDHIUtils;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        /// <summary>
        /// Move events
        /// </summary>
        public partial class HCharaAdjusmentXController : CharaCustomFunctionController
        {
            public static event EventHandler<MoveRequestEventArgs> OnMoveRequest;

            public class MoveRequestEventArgs : EventArgs
            {
                public MoveEvent.MoveType Move { get; }
                public CharacterType ChaType { get; }
                public MoveRequestEventArgs(CharacterType chaType, MoveEvent.MoveType move)
                {
                    ChaType = chaType;
                    Move = move;
                }
            }

            /// <summary>
            /// Trigger OnMoveRequest request event
            /// </summary>
            /// <param name="_sender"></param>
            /// <param name="_args"></param>
            internal static void InvokeOnMoveRequest(object _sender, MoveRequestEventArgs _args)
            {
                OnMoveRequest?.Invoke(_sender, _args);
            }

            /// <summary>
            /// Add action to OnMoveRequest event
            /// </summary>
            internal static void RegisterMovementEvents()
            {
                OnMoveRequest += (_sender, _args) => 
                {
                    _Log.Info($"[RegisterMovementEvents] Call to action {_args.Move} - {_args.ChaType}");
                    CharMovement.Move(_args.ChaType, _args.Move);
                };
            }
        }
    }
}
