using UnityEngine;

public class MouseRenderer : MonoBehaviour
{
    private static MouseRenderer _instance;

    void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = CameraManager.MainCamera.ScreenToWorldPoint(Input.mousePosition);
            var hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider) 
            {
                hit.collider.gameObject.GetComponent<WireCell>()?.RotateToLeft();
            }
        }
    }
}
