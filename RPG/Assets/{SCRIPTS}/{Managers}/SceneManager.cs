#region Documentation
// file:	RPG\Assets\{SCRIPTS}\{Managers}\SceneManager.cs
//
// summary:	Implements the scene manager class
#endregion

using System.Collections.Generic;
using UnityEngine;

#region Documentation
/// <summary>   Manager for scenes. </summary>
///
/// <remarks>   Dave, 10/26/2014. </remarks>
#endregion

[AddComponentMenu("RPG/Manager/Scene")]
public class SceneManager : MonoBehaviour
{
    /// <summary>   The player spawn point. </summary>
    public Transform PlayerSpawnPoint;
    /// <summary>   The player prefab. </summary>
    public GameObject PlayerPrefab;

    /// <summary>   The player. </summary>
    private GameObject player;
    /// <summary>   true if initialized. </summary>
    private bool Initialized = false;
    /// <summary>   The pivot. </summary>
    private GameObject pivot;

    /// <summary>   The instance. </summary>
    public static SceneManager Instance;

    #region Documentation
    /// <summary>   Starts this object. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    #endregion

    void Start()
    {
        Instance = this;
        if (!Initialized)
            Initialize();
    }

    #region Documentation
    /// <summary>   Initializes this object. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    #endregion

    void Initialize()
    {
        SpawnPlayer();
        CameraSetup();
        FinalInitialize();
        SetPlayerMeshes();
        RPGCamera.Instance.Initialize();
        Initialized = true;
    }

    #region Documentation
    /// <summary>   Sets player meshes. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    #endregion

    void SetPlayerMeshes()
    {
        var objList= new List<GameObject>();
        for (var i = 0; i < player.transform.childCount; i++)
        {
            if (player.transform.GetChild(i).GetComponent<Renderer>())
                objList.Add(player.transform.GetChild(i).gameObject);
        }
        if (player.GetComponent<Renderer>())
            objList.Add(player);
        GameObject[] tmp = objList.ToArray();
        RPGCamera.Instance.PlayerMesh = new GameObject[tmp.Length];
        RPGCamera.Instance.PlayerMesh = tmp;
    }

    #region Documentation
    /// <summary>   Final initialize. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    #endregion

    void FinalInitialize()
    {
        
        pivot.transform.position = new Vector3(player.transform.position.x,
            player.transform.position.y + RPGCamera.Instance.CameraBottomDistance,
            player.transform.position.z);
        pivot.transform.parent = player.transform;
        //! Here is the place u add future meshes to be loaded on character, any mesh that will be mounted on character
        //! Like Weapons, Armor, Hair, etc should be loaded here in future
    }

    #region Documentation
    /// <summary>   Camera setup. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    #endregion

    void CameraSetup()
    {
        if (Camera.main == null)
        {
            var tmp = new GameObject("Main Camera");
            tmp.AddComponent<Camera>();
            tmp.tag = "MainCamera";
        }
        
        if (GameObject.Find("Camera Pivot") == null)
            pivot = new GameObject("Camera Pivot");
        pivot = GameObject.Find("Camera Pivot");
        
        if (Camera.main != null && Camera.main.gameObject.GetComponent<RPGCamera>() == null)
            Camera.main.gameObject.AddComponent<RPGCamera>();
        if (Camera.main != null && Camera.main.gameObject.GetComponent<RPGCamera>().CameraPivot == null)
            Camera.main.gameObject.GetComponent<RPGCamera>().CameraPivot = pivot.transform;
        
    }

    #region Documentation
    /// <summary>   Spawn player. </summary>
    ///
    /// <remarks>   Dave, 10/26/2014. </remarks>
    #endregion

    void SpawnPlayer()
    {
        player =
            Instantiate(PlayerPrefab, PlayerSpawnPoint.transform.position, PlayerSpawnPoint.transform.rotation) as
                GameObject;
        if (player != null && player.GetComponent<RPGPlayerMotor>() == null)
        {
            player.AddComponent<RPGPlayerMotor>();
            player.gameObject.name = "Player";
            player.tag = "Player";
        }
    }
}