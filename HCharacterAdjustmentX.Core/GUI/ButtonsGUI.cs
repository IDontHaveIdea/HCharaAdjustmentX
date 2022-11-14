//
// Buttons interface for character movements
//
using System.Collections.Generic;
using UnityEngine;

using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;
using static IDHIPlugins.HCharaAdjustmentX;

namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        public class ButtonsGUI
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
            public float Height => _heigth;
            public float Width => _width;
            #endregion

            // Rect(x, y, width, height)

            /// <summary>
            /// Buttons Interface for character movement
            /// </summary>
            /// <param name="chaType"></param>
            /// <param name="xMargin"></param>
            /// <param name="yMargin"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <param name="xOffset"></param>
            /// <param name="yOffset"></param>
            internal ButtonsGUI(CharacterType chaType, float xMargin, float yMargin,
                float width, float height,
                float xOffset = 0f, float yOffset = 0f)
            {
                var x = Screen.width * xMargin;
                var y = Screen.height * yMargin;

                x = (x > 0f) ? x : Screen.width + x;
                x += xOffset;

                y = (y > 0f) ? y : Screen.height + y;
                y += yOffset;

                var windowRect = new Rect(x, y, width, height);
                var first = true;

                _width = width * (MoveEvent.doubleWidthLabels.Count > 0 ? 2 : 1);
                _heigth = MoveEvent.EventLabel.Keys.Count * height;

                foreach (var label in MoveEvent.EventLabel.Keys)
                {
                    if (_animationKey.IsNullOrEmpty()
                        && ((label == "Save") || (label == "Load")))
                    {
                        continue;
                    }
                    if (MoveEvent.doubleWidthLabels.Contains(label))
                    {
                        windowRect.width *= 2;
                        var DWMAButton = new MoveActionButton(windowRect, label, 
                            MoveEvent.EventLabel[label], chaType);
                        _buttons.Add(DWMAButton);
                        windowRect.width /= 2;
                        windowRect.y += height;
                        continue;
                    }
                    var MAButton = new MoveActionButton(windowRect, label, 
                        MoveEvent.EventLabel[label], chaType);
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
