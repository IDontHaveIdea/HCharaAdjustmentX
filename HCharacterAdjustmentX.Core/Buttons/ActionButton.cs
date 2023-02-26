//
// ActionButton
//
using UnityEngine;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public struct ActionButton : IColorActionStateButton
    {
        #region Properties
        public string Text { get; set; }
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Rect Position { get; }
        #endregion

        #region Constructors
        public ActionButton(
            Rect position,
            string text
            )
        {
            Position = position;
            Text = text;
        }
        #endregion

        #region public methods
        /// <summary>
        /// Trigger a move event
        /// </summary>
        public void Process()
        {
#if DEBUG
            _Log.Info($"[ActionButton]: {Text} Button pressed.");
#endif
            //InvokeOnMoveRequest(null, new MoveRequestEventArgs(ChaType, Move));
        }

        public void SetState(byte state)
        {
        }
        #endregion
        
    }
}
