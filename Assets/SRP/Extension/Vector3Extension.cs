using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension
{
    ///<summary>
    /// Return a float array representation of a vector3
    ///</summary>
    public static float[] ToArray(this Vector3 v)
    {
        return new float[]{v.x, v.y, v.z};
    }

    public static int SizeOf()
    {
        return sizeof(float) * 3;
    }
}
