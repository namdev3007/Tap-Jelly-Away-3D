using UnityEngine;

[RequireComponent(typeof(BoxCollider))]  // Giả sử cube dùng BoxCollider
public class Block : MonoBehaviour
{
    public enum Direction { Forward, Backward, Left, Right, Up, Down }  // Giữ nguyên
    public Direction myDirection;  // Public để set từ GridManager hoặc Inspector
    private IObstacleChecker obstacleChecker;
    private BoxCollider collider;  // Sử dụng Box để chính xác với cube
    private Renderer rend;  // Để glow nếu cần

    private void Awake()
    {
        collider = GetComponent<BoxCollider>();
        obstacleChecker = new RaycastObstacleChecker();  // Khởi tạo implementation cụ thể
        rend = GetComponent<Renderer>();
    }
    private GridManager gridManager;
    private Vector3Int gridPosition;

    public void Initialize(GridManager manager, Vector3Int position, Direction direction, Color color)
    {
        gridManager = manager;
        gridPosition = position;
        myDirection = direction;

        rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = color;
        }
    }


    // Hàm kiểm tra và bay nếu clear (gọi từ GridManager khi hit)
    public bool CanRemove()
    {
        Vector3 origin = collider.bounds.center;
        Vector3 direction = GetDirectionVector(myDirection);
        float checkDistance = 100f;  // Lớn để check đến biên scene, hoặc tính dựa trên grid

        bool hasObstacle = obstacleChecker.HasObstacle(origin, direction, checkDistance);
        return !hasObstacle;  // Return true nếu không chặn
    }

    public void FlyAway()
    {
        Vector3 moveDir = GetDirectionVector(myDirection);  // Bay theo local direction (vì block đã xoay)
        StartCoroutine(AnimateRemoval(moveDir));
    }

    private System.Collections.IEnumerator AnimateRemoval(Vector3 dir)
    {
        // Glow effect (tùy chọn, như code cũ)
        Color startColor = rend.material.color;
        Color glowColor = Color.white;
        Vector3 startScale = transform.localScale;
        float glowDuration = 0.3f;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / glowDuration;
            rend.material.color = Color.Lerp(startColor, glowColor, t);
            transform.localScale = Vector3.Lerp(startScale, startScale * 1.2f, t);
            yield return null;
        }

        // Bay đi
        float flyDuration = 0.5f;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + dir * 10f;
        t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / flyDuration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        Destroy(gameObject);  // Remove, và gọi RemoveBlock từ GridManager nếu cần
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (collider == null) collider = GetComponent<BoxCollider>();
        Vector3 origin = collider.bounds.center;
        Vector3 direction = GetDirectionVector(myDirection);
        float checkDistance = 100f;  // Để vẽ dài
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(origin, direction * checkDistance);
        obstacleChecker?.DrawRay(origin, direction, checkDistance);
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