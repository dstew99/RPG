// Project : RPG-csharp - C#
// File : SwimmingTrigger.cs
// Developer : Alireza Khodakarami
// Created : 2014 / 07 / 23
// Last Update : 2014 / 07 / 23

/*
 *
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SwimmingTrigger : MonoBehaviour
{
    #region Public Variables

    //public float FixMultiplier = 1.5f;

    #endregion

    #region Private Variables

    //private bool fixWaterLevel;
    //private bool fixedWaterLevel;
    //private Vector3 position;

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void Start()
    {

    }

    private void Update()
    {
        //if (!fixWaterLevel || fixedWaterLevel)
        //    return;
        //Debug.DrawLine(RPGPlayerMotor.Instance.gameObject.transform.position,position,Color.yellow);
        //var dis = Vector3.Distance(RPGPlayerMotor.Instance.transform.position , position);
        //if ( dis > 0.01 && fixWaterLevel )
        //    RPGPlayerMotor.Instance.gameObject.transform.position =
        //        Vector3.Lerp(RPGPlayerMotor.Instance.gameObject.transform.position , position , Time.deltaTime);
        //else if ( dis <= 0.01 )
        //{
        //    fixWaterLevel = false;
        //    fixedWaterLevel = true;
        //}

    }

    private void LateUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(!other.GetComponent<RPGPlayerMotor>() &&
            !other.GetComponent<RPGCamera>())
            return;
        //Vector3 pos = RPGPlayerMotor.Instance.gameObject.transform.position;
        //RaycastHit hitInfo;
        //if ( Physics.Raycast(pos + (RPGPlayerMotor.Instance.transform.up * 2) ,
        //    -RPGPlayerMotor.Instance.transform.up * 2 , out hitInfo) && !fixedWaterLevel)
        //{
        //    //if ( hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Water") )
        //    //{
        //        fixWaterLevel = true;
        //        position = hitInfo.point - RPGPlayerMotor.Instance.transform.up * FixMultiplier;
        //    //}
        //}
        other.GetComponent<RPGPlayerMotor>().IsSwimming = true;
        RPGPlayerMotor.Instance.ResetMoveVector();
        RPGAnimator.Instance.Swim();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.GetComponent<RPGPlayerMotor>() &&
            !other.GetComponent<RPGCamera>())
            return;
        //if (fixWaterLevel)
        //    return;
        other.GetComponent<RPGPlayerMotor>().IsSwimming = false;
        RPGAnimator.Instance.Swim();
    }

    #endregion
}