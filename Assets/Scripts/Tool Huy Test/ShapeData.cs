using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "Custom/ShapeData", order = 1)]
public class ShapeData : ScriptableObject
{
    [Header("Grid Size (X = Width, Y = Height, Z = Depth)")]
    public Vector3Int size = new Vector3Int(7, 6, 7);

    [SerializeField, HideInInspector]
    private bool[] blocksFlat; // Mảng 1D có thể serialize được

    // =============================
    //  Khởi tạo mảng block
    // =============================
    public void Initialize()
    {
        int total = size.x * size.y * size.z;
        blocksFlat = new bool[total];
    }

    // =============================
    //  Tính index 1D từ tọa độ 3D
    // =============================
    private int Index(int x, int y, int z)
    {
        return x + size.x * (y + size.y * z);
    }

    // =============================
    //  Lấy giá trị block tại vị trí
    // =============================
    public bool GetBlock(int x, int y, int z)
    {
        if (!IsInside(x, y, z)) return false;
        return blocksFlat[Index(x, y, z)];
    }

    // =============================
    //  Set giá trị block tại vị trí
    // =============================
    public void SetBlock(int x, int y, int z, bool value)
    {
        if (!IsInside(x, y, z)) return;
        blocksFlat[Index(x, y, z)] = value;
    }

    private bool IsInside(int x, int y, int z)
    {
        return x >= 0 && x < size.x &&
               y >= 0 && y < size.y &&
               z >= 0 && z < size.z;
    }
}
