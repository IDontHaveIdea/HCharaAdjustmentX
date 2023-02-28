//
// ActionButton
//
using UnityEngine;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public struct RotationActionButton : IColorButton
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
            ButtonProperties b,
            CharacterType chaType
            )
        {
            Position = position;
            Text = b.Label;
            Move = b.MoveType;
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
            MoveEvent.InvokeOnRotationEvent(
                null,
                new MoveEvent.MoveEventArgs(ChaType, Move));
        }
        #endregion
        
    }
}
