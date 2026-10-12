//using UnityEngine;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEngine.SceneManagement;
//using System.IO;

//public class DumpHierarchy
//{
//    [MenuItem(""Tools/Dump Hierarchy"")]
//    public static void Dump()
//    {
//        try 
//        {
//            Scene scene = EditorSceneManager.OpenScene(""Assets/_Project/Scenes/1_Gameplay.unity"");
//            string output = ""--- HIERARCHY ---\n"";
//            foreach (GameObject root in scene.GetRootGameObjects())
//            {
//                output += DumpObject(root, """");
//            }
//            File.WriteAllText(Application.dataPath + ""/../hierarchy_dump.txt"", output);
//            Debug.Log(""Successfully dumped hierarchy."");
//        }
//        catch (System.Exception e)
//        {
//            File.WriteAllText(Application.dataPath + ""/../hierarchy_dump.txt"", ""ERROR: "" + e.Message);
//        }
//        EditorApplication.Exit(0);
//    }

//    private static string DumpObject(GameObject go, string indent)
//    {
//        string output = indent + go.name + ""\n"";
//        foreach (Transform child in go.transform)
//        {
//            output += DumpObject(child.gameObject, indent + ""  "");
//        }
//        return output;
//    }
//}
