using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor.Events;

using Game.Core.Managers;
using Game.UI;

public static class MasterArchitectureFixer
{
    [MenuItem("KidsGame/Wire Phase 1 Master Architecture")]
    public static void ManualRunFix()
    {
        EditorApplication.delayCall += () => 
        {
            try
            {
                WirePhase1UI();
                Debug.Log("[MasterArchitectureFixer] SUCCESS: Phase 1 Master Architecture wired!");
            }
            catch(System.Exception e)
            {
                Debug.LogError("[MasterArchitectureFixer] ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        };
    }

    [InitializeOnLoadMethod]
    private static void AutoRunFix()
    {
        if (EditorPrefs.GetBool("Phase1_Master1", false)) return;
        EditorPrefs.SetBool("Phase1_Master1", true);
        EditorApplication.delayCall += ManualRunFix;
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

    private static RectTransform CreateUI(string name, Transform parent, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        if (color != Color.clear)
        {
            var img = go.AddComponent<Image>();
            img.color = color;
        }
        return rt;
    }

    private static Button AddButton(Transform parent, string name, Color color, UnityAction action)
    {
        var rt = CreateUI(name, parent, color);
        rt.sizeDelta = new Vector2(300, 80);
        var btn = rt.gameObject.AddComponent<Button>();
        
        var txtGo = new GameObject("Text", typeof(RectTransform));
        txtGo.transform.SetParent(rt, false);
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
        
        if (action != null) UnityEventTools.AddPersistentListener(btn.onClick, action);
        return btn;
    }

    private static void WirePhase1UI()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/1_Gameplay.unity");
        var canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null) return;
        
        var nav = canvasGo.GetComponent<GameNavigation>();
        Color btnBlue = new Color(0.2f, 0.6f, 0.9f);
        Color btnYellow = new Color(0.9f, 0.7f, 0.1f);

        // 1. MAIN MENU EXPANSION
        var mmPanel = FindChildRecursive(canvasGo.transform, "MainMenuPanel");
        if (mmPanel != null)
        {
            // The MainMenuPanel has a VerticalLayoutGroup. Add missing buttons.
            //AddButton(mmPanel, "Mini Games", btnBlue, new UnityAction(nav.ShowMiniGames)).transform.SetSiblingIndex(3);
            
            // Add a horizontal group for the utility buttons at the bottom
            var utilPanel = CreateUI("Utilities", mmPanel, Color.clear);
            utilPanel.sizeDelta = new Vector2(800, 100);
            var hlg = utilPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.spacing = 20; hlg.childControlHeight = false; hlg.childControlWidth = false;
            
            AddButton(utilPanel, "Parent Lock", Color.gray, null);
            AddButton(utilPanel, "Privacy Policy", Color.gray, null);
            AddButton(utilPanel, "More Apps", Color.gray, null);
        }

        // 2. SETTINGS PANEL EXPANSION
        var setPanel = FindChildRecursive(canvasGo.transform, "SettingsPanel");
        if (setPanel != null)
        {
            var vlg = setPanel.gameObject.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
            {
                vlg = setPanel.gameObject.AddComponent<VerticalLayoutGroup>();
                vlg.childAlignment = TextAnchor.MiddleCenter;
                vlg.spacing = 30; vlg.childControlHeight = false; vlg.childControlWidth = false;
            }
            AddButton(setPanel, "Toggle Music", btnBlue, null);
            AddButton(setPanel, "Toggle Sound", btnBlue, null);
            // Push back button to bottom
            var backBtn = FindChildRecursive(setPanel, "Back");
            if (backBtn != null) backBtn.SetAsLastSibling();
        }

        // 3. MINI GAMES MENU
        var miniGamesPanel = FindChildRecursive(canvasGo.transform, "MiniGamesPanel");
        if (miniGamesPanel == null)
        {
            miniGamesPanel = CreateUI("MiniGamesPanel", canvasGo.transform, Color.clear);
            var mgRt = miniGamesPanel.GetComponent<RectTransform>();
            if (mgRt == null) mgRt = miniGamesPanel.gameObject.AddComponent<RectTransform>();
            mgRt.anchorMin = Vector2.zero; mgRt.anchorMax = Vector2.one;
            mgRt.sizeDelta = Vector2.zero; mgRt.anchoredPosition = Vector2.zero;
            
            var grid = CreateUI("Grid", miniGamesPanel, Color.clear);
            grid.sizeDelta = new Vector2(900, 600);
            grid.anchoredPosition = new Vector2(0, -50);
            var glg = grid.gameObject.AddComponent<GridLayoutGroup>();
            glg.cellSize = new Vector2(160, 160);
            glg.spacing = new Vector2(20, 20);

            string[] mgNames = { "Balloon Pop", "Letter Match", "Upper/Lower Match", "Missing Letter", "Find Correct", "Listen & Choose", "Word Builder", "Alphabet Puzzle", "Letter Coloring", "Memory Cards" };
            foreach (var mg in mgNames)
            {
                var btn = AddButton(grid, mg, btnYellow, null);
                btn.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 160); // Override for grid
            }

            var backBtn = AddButton(miniGamesPanel, "Back", btnYellow, new UnityAction(nav.ShowMainMenu));
            backBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-400, 300);

            //nav.MiniGamesPanel = miniGamesPanel.gameObject;
            miniGamesPanel.gameObject.SetActive(false);
        }

        // 4. LEARNING SYSTEM EXPANSION
        var learnPanel = FindChildRecursive(canvasGo.transform, "LearningPanel");
        if (learnPanel != null)
        {
            var optionsPanel = FindChildRecursive(learnPanel, "Options");
            if (optionsPanel == null)
            {
                optionsPanel = CreateUI("Options", learnPanel, Color.clear);
                var opRt = optionsPanel.GetComponent<RectTransform>();
                if (opRt == null) opRt = optionsPanel.gameObject.AddComponent<RectTransform>();
                opRt.anchoredPosition = new Vector2(0, -350);
                opRt.sizeDelta = new Vector2(600, 80);
                var hlg = optionsPanel.gameObject.AddComponent<HorizontalLayoutGroup>();
                hlg.childAlignment = TextAnchor.MiddleCenter; hlg.spacing = 20;
                hlg.childControlHeight = false; hlg.childControlWidth = false;

                AddButton(optionsPanel, "Uppercase", btnBlue, null);
                AddButton(optionsPanel, "Lowercase", btnBlue, null);
            }
        }

        // 5. REWARD HUD
        var rewardHud = FindChildRecursive(canvasGo.transform, "RewardHUD");
        if (rewardHud == null)
        {
            rewardHud = CreateUI("RewardHUD", canvasGo.transform, Color.clear);
            var rhRt = rewardHud.GetComponent<RectTransform>();
            if (rhRt == null) rhRt = rewardHud.gameObject.AddComponent<RectTransform>();
            rhRt.anchorMin = new Vector2(0, 1); rhRt.anchorMax = new Vector2(1, 1);
            rhRt.sizeDelta = new Vector2(0, 100);
            rhRt.anchoredPosition = new Vector2(0, -50);

            var hlg = rewardHud.gameObject.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleRight; hlg.spacing = 20; hlg.padding = new RectOffset(0, 50, 0, 0);
            hlg.childControlHeight = false; hlg.childControlWidth = false;

            var coins = AddButton(rewardHud, "Coins: 0", btnYellow, null);
            coins.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 60);
            
            var stars = AddButton(rewardHud, "Stars: 0", btnYellow, null);
            stars.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 60);
        }

        EditorSceneManager.SaveScene(scene);
    }
}
