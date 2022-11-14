//
// Buttons interface for character movements
//
using System.Collections.Generic;

using UnityEngine;

using static IDHIPlugins.HCharaAdjustmentX;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        public class ButtonsGUI2
        {
            // The position of the scrolling view-port
            #region private fields
            //private const float height = 25f;
            //private const float width = 57f;
            private readonly float _width;
            private readonly float _heigth;
            private List<MoveActionButton> _buttons = new();
            #endregion

            #region public properties
            internal List<MoveActionButton> Buttons => _buttons;

            internal float Height => _heigth;

            internal float Width => _width;

            internal static CharacterType CharType
            {
                get; private set;
            } = CharacterType.Unknown;

            #endregion

            // Rect(x, y, width, height)


            /// <summary>
            /// Buttons interface
            /// </summary>
            /// <param name="xMargin"></param>
            /// <param name="yMargin"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="xOffset"></param>
            /// <param name="yOffset"></param>
            public ButtonsGUI2(float xMargin, float yMargin,
                float width, float height,
                float xOffset = 0f, float yOffset = 0f)
            {
                var x = Screen.width * xMargin;
                var y = Screen.height * yMargin;

                x = (x > 0f) ? x : Screen.width + x;
                x += xOffset;

                y = (y > 0f) ? y : Screen.width + y;
                y += yOffset;

                var windowRect = new Rect(x, y, width, height);
                var first = true;

                _width = width * (MoveEvent.doubleWidthLabels.Count > 0 ? 2 : 1);
                _heigth = MoveEvent.EventLabel.Keys.Count * height;

                foreach (var label in MoveEvent.EventLabel.Keys)
                {
                    if (MoveEvent.doubleWidthLabels.Contains(label))
                    {
                        windowRect.width *= 2;
                        var DWMAButton = new MoveActionButton(windowRect, label,
                            MoveEvent.EventLabel[label], CharType);
                        _buttons.Add(DWMAButton);
                        windowRect.width /= 2;
                        windowRect.y += height;
                        continue;
                    }
                    var MAButton = new MoveActionButton(windowRect, label,
                        MoveEvent.EventLabel[label], CharType);
                    _buttons.Add(MAButton);
                    if (first)
                    {
                        windowRect.x += width;
                        first = false;
                    }
                    else
                    {
                        windowRect.x -= width;
                        windowRect.y += height;
                        first = true;
                    }
                }
            }
        }
    }
}
