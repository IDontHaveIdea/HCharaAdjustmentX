//
// Move action button definition
//
using UnityEngine;

using IDHIUtils;

using SHCA = IDHIPlugins.HCharaAdjustmentX;
using static IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;
using static IDHIPlugins.MoveEvent;

namespace IDHIPlugins
{
    public struct MoveActionButton : IColorButtonEventTrigger
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
        public void TriggerEvent()
        {
#if DEBUG
            SHCA._Log.Info($"SHCA0035: {Text} Button pressed for {ChaType} ");
#endif
            InvokeOnMoveRequest(null, new MoveRequestEventArgs(ChaType, Move));
        }
        #endregion
    }
}
