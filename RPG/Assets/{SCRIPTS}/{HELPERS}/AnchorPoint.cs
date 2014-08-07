// Project : RPG-csharp - C#
// File : AnchorPoint.cs
// Developer : Alireza Khodakarami
// Created : 2014 / 07 / 26
// Last Update : 2014 / 07 / 26

/*
 *
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AnchorPoint : MonoBehaviour
{
    #region Public Variables

    public float GizmoSize = 0.5f;
    public float SphereScale = 0.1f;
    public bool SpherePoint = true;
    public Color SphereColor = Color.red;
    public Color FrontColor = Color.blue;
    public Color LineColor = Color.yellow;

    #endregion

    #region Private Variables

    private Color sphereColor = new Color(0 , 0 , 0 , 0.1f);

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void OnDrawGizmos()
    {
        Color tmp = Gizmos.color;
        {
            Gizmos.color = SphereColor;
            Gizmos.DrawWireSphere(transform.position, GizmoSize);
            Gizmos.color = FrontColor;
            Gizmos.DrawRay(transform.position, transform.forward * GizmoSize * 2);
            Gizmos.color = LineColor;
            Gizmos.DrawRay(transform.position, transform.up * GizmoSize * 2);
            Gizmos.DrawRay(transform.position, transform.right * GizmoSize * 2);
            Gizmos.DrawRay(transform.position, -transform.right * GizmoSize * 2);
            Gizmos.DrawRay(transform.position, -transform.up * GizmoSize * 2);
            Gizmos.DrawRay(transform.position, -transform.forward * GizmoSize * 2);
            Gizmos.DrawLine(transform.position - transform.forward * .1f, transform.position - transform.forward * .1f + transform.right * .4f * GizmoSize * 2);
            Gizmos.DrawLine(transform.position - transform.forward * .1f, transform.position - transform.forward * .1f - transform.right * .4f * GizmoSize * 2);
            if (SpherePoint)
            {
                Gizmos.color = sphereColor;
                Gizmos.DrawSphere(transform.position, SphereScale * GizmoSize);
            }
        }
        Gizmos.color = tmp;
    }

    #endregion
}