using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using Game.Gameplay.FillLetter;
using Game.Gameplay.Tracing;

public class LetterPathsGenerator
{
    [MenuItem("Tools/Generate Letter Paths")]
    public static void GeneratePaths()
    {
        string scenePath = "Assets/_Project/Scenes/1_Gameplay.unity";
        var scene = EditorSceneManager.OpenScene(scenePath);

        FillLetterManager fillManager = GameObject.FindObjectOfType<FillLetterManager>(true);
        if (fillManager == null) return;

        //FillLetterStrokeGuide guide = fillManager.GetComponent<FillLetterStrokeGuide>();
        //if (guide == null) return;

        //SerializedObject soGuide = new SerializedObject(guide);
        //SerializedProperty levelStrokesProp = soGuide.FindProperty("LevelLettersStrokes");
        //levelStrokesProp.ClearArray();

        GameObject container = GameObject.Find("StrokePointsContainer");
        if (container != null) GameObject.DestroyImmediate(container);
        container = new GameObject("StrokePointsContainer");

        // Define basic paths for A-Z
        Vector3[][][] alphabetPaths = new Vector3[][][] {
            new Vector3[][] { new Vector3[] { new Vector3(0, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(0, 2, 0), new Vector3(1, -2, 0) }, new Vector3[] { new Vector3(-0.5f, 0, 0), new Vector3(0.5f, 0, 0) } }, // A
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 1, 0), new Vector3(-1, 0, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, -1, 0), new Vector3(-1, -2, 0) } }, // B
            new Vector3[][] { new Vector3[] { new Vector3(1, 2, 0), new Vector3(-1, 2, 0), new Vector3(-1, -2, 0), new Vector3(1, -2, 0) } }, // C
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 0, 0), new Vector3(-1, -2, 0) } }, // D
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0) }, new Vector3[] { new Vector3(-1, -2, 0), new Vector3(1, -2, 0) } }, // E
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0) } }, // F
            new Vector3[][] { new Vector3[] { new Vector3(1, 2, 0), new Vector3(-1, 2, 0), new Vector3(-1, -2, 0), new Vector3(1, -2, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 0) } }, // G
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(1, -2, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, 0, 0) } }, // H
            new Vector3[][] { new Vector3[] { new Vector3(0, 2, 0), new Vector3(0, -2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(-1, -2, 0), new Vector3(1, -2, 0) } }, // I
            new Vector3[][] { new Vector3[] { new Vector3(1, 2, 0), new Vector3(1, -2, 0), new Vector3(-1, -2, 0), new Vector3(-1, 0, 0) } }, // J
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(-1, 0, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, -2, 0) } }, // K
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0) }, new Vector3[] { new Vector3(-1, -2, 0), new Vector3(1, -2, 0) } }, // L
            new Vector3[][] { new Vector3[] { new Vector3(-1, -2, 0), new Vector3(-1, 2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(0, 0, 0) }, new Vector3[] { new Vector3(0, 0, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(1, 2, 0), new Vector3(1, -2, 0) } }, // M
            new Vector3[][] { new Vector3[] { new Vector3(-1, -2, 0), new Vector3(-1, 2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, -2, 0) }, new Vector3[] { new Vector3(1, -2, 0), new Vector3(1, 2, 0) } }, // N
            new Vector3[][] { new Vector3[] { new Vector3(0, 2, 0), new Vector3(-1, 0, 0), new Vector3(0, -2, 0), new Vector3(1, 0, 0), new Vector3(0, 2, 0) } }, // O
            new Vector3[][] { new Vector3[] { new Vector3(-1, -2, 0), new Vector3(-1, 2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 1, 0), new Vector3(-1, 0, 0) } }, // P
            new Vector3[][] { new Vector3[] { new Vector3(0, 2, 0), new Vector3(-1, 0, 0), new Vector3(0, -2, 0), new Vector3(1, 0, 0), new Vector3(0, 2, 0) }, new Vector3[] { new Vector3(0, -1, 0), new Vector3(1, -2, 0) } }, // Q
            new Vector3[][] { new Vector3[] { new Vector3(-1, -2, 0), new Vector3(-1, 2, 0) }, new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 1, 0), new Vector3(-1, 0, 0) }, new Vector3[] { new Vector3(-1, 0, 0), new Vector3(1, -2, 0) } }, // R
            new Vector3[][] { new Vector3[] { new Vector3(1, 2, 0), new Vector3(-1, 2, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0), new Vector3(1, -2, 0), new Vector3(-1, -2, 0) } }, // S
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(1, 2, 0) }, new Vector3[] { new Vector3(0, 2, 0), new Vector3(0, -2, 0) } }, // T
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-1, -2, 0), new Vector3(1, -2, 0), new Vector3(1, 2, 0) } }, // U
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(0, -2, 0) }, new Vector3[] { new Vector3(0, -2, 0), new Vector3(1, 2, 0) } }, // V
            new Vector3[][] { new Vector3[] { new Vector3(-1, 2, 0), new Vector3(-0.5f, -2, 0) }, new Vector3[] { new Vector3(-0.5f, -2, 0), new Vector3(0, 0, 0) }, new Vector3[] { new Vector3(0, 0, 0), new Vector3(0.5f, -2, 0) }, new Vector3[] { new Vector3(0.5f, -2, 0), new Vector3(1, 2, 0) } }, // W
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

            for (int s = 0; s < letterPaths.Length; s++)
            {
                //strokesProp.InsertArrayElementAtIndex(s);
                //SerializedProperty strokeDataProp = strokesProp.GetArrayElementAtIndex(s);
                //SerializedProperty pathPointsProp = strokeDataProp.FindPropertyRelative("PathPoints");
                //pathPointsProp.ClearArray();

                for (int p = 0; p < letterPaths[s].Length; p++)
                {
                    GameObject pointObj = new GameObject($"L{i}_S{s}_P{p}");
                    pointObj.transform.SetParent(container.transform);
                    // Standard scale modification to better match the screen
                    pointObj.transform.position = letterPaths[s][p] * 1.5f;

                    //pathPointsProp.InsertArrayElementAtIndex(p);
                    //pathPointsProp.GetArrayElementAtIndex(p).objectReferenceValue = pointObj.transform;
                }
            }
        }

        //soGuide.ApplyModifiedProperties();
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Generated A-Z paths for FillLetterStrokeGuide!");
    }
}
