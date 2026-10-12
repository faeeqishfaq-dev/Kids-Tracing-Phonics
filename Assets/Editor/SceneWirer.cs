using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Game.Gameplay.FillLetter;

public class SceneWirer
{
    //[MenuItem(""Tools/Wire Fill Letters Scene"")]
    public static void Wire()
    {
        //Scene scene = EditorSceneManager.OpenScene(""Assets/_Project/Scenes/1_Gameplay.unity"");
        
        FillLetterManager manager = Object.FindObjectOfType<FillLetterManager>(true);
        if (manager == null) 
        {
            //Debug.LogError(""FillLetterManager not found in scene!"");
            EditorApplication.Exit(1);
            return;
        }

        // Add or get Stroke Guide
        FillLetterStrokeGuide guide = manager.GetComponent<FillLetterStrokeGuide>();
        if (guide == null)
        {
            guide = manager.gameObject.AddComponent<FillLetterStrokeGuide>();
        }

        // Load Prefab
        //GameObject guidePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(""Assets/_Project/Prefabs/StrokeGuideIndicator.prefab"");
        //guide.guidePrefab = guidePrefab;
        guide.speed = 3f;
        guide.scale = 1f;

        // Find Container
        //GameObject container = GameObject.Find(""StrokePointsContainer"");
        //if (container != null)
        //{
        //    guide.strokePointsContainer = container.transform;
        //}
        //else
        //{
        //    Debug.LogWarning(""StrokePointsContainer not found!"");
        //}

        // Assign to Manager using SerializedObject
        SerializedObject so = new SerializedObject(manager);
        //SerializedProperty sp = so.FindProperty(""_strokeGuide"");
        //if (sp != null)
        //{
        //    sp.objectReferenceValue = guide;
        //    so.ApplyModifiedProperties();
        //}

        //EditorSceneManager.SaveScene(scene);
        //Debug.Log(""Scene wired successfully!"");
        EditorApplication.Exit(0);
    }
}
