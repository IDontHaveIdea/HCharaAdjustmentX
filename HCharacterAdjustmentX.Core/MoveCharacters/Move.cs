//
// MoveEvent
//
using System.Collections.Generic;

using UnityEngine;

namespace IDHIPlugins
{
    public readonly struct Move
    {
        public static readonly List<string> buttonLabels = new()
        {
            "Up",
            "Down",
            "Left",
            "Right",
            "Forward",
            "Back",
            "Save",
            "Load",
            "Rot. +",
            "Rot. -",
            "Axis",
            "R. Move",
            "R. Rot."
        };

        public static readonly List<string> doubleWidthLabels = new()
        {
            buttonLabels[10]
        };

        public static readonly Dictionary<string, MoveType> LabelType = new()
        {
            { buttonLabels[0], MoveType.UP },
            { buttonLabels[1], MoveType.DOWN },
            { buttonLabels[2], MoveType.LEFT },
            { buttonLabels[3], MoveType.RIGHT },
            { buttonLabels[4], MoveType.FORWARD },
            { buttonLabels[5], MoveType.BACK },
            { buttonLabels[6], MoveType.SAVE },
            { buttonLabels[7], MoveType.LOAD },
            { buttonLabels[8], MoveType.POSITIVEROTATION },
            { buttonLabels[9], MoveType.NEGATIVEROTATION },
            { buttonLabels[10], MoveType.AXIS },
            { buttonLabels[11], MoveType.RESETMOVE },
            { buttonLabels[12], MoveType.RESETROTATION }
        };
    }
}
