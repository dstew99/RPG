// Project : RPG-csharp - C#
// File : ControlPoints.cs
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

public class ControlPoints : MonoBehaviour
{
    #region Public Variables

    public float gizmoSize = 0.5f;
    public float SphereScale = 0.1f;
    public bool SpherePoint = true;

    #endregion

    #region Private Variables

    private Color sphereColor = new Color(0 , 0 , 0 , 0.1f);

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void OnDrawGizmos()
    {
        if (this.enabled == false)
            return;
        Color tmp = Gizmos.color;
        if ( SpherePoint )
        {
            Gizmos.color = sphereColor;
            Gizmos.DrawSphere(transform.position,SphereScale * gizmoSize);
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + (transform.forward * gizmoSize * 1.0f));
        Gizmos.DrawLine(transform.position + (transform.forward * gizmoSize * 1.0f),
            (transform.position + (transform.forward * gizmoSize * 0.8f) + (transform.up * gizmoSize * 0.2f)));
        Gizmos.DrawLine(transform.position + (transform.forward * gizmoSize * 1.0f),
            (transform.position + (transform.forward * gizmoSize * 0.8f) + (transform.up * gizmoSize * -0.2f)));
        Gizmos.DrawLine(transform.position + (transform.forward * gizmoSize * 1.0f),
            (transform.position + (transform.forward * gizmoSize * 0.8f) + (transform.right * gizmoSize * 0.2f)));
        Gizmos.DrawLine(transform.position + (transform.forward * gizmoSize * 1.0f),
            (transform.position + (transform.forward * gizmoSize * 0.8f) + (transform.right * gizmoSize * -0.2f)));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (transform.up * gizmoSize));
        Gizmos.DrawLine(transform.position + (transform.up * gizmoSize * 1.0f),
            (transform.position + (transform.up * gizmoSize * 0.8f) + (transform.forward * gizmoSize * 0.2f)));
        Gizmos.DrawLine(transform.position + (transform.up * gizmoSize * 1.0f),
            (transform.position + (transform.up * gizmoSize * 0.8f) + (transform.forward * gizmoSize * -0.2f)));
        Gizmos.DrawLine(transform.position + (transform.up * gizmoSize * 1.0f),
            (transform.position + (transform.up * gizmoSize * 0.8f) + (transform.right * gizmoSize * 0.2f)));
        Gizmos.DrawLine(transform.position + (transform.up * gizmoSize * 1.0f),
            (transform.position + (transform.up * gizmoSize * 0.8f) + (transform.right * gizmoSize * -0.2f)));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (transform.right * gizmoSize));
        Gizmos.DrawLine(transform.position + (transform.right * gizmoSize * 1.0f),
            (transform.position + (transform.right * gizmoSize * 0.8f) + (transform.up * gizmoSize * 0.2f)));
        Gizmos.DrawLine(transform.position + (transform.right * gizmoSize * 1.0f),
            (transform.position + (transform.right * gizmoSize * 0.8f) + (transform.up * gizmoSize * -0.2f)));
        Gizmos.DrawLine(transform.position + (transform.right * gizmoSize * 1.0f),
            (transform.position + (transform.right * gizmoSize * 0.8f) + (transform.forward * gizmoSize * 0.2f)));
        Gizmos.DrawLine(transform.position + (transform.right * gizmoSize * 1.0f),
            (transform.position + (transform.right * gizmoSize * 0.8f) + (transform.forward * gizmoSize * -0.2f)));

        Gizmos.color = tmp;
    }

    #endregion
}