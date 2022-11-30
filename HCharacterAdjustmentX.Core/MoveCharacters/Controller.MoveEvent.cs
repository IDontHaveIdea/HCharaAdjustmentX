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
                internal MoveEvent.MoveType Move { get; }
                internal CharacterType ChaType { get; }

                internal MoveRequestEventArgs(CharacterType chaType,
                    MoveEvent.MoveType move)
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
            internal static void InvokeOnMoveRequest(object _sender,
                MoveRequestEventArgs _args)
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
#if DEBUG
                    _Log.Info($"HCAX0049: [RegisterMovementEvents] Call to action " +
                        $"{_args.Move} - {_args.ChaType}");
#endif
                    CharMovement.Move(_args.ChaType, _args.Move);
                };
            }
        }
    }
}
