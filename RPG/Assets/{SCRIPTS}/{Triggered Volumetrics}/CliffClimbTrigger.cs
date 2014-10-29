// Project : RPG-csharp - C#
// File : CliffClimbTrigger.cs
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

public class CliffClimbTrigger : MonoBehaviour
{
    #region Public Variables

    [Range(0,90)]
    public int ClimbAngel = 30;
    
    public GameObject Anchor { get; set; }

    #endregion

    #region Private Variables

    private DebugGizmos[] Anchors;
    private bool InRange;

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void Start()
    {
        transform.collider.isTrigger = true;
        Anchors =
            transform.root.GetComponentsInChildren<DebugGizmos>()
                .Where(t => t.PointBehavior == PointBehaviors.Anchor)
                .ToArray();
    }

    private void Update()
    {
        if (Anchor == null || !InRange)
            return;
        Debug.DrawLine(RPGPlayerMotor.Instance.transform.position, Anchor.transform.position,Color.yellow);
        var angel = Quaternion.Angle(Anchor.transform.rotation , RPGPlayerMotor.Instance.gameObject.transform.rotation);
        if ( angel < ClimbAngel )
        {
            RPGAnimator.Instance.Anchor = Anchor;
            RPGPlayerMotor.Instance.DoCliffAnimation = true;
        }
        else
        {
            RPGAnimator.Instance.Anchor = null;
            RPGPlayerMotor.Instance.DoCliffAnimation = false;
        }
    }

    private void FixedUpdate()
    {
        if (!InRange)
            return;
        float positiveInfinity = float.PositiveInfinity;
        foreach ( var anchor in Anchors )
        {
            var tempDistance = Vector3.Distance(RPGPlayerMotor.Instance.transform.position , anchor.transform.position);
            if (!(tempDistance < positiveInfinity)) continue;
            Anchor = anchor.gameObject;
            positiveInfinity = tempDistance;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<RPGPlayerMotor>() == null || other.GetComponent<RPGCamera>())
            return;
        InRange = true;
        RPGPlayerMotor.Instance.InCliffRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<RPGPlayerMotor>() == null || other.GetComponent<RPGCamera>())
            return;
        InRange = false;
        RPGPlayerMotor.Instance.InCliffRange = false;
    }

    #endregion
}