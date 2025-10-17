using UnityEngine;

public class RaycastObstacleChecker : IObstacleChecker
{
    public bool HasObstacle(Vector3 origin, Vector3 direction, float distance)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, ~0))
        {
            Debug.Log($"🚧 Vật cản: {hit.collider.name}");
            return true;
        }
        return false;
    }

    public void DrawRay(Vector3 origin, Vector3 direction, float distance)
    {
#if UNITY_EDITOR
        Debug.DrawRay(origin, direction * distance, Color.yellow);
#endif
    }
}
