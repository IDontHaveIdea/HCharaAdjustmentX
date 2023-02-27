//
// ActionButton
//
using UnityEngine;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public struct RotationActionButton : IColorActionStateButton
    {
        #region Properties
        public MoveType Move { get; set; }
        public CharacterType ChaType { get; set; }
        #endregion

        #region Interface Properties
        public string Text { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Rect Position { get; }
        #endregion

        #region Constructors
        public RotationActionButton(
            Rect position,
            string text,
            MoveType move,
            CharacterType chaType
            )
        {
            Position = position;
            Text = text;
            Move = move;
            ChaType = chaType;
            ForegroundColor = Color.white;
            BackgroundColor = Color.gray;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Trigger a rotation event
        /// </summary>
        public void Process()
        {
#if DEBUG
            _Log.Info($"[RotationActionButton]: {Text} Button pressed.");
#endif
            //InvokeOnMoveRequest(null, new MoveRequestEventArgs(ChaType, Move));
        }

        public void SetState(byte state)
        {
        }
        #endregion
        
    }
}
