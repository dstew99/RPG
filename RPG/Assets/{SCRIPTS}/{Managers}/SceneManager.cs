using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("RPG/Manager/Scene")]
public class SceneManager : MonoBehaviour
{
    public Transform PlayerSpawnPoint;
    public GameObject PlayerPrefab;

    private GameObject player;
    private bool Initialized = false;
    private GameObject pivot;

    public static SceneManager Instance;

    void Start()
    {
        Instance = this;
        if (!Initialized)
            Initialize();
    }

    void Initialize()
    {
        SpawnPlayer();
        CameraSetup();
        FinalInitialize();
        SetPlayerMeshes();
        RPGCamera.Instance.Initialize();
        Initialized = true;
    }

    void SetPlayerMeshes()
    {
        int count;
        List<GameObject> obj = new List<GameObject>();
        GameObject[] tmp;
        count = player.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            if (player.transform.GetChild(i).GetComponent<Renderer>())
                obj.Add(player.transform.GetChild(i).gameObject);
        }
        if (player.GetComponent<Renderer>())
            obj.Add(player);
        tmp = obj.ToArray();
        RPGCamera.Instance.PlayerMesh = new GameObject[tmp.Length];
        RPGCamera.Instance.PlayerMesh = tmp;
    }

    void FinalInitialize()
    {
        
        pivot.transform.position = new Vector3(player.transform.position.x,
            player.transform.position.y + RPGCamera.Instance.CameraBottomDistance,
            player.transform.position.z);
        pivot.transform.parent = player.transform;
        //! Here is the place u add future meshes to be loaded on character, any mesh that will be mounted on character
        //! Like Weapons, Armor, Hair, etc should be loaded here in future
    }

    void CameraSetup()
    {
        if (Camera.main == null)
        {
            GameObject tmp = new GameObject("Main Camera");
            tmp.AddComponent<Camera>();
            tmp.tag = "MainCamera";
        }
        
        if (GameObject.Find("Camera Pivot") == null)
            pivot = new GameObject("Camera Pivot");
        pivot = GameObject.Find("Camera Pivot");
        
        if (Camera.main.gameObject.GetComponent<RPGCamera>() == null)
            Camera.main.gameObject.AddComponent<RPGCamera>();
        if (Camera.main.gameObject.GetComponent<RPGCamera>().CameraPivot == null)
            Camera.main.gameObject.GetComponent<RPGCamera>().CameraPivot = pivot.transform;
        
    }

    void SpawnPlayer()
    {
        player =
            Instantiate(PlayerPrefab, PlayerSpawnPoint.transform.position, PlayerSpawnPoint.transform.rotation) as
                GameObject;
        if (player.GetComponent<RPGPlayerMotor>() == null)
            player.AddComponent<RPGPlayerMotor>();
        player.gameObject.name = "Player";
        player.tag = "Player";
    }
}