// Project : RPG-csharp - C#
// File : RPGAnimator.cs
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

public class RPGAnimator : MonoBehaviour
{
    #region Public Variables

    public float CliffHandMatchStart = 0.17f;
    public float CliffHandMatchEnd = 0.27f;
    public float CliffFootMatchStart = 0.25f;
    public float CliffFootMatchEnd = 0.65f;

    public float WalkSpeed { get; set; }
    public float StrafeSpeed { get; set; }
    public GameObject Anchor { get; set; }

    public static RPGAnimator Instance;

    #endregion

    #region Private Variables

    private Animator animator;
    private AnimatorStateInfo stateInfo;

    #endregion

    #region Public Methods

    public void Jump()
    {
        animator.SetTrigger("Jump");
    }

    public void Swim()
    {
        animator.SetBool("Swimming",RPGPlayerMotor.Instance.IsSwimming);
    }

    public void CliffJump()
    {
        animator.applyRootMotion = true;
        RPGPlayerMotor.Instance.InCliffAnimation = true;
        animator.SetTrigger("CliffJump");
    }

    public void ResetCliffParameters()
    {
        RPGPlayerMotor.Instance.InCliffRange = false;
        RPGPlayerMotor.Instance.InCliffAnimation = false;
        RPGPlayerMotor.Instance.DoCliffAnimation = false;
        ResetRootMotion();
    }

    public void ResetRootMotion()
    {
        animator.applyRootMotion = false;
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;
        Instance = this;
    }

    private void Update()
    {
        animator.SetFloat("Speed",WalkSpeed);
        animator.SetFloat("Strafe",StrafeSpeed);
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if ( stateInfo.IsName("Cliff Climb") )
            CheckCliffClimbStat();
    }

    private void CheckCliffClimbStat()
    {
        if (Anchor == null)
            return;
        if ( stateInfo.nameHash == Animator.StringToHash("Base Layer.Cliff Climb") )
        {
            Debug.DrawLine(animator.bodyPosition,Anchor.transform.position,Color.blue);
            animator.MatchTarget(Anchor.transform.position ,
                                 Anchor.transform.rotation ,
                                 AvatarTarget.RightHand ,
                                 new MatchTargetWeightMask(new Vector3(0 , 1 , 0) , 0) ,
                                 CliffHandMatchStart ,
                                 CliffHandMatchEnd);
            animator.MatchTarget(Anchor.transform.position,
                                 Anchor.transform.rotation,
                                 AvatarTarget.RightFoot,
                                 new MatchTargetWeightMask(new Vector3(0, 1, 0), 0),
                                 CliffFootMatchStart,
                                 CliffFootMatchEnd);
        }
    }

    private void LateUpdate()
    {

    }

    #endregion
}