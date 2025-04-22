using UnityEngine;

namespace ArcaneCodex.Utilities
{
    public class Utilities2D
    {
        public static Quaternion LookAt2D(Vector2 target, Vector2 center)
        {
            Vector3 diff = target - center;
            diff.Normalize();
            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            return Quaternion.Euler(0f, 0f, rot_z);
        }
    }

    public class Utilities3D
    {

    }
}
