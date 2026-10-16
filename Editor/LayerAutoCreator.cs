#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// This attribute tells Unity to run the static constructor on editor startup/compile
[InitializeOnLoad]
public static class LayerAutoCreator
{
    // The static constructor runs automatically
    static LayerAutoCreator()
    {
        // Define the layer your library absolutely depends on
        CreateLayer("StepPoint");
        CreateLayer("ReachArea");
    }

    private static void CreateLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
        );

        SerializedProperty layersProperty = tagManager.FindProperty("layers");

        if (layersProperty == null || !layersProperty.isArray) return;

        // Check if it already exists
        for (int i = 0; i < layersProperty.arraySize; i++)
        {
            if (layersProperty.GetArrayElementAtIndex(i).stringValue == layerName)
            {
                return; // Layer is already there, do nothing
            }
        }

        // Find an empty slot (8-31) and assign it
        for (int i = 8; i < layersProperty.arraySize; i++)
        {
            SerializedProperty layerElement = layersProperty.GetArrayElementAtIndex(i);

            if (string.IsNullOrEmpty(layerElement.stringValue))
            {
                layerElement.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
                Debug.Log($"[LibraryName] Automatically setup required layer: '{layerName}' at slot {i}.");
                return;
            }
        }

        Debug.LogError($"[LibraryName] Critical Error: Could not create the required '{layerName}' layer because all 32 Unity layer slots are full!");
    }
}
#endif
