﻿//
// Plugin.Data
//
using System.Text;
using System.Collections.Generic;

using UnityEngine;
using MessagePack;

using BepInEx.Logging;

using ExtensibleSaveFormat;

using IDHIUtils;


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
            public Vector3 HeroinePosition;
            [Key(1)]
            public Vector3 PlayerPosition;
            [Key(2)]
            public Vector3 HeroineRotation;
            [Key(3)]
            public Vector3 PlayerRotation;

            public PositionDataPair()
            {
                HeroinePosition = Vector3.zero;
                PlayerPosition = Vector3.zero;
                HeroineRotation = Vector3.zero;
                PlayerRotation = Vector3.zero;
            }

            public PositionDataPair(
                Vector3 heroineAdjustment,
                Vector3 playerAdjustment)
            {
                HeroinePosition = heroineAdjustment;
                PlayerPosition = playerAdjustment;
                HeroineRotation = Vector3.zero;
                PlayerRotation = Vector3.zero;
            }

            public PositionDataPair(
                Vector3 heroineAdjustment,
                Vector3 playerAdjustment,
                Vector3 heroineRotation,
                Vector3 playerRotation)
            {
                HeroinePosition = heroineAdjustment;
                PlayerPosition = playerAdjustment;
                HeroineRotation = heroineRotation;
                PlayerRotation = playerRotation;
            }
        }

        /// <summary>
        /// Simpler easier to manage
        /// TODO: Work on rotation
        /// </summary>
        public class PositionData
        {
            public Vector3 Position;
            public Vector3 Rotation;

            public PositionData()
            {
                Position = Vector3.zero;
                Rotation = Vector3.zero;
            }

            public PositionData(
                Vector3 position,
                Vector3 rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }

        public class CharacterTypeData
        {
            private Dictionary<CharacterType, PositionData> _data;

            public CharacterTypeData()
            {
                _data = new();
                _data.Clear();
            }

            internal PositionData this[CharacterType key]
            {
                get { return _data[key]; }
                set { _data[key] = value; }
            }
        }

        public sealed class MoveData
        {
            private Dictionary<string,
                Dictionary<CharacterType, PositionData>> _data;
            private readonly ChaControl _chaControl;

            internal int Count => _data.Count;
            internal Dictionary<string,
                Dictionary<CharacterType, PositionData>> Data
            {
                get { return _data; }
                set { _data = value; }
            }

            internal MoveData(ChaControl chaControl)
            {
                _chaControl = chaControl;
                _data = new Dictionary<string,
                    Dictionary<CharacterType, PositionData>>();
                _data.Clear();
            }

            internal Dictionary<CharacterType, PositionData> this[string key]
            {
                get { return _data[key]; }
                set { _data[key] = value; }
            }

            internal void Load(PluginData data)
            {
                var name = _chaControl.chaFile?.parameter.fullname.Trim()
                    ?? "CHACONTROL FAIL";
                if (data != null)
                {
                    Dictionary<string, PositionDataPair> dataDeserialized;

                    if (data.data.TryGetValue(MoveDataID,
                            out var loadedMoveData)
                            && loadedMoveData != null)
                    {
                        dataDeserialized = MessagePackSerializer
                            .Deserialize<Dictionary<string, PositionDataPair>>
                            ((byte[])loadedMoveData);
                        if (dataDeserialized != null)
                        {
                            Data = RestoreMoveData(dataDeserialized);
                            if (DebugInfo.Value)
                            {
                                PrintData(Data, name);
                            }

                        }
                        else
                        {
                            _Log.Level(LogLevel.Error ,$"HCAX0020: [Load] [{name}] " +
                                $"Can't unpack data.");
                        }
                    }
                }
                else
                {
                    _Log.Debug($"HCAX0021: [Load] [{name}] PluginData is null.");
                    Data.Clear();
                }
            }

            internal PluginData Save()
            {
                var name = _chaControl.chaFile?.parameter.fullname.Trim()
                    ?? "CHACONTROL FAIL";

                var plugData = new PluginData {
                    version = 1
                };
                var MoveDataSerialize = PrepareSerialize();
                plugData.data.Add(MoveDataID,
                        MessagePackSerializer.Serialize(MoveDataSerialize));
                if (DebugInfo.Value)
                {
                    var Moving = RestoreMoveData(MoveDataSerialize);
                    PrintData(Moving, name);
                }
                return plugData.data.Count > 0 ? plugData : null;
            }

            private Dictionary<string, PositionDataPair> PrepareSerialize()
            {
                var MoveDataToSerialize = new Dictionary<string, PositionDataPair>();

                Vector3 HeroinePositionAdjustment;
                Vector3 PlayerPositionAdjustment;
                Vector3 HeroineRotationAdjustment;
                Vector3 PlayerRotationAdjustment;

                foreach (var item in Data)
                {
                    HeroinePositionAdjustment = Vector3.zero;
                    PlayerPositionAdjustment = Vector3.zero;
                    HeroineRotationAdjustment = Vector3.zero;
                    PlayerRotationAdjustment = Vector3.zero;

                    foreach (var character in item.Value)
                    {
                        if (character.Key == CharacterType.Heroine)
                        {
                            HeroinePositionAdjustment = character.Value.Position;
                            HeroineRotationAdjustment = character.Value.Rotation;
                        }
                        if (character.Key == CharacterType.Player)
                        {
                            PlayerPositionAdjustment = character.Value.Position;
                            PlayerRotationAdjustment = character.Value.Rotation;
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

            private static Dictionary<string,
                Dictionary<CharacterType, PositionData>> RestoreMoveData(
                    Dictionary<string, PositionDataPair> MoveDataPair)
            {
                var Position = new Dictionary<CharacterType, PositionData>();
                var MoveData = new Dictionary<string,
                    Dictionary<CharacterType, PositionData>>();

                foreach (var item in MoveDataPair)
                {
                    Position.Clear();
                    Position[CharacterType.Heroine] = new(
                        item.Value.HeroinePosition,
                        item.Value.HeroineRotation);

                    // Add Player if any vector is non zero.
                    if ((item.Value.PlayerPosition != Vector3.zero)
                        || (item.Value.PlayerRotation != Vector3.zero))
                    {
                        Position[CharacterType.Player] = new(
                        item.Value.PlayerPosition,
                        item.Value.PlayerRotation);
                    }

                    MoveData[item.Key] = new(Position);
                }

                return MoveData;
            }
        }

        internal static void PrintData(
            Dictionary<string, Dictionary<CharacterType, PositionData>> MoveData,
            string name = "")
        {
            var lines = new StringBuilder();
#if DEBUG
            var calllingMethod = Utilities.CallingMethod();
            lines.AppendLine($"HCAX0021: [PrintData] Calling Method {calllingMethod}.");
#endif
            foreach (var item in MoveData)
            {
                foreach (var character in item.Value)
                {
                    lines.AppendLine($"Position={item.Key} " +
                        $"Character={character.Key} " +
                        $"Position=" +
                        $"{character.Value.Position.ToString("F7")} " +
                        $"Rotation=" +
                        $"{character.Value.Rotation.ToString("F7")}");
                }
            }

            if (lines.Length > 0)
            {
                _Log.Debug($"HCAX0022: [PrintData] [{name}]\n\n{lines.ToString()}");
            }
        }

        internal static void PrintData(
            Dictionary<string, PositionDataPair> MoveData,
            string name = "")
        {
            var lines = new StringBuilder();

            foreach (var item in MoveData)
            {
                lines.AppendLine($"Position={item.Key} Heroine " +
                    $"Position={item.Value.HeroinePosition.ToString("F7")} " +
                    $"Rotation={item.Value.HeroineRotation.ToString("F7")}");
            }

            if (lines.Length > 0)
            {
                _Log.Debug($"HCAX0023: [PrintData]\n\n{lines.ToString()}\n");
            }
        }
    }
}
