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

        /// <summary>
        /// Adjust move vector to transform vector in Unity when using transform it looks
        /// that moving forward sometimes is in the X axis (Why?) vector representing a
        /// move are in the form (right, up, forward) this adjust the vector to the
        /// game interpretation
        /// </summary>
        /// <param name="self">object self reference</param>
        /// <param name="transform">character Transform</param>
        /// <returns></returns>
        public static Vector3 MovementTransform(this Vector3 self, Transform transform)
        {
            var result = new Vector3(0, 0, 0);

            result += transform.right * self.x;
            result += transform.up * self.y;
            result += transform.forward * self.z;

            return result;
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
