using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using Game.Gameplay.FillLetter;
using Game.Gameplay.Tracing;
using Game.UI;

[InitializeOnLoad]
public class FillLetterWiringAutoRun
{
    static FillLetterWiringAutoRun()
    {
        EditorApplication.delayCall += RunWiring;
    }

    [MenuItem("Tools/Setup Fill Letters Feature")]
    public static void RunWiring()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;
            
        if (SessionState.GetBool("FillLetterWired_V7", false))
            return;

        Debug.Log("Auto-wiring Fill Letters Stroke Guide with A-Z paths...");
        
        string scenePath = "Assets/_Project/Scenes/1_Gameplay.unity";
        // Check if the scene is already open to avoid unnecessary reloading
        UnityEngine.SceneManagement.Scene currentScene = EditorSceneManager.GetActiveScene();
        if (currentScene.path != scenePath)
        {
            EditorSceneManager.OpenScene(scenePath);
        }

        FillLetterManager fillManager = GameObject.FindObjectOfType<FillLetterManager>(true);
        if (fillManager == null) return;

        //FillLetterStrokeGuide guide = fillManager.gameObject.GetComponent<FillLetterStrokeGuide>();
        //if (guide == null) guide = fillManager.gameObject.AddComponent<FillLetterStrokeGuide>();

        StrokeValidator validator = fillManager.gameObject.GetComponent<StrokeValidator>();
        if (validator == null) validator = fillManager.gameObject.AddComponent<StrokeValidator>();

        BrushPainter brushPainter = GameObject.FindObjectOfType<BrushPainter>(true);
        FillLetterUIController uiController = GameObject.FindObjectOfType<FillLetterUIController>(true);

        //SerializedObject soGuide = new SerializedObject(guide);
        //soGuide.FindProperty("_fillLetterManager").objectReferenceValue = fillManager;
        //soGuide.FindProperty("_strokeValidator").objectReferenceValue = validator;
        //soGuide.FindProperty("_brushPainter").objectReferenceValue = brushPainter;
        //soGuide.FindProperty("_uiController").objectReferenceValue = uiController;

        if (!AssetDatabase.IsValidFolder("Assets/_Project/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
        }

        string prefabPath = "Assets/_Project/Prefabs/StrokeGuideIndicator.prefab";
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (existingPrefab != null)
        {
            AssetDatabase.DeleteAsset(prefabPath);
        }

        GameObject tempGuide = new GameObject("StrokeGuideIndicator");
        RectTransform rect = tempGuide.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);

        UnityEngine.UI.Image img = tempGuide.AddComponent<UnityEngine.UI.Image>();
        Sprite pointerSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Alphabet-Tracing-Assets-main/Sprites/handle_tracing.png");
        if (pointerSprite != null) img.sprite = pointerSprite;
        img.raycastTarget = false;
        
        // Add a slight rotation to make it look like a natural hand pointing
        rect.localRotation = Quaternion.Euler(0, 0, -25f);

        GameObject guidePrefab = PrefabUtility.SaveAsPrefabAsset(tempGuide, prefabPath);
        GameObject.DestroyImmediate(tempGuide);

        //soGuide.FindProperty("GuidePrefab").objectReferenceValue = guidePrefab;
        //soGuide.FindProperty("Speed").floatValue = 3f;
        //soGuide.FindProperty("Scale").floatValue = 1f;
        //soGuide.FindProperty("StrokeDelay").floatValue = 0.5f;
        //soGuide.FindProperty("CompletionDelay").floatValue = 1.0f;

        //SerializedProperty levelStrokesProp = soGuide.FindProperty("LevelLettersStrokes");
        //levelStrokesProp.ClearArray();
        
        GameObject container = GameObject.Find("StrokePointsContainer");
        if (container != null) GameObject.DestroyImmediate(container);
        container = new GameObject("StrokePointsContainer");

        Vector3[][][] alphabetPaths = new Vector3[][][] {
            new Vector3[][] { new Vector3[] { new Vector3(0, 2, 0), new Vector3(-1.2f, -2, 0) }, new Vector3[] { new Vector3(0, 2, 0), new Vector3(1.2f, -2, 0) }, new Vector3[] { new Vector3(-0.6f, 0, 0), new Vector3(0.6f, 0, 0) } }, // A
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(1.5f, 1, 0), new Vector3(1, 0, 0), new Vector3(-1, 0, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(1.5f, -1, 0), new Vector3(1, -2, 0), new Vector3(-1, -2, 0) } }, // B
            new Vector3[][] { new Vector3[] { new Vector3(1, 2, 0), new Vector3(0, 2.5f, 0), new Vector3(-1, 1.5f, 0), new Vector3(-1, -1.5f, 0), new Vector3(0, -2.5f, 0), new Vector3(1, -2, 0) } }, // C
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 1.5f, 0), new Vector3(1, -1.5f, 0), new Vector3(-1, -2, 0) } }, // D
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0) }, new Vector3[] { new Vector3(-1, -2, 0), new Vector3(1, -2, 0) } }, // E
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0) } }, // F
            new Vector3[][] { new Vector3[] { new Vector3(1, 2, 0), new Vector3(-1, 2, 0), new Vector3(-1, -2, 0), new Vector3(1, -2, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0) } }, // G
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(1, -2, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0) } }, // H
            new Vector3[][] { new Vector3[] { new Vector3(0, 2, 0), new Vector3(0, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(-1, -2, 0), new Vector3(1, -2, 0) } }, // I
            new Vector3[][] { new Vector3[] { new Vector3(1, 2, 0), new Vector3(1, -1, 0), new Vector3(0, -2, 0), new Vector3(-1, -1, 0) } }, // J
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(-1, 0, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, -2, 0) } }, // K
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, -2, 0), new Vector3(1, -2, 0) } }, // L
            new Vector3[][] { new Vector3[] { new Vector3(-1.5f, -2, 0), new Vector3(-1, 2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(0, 0, 0) }, new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(1.5f, -2, 0) } }, // M
            new Vector3[][] { new Vector3[] { new Vector3(-1, -2, 0), new Vector3(-1, 2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, -2, 0) }, new Vector3[] { new Vector3(1, -2, 0), new Vector3(1, 2, 0) } }, // N
            new Vector3[][] { new Vector3[] { new Vector3(0, 2.5f, 0), new Vector3(-1, 1.5f, 0), new Vector3(-1, -1.5f, 0), new Vector3(0, -2.5f, 0), new Vector3(1, -1.5f, 0), new Vector3(1, 1.5f, 0), new Vector3(0, 2.5f, 0) } }, // O
            new Vector3[][] { new Vector3[] { new Vector3(-1, -2, 0), new Vector3(-1, 2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(1.5f, 1, 0), new Vector3(1, 0, 0), new Vector3(-1, 0, 0) } }, // P
            new Vector3[][] { new Vector3[] { new Vector3(0, 2, 0), new Vector3(-1, 0, 0), new Vector3(0, -2, 0), new Vector3(1, 0, 0), new Vector3(0, 2, 0) }, new Vector3[] { new Vector3(0, -1, 0), new Vector3(1, -2, 0) } }, // Q
            new Vector3[][] { new Vector3[] { new Vector3(-1, -2, 0), new Vector3(-1, 2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0), new Vector3(1.5f, 1, 0), new Vector3(1, 0, 0), new Vector3(-1, 0, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, -2, 0) } }, // R
            new Vector3[][] { new Vector3[] { new Vector3(1, 2, 0), new Vector3(0, 2.5f, 0), new Vector3(-1, 1.5f, 0), new Vector3(0, 0.5f, 0), new Vector3(1, -0.5f, 0), new Vector3(0, -2.5f, 0), new Vector3(-1, -2, 0) } }, // S
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(0, 2, 0), new Vector3(0, -2, 0) } }, // T
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -1.5f, 0), new Vector3(0, -2.5f, 0), new Vector3(1, -1.5f, 0), new Vector3(1, 2, 0) } }, // U
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(0, -2, 0) }, new Vector3[] { new Vector3(0, -2, 0), new Vector3(1, 2, 0) } }, // V
            new Vector3[][] { new Vector3[] { new Vector3(-1.5f, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, -2, 0), new Vector3(0, 0, 0) }, new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, -2, 0) }, new Vector3[] { new Vector3(1, -2, 0), new Vector3(1.5f, 2, 0) } }, // W
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, -2, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(-1, -2, 0) } }, // X
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(0, 0, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(0, 0, 0) }, new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, -2, 0) } }, // Y
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, -2, 0), new Vector3(1, -2, 0) } } // Z
        };

        for (int i = 0; i < 26; i++)
        {
            if (i >= alphabetPaths.Length) break;
            Vector3[][] letterPaths = alphabetPaths[i];

            //levelStrokesProp.InsertArrayElementAtIndex(i);
            //SerializedProperty letterProp = levelStrokesProp.GetArrayElementAtIndex(i);
            //SerializedProperty strokesProp = letterProp.FindPropertyRelative("Strokes");
            //strokesProp.ClearArray();

            char c = (char)('A' + i);
            for (int s = 0; s < letterPaths.Length; s++)
            {
                //strokesProp.InsertArrayElementAtIndex(s);
                //SerializedProperty strokeDataProp = strokesProp.GetArrayElementAtIndex(s);
                //SerializedProperty pathPointsProp = strokeDataProp.FindPropertyRelative("PathPoints");
                //pathPointsProp.ClearArray();
                
                string guidePath = $"Assets/Alphabet-Tracing-Assets-main/Sprites/alphabet/{c}/guide {s * 2 + 1}_{s * 2 + 2}.png";
                Sprite guideSprite = AssetDatabase.LoadAssetAtPath<Sprite>(guidePath);
                //SerializedProperty guideSpriteProp = strokeDataProp.FindPropertyRelative("GuideSprite");
                //if (guideSpriteProp != null)
                //{
                //    guideSpriteProp.objectReferenceValue = guideSprite;
                //}

                for (int p = 0; p < letterPaths[s].Length; p++)
                {
                    GameObject pointObj = new GameObject($"L{i}_S{s}_P{p}");
                    pointObj.transform.SetParent(container.transform);
                    pointObj.transform.position = letterPaths[s][p] * 1.8f; // Scale up slightly to match letter sizes

                    //pathPointsProp.InsertArrayElementAtIndex(p);
                    //pathPointsProp.GetArrayElementAtIndex(p).objectReferenceValue = pointObj.transform;
                }
            }
        }

        //if (uiController != null)
        //{
        //    SerializedObject soUI = new SerializedObject(uiController);
        //    SerializedProperty alphaSpritesProp = soUI.FindProperty("alphabetSprites");
        //    SerializedProperty alphaMasksProp = soUI.FindProperty("alphabetMaskSprites");
            
        //    alphaSpritesProp.ClearArray();
        //    alphaMasksProp.ClearArray();
            
        //    for (int i = 0; i < 26; i++)
        //    {
        //        char c = (char)('A' + i);
        //        string letterPath = $"Assets/Alphabet-Tracing-Assets-main/Sprites/alphabet/{c}.png";
        //        string maskPath1 = $"Assets/Alphabet-Tracing-Assets-main/Sprites/alphabet/{c}/{c}_hole.png";
        //        string maskPath2 = $"Assets/Alphabet-Tracing-Assets-main/Sprites/alphabet/{c}/{char.ToLower(c)}_hole.png";
                
        //        Sprite letterSprite = AssetDatabase.LoadAssetAtPath<Sprite>(letterPath);
        //        Sprite maskSprite = AssetDatabase.LoadAssetAtPath<Sprite>(maskPath1);
        //        if (maskSprite == null) maskSprite = AssetDatabase.LoadAssetAtPath<Sprite>(maskPath2);
                
        //        alphaSpritesProp.InsertArrayElementAtIndex(i);
        //        alphaSpritesProp.GetArrayElementAtIndex(i).objectReferenceValue = letterSprite;
                
        //        alphaMasksProp.InsertArrayElementAtIndex(i);
        //        alphaMasksProp.GetArrayElementAtIndex(i).objectReferenceValue = maskSprite;
        //    }
        //    soUI.ApplyModifiedProperties();
        //}

        //soGuide.ApplyModifiedProperties();
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        SessionState.SetBool("FillLetterWired_V7", true);
        Debug.Log("Successfully wired FillLetterStrokeGuide and injected new Alphabet-Tracing-Assets!");
    }
}
