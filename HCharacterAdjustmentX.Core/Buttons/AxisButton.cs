//
// ActionButton
//
using System.Collections.Generic;

using UnityEngine;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public enum Axis { UNKNOWN = -1, X = 0, Y = 1, Z = 2 }
    public struct AxisButton : IColorStateToggleButton
    {
        private Axis _currentAxis;

        /*private static readonly Dictionary<byte, string> _axisStateNames = new()
        {
            {0, "X"},
            {1, "Y"},
            {2, "Z"}
        };

        public static readonly Dictionary<string, byte> _axisState = new()
        {
            {"X", 0},
            {"Y", 1},
            {"Z", 2}
        };*/

        #region Properties
        public Axis Axis => _currentAxis;
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

        public void NextState()
        {
            _currentAxis = (Axis)(((int)_currentAxis + 1) % 3);
        }
        #endregion

    }
}
