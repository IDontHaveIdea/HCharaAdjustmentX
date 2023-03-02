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

        #region Interface Properties
        public string Text => $"Axis: {_currentAxis} ({AxisVector.Color[_currentAxis]})";
        public Color BackgroundColor { get; set; }
        public Color ForegroundColor { get; set; }
        public Rect Position { get; }
        #endregion

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
#if DEBUG
                _Log.Warning($"[Axis] _currentAxis={_currentAxis} for ChaType={ChaType} " +
                    $"set to={_controller.CurrentAxis}");
#endif
            }
        }
        public CharacterType ChaType { get; set; }
#endregion

        #region Constructors
        public AxisButton(
            Rect position,
            CharacterType chaType
            )
        {
            ChaType = chaType;
            _controller = GetControllerByType(chaType);
            Position = position;
            Axis = Axis.Y;
            ForegroundColor = Color.white;
            BackgroundColor = Color.gray;
#if DEBUG
            _Log.Warning($"[Axis] Constructor for {chaType} set ChaType={ChaType} " +
                $"set axis to={_controller.CurrentAxis} controller " +
                $"type={_controller.ChaType}");
#endif
        }
#endregion

        #region Interface public methods
        public void Process()
        {
#if DEBUG
            var axis = _controller.CurrentAxis;
#endif
            Axis = (Axis)(((int)_currentAxis + 1) % 3);
#if DEBUG
            _Log.Info($"[Axis.Process] Change axis from {axis} to {Axis}.");
#endif
        }
        #endregion
    }
}
