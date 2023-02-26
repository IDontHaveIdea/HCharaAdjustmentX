//
// Move action button definition
//
using UnityEngine;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;
using static IDHIPlugins.Move;
using static IDHIPlugins.HCharaAdjustmentX;

namespace IDHIPlugins
{
    public struct MoveActionButton : IColorActionStateButton
    {
        #region public properties
        public string Text { get; set;  }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Rect Position { get; }
        public MoveType Move { get; set; }
        public CharacterType ChaType { get; set; }
        #endregion

        #region constructor
        public MoveActionButton(
            Rect position,
            string text,
            MoveType move,
            CharacterType chaType) : this()
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
        /// Trigger a move event
        /// </summary>
        public void Process()
        {
#if DEBUG
            _Log.Info($"HCAX0050: {Text} Button pressed for {ChaType} ");
#endif
            InvokeOnMoveRequest(null, new MoveRequestEventArgs(ChaType, Move));
        }

        public void SetState(byte state)
        {
        }
        #endregion
    }
}
