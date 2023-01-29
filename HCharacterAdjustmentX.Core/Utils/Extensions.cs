//
// Extensions
//
using UnityEngine;

using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;


namespace IDHIPlugins
{
    internal static class Extensions
    {
        public static string FormatVector(this Vector3 self)
        {
            var formatString = $"( {self.x,12:F7} , {self.y,12:F7}, {self.z,12:F7} )";

            return formatString;
        }

        public static CTRL GetController(this ChaControl chaControl)
        {
            return chaControl == null
                ? null : chaControl.GetComponent<CTRL>();
        }

        public static CTRL GetController(this SaveData.Heroine heroine)
        {
            return GetController(heroine?.chaCtrl);
        }
    }
}
