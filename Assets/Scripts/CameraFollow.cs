using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;     // Mario (auto-found by tag if left empty)
    public Transform endLimit;   // GameObject marking the end of the map

    private float offset;        // horizontal distance between camera and Mario at the start
    private float startX;        // camera can't go left of this
    private float endX;          // camera can't go right of this

    void Awake()
    {
        // only search if you didn't drag Mario in yourself
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogError("CameraFollow: no player found. Tag Mario as 'Player' or assign it in the Inspector.", this);
            enabled = false;
            return;
        }
        if (endLimit == null)
        {
            Debug.LogError("CameraFollow: endLimit is not assigned in the Inspector.", this);
            enabled = false;
            return;
        }

        Camera cam = GetComponent<Camera>();
        float viewportHalfWidth = cam.orthographicSize * cam.aspect;

        offset = transform.position.x - player.position.x;
        startX = transform.position.x;
        endX = endLimit.position.x - viewportHalfWidth;

       
    }

    void LateUpdate()
    {
        float desiredX = Mathf.Clamp(player.position.x + offset, startX, Mathf.Max(startX, endX));
        transform.position = new Vector3(desiredX, transform.position.y, transform.position.z);
   
    }
}