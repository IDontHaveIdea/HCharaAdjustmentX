//
// Plugin.Data
//
using System.Collections.Generic;

using UnityEngine;
using MessagePack;

using IDHIUtils;

using CTRL = IDHIPlugins.HCharaAdjustmentX.HCharaAdjusmentXController;
using System.Text;
using System;
using ExtensibleSaveFormat;

namespace IDHIPlugins
{
    public partial class HCharaAdjustmentX
    {
        /// <summary>
        /// This can be serialized by MessagePack
        /// </summary>
        [MessagePackObject(true)]
        public class PositionDataPair
        {
            [Key(0)]
            public Vector3 HeroinePositionAdjustment;
            [Key(1)]
            public Vector3 PlayerPositionAdjustment;
            [Key(2)]
            public Vector3 HeroineRotationAdjustment;
            [Key(3)]
            public Vector3 PlayerRotationAdjustment;

            public PositionDataPair()
            {
                HeroinePositionAdjustment = Vector3.zero;
                PlayerPositionAdjustment = Vector3.zero;
                HeroineRotationAdjustment = Vector3.zero;
                PlayerRotationAdjustment = Vector3.zero;
            }

            public PositionDataPair(
                Vector3 heroineAdjustment,
                Vector3 playerAdjustment)
            {
                HeroinePositionAdjustment = heroineAdjustment;
                PlayerPositionAdjustment = playerAdjustment;
                HeroineRotationAdjustment = Vector3.zero;
                PlayerRotationAdjustment = Vector3.zero;
            }

            public PositionDataPair(
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

        /// <summary>
        /// Simpler easier to manage
        /// TODO: Work on rotation
        /// </summary>
        [MessagePackObject]
        public class PositionData
        {
            [Key(0)]
            public Vector3 PositionAdjustment;
            [Key(1)]
            public Vector3 RotationAdjustment;

            public PositionData()
            {
                PositionAdjustment = Vector3.zero;
                RotationAdjustment = Vector3.zero;
            }

            public PositionData(
                Vector3 position,
                Vector3 rotation)
            {
                PositionAdjustment = position;
                RotationAdjustment = rotation;
            }
        }

        /// <summary>
        /// Converts MoveData to a format that can be serialized
        /// </summary>
        /// <param name="MoveData">movement data</param>
        /// <returns></returns>
        internal static Dictionary<string, PositionDataPair> PrepareSerialize(
            Dictionary<string,
                Dictionary<CTRL.CharacterType, PositionData>> MoveData)
        {
            var MoveDataToSerialize = new Dictionary<string, PositionDataPair>();

            Vector3 HeroinePositionAdjustment;
            Vector3 PlayerPositionAdjustment;
            Vector3 HeroineRotationAdjustment;
            Vector3 PlayerRotationAdjustment;

            foreach (var item in MoveData)
            {
                HeroinePositionAdjustment = Vector3.zero;
                PlayerPositionAdjustment = Vector3.zero;
                HeroineRotationAdjustment = Vector3.zero;
                PlayerRotationAdjustment = Vector3.zero;

                foreach (var character in item.Value)
                {
                    if (character.Key == CTRL.CharacterType.Heroine)
                    {
                        HeroinePositionAdjustment = character.Value.PositionAdjustment;
                        HeroineRotationAdjustment = character.Value.RotationAdjustment;
                    }
                    if (character.Key == CTRL.CharacterType.Player)
                    {
                        PlayerPositionAdjustment = character.Value.PositionAdjustment;
                        PlayerRotationAdjustment = character.Value.RotationAdjustment;
                    }
                }
                MoveDataToSerialize[item.Key] = new PositionDataPair(
                    HeroinePositionAdjustment,
                    PlayerPositionAdjustment,
                    HeroineRotationAdjustment,
                    PlayerRotationAdjustment);
            }
            return MoveDataToSerialize;
        }

        /// <summary>
        /// Converts serialized friendly data to MoveData
        /// </summary>
        /// <param name="MoveDataPair"></param>
        /// <returns></returns>
        internal static Dictionary<string,
                Dictionary<CTRL.CharacterType, PositionData>> RestoreMoveData(
            Dictionary<string, PositionDataPair> MoveDataPair)
        {
            var Position = new Dictionary<CTRL.CharacterType, PositionData>();
            var MoveData = new Dictionary<string,
                Dictionary<CTRL.CharacterType, PositionData>>();

            foreach (var item in MoveDataPair)
            {
                Position.Clear();
                Position[CTRL.CharacterType.Heroine] = new(
                    item.Value.HeroinePositionAdjustment,
                    item.Value.HeroineRotationAdjustment);
                // Add Player if any vector is non zero.
                if ((item.Value.PlayerPositionAdjustment != Vector3.zero)
                    || (item.Value.PlayerRotationAdjustment != Vector3.zero))
                {
                    Position[CTRL.CharacterType.Player] = new(
                    item.Value.PlayerPositionAdjustment,
                    item.Value.PlayerRotationAdjustment);
                }

                MoveData[item.Key] = new(Position);
            }

            return MoveData;
        }

        internal static void PrintData(Dictionary<string,
                Dictionary<CTRL.CharacterType, PositionData>> MoveData)
        {
            var lines = new StringBuilder();

            foreach (var item in MoveData)
            {
                foreach (var character in item.Value)
                {
                    lines.AppendLine($"Position={item.Key} " +
                        $"Character={character.Key} " +
                        $"Position=" +
                        $"{character.Value.PositionAdjustment.ToString("F7")} " +
                        $"Rotation=" +
                        $"{character.Value.RotationAdjustment.ToString("F7")}");
                }
            }

            if (lines.Length > 0)
            {
                _Log.Warning($"[PrintData]\n\n{lines.ToString()}\n");
            }
        }

        internal static void PrintData(
            Dictionary<string, PositionDataPair> MoveData)
        {
            var lines = new StringBuilder();

            foreach (var item in MoveData)
            {
                lines.AppendLine($"Position={item.Key} Heroine" +
                    $"Position={item.Value.HeroinePositionAdjustment.ToString("F7")} " +
                    $"Rotation={item.Value.HeroineRotationAdjustment.ToString("F7")}");
            }

            if (lines.Length > 0)
            {
                _Log.Warning($"[PrintData]\n\n{lines.ToString()}\n");
            }
        }


        /*
        public sealed class CMoveData
        {
            private Dictionary<string,
                Dictionary<CTRL.CharacterType, PositionData>> _MoveData;

            public Dictionary<string,
                Dictionary<CTRL.CharacterType, PositionData>> MoveData
            {
                get { return _MoveData; }
                set { _MoveData = value; }
            }

            public CMoveData()
            {
                _MoveData = new Dictionary<string,
                    Dictionary<CTRL.CharacterType, PositionData>>();
            }

            public Dictionary<CTRL.CharacterType, PositionData> this[string key]
            {
                get { return _MoveData[key]; }
                set { _MoveData[key] = value; }
            }


            public void Load(PluginData data)
            {
                Dictionary<string, PositionDataPair> MoveDataSerialize;

                if (data.data.TryGetValue(nameof(MoveData),
                        out var loadedMoveData)
                        && loadedMoveData != null)
                {
                    MoveDataSerialize = MessagePackSerializer
                        .Deserialize<Dictionary<string, PositionDataPair>>
                        ((byte[])loadedMoveData);
                    if (MoveDataSerialize != null)
                    {
                        MoveData = RestoreMoveData(MoveDataSerialize);
#if DEBUG
                        PrintData(MoveData);
#endif
                    }
                    else
                    {
                        _Log.Error($"[ReadData] Can't unpack data for.");
                    }
                }
            }

            public PluginData Save()
            {
                var plugData = new PluginData {
                    version = 1
                };
                var MoveDataSerialize = PrepareSerialize();
                plugData.data.Add(nameof(MoveData),
                        MessagePackSerializer.Serialize(MoveDataSerialize));

                return plugData.data.Count > 0 ? plugData : null;
            }

            private Dictionary<string, PositionDataPair> PrepareSerialize()
            {
                var MoveDataToSerialize = new Dictionary<string, PositionDataPair>();

                Vector3 HeroinePositionAdjustment;
                Vector3 PlayerPositionAdjustment;
                Vector3 HeroineRotationAdjustment;
                Vector3 PlayerRotationAdjustment;

                foreach (var item in MoveData)
                {
                    HeroinePositionAdjustment = Vector3.zero;
                    PlayerPositionAdjustment = Vector3.zero;
                    HeroineRotationAdjustment = Vector3.zero;
                    PlayerRotationAdjustment = Vector3.zero;

                    foreach (var character in item.Value)
                    {
                        if (character.Key == CTRL.CharacterType.Heroine)
                        {
                            HeroinePositionAdjustment = character.Value.PositionAdjustment;
                            HeroineRotationAdjustment = character.Value.RotationAdjustment;
                        }
                        if (character.Key == CTRL.CharacterType.Player)
                        {
                            PlayerPositionAdjustment = character.Value.PositionAdjustment;
                            PlayerRotationAdjustment = character.Value.RotationAdjustment;
                        }
                    }
                    MoveDataToSerialize[item.Key] = new PositionDataPair(
                        HeroinePositionAdjustment,
                        PlayerPositionAdjustment,
                        HeroineRotationAdjustment,
                        PlayerRotationAdjustment);
                }
                return MoveDataToSerialize;
            }


            private Dictionary<string,
                Dictionary<CTRL.CharacterType, PositionData>> RestoreMoveData(
            Dictionary<string, PositionDataPair> MoveDataPair)
            {
                var Position = new Dictionary<CTRL.CharacterType, PositionData>();
                var MoveData = new Dictionary<string,
                    Dictionary<CTRL.CharacterType, PositionData>>();

                foreach (var item in MoveDataPair)
                {
                    Position.Clear();
                    Position[CTRL.CharacterType.Heroine] = new(
                        item.Value.HeroinePositionAdjustment,
                        item.Value.HeroineRotationAdjustment);
                    // Add Player if any vector is non zero.
                    if ((item.Value.PlayerPositionAdjustment != Vector3.zero)
                        || (item.Value.PlayerRotationAdjustment != Vector3.zero))
                    {
                        Position[CTRL.CharacterType.Player] = new(
                        item.Value.PlayerPositionAdjustment,
                        item.Value.PlayerRotationAdjustment);
                    }

                    MoveData[item.Key] = new(Position);
                }

                return MoveData;
            }
        }*/
    }
}
