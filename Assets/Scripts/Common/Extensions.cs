using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Returns a normalized rotation value for the given float.
    /// Values can range from [-180, 180)
    /// </summary>
    /// <param name="rot"></param>
    /// <returns></returns>
    public static float NormalizedRotation(this float rot)
    {
        rot %= 360;
        rot = rot > 180 ? rot - 360 : rot;
        rot = rot < -180 ? rot + 360 : rot;
        //180 = -180. directly in front (0 deg) is treated as positive [by arbitrary convetion: (value < 0 ? negative : positive)], so directly behind will be negative for parity.
        rot = rot == 180 ? -180 : rot;
        return rot;
    }

    public static Vector3 NewX(this Vector3 vec, float val)
    {
        return new Vector3(val, vec.y, vec.z);
    }

    public static Vector3 NewY(this Vector3 vec, float val)
    {
        return new Vector3(vec.x, val, vec.z);
    }

    public static Vector3 NewZ(this Vector3 vec, float val)
    {
        return new Vector3(vec.x, vec.y, val);
    }

    public static Quaternion AsQuaternion(this Vector3 vec)
    {
        return Quaternion.Euler(vec);
    }

    public static void SetEulerRotationY(this GameObject @object, float y)
    {
        @object.transform.rotation = 
            @object.transform.rotation.eulerAngles
            .NewY(y)
            .AsQuaternion();
    }

    public static void AddLerpedEulerRotationY(this GameObject @object, float yDeg, float deltaTime)
    {
        @object.transform.rotation =
            Vector3.Lerp(
                @object.transform.rotation.eulerAngles,
                @object.transform.rotation.eulerAngles.NewY(@object.transform.rotation.eulerAngles.y + yDeg),
                deltaTime)
            .AsQuaternion();
    }
}
