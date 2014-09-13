// Project : RPG-csharp - C#
// File : DebugGizmosEditor.cs
// Developer : Alireza Khodakarami
// Created : 2014 / 08 / 05
// Last Update : 2014 / 08 / 05

/*
 *
 */

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[CanEditMultipleObjects]
[CustomEditor(typeof(DebugGizmos))]
public class DebugGizmosEditor : Editor
{

    #region Private Variables

    private static GizmoTypes GizmoType;
    private static  PointBehaviors Behavior;
    private static  float GizmoSize = 1;
    private static  float Radius = 1;
    private static float  Height = 2;
    private static  float PoinSpehereRadius = 0.5f;
    private static  Vector3 Direction = Vector3.zero;
    private static  Vector3 Size = Vector3.zero;
    private static  Vector3 IconOffset = new Vector3(0, 4, 0);
    private static Vector3 CircleRotation;
    private static  int Angle;
    private static  int Sections = 3;
    private static  string Text = string.Empty;
    private static Vector3 TextOffset = new Vector3(0, 4, 0);
    private static  string Icon = string.Empty;
    private static  bool DrawBoundings = true;
    private static  bool DrawCamera;
    private static  bool DrawVolumetric = true;
    private static  bool DrawTexture = true;
    private static  bool PointSphere = true;
    private static  bool GetExternalRadius;
    private static  bool GetExternalAngel;
    private static  bool GetExternalSize;
    private static bool GetExternalHeight;
    private static bool GetExternalCircleRotation;
    private static  bool GetExternalDirection;
    private static  bool GetExternalText;
    private static  bool UseLocalMatrix = true;
    private static  Color Outline = Color.black;
    private static  Color GizmoLine = Color.yellow;
    private static  Color TextColor = Color.magenta;
    private static  Color Extra = Color.blue;
    private static  Color Volumetric = new Color(0, 1, 0, 0.3f);
    private static GUIStyle line;
    private bool Draw ;
    private static bool CanDrawBounds;
    private static bool CanDrawCamera;

    private DebugGizmos targetScript;
    private const int BIG_FONT_SIZE = 12;
    private const int SMALL_FONT_SIZE = 10;
    private GameObject gameObject;

    #endregion

    #region Public Methods

    public override void OnInspectorGUI()
    {
        string emp = string.Empty;
        line = new GUIStyle("box");
        line.border.top = line.border.bottom = 1;
        line.margin.top = line.margin.bottom = 1;
        line.padding.top = line.padding.bottom = 1;
        GetValues(); 
        targetScript.Update();
        {
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.label.fontStyle = FontStyle.Normal;
            GUI.skin.label.fontSize = BIG_FONT_SIZE;
            EditorGUILayout.BeginHorizontal();
            {
                Draw = EditorGUILayout.Toggle(Draw, GUILayout.Width(15), GUILayout.MaxWidth(15),
                    GUILayout.ExpandWidth(false));
                GUILayout.Label("Gizmo Type : "+gameObject.name);
            }
            EditorGUILayout.EndHorizontal();
            if ( !Draw )
            {
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(target);
                    SetValues();
                }
                return;
            }
            GUILayout.BeginHorizontal();
            {
                GizmoType = (GizmoTypes) EditorGUILayout.EnumPopup(GizmoType);
                Behavior = (PointBehaviors)EditorGUILayout.EnumPopup(Behavior);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3);
            GUILayout.Box(GUIContent.none,line,GUILayout.ExpandWidth(true),GUILayout.Height(1f));
            GUILayout.Label("Gizmo Settings");
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Selectable",GUILayout.Width(65));
                PointSphere = EditorGUILayout.Toggle(PointSphere , GUILayout.Width(15) , GUILayout.MaxWidth(15) ,
                    GUILayout.ExpandWidth(false));
                if ( PointSphere )
                    PoinSpehereRadius = EditorGUILayout.Slider(PoinSpehereRadius , 0.1f , 10);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Local", GUILayout.Width(35));
                UseLocalMatrix = EditorGUILayout.Toggle(UseLocalMatrix, GUILayout.Width(15), GUILayout.MaxWidth(15),
                    GUILayout.ExpandWidth(false));
                EditorGUILayout.LabelField(
                    UseLocalMatrix ? "Switched Gizmo to Local Matrix" : "Switched Gizmo to World Matrix" ,
                    line,GUILayout.ExpandWidth(true));
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Size", GUILayout.Width(50));
                GizmoSize = EditorGUILayout.Slider(GizmoSize , 0.1f , 10);
            }
            GUILayout.EndHorizontal();
            if ( GizmoType == GizmoTypes.Capsule )
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Height" , GUILayout.Width(50));
                    Height = EditorGUILayout.Slider(Height , 0.1f , 10);
                }
                GUILayout.EndHorizontal();
            }
            if ( (GizmoType == GizmoTypes.Anchor && DrawVolumetric) ||
                 GizmoType == GizmoTypes.Circle ||
                 GizmoType == GizmoTypes.Sphere ||
                 GizmoType == GizmoTypes.Capsule)
            {
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Radius", GUILayout.Width(50));
                    Radius = EditorGUILayout.Slider(Radius, 0.1f, 10);
                }
                GUILayout.EndHorizontal();
            }
            if ( GizmoType == GizmoTypes.Circle )
            {
                GUILayout.Space(3);
                GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
                GUILayout.Label("Circle Settings");
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Angle",GUILayout.Width(50));
                    Angle = EditorGUILayout.IntSlider(Angle, 0, 360);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Section", GUILayout.Width(50));
                    Sections = EditorGUILayout.IntSlider(Sections, 3, 30);
                }
                GUILayout.EndHorizontal();
                CircleRotation = EditorGUILayout.Vector3Field(emp,CircleRotation);
            }
            if (GizmoType == GizmoTypes.Box)
            {
                GUILayout.Space(3);
                GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
                GUILayout.Label("Box Settings");
                Size = EditorGUILayout.Vector3Field(emp, Size);
            }
            if (GizmoType == GizmoTypes.Direction)
            {
                GUILayout.Space(3);
                GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
                GUILayout.Label("Direction Settings");
                Direction = EditorGUILayout.Vector3Field(emp, Direction);
            }
            if (GizmoType == GizmoTypes.Text)
            {
                GUILayout.Space(3);
                GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
                GUILayout.Label("Text Settings");
                EditorGUILayout.BeginHorizontal();
                {
                    Text = EditorGUILayout.TextField(Text);
                    TextColor = EditorGUILayout.ColorField(TextColor);
                }
                EditorGUILayout.EndHorizontal();
                TextOffset = EditorGUILayout.Vector3Field(emp,TextOffset);
            }
            GUILayout.Space(3);
            GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
            GUILayout.Label("Extra Gizmos");
            if ( CanDrawBounds || CanDrawCamera)
            {
                GUILayout.BeginHorizontal();
                {
                    if ( CanDrawBounds)
                    {
                        EditorGUILayout.LabelField("Draw Bounds", GUILayout.Width(80),GUILayout.ExpandWidth(true));
                        DrawBoundings = EditorGUILayout.Toggle(DrawBoundings , GUILayout.Width(15) ,
                            GUILayout.MaxWidth(15) , GUILayout.ExpandWidth(true));
                    }
                    if (CanDrawCamera)
                    {
                        EditorGUILayout.LabelField("Draw Camera", GUILayout.Width(80), GUILayout.ExpandWidth(true));
                        DrawCamera = EditorGUILayout.Toggle(DrawCamera, GUILayout.Width(15),
                            GUILayout.MaxWidth(15), GUILayout.ExpandWidth(true));
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
                GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
            }
            GUILayout.Label("Gizmo Icon");
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Icon",GUILayout.Width(30));
                DrawTexture = EditorGUILayout.Toggle(DrawTexture, GUILayout.Width(15), GUILayout.MaxWidth(15),
                    GUILayout.ExpandWidth(false));
                if ( DrawTexture )
                    Icon = EditorGUILayout.TextField(Icon);
            }
            GUILayout.EndHorizontal();
            if ( DrawTexture )
                IconOffset = EditorGUILayout.Vector3Field(emp , IconOffset);
            GUILayout.Space(3);
            GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
            GUILayout.Label("Gizmo Colors");
            GUI.skin.label.fontSize = SMALL_FONT_SIZE;
            string tm = "Outline / Line / Extra";
            if ( Behavior == PointBehaviors.Anchor && GizmoType != GizmoTypes.Text )
                tm += " / Directing";
            GUILayout.Label(tm);
            GUI.skin.label.fontSize = BIG_FONT_SIZE;
            GUILayout.BeginHorizontal();
            {
                Outline = EditorGUILayout.ColorField(Outline);
                GizmoLine = EditorGUILayout.ColorField(GizmoLine);
                Extra = EditorGUILayout.ColorField(Extra);
                if (Behavior == PointBehaviors.Anchor && GizmoType != GizmoTypes.Text)
                    TextColor = EditorGUILayout.ColorField(TextColor);
            }
            GUILayout.EndHorizontal();
            if ( GizmoType == GizmoTypes.Anchor ||
                 GizmoType == GizmoTypes.Box ||
                 GizmoType == GizmoTypes.Sphere ||
                 (DrawBoundings && gameObject.GetComponent<CharacterController>() == null))
            {
                GUI.skin.label.fontSize = SMALL_FONT_SIZE;
                GUILayout.Label("Volumetric Settings");
                GUI.skin.label.fontSize = BIG_FONT_SIZE;
                GUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Volumetric",GUILayout.Width(70));
                    DrawVolumetric = EditorGUILayout.Toggle(DrawVolumetric, GUILayout.Width(15), GUILayout.MaxWidth(15),
                    GUILayout.ExpandWidth(false));
                    if ( DrawVolumetric )
                        Volumetric = EditorGUILayout.ColorField(Volumetric);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(3);
            GUILayout.Box(GUIContent.none, line, GUILayout.ExpandWidth(true), GUILayout.Height(1f));
            GUILayout.Label("Get Values From External Source");
            GUILayout.BeginHorizontal();
            {
                if ( (GizmoType == GizmoTypes.Anchor && DrawVolumetric) ||
                     GizmoType == GizmoTypes.Circle ||
                     GizmoType == GizmoTypes.Sphere ||
                     GizmoType == GizmoTypes.Capsule)
                {
                    EditorGUILayout.LabelField("Radius",GUILayout.Width(50),GUILayout.ExpandWidth(true));
                    GetExternalRadius = EditorGUILayout.Toggle(GetExternalRadius, GUILayout.Width(15), GUILayout.MaxWidth(15),
                        GUILayout.ExpandWidth(false));
                }
                if (GizmoType == GizmoTypes.Circle)
                {
                    EditorGUILayout.LabelField("Angle", GUILayout.Width(50), GUILayout.ExpandWidth(true));
                    GetExternalAngel = EditorGUILayout.Toggle(GetExternalAngel, GUILayout.Width(15), GUILayout.MaxWidth(15),
                        GUILayout.ExpandWidth(false));
                    EditorGUILayout.LabelField("Rotation", GUILayout.Width(50), GUILayout.ExpandWidth(true));
                    GetExternalCircleRotation = EditorGUILayout.Toggle(GetExternalCircleRotation, GUILayout.Width(15), GUILayout.MaxWidth(15),
                        GUILayout.ExpandWidth(false));
                }
                if (GizmoType == GizmoTypes.Box)
                {
                    EditorGUILayout.LabelField("Size", GUILayout.Width(50), GUILayout.ExpandWidth(true));
                    GetExternalSize = EditorGUILayout.Toggle(GetExternalSize, GUILayout.Width(15), GUILayout.MaxWidth(15),
                        GUILayout.ExpandWidth(false));
                }
                if (GizmoType == GizmoTypes.Direction)
                {
                    EditorGUILayout.LabelField("Direction", GUILayout.Width(50), GUILayout.ExpandWidth(true));
                    GetExternalDirection = EditorGUILayout.Toggle(GetExternalDirection, GUILayout.Width(15), GUILayout.MaxWidth(15),
                        GUILayout.ExpandWidth(false));
                }
                if (GizmoType == GizmoTypes.Text)
                {
                    EditorGUILayout.LabelField("Text", GUILayout.Width(50), GUILayout.ExpandWidth(true));
                    GetExternalText = EditorGUILayout.Toggle(GetExternalText, GUILayout.Width(15), GUILayout.MaxWidth(15),
                        GUILayout.ExpandWidth(false));
                }
                if ( GizmoType == GizmoTypes.Capsule )
                {
                    EditorGUILayout.LabelField("Height", GUILayout.Width(50), GUILayout.ExpandWidth(true));
                    GetExternalHeight = EditorGUILayout.Toggle(GetExternalHeight, GUILayout.Width(15), GUILayout.MaxWidth(15),
                        GUILayout.ExpandWidth(false));
                }
            }
            GUILayout.EndHorizontal();
        }
        if ( GUI.changed )
        {
            EditorUtility.SetDirty(target);
            SetValues();
        }
    }

    #endregion

    #region Private Methods

    private void OnEnable()
    {
        targetScript = target as DebugGizmos;
        gameObject = targetScript.gameObject;
        GetValues();
    }

    private void GetValues()
    {
        PointSphere = targetScript.PointSphere;
        PoinSpehereRadius = targetScript.PoinSpehereRadius;
        GizmoType = targetScript.GizmoType;
        Behavior = targetScript.PointBehavior;
        GizmoSize = targetScript.GizmoSize;
        Radius = targetScript.Radius;
        Angle = targetScript.Angle;
        Sections = targetScript.Sections;
        DrawBoundings = targetScript.DrawBoundings;
        DrawCamera = targetScript.DrawCamera;
        DrawTexture = targetScript.DrawTexture;
        Icon = targetScript.Icon;
        IconOffset = targetScript.IconOffset;
        TextOffset = targetScript.TextOffset;
        Outline = targetScript.Outline;
        GizmoLine = targetScript.GizmoLine;
        Extra = targetScript.Extra;
        TextColor = targetScript.TextColor;
        DrawVolumetric = targetScript.DrawVolumetric;
        Volumetric = targetScript.Volumetric;
        Size = targetScript.Size;
        Direction = targetScript.Direction;
        GetExternalAngel = targetScript.GetExternalAngel;
        GetExternalDirection = targetScript.GetExternalDirection;
        GetExternalRadius = targetScript.GetExternalRadius;
        GetExternalSize = targetScript.GetExternalSize;
        GetExternalText = targetScript.GetExternalText;
        GetExternalHeight = targetScript.GetExternalHeight;
        GetExternalCircleRotation = targetScript.GetExternalCircleRotation;
        UseLocalMatrix = targetScript.UseLocalMatrix;
        Text = targetScript.Text;
        CanDrawBounds = targetScript.CanDrawBounds;
        CanDrawCamera = targetScript.CanDrawCamera;
        Height = targetScript.Height;
        Draw = targetScript.Draw;
        CircleRotation = targetScript.CircleRotation;
    }

    private void SetValues()
    {
        targetScript.PointSphere = PointSphere;
        targetScript.PoinSpehereRadius = PoinSpehereRadius;
        targetScript.GizmoType = GizmoType;
        targetScript.PointBehavior = Behavior;
        targetScript.GizmoSize = GizmoSize;
        targetScript.Radius = Radius;
        targetScript.Angle = Angle;
        targetScript.Sections = Sections;
        targetScript.DrawBoundings = DrawBoundings;
        targetScript.DrawCamera = DrawCamera;
        targetScript.DrawTexture = DrawTexture;
        targetScript.Icon = Icon;
        targetScript.IconOffset = IconOffset;
        targetScript.TextOffset = TextOffset;
        targetScript.Outline = Outline;
        targetScript.GizmoLine = GizmoLine;
        targetScript.Extra = Extra;
        targetScript.DrawVolumetric = DrawVolumetric;
        targetScript.Volumetric = Volumetric;
        targetScript.Size = Size;
        targetScript.Direction = Direction;
        targetScript.GetExternalAngel = GetExternalAngel;
        targetScript.GetExternalDirection = GetExternalDirection;
        targetScript.GetExternalRadius = GetExternalRadius;
        targetScript.GetExternalSize = GetExternalSize;
        targetScript.GetExternalText = GetExternalText;
        targetScript.GetExternalHeight = GetExternalHeight;
        targetScript.GetExternalCircleRotation = GetExternalCircleRotation;
        targetScript.UseLocalMatrix = UseLocalMatrix;
        targetScript.TextColor = TextColor;
        targetScript.Text = Text;
        targetScript.Height = Height;
        targetScript.Draw = Draw;
        targetScript.CircleRotation = CircleRotation;
    }

    #endregion
}