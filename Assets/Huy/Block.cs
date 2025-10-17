using UnityEngine;

public class Block : MonoBehaviour
{
    public enum Direction { Left, Right, Up, Down }
    public Direction myDirection;
    private GridManager gridManager;
    private Vector3Int gridPos;
    private Renderer rend;  // Để lerp color glow

    public void Initialize(GridManager manager, Vector3Int pos, Direction dir, Color color)
    {
        gridManager = manager;
        gridPos = pos;
        myDirection = dir;
        rend = GetComponent<Renderer>();
        rend.material.color = color;

        // Xoay toàn bộ block để mũi tên chỉ đúng (giả sử asset arrow hướng local -X mặc định)
        switch (dir)
        {
            case Direction.Right: transform.rotation = Quaternion.Euler(0, 180, 0); break;  // World +X
            case Direction.Left: transform.rotation = Quaternion.Euler(0, 0, 0); break;    // World -X
            case Direction.Up: transform.rotation = Quaternion.Euler(0, -90, 0); break;   // World +Z
            case Direction.Down: transform.rotation = Quaternion.Euler(0, 90, 0); break;  // World -Z
        }
    }

    public bool CanRemove()
    {
        // Giữ nguyên như trước (kiểm tra đường đi theo hướng)
        Vector3Int size = gridManager.gridSize;
        switch (myDirection)
        {
            case Direction.Right:
                for (int x = gridPos.x + 1; x < size.x; x++)
                    if (gridManager.grid[x, gridPos.y, gridPos.z] != null) return false;
                return true;
            case Direction.Left:
                for (int x = gridPos.x - 1; x >= 0; x--)
                    if (gridManager.grid[x, gridPos.y, gridPos.z] != null) return false;
                return true;
            case Direction.Up:
                for (int z = gridPos.z + 1; z < size.z; z++)
                    if (gridManager.grid[gridPos.x, gridPos.y, z] != null) return false;
                return true;
            case Direction.Down:
                for (int z = gridPos.z - 1; z >= 0; z--)
                    if (gridManager.grid[gridPos.x, gridPos.y, z] != null) return false;
                return true;
        }
        return false;
    }

    public void FlyAway()
    {
        Vector3 moveDir = Vector3.zero;
        switch (myDirection)
        {
            case Direction.Right: moveDir = Vector3.right; break;
            case Direction.Left: moveDir = Vector3.left; break;
            case Direction.Up: moveDir = Vector3.forward; break;
            case Direction.Down: moveDir = Vector3.back; break;
        }
        StartCoroutine(AnimateRemoval(moveDir));
    }

    private System.Collections.IEnumerator AnimateRemoval(Vector3 dir)
    {
        // Glow effect: Lerp color sang sáng hơn và scale up
        Color startColor = rend.material.color;
        Color glowColor = Color.white;  // Hoặc startColor * 2f cho sáng
        Vector3 startScale = transform.localScale;
        float glowDuration = 0.3f;  // Thời gian glow trước bay
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / glowDuration;
            rend.material.color = Color.Lerp(startColor, glowColor, t);
            transform.localScale = Vector3.Lerp(startScale, startScale * 1.2f, t);
            yield return null;
        }

        // Spawn particle sparkle (gán prefab trong GridManager)
        if (gridManager.sparklePrefab != null)
        {
            Instantiate(gridManager.sparklePrefab, transform.position, Quaternion.identity);
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

        gridManager.RemoveBlock(gridPos);
        Destroy(gameObject);
    }
}