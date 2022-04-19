using System.Collections.Generic;


namespace IDHIPlugins
{
    public struct MoveEvent
    {
        public static readonly List<string> buttonLabels =
            new()
            {
                "Up",
                "Down",
                "Left",
                "Right",
                "Apart",
                "Closer",
                "Save",
                "Load",
                "Reset"
            };
        public static readonly List<string> doubleWidthLabels =
            new()
            {
                buttonLabels[8]
            };
        public enum MoveType { UP, DOWN, LEFT, RIGHT, APART, CLOSER, SAVE, LOAD, RESET, UNKNOWN }

        public static readonly Dictionary<string, MoveType> EventLabel =
            new()
            {
                { buttonLabels[0], MoveType.UP },
                { buttonLabels[1], MoveType.DOWN },
                { buttonLabels[2], MoveType.LEFT },
                { buttonLabels[3], MoveType.RIGHT },
                { buttonLabels[4], MoveType.APART },
                { buttonLabels[5], MoveType.CLOSER },
                { buttonLabels[6], MoveType.SAVE },
                { buttonLabels[7], MoveType.LOAD },
                { buttonLabels[8], MoveType.RESET }
            };
    }
}
