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
#if DEBUG
                "Test1",
                "Test2",
#endif
                "Reset"
            };
        public static readonly List<string> doubleWidthLabels =
            new()
            {
                buttonLabels[10]
            };
        public enum MoveType
        {
            UP,
            DOWN,
            LEFT,
            RIGHT,
            APART,
            CLOSER,
            SAVE,
            LOAD,
#if DEBUG
            TEST1,
            TEST2,
#endif
            RESET,
            MOVE,
            UNKNOWN }

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
#if DEBUG
                { buttonLabels[8], MoveType.TEST1 },
                { buttonLabels[9], MoveType.TEST2 },
                { buttonLabels[10], MoveType.RESET }
#else
                { buttonLabels[8], MoveType.RESET }
#endif
            };
    }
}
