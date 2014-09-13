// Project : RPG-csharp - C#
// File : DebugGizmos.cs
// Developer : Alireza Khodakarami
// Created : 2014 / 08 / 04
// Last Update : 2014 / 08 / 04

/*
 *
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class DebugGizmos : MonoBehaviour
{
    #region Public Variables

    public GizmoTypes GizmoType;
    public PointBehaviors PointBehavior;
    public float GizmoSize = 1;
    public float Radius = 1;
    public float Height = 2;
    public float PoinSpehereRadius = 0.5f;
    public Vector3 Direction = Vector3.zero;
    public Vector3 Size = Vector3.zero;
    public Vector3 IconOffset = new Vector3(0 , 4 , 0);
    public Vector3 TextOffset = new Vector3(0, 4, 0);
    public Vector3 CircleRotation = Vector3.zero;
    public int Angle = 360;
    public int Sections = 3;
    public string Text = string.Empty;
    public string Icon = string.Empty;
    public bool DrawBoundings = true;
    public bool DrawCamera = true;
    public bool DrawVolumetric = true;
    public bool DrawTexture = true;
    public bool PointSphere = true;
    public bool GetExternalRadius;
    public bool GetExternalAngel;
    public bool GetExternalSize;
    public bool GetExternalDirection;
    public bool GetExternalText;
    public bool GetExternalHeight;
    public bool GetExternalCircleRotation;
    public bool UseLocalMatrix = true;
    public Color Outline = Color.black;
    public Color GizmoLine = Color.yellow;
    public Color TextColor = Color.magenta;
    public Color Extra = Color.blue;
    public Color Volumetric = new Color(0 , 1 , 0 , 0.3f);

    public float ExternalRadius { get; set; }
    public int ExternalAngel { get; set; }
    public Vector3 ExternalSize { get; set; }
    public Vector3 ExternalDirection { get; set; }
    public Vector3 ExternalCircleRotation { get; set; }

    public string ExternalText { get; set; }
    public float ExternalHeight { get; set; }
    public bool CanDrawBounds;
    public bool CanDrawCamera;
    public bool Draw = true;

    #endregion

    #region Private Variables

    private Color pointSphereColor = new Color(0 , 0 , 0 , 0.01f);

    #endregion

    #region Public Methods

    public void Update()
    {
        if (GetExternalAngel)
            Angle = ExternalAngel;
        if (GetExternalDirection)
            Direction = ExternalDirection;
        if (GetExternalRadius)
            Radius = ExternalRadius;
        if (GetExternalSize)
            Size = ExternalSize;
        if (GetExternalText)
            Text = ExternalText;
        if ( GetExternalHeight )
            Height = ExternalHeight;
        if ( GetExternalCircleRotation )
            CircleRotation = ExternalCircleRotation;
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        if ( gameObject.collider && GetComponent<CapsuleCollider>() == null)
            gameObject.collider.isTrigger = true;
    }
    
    private void OnDrawGizmos()
    {
        if (!enabled)
            return;
        if ( GetComponent<MeshFilter>() != null || gameObject.collider != null )
            CanDrawBounds = GetComponent<CharacterController>() == null;
        else
            CanDrawBounds = false;
        CanDrawCamera = GetComponent<Camera>();
        Color tmp = Gizmos.color;
        Matrix4x4 mat = Gizmos.matrix;
        switch ( GizmoType )
        {
            case GizmoTypes.Anchor:
                Gizmos.color = Outline;
                Gizmos.DrawWireSphere(transform.position , GizmoSize);
                Gizmos.DrawRay(transform.position ,transform.forward * GizmoSize * 2);
                Gizmos.color = Extra;
                Gizmos.DrawRay(transform.position,transform.up * GizmoSize * 2);
                Gizmos.DrawLine(transform.position - transform.forward * 0.1f * GizmoSize ,
                    transform.position - transform.forward * 0.1f * GizmoSize + transform.right * 0.4f * GizmoSize * 2);
                Gizmos.DrawLine(transform.position - transform.forward * 0.1f * GizmoSize,
                    transform.position - transform.forward * 0.1f * GizmoSize - transform.right * 0.4f * GizmoSize * 2);
                Gizmos.color = GizmoLine;
                Gizmos.DrawRay(transform.position, transform.right * GizmoSize * 2);
                Gizmos.color = TextColor;
                Gizmos.DrawRay(transform.position, -transform.up * GizmoSize * 2);
                Gizmos.DrawRay(transform.position, -transform.right * GizmoSize * 2);
                Gizmos.DrawRay(transform.position, -transform.forward * GizmoSize * 2);
                if ( DrawVolumetric )
                {
                    Gizmos.color = Volumetric;
                    Gizmos.DrawSphere(transform.position , Radius * GizmoSize);
                }
            break;
            case GizmoTypes.Arrows:
                HELPER.DrawArrow(transform.position,transform.forward * GizmoSize,Color.blue);
                HELPER.DrawArrow(transform.position, transform.up * GizmoSize, Color.green);
                HELPER.DrawArrow(transform.position, transform.right * GizmoSize, Color.red);
            break;
            case GizmoTypes.Box:
                Gizmos.color = GizmoLine;
                Gizmos.DrawWireCube(transform.position,Size * GizmoSize);
                if ( DrawVolumetric )
                {
                    Gizmos.color = Volumetric;
                    Gizmos.DrawCube(transform.position,Size * GizmoSize);
                }
            break;
            case GizmoTypes.Circle:
                HELPER.DrawCircle(transform.position,Radius,Sections,CircleRotation,Angle,GizmoLine);
            break;
            case GizmoTypes.Direction:
                if (Direction != Vector3.zero)
                    HELPER.DrawArrow(transform.position,Direction,GizmoLine);
            break;
            case GizmoTypes.Sphere:
                Gizmos.color = GizmoLine;
                Gizmos.DrawWireSphere(transform.position,Radius * GizmoSize);
                if ( DrawVolumetric )
                {
                    Gizmos.color = Volumetric;
                    Gizmos.DrawSphere(transform.position, Radius * GizmoSize);
                }
            break;
            case GizmoTypes.Text:
                if ( !string.IsNullOrEmpty(Text) && TextOffset != Vector3.zero )
                {
                    Gizmos.color = new Color(TextColor.r, TextColor.g, TextColor.b, 0.3f);
                    Gizmos.DrawLine(transform.position, transform.position + TextOffset);
                }
                GUIStyle style = new GUIStyle("box");
                style.normal.textColor = TextColor;    
                Handles.BeginGUI();
                Vector3 pos = transform.position + TextOffset;
                Vector2 pos2D = HandleUtility.WorldToGUIPoint(pos);
                GUI.Label(new Rect(pos2D.x, pos2D.y, 100, 30), Text, style);
                Handles.EndGUI();
            break;
            case GizmoTypes.Capsule:
                HELPER.DrawCapsule(transform,transform.position,Radius,Height,GizmoLine);
            break;
            case GizmoTypes.Character:
                // TODO: Future Gizmo for the npc/Enemy motor script !
            break;
            case GizmoTypes.Player:
                if ( GetComponent<RPGPlayerMotor>() != null )
                {
                    Vector3 position = transform.position + GetComponent<CharacterController>().center;
                    position = new Vector3(position.x, position.y - GetComponent<CharacterController>().height / 2f, position.z);
                    HELPER.DrawCapsule(transform, position, GetComponent<CharacterController>().radius, GetComponent<CharacterController>().height, GizmoLine);
                    HELPER.DrawArrow(transform.position,RPGPlayerMotor.Instance.MoveDirection,Extra);
                }
            break;
            case GizmoTypes.None:
            break;
        }
        if ( PointSphere )
        {
            Gizmos.color = pointSphereColor;
            Gizmos.DrawSphere(transform.position,PoinSpehereRadius * GizmoSize);
        }
        if ( DrawBoundings )
        {
            if ( GetComponent<MeshFilter>() != null || gameObject.collider != null )
            {
                Gizmos.color = Outline;
                if (GetComponent<SphereCollider>() == null && GetComponent<CapsuleCollider>() == null && GetComponent<CharacterController>() == null)
                {
                    var bounds = GetComponent<MeshFilter>()
                        ? GetComponent<MeshFilter>().sharedMesh.bounds
                        : gameObject.collider.bounds;
                    if ( UseLocalMatrix )
                        Gizmos.matrix = transform.localToWorldMatrix;
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                    if ( DrawVolumetric )
                    {
                        Gizmos.color = Volumetric;
                        Gizmos.DrawCube(bounds.center,bounds.size);
                    }
                    Gizmos.matrix = mat;
                }
                else if (GetComponent<SphereCollider>())
                {
                    Gizmos.DrawWireSphere(transform.position,GetComponent<SphereCollider>().radius);
                    if ( DrawVolumetric )
                    {
                        Gizmos.color = Volumetric;
                        Gizmos.DrawSphere(transform.position, GetComponent<SphereCollider>().radius);
                    }
                }
                else if ( GetComponent<CapsuleCollider>() ) 
                {
                    var radius = GetComponent<CapsuleCollider>().radius;
                    var height = GetComponent<CapsuleCollider>().height;
                    Vector3 position = transform.position + GetComponent<CapsuleCollider>().center;
                    position = new Vector3(position.x, position.y - height / 2f,position.z);
                    HELPER.DrawCapsule(transform,position,radius,height,GizmoLine);
                }
            }
        }
        if ( DrawCamera && GetComponent<Camera>() )
        {
            Vector3[] nearCorners = new Vector3[4];
            Vector3[] farCorners = new Vector3[4];
            Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(GetComponent<Camera>());
            Plane temp = camPlanes[1];
            camPlanes[1] = camPlanes[2];
            camPlanes[2] = temp;
            for ( int i = 0; i < 4; i++ )
            {
                nearCorners[i] = HELPER.Plan2Intersect(camPlanes[4] , camPlanes[i] , camPlanes[(i + 1) % 4]);
                farCorners[i] = HELPER.Plan2Intersect(camPlanes[5], camPlanes[i], camPlanes[(i + 1) % 4]);
            }
            for ( int i = 0; i < 4; i++ )
            {
                Debug.DrawLine(nearCorners[i],nearCorners[(i+1) % 4], Outline,Time.deltaTime,true);
                Debug.DrawLine(farCorners[i], farCorners[(i + 1) % 4], Outline, Time.deltaTime, true);
                Debug.DrawLine(nearCorners[i], farCorners[i], GizmoLine, Time.deltaTime, true);
            }
        }
        if ( DrawTexture && !string.IsNullOrEmpty(Icon) )
        {
            Gizmos.DrawIcon(transform.position + IconOffset , Icon);
            Gizmos.color = new Color(Outline.r , Outline.g , Outline.b , 0.3f);
            Gizmos.DrawLine(transform.position,transform.position + IconOffset);
        }
        if ( PointBehavior == PointBehaviors.PlayerSpawn || PointBehavior == PointBehaviors.EnemySpawn ||
             PointBehavior == PointBehaviors.NPCSpawn )
        {
            HELPER.DrawCircle(transform.position , 0.4f , 24 , Vector3.zero, 360 , GizmoLine);
        }
        Gizmos.color = tmp;

        
    }

    #endregion
}

