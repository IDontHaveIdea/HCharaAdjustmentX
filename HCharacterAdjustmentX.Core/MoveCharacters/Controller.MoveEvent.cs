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
            static public event EventHandler<MoveRequestEventArgs> OnMoveRequest;

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
            static internal void InvokeOnMoveRequest(object _sender, MoveRequestEventArgs _args)
            {
                OnMoveRequest?.Invoke(_sender, _args);
            }

            // static internal void InvokeOnMoveRequest(object _sender, MoveRequestEventArgs _args) =>
            //     OnMoveRequest?.Invoke(_sender, _args);


            /// <summary>
            /// Add action to OnMoveRequest event
            /// </summary>
            static internal void RegisterMovementEvents()
            {
                OnMoveRequest += (_sender, _args) => 
                {
                    _Log.Info($"Call to action {_args.Move} - {_args.ChaType}");
                    CharMovement.Move(_args.ChaType, _args.Move);
                };
            }
        }
    }
}
