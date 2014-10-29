using UnityEngine;
using UnityEditor;

public class SceneChaseCam : EditorWindow
{
    bool active = false;
    bool followSelection = false;
    Transform toFollow;

    // Add menu named "Scene Chase Cam" to the Window menu
    [MenuItem("Window/Scene Chase Cam")]
    static void Init()
    {
        // Get/create window and focus
        (EditorWindow.GetWindow<SceneChaseCam>()).Focus();
    }

    void OnGUI()
    {
        // basic options
        active = EditorGUILayout.Toggle("Active:", active);
        followSelection = EditorGUILayout.Toggle("Follow selection:", followSelection);

        // slight aesthetic gap
        GUILayout.Space(10);

        EditorGUILayout.LabelField("Transform to follow:");
        toFollow = EditorGUILayout.ObjectField(toFollow, typeof(Transform), true) as Transform;

        // automatically follow selected gameobject?
        if (followSelection && Selection.activeTransform != null)
            toFollow = Selection.activeTransform;

        // don't bother showing vector field unless we have a transform selected
        if (toFollow != null)
            toFollow.transform.position = EditorGUILayout.Vector3Field(toFollow.gameObject.name + " position",
            toFollow.transform.position);

        Repaint();
    }

    void Update()
    {
        // must be active, playing and following some object
        if (!active || !Application.isPlaying || toFollow == null) return;
        foreach (SceneView scene in SceneView.sceneViews)
        {
            scene.pivot = toFollow.position;
            scene.Repaint();
        }
    }
}