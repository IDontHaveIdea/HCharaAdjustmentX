using System.Collections.Generic;

using UnityEngine;
using MessagePack;

using HSceneUtility;

using KKAPI;
using KKAPI.Chara;

using IDHIUtils;


namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        [MessagePackObject]
        public class PositionData
        {
            [Key(0)]
            public Vector3 HeroinePositionAdjustment;
            [Key(1)]
            public Vector3 PlayerPositionAdjustment;
            [Key(2)]
            public Vector3 HeroineRotationAdjustment;
            [Key(3)]
            public Vector3 PlayerRotationAdjustment;

            public PositionData()
            {
                HeroinePositionAdjustment = Vector3.zero;
                PlayerPositionAdjustment = Vector3.zero;
                HeroineRotationAdjustment = Vector3.zero;
                PlayerRotationAdjustment = Vector3.zero;
            }

            public PositionData(
                Vector3 heroineAdjustment,
                Vector3 playerAdjustment)
            {
                HeroinePositionAdjustment = heroineAdjustment;
                PlayerPositionAdjustment = playerAdjustment;
                HeroineRotationAdjustment = Vector3.zero;
                PlayerRotationAdjustment = Vector3.zero;
            }

            public PositionData(
                Vector3 heroineAdjustment,
                Vector3 playerAdjustment,
                Vector3 heroineRotation,
                Vector3 playerRotation)
            {
                HeroinePositionAdjustment = heroineAdjustment;
                PlayerPositionAdjustment = playerAdjustment;
                HeroineRotationAdjustment = heroineRotation;
                PlayerRotationAdjustment = playerRotation;
            }
        }
    }
}
