using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BlockController : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float checkDistance = 2f;
    [SerializeField] private Direction rayDirection = Direction.Forward;

    private IObstacleChecker _obstacleChecker;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _obstacleChecker = new RaycastObstacleChecker();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryCheckObstacle();
        }
    }

    private void TryCheckObstacle()
    {
        Vector3 origin = _collider.bounds.center;
        Vector3 direction = GetDirectionVector(rayDirection);
        bool hasObstacle = _obstacleChecker.HasObstacle(origin, direction, checkDistance);

        if (hasObstacle)
            Debug.Log("?? Block b? ch?n, không th? di chuy?n!");
        else
            Debug.Log("? Block có th? thoát ra phía tr??c!");
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (_collider == null) _collider = GetComponent<Collider>();
        Vector3 origin = _collider.bounds.center;
        Vector3 direction = GetDirectionVector(rayDirection);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, direction * checkDistance);
        _obstacleChecker?.DrawRay(origin, direction, checkDistance);
    }
#endif

    private Vector3 GetDirectionVector(Direction dir)
    {
        return dir switch
        {
            Direction.Forward => transform.forward,
            Direction.Backward => -transform.forward,
            Direction.Left => -transform.right,
            Direction.Right => transform.right,
            Direction.Up => transform.up,
            Direction.Down => -transform.up,
            _ => transform.forward
        };
    }
}

public enum Direction
{
    Forward,
    Backward,
    Left,
    Right,
    Up,
    Down
}
