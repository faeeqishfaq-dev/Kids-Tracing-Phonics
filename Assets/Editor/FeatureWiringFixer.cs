using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor.Events;
using System.Reflection;

using Game.Core.Managers;
using Game.Gameplay.FillLetter;
using Game.UI;

public static class FeatureWiringFixer
{
    [MenuItem("KidsGame/Wire Final Gameplay Features")]
    public static void ManualRunFix()
    {
        EditorApplication.delayCall += () => 
        {
            try
            {
                WireLoadingScene();
                WireGameplayScene();
                Debug.Log("[FeatureWiringFixer] SUCCESS: Loading and Gameplay features are fully wired!");
            }
            catch(System.Exception e)
            {
                Debug.LogError("[FeatureWiringFixer] ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        };
    }

    [InitializeOnLoadMethod]
    private static void AutoRunFix()
    {
        if (EditorPrefs.GetBool("FeatWired_Final1", false)) return;
        EditorPrefs.SetBool("FeatWired_Final1", true);
        EditorApplication.delayCall += ManualRunFix;
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (field != null) field.SetValue(target, value);
    }

    private static Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name) return parent;
        for (int i = 0; i < parent.childCount; i++)
        {
            var found = FindChildRecursive(parent.GetChild(i), name);
            if (found != null) return found;
        }
        return null;
    }

    private static Material GetOrCreateBrushMaterial()
    {
        string matPath = "Assets/_Project/Sprites/BrushMaterial.mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Sprites/Default"));
            AssetDatabase.CreateAsset(mat, matPath);
        }
        return mat;
    }

    private static void WireLoadingScene()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/0_Loading.unity");
        
        GameObject managers = GameObject.Find("Managers");
        if (managers != null)
        {
            var autoStarter = managers.GetComponent<LoadingAutoStarter>();
            if (autoStarter != null) Object.DestroyImmediate(autoStarter);

            var animator = managers.GetComponent<AnimatedLoadingScreen>();
            if (animator == null) animator = managers.AddComponent<AnimatedLoadingScreen>();

            Transform canvas = GameObject.Find("Canvas")?.transform;
            if (canvas != null)
            {
                var title = FindChildRecursive(canvas, "Title");
                if (title != null) animator.loadingText = title.GetComponent<Text>();

                var fillAreaParent = new GameObject("ProgressBG", typeof(RectTransform));
                fillAreaParent.transform.SetParent(canvas, false);
                var fapRt = fillAreaParent.GetComponent<RectTransform>();
                fapRt.anchoredPosition = new Vector2(0, -200);
                fapRt.sizeDelta = new Vector2(600, 40);
                var fapImg = fillAreaParent.AddComponent<Image>();
                fapImg.color = Color.gray;

                var fillRect = new GameObject("ProgressFill", typeof(RectTransform));
                fillRect.transform.SetParent(fillAreaParent.transform, false);
                var fillRt = fillRect.GetComponent<RectTransform>();
                fillRt.anchorMin = new Vector2(0, 0);
                fillRt.anchorMax = new Vector2(0, 1);
                fillRt.sizeDelta = new Vector2(0, 0);
                fillRt.anchoredPosition = new Vector2(0, 0);
                
                var fillImg = fillRect.AddComponent<Image>();
                fillImg.color = new Color(0.3f, 0.7f, 0.3f); // Green

                animator.fillRect = fillRt;
            }
        }
        EditorSceneManager.SaveScene(scene);
    }

    private static void WireGameplayScene()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/1_Gameplay.unity");

        var canvasGo = GameObject.Find("Canvas");
        if (canvasGo != null)
        {
            var canvas = canvasGo.GetComponent<Canvas>();
            if (canvas.renderMode != RenderMode.ScreenSpaceCamera)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                canvas.planeDistance = 10f; 
            }
            
            // ================= FILL LETTER WIRING =================
            var fillPanel = FindChildRecursive(canvasGo.transform, "FillLetterPanel");
            if (fillPanel != null)
            {
                // Clean up old wiring
                var oldActions = FindChildRecursive(fillPanel, "ActionButtons");
                if (oldActions != null) Object.DestroyImmediate(oldActions.gameObject);
                
                var oldBack = FindChildRecursive(fillPanel, "Back");
                if (oldBack != null) oldBack.gameObject.SetActive(false); // Hide global back button, we'll put it in SelectionMenu

                var fillCanvas = FindChildRecursive(fillPanel, "FillCanvas");
                
                // Create Selection Menu
                var selectionMenu = FindChildRecursive(fillPanel, "SelectionMenu");
                if (selectionMenu == null)
                {
                    selectionMenu = new GameObject("SelectionMenu", typeof(RectTransform)).transform;
                    selectionMenu.SetParent(fillPanel, false);
                    var smRt = selectionMenu.GetComponent<RectTransform>();
                    smRt.anchorMin = Vector2.zero; smRt.anchorMax = Vector2.one;
                    smRt.sizeDelta = Vector2.zero; smRt.anchoredPosition = Vector2.zero;

                    var grid = new GameObject("Grid", typeof(RectTransform));
                    grid.transform.SetParent(selectionMenu, false);
                    var gRt = grid.GetComponent<RectTransform>();
                    gRt.sizeDelta = new Vector2(800, 600);
                    gRt.anchoredPosition = new Vector2(0, -100);
                    var glg = grid.AddComponent<GridLayoutGroup>();
                    glg.cellSize = new Vector2(100, 100);
                    glg.spacing = new Vector2(10, 10);
                }

                // Attach Controller
                var controller = fillPanel.gameObject.GetComponent<FillLetterUIController>();
                if (controller == null) controller = fillPanel.gameObject.AddComponent<FillLetterUIController>();

                // Configure Painting logic
                var paintValidator = fillCanvas.gameObject.GetComponent<PaintValidator>();
                if (paintValidator == null) paintValidator = fillCanvas.gameObject.AddComponent<PaintValidator>();

                var brushPainter = fillCanvas.gameObject.GetComponent<BrushPainter>();
                if (brushPainter == null) brushPainter = fillCanvas.gameObject.AddComponent<BrushPainter>();

                var fillManager = fillCanvas.gameObject.GetComponent<FillLetterManager>();
                if (fillManager == null) fillManager = fillCanvas.gameObject.AddComponent<FillLetterManager>();

                var outline = FindChildRecursive(fillCanvas, "Outline");
                if (outline != null)
                {
                    var mask = outline.gameObject.GetComponent<SpriteMask>();
                    if (mask == null) mask = outline.gameObject.AddComponent<SpriteMask>();
                    
                    var img = outline.GetComponent<Image>();
                    if (img != null && img.sprite != null) mask.sprite = img.sprite;
                    
                    controller.outlineImage = img;
                    controller.outlineMask = mask;
                }

                SetPrivateField(fillManager, "_brushPainter", brushPainter);
                SetPrivateField(fillManager, "_paintValidator", paintValidator);
                SetPrivateField(brushPainter, "_brushMaterial", GetOrCreateBrushMaterial());
                SetPrivateField(brushPainter, "_strokesContainer", fillCanvas);

                controller.selectionMenu = selectionMenu.gameObject.GetComponent<RectTransform>();
                controller.paintingCanvas = fillCanvas.gameObject;
                controller.brushPainter = brushPainter;
                controller.fillManager = fillManager;

                // Action Buttons for Paint Screen
                var actionPanel = new GameObject("ActionButtons", typeof(RectTransform));
                actionPanel.transform.SetParent(fillCanvas, false);
                var actRt = actionPanel.GetComponent<RectTransform>();
                actRt.anchoredPosition = new Vector2(0, -350);
                actRt.sizeDelta = new Vector2(800, 100);
                var hlg = actionPanel.AddComponent<HorizontalLayoutGroup>();
                hlg.childAlignment = TextAnchor.MiddleCenter;
                hlg.spacing = 20;
                hlg.childControlHeight = false; hlg.childControlWidth = false;

                AddActionButton(actionPanel.transform, "Back to Menu", new UnityAction(controller.ReturnToMenu));
                AddActionButton(actionPanel.transform, "Clear", new UnityAction(brushPainter.ClearAll));
                AddActionButton(actionPanel.transform, "Undo", new UnityAction(brushPainter.Undo));
                AddActionButton(actionPanel.transform, "Redo", new UnityAction(brushPainter.Redo));

                // Global back to Main Menu on Selection Screen
                var nav = canvasGo.GetComponent<GameNavigation>();
                var backMain = new GameObject("BackMain", typeof(RectTransform));
                backMain.transform.SetParent(selectionMenu, false);
                var bmRt = backMain.GetComponent<RectTransform>();
                bmRt.anchoredPosition = new Vector2(-400, 300);
                var bmImg = backMain.AddComponent<Image>();
                bmImg.color = new Color(0.9f, 0.7f, 0.1f);
                var bmBtn = backMain.AddComponent<Button>();
                UnityEventTools.AddPersistentListener(bmBtn.onClick, new UnityAction(nav.ShowMainMenu));

                // Generate A-Z Buttons dynamically in the Grid
                var gridTransform = FindChildRecursive(selectionMenu, "Grid");
                if (gridTransform != null)
                {
                    // clear old
                    for (int i = gridTransform.childCount - 1; i >= 0; i--) Object.DestroyImmediate(gridTransform.GetChild(i).gameObject);
                    
                    for(int i=0; i<26; i++) {
                        string letterName = ((char)('A' + i)).ToString();
                        Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/_Project/Sprites/Letter_{letterName}.png");
                        
                        var tileGo = new GameObject($"L_{i}", typeof(RectTransform));
                        tileGo.transform.SetParent(gridTransform, false);
                        var tileImg = tileGo.AddComponent<Image>();
                        tileImg.sprite = s;
                        var tileBtn = tileGo.AddComponent<Button>();
                        
                        var txtGo = new GameObject("Text", typeof(RectTransform));
                        txtGo.transform.SetParent(tileGo.transform, false);
                        var txtRt = txtGo.GetComponent<RectTransform>();
                        txtRt.anchorMin = Vector2.zero; txtRt.anchorMax = Vector2.one;
                        txtRt.sizeDelta = Vector2.zero; txtRt.anchoredPosition = Vector2.zero;
                        var txt = txtGo.AddComponent<Text>();
                        txt.text = letterName;
                        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                        txt.fontSize = 60;
                        txt.color = Color.black;
                        txt.alignment = TextAnchor.MiddleCenter;

                        // Register persistent listener safely
                        var action = new UnityAction<int>(controller.SelectLetter);
                        UnityEventTools.AddIntPersistentListener(tileBtn.onClick, action, i);
                    }
                }

                // Initial State
                if (controller.selectionMenu != null) controller.selectionMenu.gameObject.SetActive(true);
                if (controller.paintingCanvas != null) controller.paintingCanvas.SetActive(false);
            }
        }

        EditorSceneManager.SaveScene(scene);
    }

    private static void AddActionButton(Transform parent, string name, UnityAction action)
    {
        var btnGo = new GameObject(name, typeof(RectTransform));
        btnGo.transform.SetParent(parent, false);
        var rt = btnGo.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(150, 80);
        var img = btnGo.AddComponent<Image>();
        img.color = new Color(0.9f, 0.7f, 0.1f);
        var btn = btnGo.AddComponent<Button>();
        
        var txtGo = new GameObject("Text", typeof(RectTransform));
        txtGo.transform.SetParent(btnGo.transform, false);
        var txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero; txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero; txtRt.anchoredPosition = Vector2.zero;
        
        var txt = txtGo.AddComponent<Text>();
        txt.text = name;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (txt.font == null) txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 24;
        txt.color = Color.black;
        txt.alignment = TextAnchor.MiddleCenter;
        
        UnityEventTools.AddPersistentListener(btn.onClick, action);
    }
}
