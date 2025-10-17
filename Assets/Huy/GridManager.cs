using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject sparklePrefab;  // Gán prefab ParticleSystem cho sparkle
    public Vector3Int gridSize = new Vector3Int(5, 5, 5);  // Tăng height để chồng nhiều hơn như ảnh
    public float blockSpacing = 1.1f;
    public Block[,,] grid;

    private List<Color> colors = new List<Color> { Color.red, Color.yellow, Color.green, Color.blue, Color.magenta };
    private Block.Direction[] directions = { Block.Direction.Left, Block.Direction.Right, Block.Direction.Up, Block.Direction.Down };

    void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        grid = new Block[gridSize.x, gridSize.y, gridSize.z];

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    // Random skip để tạo stack chồng bất規則 (giống ảnh, không full grid)
                    if (Random.value > 0.6f) continue;  // 40% chance skip, điều chỉnh để ít/mật độ hơn

                    Vector3 pos = new Vector3(x * blockSpacing, y * blockSpacing, z * blockSpacing);
                    GameObject blockObj = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                    Block block = blockObj.GetComponent<Block>();

                    Color randomColor = colors[Random.Range(0, colors.Count)];
                    Block.Direction randomDir = directions[Random.Range(0, directions.Length)];

                    block.Initialize(this, new Vector3Int(x, y, z), randomDir, randomColor);
                    grid[x, y, z] = block;
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Block block = hit.collider.GetComponent<Block>();
                if (block != null && block.CanRemove())
                {
                    block.FlyAway();
                }
            }
        }
    }

    public void RemoveBlock(Vector3Int pos)
    {
        grid[pos.x, pos.y, pos.z] = null;
    }
}