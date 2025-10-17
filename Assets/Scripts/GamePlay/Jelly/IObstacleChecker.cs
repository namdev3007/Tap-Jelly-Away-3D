using UnityEngine;

public interface IObstacleChecker
{
    bool HasObstacle(Vector3 origin, Vector3 direction, float distance);
    void DrawRay(Vector3 origin, Vector3 direction, float distance);
}