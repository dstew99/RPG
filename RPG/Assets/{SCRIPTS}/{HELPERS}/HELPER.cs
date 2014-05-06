using UnityEngine;

public static class HELPER
{
    public static float ClampAngle(float angle, float min, float max)
    {
        while (angle <-360 || angle > 360)
        {
            if (angle > 360)
                angle -= 360;
            if (angle < -360)
                angle += 360;
        }
        return Mathf.Clamp(angle, min, max);
    }
}