//
// Buttons GUI interface for character movements
//
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using IDHIUtils;


namespace IDHIPlugIns
{
    public partial class HCharaAdjustmentX
    {
        public class ButtonsGUI
        {
            // The position of the scrolling view-port
            #region private fields
            private readonly float _width;
            private readonly float _heigth;
            private readonly int _heightBase = 1080;
            //private int _widthBase = 1920;
            private List<IColorButton> _buttons = new();
            #endregion

            #region public properties
            internal List<IColorButton> Buttons => _buttons;
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
                // Adjust for screen size
                if (Screen.height != _heightBase)
                {
                    height = (int)((Screen.height * (int)height) / _heightBase);
                }

                // TODO: work with width have to change font size 720 may be to small
                // altering only the height kind of works
                //if (Screen.width != _widthBase)
                //{
                //    width = (int)((Screen.width * (int)width) / _widthBase);
                //}

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

                _width = width * (Move.Buttons.Where(x => x.DoubleWide).Any() ? 2 : 1);
                _heigth = Move.Buttons.Count * height;

                foreach (var b in Move.Buttons)
                {
                    if (AnimationKey.IsNullOrEmpty()
                        && ((b.MoveType == MoveType.SAVE) || (b.MoveType == MoveType.LOAD)))
                    {
                        continue;
                    }

                    if (b.DoubleWide)
                    {
                        windowRect.width *= 2;

                        _buttons.Add(NewButton(windowRect, b, chaType));

                        windowRect.width /= 2;
                        windowRect.y += height;
                        continue;
                    }

                    _buttons.Add(NewButton(windowRect, b, chaType));

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

            private static IColorButton NewButton(
                Rect windowRect,
                ButtonProperties b,
                CharacterType chaType)
            {
                IColorButton button;

                if (b.MoveType == MoveType.AXIS)
                {
                    button = new AxisButton(
                        windowRect,
                        chaType);
                }
                else if (b.ActionType == ActionType.ROTATION)
                {
                    button = new RotationActionButton(
                        windowRect,
                        b,
                        chaType);
                }
                else
                {
                    button = new MoveActionButton(
                        windowRect,
                        b,
                        chaType);
                }

                return button;
            }
        }
    }
}
