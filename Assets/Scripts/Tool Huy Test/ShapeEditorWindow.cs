using UnityEditor;
using UnityEngine;

public class ShapeEditorWindow : EditorWindow
{
    private ShapeData shapeData;
    private int currentLayer = 0;
    private Vector3Int tempSize = new Vector3Int(7, 6, 7);

    [MenuItem("Tools/Shape Editor")]
    public static void OpenWindow()
    {
        GetWindow<ShapeEditorWindow>("Shape Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("🧱 Shape Editor Tool", EditorStyles.boldLabel);

        // -----------------------------
        // Chọn hoặc tạo asset ShapeData
        // -----------------------------
        shapeData = (ShapeData)EditorGUILayout.ObjectField("Shape Data", shapeData, typeof(ShapeData), false);
        if (shapeData == null)
        {
            if (GUILayout.Button("Create New Shape Data"))
            {
                shapeData = ScriptableObject.CreateInstance<ShapeData>();
                shapeData.Initialize();
                AssetDatabase.CreateAsset(shapeData, "Assets/ShapeData.asset");
                AssetDatabase.SaveAssets();
            }
            return;
        }

        // -----------------------------
        // Thay đổi kích thước grid
        // -----------------------------
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Grid Size:");
        tempSize = EditorGUILayout.Vector3IntField("", tempSize);
        if (GUILayout.Button("Apply Size", GUILayout.Width(100)) && tempSize != shapeData.size)
        {
            shapeData.size = tempSize;
            shapeData.Initialize();
            EditorUtility.SetDirty(shapeData);
        }
        EditorGUILayout.EndHorizontal();

        // -----------------------------
        // Chọn layer Y (chiều cao)
        // -----------------------------
        currentLayer = EditorGUILayout.IntSlider("Layer Y (Height)", currentLayer, 0, shapeData.size.y - 1);
        GUILayout.Space(10);
        GUILayout.Label($"Editing Layer Y: {currentLayer}", EditorStyles.boldLabel);

        // -----------------------------
        // Hiển thị grid XZ cho layer hiện tại
        // -----------------------------
        for (int z = shapeData.size.z - 1; z >= 0; z--) // Lật Z cho dễ nhìn
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < shapeData.size.x; x++)
            {
                bool current = shapeData.GetBlock(x, currentLayer, z);
                GUI.color = current ? Color.green : new Color(0.3f, 0.3f, 0.3f);
                if (GUILayout.Button("", GUILayout.Width(20), GUILayout.Height(20)))
                {
                    shapeData.SetBlock(x, currentLayer, z, !current);
                    EditorUtility.SetDirty(shapeData);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        GUI.color = Color.white;
        GUILayout.Space(10);

        // -----------------------------
        // Các nút tiện ích
        // -----------------------------
        if (GUILayout.Button("Clear Current Layer"))
        {
            for (int z = 0; z < shapeData.size.z; z++)
                for (int x = 0; x < shapeData.size.x; x++)
                    shapeData.SetBlock(x, currentLayer, z, false);
            EditorUtility.SetDirty(shapeData);
        }

        if (GUILayout.Button("Clear All Layers"))
        {
            shapeData.Initialize();
            EditorUtility.SetDirty(shapeData);
        }

        if (GUILayout.Button("Save Asset"))
        {
            AssetDatabase.SaveAssets();
        }
    }
}
