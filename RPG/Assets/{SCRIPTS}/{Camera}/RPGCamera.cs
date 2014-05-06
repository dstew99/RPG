using UnityEngine;

[System.Serializable]
public class RPGCameraSettings
{
    public float Distance = 5f;
    public float DistanceMax = 30f;
    public float MouseSpeed = 8f;
    public float MouseScroll = 15f;
    public float MouseSmoothingFactor = 0.08f;
    public float CameraDistanceSpeed = 0.7f;
    public float CameraBottomDistance = 1f;
    public float FirstPersonThreshold = 0.8f;
    public float CharacterFadeThreshold = 1.8f;
    public float NormalFOV = 60f;
    public float SprintingFOV = 75f;
    public float FovDampingTime = 3f;
    public string PlayerTag = "Player";
}

[System.Serializable]
public class RPGCameraAxises
{
    public string MouseX = "Mouse X";
    public string MouseY = "Mouse Y";
    public string MouseWheel = "Mouse ScrollWheel";
    public string Horizontal = "Horizontal";
}

[AddComponentMenu("RPG/Camera/TPS")]
public class RPGCamera : MonoBehaviour
{
    public Transform CameraPivot;
    public GameObject[] PlayerMesh;
    public RPGCameraSettings Settings;
    public RPGCameraAxises Axises;

    #region private and static

    public static RPGCamera Instance;
    private Vector3 desiredPosition;
    private float desiredDistance;
    private float lastDistance;
    private float mouseX = 0;
    private float mouseXSmooth = 0;
    private float mouseXVel;
    private float mouseY = 0;
    private float mouseYSmooth = 0;
    private float mouseYVel;
    private float mouseYMin = -89.5f;
    private float mouseYMax = 89.5f;
    private float distanceVel;
    private bool camBottom;
    private bool constraint;

    private static float halfFieldOfView;
    private static float planeAspect;
    private static float halfPlaneHeight;
    private static float halfPlaneWidth;

    #endregion

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CameraSetup();
        Settings.Distance = Mathf.Clamp(Settings.Distance, 0.0f, Settings.DistanceMax);
        desiredDistance = Settings.Distance;
        halfFieldOfView = (Camera.main.fieldOfView/2)*Mathf.Deg2Rad;
        planeAspect = Camera.main.aspect;
        halfPlaneHeight = Camera.main.nearClipPlane*Mathf.Tan(halfFieldOfView);
        halfPlaneWidth = halfPlaneHeight*planeAspect;
        mouseX = 0;
        mouseY = 15;
    }

    void LateUpdate()
    {
        if (CameraPivot == null)
        {
            Debug.LogError("No Camera Pivot found!");
            return;
        }
        GetInput();
        GetDesiredPosition();
        PositionUpdate();
        CharacterFade();
    }

    private void GetInput()
    {
        if (Settings.Distance > 0.1)
        {
            Debug.DrawLine(transform.position,transform.position - Vector3.up* Settings.CameraBottomDistance,Color.green);
            camBottom = Physics.Linecast(transform.position,
                transform.position - Vector3.up*Settings.CameraBottomDistance);
        }
        bool constrainMouseY = camBottom && transform.position.y - CameraPivot.transform.position.y <= 0;
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Screen.showCursor = false;
            mouseX += Input.GetAxis(Axises.MouseX)*Settings.MouseSpeed;
            if (constrainMouseY)
            {
                if (Input.GetAxis(Axises.MouseY) < 0)
                    mouseY -= Input.GetAxis(Axises.MouseY)*Settings.MouseSpeed;
            }
            else
                mouseY -= Input.GetAxis(Axises.MouseY)*Settings.MouseSpeed;
        }
        else
            Screen.showCursor = true;
        mouseY = HELPER.ClampAngle(mouseY, mouseYMin, mouseYMax);
        mouseXSmooth = Mathf.SmoothDamp(mouseXSmooth, mouseX, ref mouseXVel, Settings.MouseSmoothingFactor);
        mouseYSmooth = Mathf.SmoothDamp(mouseYSmooth, mouseY, ref mouseYVel, Settings.MouseSmoothingFactor);
        if (constrainMouseY)
            mouseYMin = mouseY;
        else
            mouseYMin = -89.5f;
        mouseYSmooth = HELPER.ClampAngle(mouseYSmooth, mouseYMin, mouseYMax);
        if (Input.GetMouseButton(1))
            RPGPlayerMotor.Instance.transform.rotation =
                Quaternion.Euler(RPGPlayerMotor.Instance.transform.eulerAngles.x,
                                Camera.main.transform.eulerAngles.y,
                                RPGPlayerMotor.Instance.transform.eulerAngles.z);
        desiredDistance = desiredDistance - Input.GetAxis(Axises.MouseWheel)*Settings.MouseScroll;
        if (desiredDistance > Settings.DistanceMax)
            desiredDistance = Settings.DistanceMax;
        if (desiredDistance < 0.05f)
            desiredDistance = 0.05f;
    }

    private void GetDesiredPosition()
    {
        Settings.Distance = desiredDistance;
        desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, Settings.Distance);
        float closestDistance;
        constraint = false;
        closestDistance = CheckCameraClipPlane(CameraPivot.position, desiredPosition);
        if (closestDistance != -1)
        {
            Settings.Distance = closestDistance;
            desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, Settings.Distance);
            constraint = true;
        }
        Settings.Distance -= Camera.main.nearClipPlane;
        if (lastDistance < Settings.Distance || !constraint)
            Settings.Distance = Mathf.SmoothDamp(lastDistance, Settings.Distance, ref distanceVel,
                Settings.CameraDistanceSpeed);
        if (Settings.Distance < 0.05f)
            Settings.Distance = 0.05f;
        lastDistance = Settings.Distance;
        desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, Settings.Distance);
    }

    private void PositionUpdate()
    {
        transform.position = desiredPosition;
        if (Settings.Distance > 0.05f)
            transform.LookAt(CameraPivot);
    }

    private void CharacterFade()
    {
        if (PlayerMesh.Length == 0)
            return;
        if (Settings.Distance < Settings.FirstPersonThreshold)
            foreach (var playerMesh in PlayerMesh)
                playerMesh.renderer.enabled = false;
        else if (Settings.Distance < Settings.CharacterFadeThreshold)
        {
            float charcaterAlpha = 1 -
                                   (Settings.CharacterFadeThreshold - Settings.Distance)/
                                   (Settings.CharacterFadeThreshold - Settings.FirstPersonThreshold);
            foreach (var playerMesh in PlayerMesh)
            {
                playerMesh.renderer.enabled = true;
                if (playerMesh.renderer.material.color.a != charcaterAlpha)
                    playerMesh.renderer.material.color = new Color(playerMesh.renderer.material.color.r,
                                                                    playerMesh.renderer.material.color.g,
                                                                    playerMesh.renderer.material.color.b,
                                                                    charcaterAlpha);
            }
        }
        else
        {
            foreach (var playerMesh in PlayerMesh)
            {
                playerMesh.renderer.enabled = true;
                if (playerMesh.renderer.material.color.a != 1)
                    playerMesh.renderer.material.color = new Color(playerMesh.renderer.material.color.r,
                                                                    playerMesh.renderer.material.color.g,
                                                                    playerMesh.renderer.material.color.b,
                                                                    1);
            }
        }
    }

    private Vector3 GetCameraPosition(float xAxis, float yAxis, float distance)
    {
        Vector3 offset = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(xAxis, yAxis, 0);
        return CameraPivot.position + rotation*offset;
    }

    public struct ClipPlabeVertices
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    private float CheckCameraClipPlane(Vector3 from, Vector3 to)
    {
        var closestDistance = -1f;
        RaycastHit hitInfo;
        ClipPlabeVertices clipPlane = GetClipPlaneAt(to);
        Debug.DrawLine(clipPlane.UpperLeft,clipPlane.UpperRight);
        Debug.DrawLine(clipPlane.UpperRight,clipPlane.LowerRight);
        Debug.DrawLine(clipPlane.LowerRight,clipPlane.LowerLeft);
        Debug.DrawLine(clipPlane.LowerLeft,clipPlane.UpperLeft);
        Debug.DrawLine(from,to,Color.red);
        Debug.DrawLine(from - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperLeft, Color.cyan);
        Debug.DrawLine(from + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperRight, Color.cyan);
        Debug.DrawLine(from - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.LowerLeft, Color.cyan);
        Debug.DrawLine(from + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.LowerRight, Color.cyan);
        if (Physics.Linecast(from, to, out hitInfo) && hitInfo.collider.tag != Settings.PlayerTag)
            closestDistance = hitInfo.distance - Camera.main.nearClipPlane;
        if (Physics.Linecast(from - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != Settings.PlayerTag)
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance =
                    Vector3.Distance(hitInfo.point + transform.right*halfPlaneWidth - transform.up*halfPlaneHeight, from);
        if (Physics.Linecast(from + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != Settings.PlayerTag)
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance =
                    Vector3.Distance(hitInfo.point - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, from);
        if (Physics.Linecast(from - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != Settings.PlayerTag)
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance =
                    Vector3.Distance(hitInfo.point + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, from);
        if (Physics.Linecast(from + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != Settings.PlayerTag)
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance =
                    Vector3.Distance(hitInfo.point - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, from);
        return closestDistance;
    }

    private ClipPlabeVertices GetClipPlaneAt(Vector3 pos)
    {
        var clipPlane = new ClipPlabeVertices();
        if (Camera.main == null)
            return clipPlane;
        Transform transform = Camera.main.transform;
        float offset = Camera.main.nearClipPlane;
        clipPlane.UpperLeft = pos - transform.right*halfPlaneWidth;
        clipPlane.UpperLeft += transform.up*halfPlaneHeight;
        clipPlane.UpperLeft += transform.forward*offset;
        clipPlane.UpperRight = pos + transform.right * halfPlaneWidth;
        clipPlane.UpperRight += transform.up * halfPlaneHeight;
        clipPlane.UpperRight += transform.forward * offset;
        clipPlane.LowerLeft = pos - transform.right * halfPlaneWidth;
        clipPlane.LowerLeft -= transform.up * halfPlaneHeight;
        clipPlane.LowerLeft += transform.forward * offset;
        clipPlane.LowerRight = pos + transform.right * halfPlaneWidth;
        clipPlane.LowerRight -= transform.up * halfPlaneHeight;
        clipPlane.LowerRight += transform.forward * offset;
        return clipPlane;
    }

    public void CameraSetup()
    {
        GameObject cameraUsed;
        GameObject cameraPivot;
        RPGCamera cameraScript;
        if (Camera.main != null)
            cameraUsed = Camera.main.gameObject;
        else
        {
            cameraUsed = new GameObject("Main Camera");
            cameraUsed.AddComponent<Camera>();
            cameraUsed.tag = "MainCamera";
        }
        if (!cameraUsed.GetComponent<RPGCamera>())
            cameraUsed.AddComponent<RPGCamera>();
        cameraScript = cameraUsed.GetComponent<RPGCamera>();
        if (CameraPivot == null)
        {
            cameraPivot = GameObject.Find("Camera Pivot") as GameObject;
            if (cameraPivot == null)
            {
                cameraPivot = new GameObject("Camera Pivot");
                cameraPivot.transform.position = new Vector3(RPGPlayerMotor.Instance.gameObject.transform.position.x,
                                                            RPGPlayerMotor.Instance.gameObject.transform.position.y + 1,
                                                            RPGPlayerMotor.Instance.gameObject.transform.position.z);
                cameraPivot.transform.parent = RPGPlayerMotor.Instance.gameObject.transform;
            }
            cameraScript.CameraPivot = cameraPivot.transform;
        }
    }
}