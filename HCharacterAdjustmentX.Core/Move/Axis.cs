//
// MoveEvent
//
using System.Collections.Generic;

using UnityEngine;

namespace IDHIPlugins
{
    public enum Axis
    {
        UNKNOWN = -1,
        X = 0,
        Y = 1,
        Z = 2
    }

    public readonly struct AxisVector
    {
        private readonly Dictionary<Axis, Vector3> _values = new()
        {
            {Axis.X, Vector3.right},
            {Axis.Y, Vector3.up},
            {Axis.Z, Vector3.forward}
        };

        public AxisVector()
        {
        }

        public Vector3? this[Axis key]
        {
            get
            {
                if (_values.TryGetValue(Axis.X, out var value))
                {
                    return value;
                }
                return null;
            }
        }
    }
}
