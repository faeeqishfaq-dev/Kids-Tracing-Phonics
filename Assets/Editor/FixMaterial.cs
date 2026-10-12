//using UnityEngine;
//using UnityEditor;
//public class FixMaterial
//{
//    [MenuItem(""Tools/Fix Material"")]
//    public static void DoIt()
//    {
//        var mat = AssetDatabase.LoadAssetAtPath<Material>(""Assets/_Project/Sprites/BrushMaterial.mat"");
//        if (mat != null)
//        {
//            mat.shader = Shader.Find(""Sprites/Default"");
//            EditorUtility.SetDirty(mat);
//            AssetDatabase.SaveAssets();
//            Debug.Log(""Material fixed to Sprites/Default"");
//        }
//    }
//}
