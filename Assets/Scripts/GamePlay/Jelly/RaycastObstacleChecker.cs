using UnityEngine;

public class RaycastObstacleChecker : IObstacleChecker
{
    public bool HasObstacle(Vector3 origin, Vector3 direction, float distance)
    {
        // Sử dụng LayerMask để chỉ hit blocks (giả sử layer "Block")
        int layerMask = 1 << LayerMask.NameToLayer("Block");

        RaycastHit[] hits = Physics.RaycastAll(origin, direction, distance, layerMask);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != null && hit.collider.gameObject.GetComponent<Block>() != null)
            {
                return true;  // Có block khác chặn
            }
        }
        return false;  // Không chặn
    }

    public void DrawRay(Vector3 origin, Vector3 direction, float distance)
    {
        // Có thể thêm vẽ hit points nếu cần, nhưng hiện giữ đơn giản
    }
}