//
// MoveEvent
//
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

    public class AxisVector
    {
        private static readonly Dictionary<Axis, Vector3> _values = new()
        {
            {Axis.X, Vector3.right},
            {Axis.Y, Vector3.up},
            {Axis.Z, Vector3.forward}
        };

        public static bool TryGetValue(Axis key, out Vector3 result)
        {
            return _values.TryGetValue(key, out result);
        }

        public Vector3 this[Axis key]
        {
            get { return _values[key]; }
            set { _values[key] = value;}
        }
    }

}
