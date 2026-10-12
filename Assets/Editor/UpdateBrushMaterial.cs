//using UnityEngine;
//using UnityEditor;

//public class UpdateBrushMaterial
//{
//    [MenuItem(""Tools/Update Brush Material"")]
//    public static void DoIt()
//    {
//        var mat = AssetDatabase.LoadAssetAtPath<Material>(""Assets/_Project/Sprites/BrushMaterial.mat"");
//        var shader = Shader.Find(""Custom/LineMasked"");
//        if (shader != null && mat != null)
//        {
//            mat.shader = shader;
//            mat.SetFloat(""_StencilComp"", 6); // NotEqual
//            mat.SetFloat(""_Stencil"", 1);     // Ref 1
//            EditorUtility.SetDirty(mat);
//            AssetDatabase.SaveAssets();
//            Debug.Log(""Material updated!"");
//        }
//        else
//        {
//            Debug.LogError($""Failed to find shader or mat. shader={shader}, mat={mat}"");
//        }
//    }
//}
