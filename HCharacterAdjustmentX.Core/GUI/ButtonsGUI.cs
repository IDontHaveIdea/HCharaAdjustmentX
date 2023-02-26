// Buttons interface for character movements
//
using System.Collections.Generic;
using UnityEngine;

using IDHIUtils;

using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;
using static IDHIPlugins.HCharaAdjustmentX;
using static FaceScreenShot;

namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        public class ButtonsGUI
        {
            // The position of the scrolling view-port
            #region private fields
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
            /// Margins calculation use percent amounts
            /// 1 - Get percent of x or y margins for the start pixel position
            /// 2 - To that result add offset
            ///
            /// Normally there will be a margin percent or an offset.
            /// A margin equal to 0 and a negative offset compute the position
            /// for x from the left and for y off the bottom.
            /// 
            /// </summary>
            /// <param name="chaType">character type</param>
            /// <param name="xMargin">x margin percent</param>
            /// <param name="yMargin">y margin percent</param>
            /// <param name="width">button width</param>
            /// <param name="height">button height</param>
            /// <param name="xOffset">x margin offset</param>
            /// <param name="yOffset">y margin offset</param>
            internal ButtonsGUI(CharacterType chaType, float xMargin, float yMargin,
                float width, float height,
                float xOffset = 0f, float yOffset = 0f)
            {
                // Get margin percent base on screen width and height
                var x = Screen.width * xMargin;
                var y = Screen.height * yMargin;

                x = (x > 0f) ? x : Screen.width;
                // Add offset with x equal to 0 and negative offset position moves from
                // right edge
                x += xOffset;

                y = (y > 0f) ? y : Screen.height;
                // Add offset with y equal to 0 and negative offset position moves from
                // bottom edge
                y += yOffset;

                var windowRect = new Rect(x, y, width, height);
                var first = true;

                _width = width * (Move.doubleWidthLabels.Count > 0 ? 2 : 1);
                _heigth = Move.LabelType.Keys.Count * height;

                foreach (var label in Move.LabelType.Keys)
                {
                    if (_animationKey.IsNullOrEmpty()
                        && ((label == "Save") || (label == "Load")))
                    {
                        continue;
                    }
                    if (Move.doubleWidthLabels.Contains(label))
                    {
                        windowRect.width *= 2;
                        var DWMAButton = new MoveActionButton(windowRect, label, 
                            Move.LabelType[label], chaType);
                        _buttons.Add(DWMAButton);
                        windowRect.width /= 2;
                        windowRect.y += height;
                        continue;
                    }
                    var MAButton = new MoveActionButton(windowRect, label, 
                        Move.LabelType[label], chaType);
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

        public class ButtonsGUI2
        {
            // The position of the scrolling view-port
            #region private fields
            private readonly float _width;
            private readonly float _heigth;
            private List<IColorActionStateButton> _buttons = new();
            #endregion

            #region public properties
            internal List<IColorActionStateButton> Buttons => _buttons;
            public float Height => _heigth;
            public float Width => _width;
            #endregion

            // Rect(x, y, width, height)

            /// <summary>
            /// Buttons Interface for character movement
            /// Margins calculation use percent amounts
            /// 1 - Get percent of x or y margins for the start pixel position
            /// 2 - To that result add offset
            ///
            /// Normally there will be a margin percent or an offset.
            /// A margin equal to 0 and a negative offset compute the position
            /// for x from the left and for y off the bottom.
            /// 
            /// </summary>
            /// <param name="chaType">character type</param>
            /// <param name="xMargin">x margin percent</param>
            /// <param name="yMargin">y margin percent</param>
            /// <param name="width">button width</param>
            /// <param name="height">button height</param>
            /// <param name="xOffset">x margin offset</param>
            /// <param name="yOffset">y margin offset</param>
            internal ButtonsGUI2(CharacterType chaType, float xMargin, float yMargin,
                float width, float height,
                float xOffset = 0f, float yOffset = 0f)
            {
                // Get margin percent base on screen width and height
                var x = Screen.width * xMargin;
                var y = Screen.height * yMargin;

                x = (x > 0f) ? x : Screen.width;
                // Add offset with x equal to 0 and negative offset position moves from
                // right edge
                x += xOffset;

                y = (y > 0f) ? y : Screen.height;
                // Add offset with y equal to 0 and negative offset position moves from
                // bottom edge
                y += yOffset;

                var windowRect = new Rect(x, y, width, height);
                var first = true;

                _width = width * (Move.doubleWidthLabels.Count > 0 ? 2 : 1);
                _heigth = Move.LabelType.Keys.Count * height;

                foreach (var label in Move.LabelType.Keys)
                {
                    if (_animationKey.IsNullOrEmpty()
                        && ((label == "Save") || (label == "Load")))
                    {
                        continue;
                    }

                    if (Move.doubleWidthLabels.Contains(label))
                    {
                        windowRect.width *= 2;

                        var DWMAButton = NewButton(windowRect, label, chaType);
                        _buttons.Add(DWMAButton);

                        windowRect.width /= 2;
                        windowRect.y += height;
                        continue;
                    }

                    var MAButton = NewButton(windowRect, label, chaType);
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

            private static IColorActionStateButton NewButton(
                Rect windowRect,
                string label,
                CharacterType chaType)
            {
                IColorActionStateButton button;

                if (label == "Axis")
                {
                    button = new AxisButton(
                        windowRect);
                }
                else
                {
                    button = new MoveActionButton(
                        windowRect,
                        label,
                        Move.LabelType[label],
                        chaType);
                }

                return button;
            }
        }
    }
}
