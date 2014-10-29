#region Documentation
// file:	RPG\Assets\{SCRIPTS}\{HELPERS}\HELPER.cs
//
// summary:	Implements the helper class
#endregion

using UnityEngine;

#region Documentation
/// <summary>   A helper. </summary>
///
/// <remarks>   Dave, 10/26/2014. </remarks>
#endregion

public static class HELPER
{
    #region Documentation
    /// <summary>   Clamp angle. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    ///
    /// <param name="angle">    The angle. </param>
    /// <param name="min">      The minimum. </param>
    /// <param name="max">      The maximum. </param>
    ///
    /// <returns>   A float. </returns>
    #endregion

    public static float ClampAngle(float angle, float min, float max)
    {
        while (angle <-360f || angle > 360f)
        {
            if (angle > 360f)
                angle -= 360f;
            if (angle < -360f)
                angle += 360f;
        }
        return Mathf.Clamp(angle, min, max);
    }

    #region Documentation
    /// <summary>   Draw circle. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    ///
    /// <param name="center">   The center. </param>
    /// <param name="radius">   The radius. </param>
    /// <param name="segments"> The segments. </param>
    /// <param name="rotation"> The rotation. </param>
    /// <param name="Angle">    The angle. </param>
    /// <param name="color">    The color. </param>
    #endregion

    public static void DrawCircle(Vector3 center, float radius, int segments, Vector3 rotation,float Angle, Color color)
    {
        if (segments < 3)
            segments = 3;
        rotation *= Mathf.Deg2Rad;
        for (var i = 0; i < segments; i++)
        {
            var angel1 = Angle / segments * i * Mathf.Deg2Rad;
            var angel2 = Angle / segments * (i + 1) * Mathf.Deg2Rad;
            var x1 = Mathf.Cos(angel1) * radius;
            var x2 = Mathf.Cos(angel2) * radius;
            var z1 = Mathf.Sin(angel1) * radius;
            var z2 = Mathf.Sin(angel2) * radius;
            var point1 = center + new Vector3(x1 , 0 , z1);
            var point2 = center + new Vector3(x2 , 0 , z2);
            var rotEuler = Quaternion.Euler(rotation.x * 57.777f, rotation.y * 57.777f, rotation.z * 57.777f);
            point1 = RotateAroundPoint(point1, center, rotEuler);
            point2 = RotateAroundPoint(point2, center, rotEuler);
            Gizmos.color = color;
            Gizmos.DrawLine(point1, point2);
        }
    }

    #region Documentation
    /// <summary>   Plan 2 intersect. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    ///
    /// <param name="p1">   The first Plane. </param>
    /// <param name="p2">   The second Plane. </param>
    /// <param name="p3">   The third Plane. </param>
    ///
    /// <returns>   A Vector3. </returns>
    #endregion

    public static Vector3 Plan2Intersect(Plane p1 , Plane p2 , Plane p3)
    {
        return ((-p1.distance * Vector3.Cross(p2.normal , p3.normal)) +
                (-p2.distance * Vector3.Cross(p3.normal , p1.normal)) +
                (-p3.distance * Vector3.Cross(p1.normal , p2.normal))) /
                (Vector3.Dot(p1.normal , Vector3.Cross(p2.normal , p3.normal)));
    }

    #region Documentation
    /// <summary>   Rotate around point. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    ///
    /// <param name="point">    The point. </param>
    /// <param name="pivot">    The pivot. </param>
    /// <param name="Angle">    The angle. </param>
    ///
    /// <returns>   A Vector3. </returns>
    #endregion

    public static Vector3 RotateAroundPoint(Vector3 point , Vector3 pivot , Quaternion Angle)
    {
        return Angle * (point - pivot) + pivot;
    }

    #region Documentation
    /// <summary>   Draw capsule. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    ///
    /// <param name="transform">    The transform. </param>
    /// <param name="position">     The position. </param>
    /// <param name="radius">       The radius. </param>
    /// <param name="height">       The height. </param>
    /// <param name="GizmoLine">    The gizmo line. </param>
    #endregion

    public static void DrawCapsule(Transform transform , Vector3 position , float radius , float height ,
        Color GizmoLine)
    {
        DrawCircle(position + transform.up * radius , radius , 24 , new Vector3(90 , 0 , 0) , 180 , GizmoLine);
        DrawCircle(position + transform.up * radius, radius, 24, new Vector3(90, 90, 0), 180, GizmoLine);
        DrawCircle(position + transform.up * radius, radius, 24, new Vector3(0, 0, 0), 360, GizmoLine);

        DrawCircle(position + transform.up * (height - radius), radius, 24, new Vector3(-90, 0, 0), 180, GizmoLine);
        DrawCircle(position + transform.up * (height - radius), radius, 24, new Vector3(-90, 90, 0), 180, GizmoLine);
        DrawCircle(position + transform.up * (height - radius), radius, 24, new Vector3(0, 0, 0), 360, GizmoLine);
        Gizmos.color = GizmoLine;
        Gizmos.DrawLine(position + transform.up * radius + transform.right * radius, position + transform.up * (height - radius) + transform.right * radius);
        Gizmos.DrawLine(position + transform.up * radius - transform.right * radius, position + transform.up * (height - radius) - transform.right * radius);
        Gizmos.DrawLine(position + transform.up * radius + transform.forward * radius, position + transform.up * (height - radius) + transform.forward * radius);
        Gizmos.DrawLine(position + transform.up * radius - transform.forward * radius, position + transform.up * (height - radius) - transform.forward * radius);
    }

    #region Documentation
    /// <summary>   Draw arrow. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    ///
    /// <param name="Position">     The position. </param>
    /// <param name="Direction">    The direction. </param>
    /// <param name="color">        The color. </param>
    /// <param name="HeadLength">   (Optional) length of the head. </param>
    /// <param name="HeadAngle">    (Optional) the head angle. </param>
    #endregion

    public static void DrawArrow(Vector3 Position , Vector3 Direction , Color color , float HeadLength = 0.2f , float HeadAngle = 20f)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(Position,Direction);
        DrawArrowEnd(Position , Direction , color , HeadLength , HeadAngle);
    }

    #region Documentation
    /// <summary>   Draw arrow end. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    ///
    /// <param name="Position">         The position. </param>
    /// <param name="Direction">        The direction. </param>
    /// <param name="color">            The color. </param>
    /// <param name="HeadLength">       (Optional) length of the head. </param>
    /// <param name="ArrowHeadAngle">   (Optional) the arrow head angle. </param>
    #endregion

    private static void DrawArrowEnd(Vector3 Position , Vector3 Direction , Color color , float HeadLength = 0.25f ,
        float ArrowHeadAngle = 20f)
    {
        Vector3 right = Quaternion.LookRotation(Direction) * Quaternion.Euler(ArrowHeadAngle , 0 , 0) * Vector3.back;
        Vector3 left = Quaternion.LookRotation(Direction) * Quaternion.Euler(-ArrowHeadAngle, 0, 0) * Vector3.back;
        Vector3 up = Quaternion.LookRotation(Direction) * Quaternion.Euler(0,ArrowHeadAngle, 0) * Vector3.back;
        Vector3 down = Quaternion.LookRotation(Direction) * Quaternion.Euler(0,-ArrowHeadAngle, 0) * Vector3.back;
        Gizmos.color = color;
        Gizmos.DrawRay(Position + Direction,right * HeadLength);
        Gizmos.DrawRay(Position + Direction, left * HeadLength);
        Gizmos.DrawRay(Position + Direction, up * HeadLength);
        Gizmos.DrawRay(Position + Direction, down * HeadLength);
    }
}