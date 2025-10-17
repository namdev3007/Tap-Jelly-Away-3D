#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class BlockSpawnerWindow : EditorWindow
{
    private GameObject blockPrefab;
    private int sizeX = 4;
    private int sizeY = 4;
    private int sizeZ = 4;
    private float spacing = 1f;
    private Vector3 startPosition = Vector3.zero;
    private Transform parent;

    private static readonly Direction[] allDirections = new Direction[]
    {
        Direction.Forward,
        Direction.Backward,
        Direction.Left,
        Direction.Right,
        Direction.Up,
        Direction.Down
    };

    [MenuItem("Tools/Tap Away/Smart Block Spawner")]
    public static void ShowWindow()
    {
        GetWindow<BlockSpawnerWindow>("Smart Block Spawner");
    }

    private void OnGUI()
    {
        GUILayout.Label("🧠 Smart Block Spawner Tool", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        blockPrefab = (GameObject)EditorGUILayout.ObjectField("Block Prefab", blockPrefab, typeof(GameObject), false);
        parent = (Transform)EditorGUILayout.ObjectField("Parent (Optional)", parent, typeof(Transform), true);

        EditorGUILayout.Space();
        GUILayout.Label("📐 Spawn Settings", EditorStyles.boldLabel);

        sizeX = EditorGUILayout.IntField("Size X", sizeX);
        sizeY = EditorGUILayout.IntField("Size Y", sizeY);
        sizeZ = EditorGUILayout.IntField("Size Z", sizeZ);
        spacing = EditorGUILayout.FloatField("Spacing", spacing);
        startPosition = EditorGUILayout.Vector3Field("Start Position", startPosition);

        EditorGUILayout.Space(10);

        if (GUILayout.Button("🚀 Spawn Random Level"))
        {
            SpawnRandomBlocks();
        }

        if (GUILayout.Button("🧹 Clear Spawned Blocks"))
        {
            ClearSpawnedBlocks();
        }
    }

    private void SpawnRandomBlocks()
    {
        if (blockPrefab == null)
        {
            EditorUtility.DisplayDialog("⚠️ Lỗi", "Bạn chưa gán Prefab block!", "OK");
            return;
        }

        Transform container = parent != null ? parent : new GameObject("BlockContainer").transform;
        ClearSpawnedBlocks();

        List<Vector3> availablePositions = new List<Vector3>();

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    Vector3 pos = startPosition + new Vector3(x * spacing, y * spacing, z * spacing);
                    availablePositions.Add(pos);
                }
            }
        }

        int count = 0;
        foreach (var pos in availablePositions)
        {
            GameObject block = (GameObject)PrefabUtility.InstantiatePrefab(blockPrefab, container);
            block.transform.position = pos;
            block.transform.localScale = Vector3.one;
            block.name = $"Block_{count++}";

            if (block.GetComponent<BlockController>() == null)
                block.AddComponent<BlockController>();

            Direction randomDir = allDirections[Random.Range(0, allDirections.Length)];

            var controller = block.GetComponent<BlockController>();
            if (controller != null)
            {
                var field = typeof(BlockController).GetField("rayDirection",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(controller, randomDir);
                }
            }

            Transform arrow = block.transform.Find("Arrow");
            if (arrow != null)
            {
                arrow.rotation = Quaternion.LookRotation(GetVectorFromDirection(randomDir));
            }
        }

        EditorUtility.DisplayDialog("✅ Hoàn thành", "Spawn block ngẫu nhiên hướng thành công!", "OK");
    }

    private Vector3 GetVectorFromDirection(Direction dir)
    {
        return dir switch
        {
            Direction.Forward => Vector3.forward,
            Direction.Backward => Vector3.back,
            Direction.Left => Vector3.left,
            Direction.Right => Vector3.right,
            Direction.Up => Vector3.up,
            Direction.Down => Vector3.down,
            _ => Vector3.forward
        };
    }

    private void ClearSpawnedBlocks()
    {
        if (parent != null)
        {
            while (parent.childCount > 0)
                DestroyImmediate(parent.GetChild(0).gameObject);
        }
        else
        {
            var container = GameObject.Find("BlockContainer");
            if (container != null)
                DestroyImmediate(container);
        }
    }
}
#endif
