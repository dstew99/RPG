using UnityEngine;

[AddComponentMenu("RPG/Camera/TPS")]
public class RPGCamera : MonoBehaviour
{
    public float Distance = 5f;
    public float DistanceMax = 30f;
    public float SafeMinDistance = 0.0501f;
    public float MouseSpeed = 8f;
    public float MouseScroll = 15f;
    public float MouseSmoothingFactor = 0.08f;
    public float CameraDistanceSpeed = 0.7f;
    public float CameraBottomDistance = 2f;
    public float FirstPersonThreshold = 0.8f;
    public float CharacterFadeThreshold = 1.8f;
    public float NormalFOV = 60f;
    public float SprintingFOV = 75f;
    public float FovDampingTime = 3f;
    public string PlayerTag = "Player";
    public string MouseX = "Mouse X";
    public string MouseY = "Mouse Y";
    public string MouseWheel = "Mouse ScrollWheel";
    public string Horizontal = "Horizontal";


    public Transform CameraPivot;
    public GameObject[] PlayerMesh = new GameObject[0];
    

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
    private bool initialized = false;

    private static float halfFieldOfView;
    private static float planeAspect;
    private static float halfPlaneHeight;
    private static float halfPlaneWidth;

    #endregion

    public void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        if (CameraPivot != null)
        {
            Distance = Mathf.Clamp(Distance, 0.0f, DistanceMax);
            desiredDistance = Distance;
            halfFieldOfView = (Camera.main.fieldOfView/2)*Mathf.Deg2Rad;
            planeAspect = Camera.main.aspect;
            halfPlaneHeight = Camera.main.nearClipPlane*Mathf.Tan(halfFieldOfView);
            halfPlaneWidth = halfPlaneHeight*planeAspect;
            mouseX = Quaternion.Angle(SceneManager.Instance.PlayerSpawnPoint.rotation,transform.rotation);
            mouseY = 15;
            initialized = true;
        }
    }

    void LateUpdate()
    {
        if (CameraPivot == null || !initialized)
            return;
        GetInput();
        GetDesiredPosition();
        PositionUpdate();
        if (PlayerMesh.Length != 0)
            CharacterFade();
    }

    private void GetInput()
    {
        if (Distance > 0.1)
        {
            Debug.DrawLine(transform.position,transform.position - Vector3.up* CameraBottomDistance,Color.green);
            camBottom = Physics.Linecast(transform.position,
                transform.position - Vector3.up*CameraBottomDistance);
        }
        bool constrainMouseY = camBottom && transform.position.y - CameraPivot.transform.position.y <= 0;
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Screen.showCursor = false;
            mouseX += Input.GetAxis(MouseX)*MouseSpeed;
            if (constrainMouseY)
            {
                if (Input.GetAxis(MouseY) < 0)
                    mouseY -= Input.GetAxis(MouseY)*MouseSpeed;
            }
            else
                mouseY -= Input.GetAxis(MouseY)*MouseSpeed;
        }
        else
            Screen.showCursor = true;
        mouseY = HELPER.ClampAngle(mouseY, mouseYMin, mouseYMax);
        mouseXSmooth = Mathf.SmoothDamp(mouseXSmooth, mouseX, ref mouseXVel, MouseSmoothingFactor);
        mouseYSmooth = Mathf.SmoothDamp(mouseYSmooth, mouseY, ref mouseYVel, MouseSmoothingFactor);
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
        desiredDistance = desiredDistance - Input.GetAxis(MouseWheel)*MouseScroll;
        if (desiredDistance > DistanceMax)
            desiredDistance = DistanceMax;
        if (desiredDistance < SafeMinDistance)
            desiredDistance = SafeMinDistance;
    }

    private void GetDesiredPosition()
    {
        Distance = desiredDistance;
        desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, Distance);
        float closestDistance;
        constraint = false;
        closestDistance = CheckCameraClipPlane(CameraPivot.position, desiredPosition);
        if (closestDistance != -1)
        {
            Distance = closestDistance;
            desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, Distance);
            constraint = true;
        }
        Distance -= Camera.main.nearClipPlane;
        if (lastDistance < Distance || !constraint)
            Distance = Mathf.SmoothDamp(lastDistance, Distance, ref distanceVel,
                CameraDistanceSpeed);
        if (Distance < SafeMinDistance)
            Distance = SafeMinDistance;
        lastDistance = Distance;
        desiredPosition = GetCameraPosition(mouseYSmooth, mouseXSmooth, Distance);
    }

    private void PositionUpdate()
    {
        transform.position = desiredPosition;
        if (Distance > SafeMinDistance - 0.0001f)
            transform.LookAt(CameraPivot);
    }

    private void CharacterFade()
    {
        if (Distance < FirstPersonThreshold)
            foreach (var playerMesh in PlayerMesh)
                playerMesh.renderer.enabled = false;
        else if (Distance < CharacterFadeThreshold)
        {
            float charcaterAlpha = 1 -
                                   (CharacterFadeThreshold - Distance)/
                                   (CharacterFadeThreshold - FirstPersonThreshold);
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
        if (Physics.Linecast(from, to, out hitInfo) && hitInfo.collider.tag != PlayerTag)
            closestDistance = hitInfo.distance - Camera.main.nearClipPlane;
        if (Physics.Linecast(from - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != PlayerTag)
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance =
                    Vector3.Distance(hitInfo.point + transform.right*halfPlaneWidth - transform.up*halfPlaneHeight, from);
        if (Physics.Linecast(from + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != PlayerTag)
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance =
                    Vector3.Distance(hitInfo.point - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, from);
        if (Physics.Linecast(from - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != PlayerTag)
            if (hitInfo.distance < closestDistance || closestDistance == -1)
                closestDistance =
                    Vector3.Distance(hitInfo.point + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, from);
        if (Physics.Linecast(from + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlane.UpperLeft, out hitInfo) && hitInfo.collider.tag != PlayerTag)
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
}