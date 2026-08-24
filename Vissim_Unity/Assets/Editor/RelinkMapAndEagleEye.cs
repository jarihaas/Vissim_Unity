using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class RelinkMapAndEagleEye
{
    [MenuItem("Tools/Relink Map and EagleEye Prefabs")]
    public static void Relink()
    {
        GameObject mapPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Map.prefab");
        GameObject eaglePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/EagleEye.prefab");

        if (mapPrefab == null || eaglePrefab == null) {
            Debug.LogError("Could not load Assets/Map.prefab or Assets/EagleEye.prefab");
            return;
        }

        int replaced = 0;
        replaced += ReplaceByName("Map", mapPrefab);
        replaced += ReplaceByName("EagleEye", eaglePrefab);

        Debug.Log($"Relink done: {replaced} object(s) replaced. Review the scene and save it (Ctrl+S).");
    }

    private static int ReplaceByName(string name, GameObject prefab)
    {
        int count = 0;
        foreach (GameObject go in Object.FindObjectsOfType<GameObject>()) {
            if (go.name != name || go.transform.parent != null)
                continue;

            Transform t = go.transform;
            Vector3 pos = t.position;
            Quaternion rot = t.rotation;
            Vector3 scale = t.localScale;
            Transform parent = t.parent;
            string instanceName = go.name;

            Object.DestroyImmediate(go);

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            Undo.RegisterCreatedObjectUndo(instance, "Relink " + name);
            if (parent != null)
                instance.transform.SetParent(parent, true);
            instance.transform.SetPositionAndRotation(pos, rot);
            instance.transform.localScale = scale;
            instance.name = instanceName;
            count++;
        }
        return count;
    }
}
