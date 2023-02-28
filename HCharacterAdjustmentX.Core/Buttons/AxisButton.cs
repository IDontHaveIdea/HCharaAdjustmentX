//
// ActionButton
//
using UnityEngine;

using IDHIUtils;

using static IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public struct AxisButton : IColorButton
    {
        private Axis _currentAxis;
        private readonly HCharaAdjusmentXController _controller;

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
                _controller.CurrentAxis = value;
            }
        }
        public CharacterType ChaType { get; private set; }
        #endregion

        #region Interface Properties
        public string Text => $"Axis: {_currentAxis}";
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Rect Position { get; }
        #endregion

        #region Constructors
        public AxisButton(
            Rect position,
            CharacterType chaType
            )
        {
            _controller = GetControllerByType(chaType);
            Position = position;
            Axis = Axis.Y;
            ChaType = chaType;
            ForegroundColor = Color.white;
            BackgroundColor = Color.gray;
        }
        #endregion

        #region public methods
        public void Process()
        {
            Axis = (Axis)(((int)_currentAxis + 1) % 3);
        }
        #endregion
    }
}
