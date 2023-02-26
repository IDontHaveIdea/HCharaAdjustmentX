//
// ActionButton
//
using System.Collections.Generic;

using UnityEngine;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public enum Axis
    {
        UNKNOWN = -1,
        X = 0,
        Y = 1,
        Z = 2
    }

    public struct AxisButton : IColorActionStateButton
    {
        private Axis _currentAxis;

        #region Properties
        public Axis Axis
        {
            get
            {
                return _currentAxis;
            }

            private set
            {
                _currentAxis = value;
                CharMovement.CurrentAxis = value;
            }
        }
        #endregion

        #region Interface Properties
        //public string Text { get; set; }
        public string Text => $"Axis: {_currentAxis}";
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Rect Position { get; }
        #endregion

        #region Constructors
        public AxisButton(
            Rect position
            )
        {
            Position = position;
            _currentAxis = Axis.X;
        }
        #endregion

        #region public methods
        public void SetState(byte state)
        {
            if ((state >= 0) && (state < 3))
            {
                _currentAxis = (Axis)state;
            }
        }

        public void SetState(Axis axis)
        {
            _currentAxis = axis;
        }

        public void Process()
        {
            _currentAxis = (Axis)(((int)_currentAxis + 1) % 3);
        }
        #endregion
    }
}
