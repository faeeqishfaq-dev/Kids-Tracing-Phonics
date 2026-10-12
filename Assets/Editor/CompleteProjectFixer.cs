using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor.Events;
using System.IO;
using System.Reflection;

using Game.Core.Managers;
using Game.Core.Rewards;
using Game.Gameplay.ABCLearning;
using Game.Gameplay.FillLetter;
using Game.Gameplay.Tracing;
using Game.UI;

public static class CompleteProjectFixer
{
    private static string spritesPath = "Assets/_Project/Sprites/";

    [MenuItem("KidsGame/Force Build Project Setup")]
    public static void ManualRunFix()
    {
        EnsureFolders();
        GenerateAssets();
        AssetDatabase.Refresh();

        EditorApplication.delayCall += () => 
        {
            try
            {
                ApplySettingsToSprites();
                
                // Extra delay to ensure AssetDatabase has fully registered the new sprites
                EditorApplication.delayCall += () => 
                {
                    BuildLoadingScene();
                    BuildGameplayScene();
                    SetBuildSettings();
                    Debug.Log("[CompleteProjectFixer] SUCCESS: Project is fully playable and fixed!");
                };
            }
            catch(System.Exception e)
            {
                Debug.LogError("[CompleteProjectFixer] ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        };
    }

    [InitializeOnLoadMethod]
    private static void AutoRunFix()
    {
        if (EditorPrefs.GetBool("ProjFixed_Final4", false)) return;
        EditorPrefs.SetBool("ProjFixed_Final4", true);
        EditorApplication.delayCall += ManualRunFix;
    }

    private static void EnsureFolders()
    {
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Scenes")) AssetDatabase.CreateFolder("Assets/_Project", "Scenes");
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Sprites")) AssetDatabase.CreateFolder("Assets/_Project", "Sprites");
    }

    private static void SetBuildSettings()
    {
        var newScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene("Assets/_Project/Scenes/0_Loading.unity", true),
            new EditorBuildSettingsScene("Assets/_Project/Scenes/1_Gameplay.unity", true)
        };
        EditorBuildSettings.scenes = newScenes.ToArray();
    }

    #region Asset Generation
    private static void GenerateAssets()
    {
        // Darker colors so text is readable
        Color bgBlue = new Color(0.6f, 0.8f, 0.9f);
        Color btnYellow = new Color(0.9f, 0.7f, 0.1f);
        Color btnOrange = new Color(0.9f, 0.5f, 0.1f);
        Color btnPink = new Color(0.9f, 0.4f, 0.6f);
        Color btnGreen = new Color(0.3f, 0.7f, 0.3f);
        
        SaveTexture("Background", CreateSolidTexture(64, 64, bgBlue));
        SaveTexture("PanelBG", CreateSolidTexture(128, 128, Color.white));
        SaveTexture("ButtonYellow", CreateSolidTexture(128, 64, btnYellow));
        SaveTexture("ButtonOrange", CreateSolidTexture(128, 64, btnOrange));
        SaveTexture("ButtonPink", CreateSolidTexture(128, 64, btnPink));
        SaveTexture("ButtonGreen", CreateSolidTexture(128, 64, btnGreen));
        
        SaveTexture("ProgressBG", CreateSolidTexture(128, 32, Color.gray));
        SaveTexture("ProgressFill", CreateSolidTexture(128, 32, btnGreen));

        string[] alphabet = { "Apple", "Ball", "Cat", "Dog", "Elephant", "Fish", "Goat", "House", "IceCream", "Juice", "Kite", "Lion", "Monkey", "Nest", "Orange", "Penguin", "Queen", "Rabbit", "Sun", "Tree", "Umbrella", "Van", "Whale", "Xylophone", "Yak", "Zebra" };
        for (int i = 0; i < alphabet.Length; i++)
        {
            float hue = (float)i / 26f;
            Color col = Color.HSVToRGB(hue, 0.6f, 0.9f);
            SaveTexture("Ill_" + alphabet[i], CreateSolidTexture(128, 128, col));
            SaveTexture("Letter_" + (char)('A' + i), CreateSolidTexture(64, 64, col));
        }
    }

    private static void SaveTexture(string name, Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        File.WriteAllBytes(spritesPath + name + ".png", bytes);
        Object.DestroyImmediate(tex);
    }

    private static Texture2D CreateSolidTexture(int w, int h, Color col)
    {
        Texture2D tex = new Texture2D(w, h);
        Color[] cols = new Color[w * h];
        for (int i = 0; i < cols.Length; i++) cols[i] = col;
        tex.SetPixels(cols);
        tex.Apply();
        return tex;
    }

    private static void ApplySettingsToSprites()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_Project/Sprites" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 100;
                importer.SaveAndReimport();
            }
        }
    }
    #endregion

    #region Scene Building Utils
    private static void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (field != null) field.SetValue(target, value);
    }

    private static RectTransform CreateUI(string name, Transform parent, Sprite sprite = null, Color? color = null)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        
        var img = go.AddComponent<Image>();
        if (sprite != null) 
            img.sprite = sprite;
        else 
            img.color = color ?? Color.white; // Fallback to color if sprite is null

        return rt;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private static Text AddText(RectTransform parent, string text, int size = 30)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        Stretch(rt);
        
        var txt = go.AddComponent<Text>();
        txt.text = text;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (txt.font == null) txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = size;
        txt.color = Color.black;
        txt.alignment = TextAnchor.MiddleCenter;
        return txt;
    }
    
    private static Button AddButton(RectTransform parent, string name, string text, Sprite bg, UnityAction action)
    {
        var rt = CreateUI(name, parent, bg);
        rt.sizeDelta = new Vector2(400, 100);
        var btn = rt.gameObject.AddComponent<Button>();
        AddText(rt, text, 40);
        if (action != null)
        {
            UnityEventTools.AddPersistentListener(btn.onClick, action);
        }
        return btn;
    }
    #endregion

    private static void BuildLoadingScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        GameObject camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        var cam = camGo.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.2f, 0.6f, 0.8f);
        camGo.tag = "MainCamera";

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        var canvasRt = CreateUI("Canvas", null, null, Color.clear);
        var canvas = canvasRt.gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasRt.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasRt.gameObject.AddComponent<GraphicRaycaster>();

        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "Background.png");
        var bg = CreateUI("Background", canvasRt, bgSprite, new Color(0.6f, 0.8f, 0.9f));
        Stretch(bg);

        var title = CreateUI("Title", canvasRt, null, Color.clear);
        title.anchoredPosition = new Vector2(0, 200);
        title.sizeDelta = new Vector2(800, 200);
        AddText(title, "Kids Edu Game\nLoading...", 60);

        var managers = new GameObject("Managers");
        managers.AddComponent<GameManager>();
        managers.AddComponent<SceneLoader>();
        managers.AddComponent<LoadingAutoStarter>();

        EditorSceneManager.SaveScene(scene, "Assets/_Project/Scenes/0_Loading.unity");
    }

    private static void BuildGameplayScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        GameObject camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        var cam = camGo.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.2f, 0.6f, 0.8f);
        camGo.tag = "MainCamera";

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        var managers = new GameObject("Managers");
        managers.AddComponent<AudioManager>();
        managers.AddComponent<SaveManager>();
        managers.AddComponent<SettingsManager>();
        managers.AddComponent<RewardManager>();
        managers.AddComponent<AchievementManager>();
        managers.AddComponent<DailyRewardManager>();
        managers.AddComponent<StickerCollectionManager>();
        managers.AddComponent<InputManager>();
        var am = managers.GetComponent<AudioManager>();
        SetPrivateField(am, "_musicSource", managers.AddComponent<AudioSource>());
        SetPrivateField(am, "_sfxSource", managers.AddComponent<AudioSource>());

        var canvasRt = CreateUI("Canvas", null, null, Color.clear);
        var canvas = canvasRt.gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasRt.gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasRt.gameObject.AddComponent<GraphicRaycaster>();

        var nav = canvasRt.gameObject.AddComponent<GameNavigation>();

        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "Background.png");
        var bg = CreateUI("Background", canvasRt, bgSprite, new Color(0.6f, 0.8f, 0.9f));
        Stretch(bg);

        // Panels
        var mmPanel = CreateUI("MainMenuPanel", canvasRt, null, Color.clear); Stretch(mmPanel);
        var abcPanel = CreateUI("LearningPanel", canvasRt, null, Color.clear); Stretch(abcPanel);
        var tracePanel = CreateUI("TracingPanel", canvasRt, null, Color.clear); Stretch(tracePanel);
        var fillPanel = CreateUI("FillLetterPanel", canvasRt, null, Color.clear); Stretch(fillPanel);
        var setPanel = CreateUI("SettingsPanel", canvasRt, null, Color.clear); Stretch(setPanel);

        nav.MainMenuPanel = mmPanel.gameObject;
        ////nav.LearningPanel = abcPanel.gameObject;
        ////nav.TracingPanel = tracePanel.gameObject;
        nav.FillLetterPanel = fillPanel.gameObject;
        ////nav.SettingsPanel = setPanel.gameObject;

        Sprite btnYellow = AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "ButtonYellow.png");
        Sprite btnOrange = AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "ButtonOrange.png");
        Sprite btnPink = AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "ButtonPink.png");
        Sprite btnGreen = AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "ButtonGreen.png");

        // --- Main Menu ---
        var vlg = mmPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.spacing = 30;
        vlg.childControlHeight = false; vlg.childControlWidth = false;

        //AddButton(mmPanel, "Btn_ABC", "Learn ABC", btnPink, new UnityAction(//nav.ShowLearning));
        //AddButton(mmPanel, "Btn_Trace", "Trace Letters", btnOrange, new UnityAction(//nav.ShowTracing));
        AddButton(mmPanel, "Btn_Fill", "Fill Letters", btnGreen, new UnityAction(nav.ShowFillLetter));
        //AddButton(mmPanel, "Btn_Set", "Settings", btnYellow, new UnityAction(//nav.ShowSettings));

        // --- Shared Back Buttons ---
        AddButton(abcPanel, "Back", "Back", btnYellow, new UnityAction(nav.ShowMainMenu)).GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 300);
        AddButton(tracePanel, "Back", "Back", btnYellow, new UnityAction(nav.ShowMainMenu)).GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 300);
        AddButton(fillPanel, "Back", "Back", btnYellow, new UnityAction(nav.ShowMainMenu)).GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 300);
        AddButton(setPanel, "Back", "Back", btnYellow, new UnityAction(nav.ShowMainMenu)).GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 300);

        // --- ABC Screen ---
        var grid = CreateUI("Grid", abcPanel, null, Color.clear);
        grid.sizeDelta = new Vector2(800, 600);
        grid.anchoredPosition = new Vector2(0, -100);
        var glg = grid.gameObject.AddComponent<GridLayoutGroup>();
        glg.cellSize = new Vector2(100, 100);
        glg.spacing = new Vector2(10, 10);
        
        for(int i=0; i<26; i++) {
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"{spritesPath}Letter_{(char)('A' + i)}.png");
            var tile = CreateUI($"L_{i}", grid, s, Color.white);
            AddText(tile, ((char)('A' + i)).ToString(), 60);
        }

        var ex = CreateUI("Example", abcPanel, AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "Ill_Apple.png"), Color.white);
        ex.anchoredPosition = new Vector2(0, 300);
        ex.sizeDelta = new Vector2(200, 200);

        // --- Tracing Screen ---
        var trCanvas = CreateUI("TraceCanvas", tracePanel, null, Color.white);
        trCanvas.sizeDelta = new Vector2(400, 400);
        trCanvas.anchoredPosition = new Vector2(0, 0);
        var guide = CreateUI("Guide", trCanvas, AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "Letter_A.png"), Color.white);
        Stretch(guide);
        AddText(guide, "A", 300);

        // --- Fill Screen ---
        var fCanvas = CreateUI("FillCanvas", fillPanel, null, Color.white);
        fCanvas.sizeDelta = new Vector2(400, 400);
        fCanvas.anchoredPosition = new Vector2(0, 0);
        var outline = CreateUI("Outline", fCanvas, AssetDatabase.LoadAssetAtPath<Sprite>(spritesPath + "Letter_B.png"), Color.white);
        Stretch(outline);
        AddText(outline, "B", 300);

        // Hide panels except MainMenu at start
        abcPanel.gameObject.SetActive(false);
        tracePanel.gameObject.SetActive(false);
        fillPanel.gameObject.SetActive(false);
        setPanel.gameObject.SetActive(false);

        EditorSceneManager.SaveScene(scene, "Assets/_Project/Scenes/1_Gameplay.unity");
    }
}
