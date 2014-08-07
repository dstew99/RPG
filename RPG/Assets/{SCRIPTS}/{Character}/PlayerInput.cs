// Project : RPG-csharp - C#
// File : PlayerInput.cs
// Developer : Alireza Khodakarami
// Created : 2014 / 07 / 22
// Last Update : 2014 / 07 / 22

/*
 *
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerInput : MonoBehaviour
{
    #region Public Variables

    public KeyCode Jump = KeyCode.Space;
    public KeyCode Use = KeyCode.E;

    public static PlayerInput Instance;

    #endregion

    #region Private Variables

    #endregion

    #region Public Methods

    #endregion

    #region Private Methods

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(Jump) && !RPGPlayerMotor.Instance.InCliffRange) ||
            (Input.GetKeyDown(Jump) && RPGPlayerMotor.Instance.InCliffRange && !RPGPlayerMotor.Instance.DoCliffAnimation))
            RPGAnimator.Instance.Jump();
        else if ((Input.GetKeyDown(Jump) && RPGPlayerMotor.Instance.InCliffRange && RPGPlayerMotor.Instance.DoCliffAnimation))
            RPGAnimator.Instance.CliffJump();
    }

    private void LateUpdate()
    {

    }

    #endregion
}