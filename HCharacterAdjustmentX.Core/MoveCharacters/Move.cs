//
// MoveEvent
//
using System.Collections.Generic;

using UnityEngine;

namespace IDHIPlugins
{
    public enum ActionType
    {
        POSITION,
        ROTATION,
        AXIS
    }

    public struct ButtonProperties
    {
        public string Label { get; set; }
        public MoveType MoveType { get; set; }
        public ActionType ActionType { get; set; }
        public bool DoubleWide { get; set; }
        public ButtonProperties(
            string label,
            MoveType moveType,
            ActionType actionType,
            bool doubleWide = false)
        {
            Label = label;
            MoveType = moveType;
            ActionType = actionType;
            DoubleWide = doubleWide;
        }
    }

    public readonly struct Move
    {
        public static readonly List<ButtonProperties> Buttons = new()
        {
            { new(     "Up", MoveType.UP, ActionType.POSITION) },
            { new(   "Down", MoveType.DOWN, ActionType.POSITION) },
            { new(   "Left", MoveType.LEFT, ActionType.POSITION) },
            { new(  "Right", MoveType.RIGHT, ActionType.POSITION) },
            { new("Forward", MoveType.FORWARD, ActionType.POSITION) },
            { new(   "Back", MoveType.BACK, ActionType.POSITION) },
            { new( "Rot. +", MoveType.POSITIVEROTATION, ActionType.ROTATION) },
            { new( "Rot. -", MoveType.NEGATIVEROTATION, ActionType.ROTATION) },
            { new(   "Axis", MoveType.AXIS, ActionType.AXIS, true) },
            { new(   "Save", MoveType.SAVE, ActionType.POSITION) },
            { new(   "Load", MoveType.LOAD, ActionType.POSITION) },
            { new("R. Move", MoveType.RESETMOVE, ActionType.POSITION) },
            { new("R. Rot.", MoveType.RESETROTATION, ActionType.ROTATION) }
        };
    }
}
