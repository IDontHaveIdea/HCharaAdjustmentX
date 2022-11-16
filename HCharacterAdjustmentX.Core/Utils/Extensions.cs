using System;

using HarmonyLib;

using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;
namespace IDHIPlugins
{
    internal static class Extensions
    {
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
