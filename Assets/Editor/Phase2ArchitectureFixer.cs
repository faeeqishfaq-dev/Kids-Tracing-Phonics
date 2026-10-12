using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor.Events;
using System.Collections.Generic;

using Game.Core.Managers;
using Game.UI;
using Game.Gameplay.FillLetter;

public static class Phase2ArchitectureFixer
{
    [MenuItem("KidsGame/Wire Phase 2 Fill Letters Win State")]
    public static void ManualRunFix()
    {
        EditorApplication.delayCall += () => 
        {
            try
            {
                WirePhase2UI();
                Debug.Log("[Phase2ArchitectureFixer] SUCCESS: Win Panel and Sliding Animations fully wired!");
            }
            catch(System.Exception e)
            {
                Debug.LogError("[Phase2ArchitectureFixer] ERROR: " + e.Message + "\n" + e.StackTrace);
            }
        };
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

    private static void WirePhase2UI()
    {
        Scene scene = EditorSceneManager.OpenScene("Assets/_Project/Scenes/1_Gameplay.unity");
        var canvasGo = GameObject.Find("Canvas");
        if (canvasGo == null) return;
        
        var nav = canvasGo.GetComponent<GameNavigation>();

        var fillPanel = FindChildRecursive(canvasGo.transform, "FillLetterPanel");
        if (fillPanel != null)
        {
            var controller = fillPanel.GetComponent<FillLetterUIController>();
            if (controller == null) return; // Should already exist from Phase 1

            var fillCanvas = FindChildRecursive(fillPanel, "FillCanvas");
            var selectionMenu = FindChildRecursive(fillPanel, "SelectionMenu");

            // Fix lost references from script changes
            if (selectionMenu != null) controller.selectionMenu = selectionMenu.GetComponent<RectTransform>();
            if (fillCanvas != null)
            {
                controller.paintingCanvas = fillCanvas.gameObject;
                var outline = FindChildRecursive(fillCanvas, "Outline");
                if (outline != null)
                {
                    controller.outlineImage = outline.GetComponent<Image>();
                    controller.outlineMask = outline.GetComponent<SpriteMask>();
                }
                controller.brushPainter = fillCanvas.GetComponent<BrushPainter>();
                controller.fillManager = fillCanvas.GetComponent<FillLetterManager>();
            }

            // 1. CREATE WIN PANEL
            var winPanel = FindChildRecursive(fillPanel, "WinPanel");
            if (winPanel == null)
            {
                var wpGo = new GameObject("WinPanel", typeof(RectTransform));
                wpGo.transform.SetParent(fillPanel, false);
                var wpRt = wpGo.GetComponent<RectTransform>();
                wpRt.anchorMin = Vector2.zero; wpRt.anchorMax = Vector2.one;
                wpRt.sizeDelta = Vector2.zero; wpRt.anchoredPosition = Vector2.zero;
                
                var bg = wpGo.AddComponent<Image>();
                bg.color = new Color(0, 0, 0, 0.7f); // Semi-transparent black

                var popup = new GameObject("Popup", typeof(RectTransform));
                popup.transform.SetParent(wpGo.transform, false);
                var pRt = popup.GetComponent<RectTransform>();
                pRt.sizeDelta = new Vector2(600, 400);
                pRt.anchoredPosition = Vector2.zero;
                var pImg = popup.AddComponent<Image>();
                pImg.color = Color.white;

                var txtGo = new GameObject("Title", typeof(RectTransform));
                txtGo.transform.SetParent(popup.transform, false);
                var txtRt = txtGo.GetComponent<RectTransform>();
                txtRt.anchoredPosition = new Vector2(0, 100);
                txtRt.sizeDelta = new Vector2(500, 100);
                var txt = txtGo.AddComponent<Text>();
                txt.text = "Level Complete!";
                txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                txt.fontSize = 60; txt.color = new Color(0.2f, 0.7f, 0.2f);
                txt.alignment = TextAnchor.MiddleCenter;

                var btnNext = AddButton(popup.transform, "Next", new Color(0.2f, 0.8f, 0.2f), new UnityAction(controller.NextLetter));
                btnNext.GetComponent<RectTransform>().anchoredPosition = new Vector2(120, -50);

                var btnMenu = AddButton(popup.transform, "Main Menu", new Color(0.8f, 0.2f, 0.2f), new UnityAction(nav.ShowMainMenu));
                btnMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, -50);

                winPanel = wpGo.transform;
            }

            controller.winPanel = winPanel.gameObject;

            // 2. SETUP ALPHABET LIST
            //controller.alphabetSprites.Clear();
            //for(int i=0; i<26; i++) 
            //{
            //    string letterName = ((char)('A' + i)).ToString();
            //    Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/_Project/Sprites/Letter_{letterName}.png");
            //    if (s != null) controller.alphabetSprites.Add(s);
            //}

            // 3. FIX GRID BUTTONS TO PASS INDEX
            if (selectionMenu != null)
            {
                var gridTransform = FindChildRecursive(selectionMenu, "Grid");
                if (gridTransform != null)
                {
                    for (int i = 0; i < gridTransform.childCount; i++)
                    {
                        var btn = gridTransform.GetChild(i).GetComponent<Button>();
                        if (btn != null && i < 26)
                        {
                            // Clear old listeners
                            while (btn.onClick.GetPersistentEventCount() > 0)
                            {
                                UnityEventTools.RemovePersistentListener(btn.onClick, 0);
                            }
                            
                            // Re-wire with integer index instead of sprite (fixes the previous issues and matches new API)
                            var action = new UnityAction<int>(controller.SelectLetter);
                            UnityEventTools.AddIntPersistentListener(btn.onClick, action, i);
                        }
                    }
                }
            }
            
            // 4. FIX THRESHOLD
            var pVal = FindChildRecursive(fillPanel, "FillCanvas").GetComponent<PaintValidator>();
            if (pVal != null)
            {
                SerializedObject so = new SerializedObject(pVal);
                so.FindProperty("_targetFillLength").floatValue = 40f;
                so.ApplyModifiedProperties();
            }
        }

        EditorSceneManager.SaveScene(scene);
    }

    private static Button AddButton(Transform parent, string name, Color color, UnityAction action)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 80);
        var img = go.AddComponent<Image>();
        img.color = color;
        var btn = go.AddComponent<Button>();
        
        var txtGo = new GameObject("Text", typeof(RectTransform));
        txtGo.transform.SetParent(go.transform, false);
        var txtRt = txtGo.GetComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero; txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero; txtRt.anchoredPosition = Vector2.zero;
        
        var txt = txtGo.AddComponent<Text>();
        txt.text = name;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (txt.font == null) txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 24;
        txt.color = Color.white;
        txt.alignment = TextAnchor.MiddleCenter;
        
        if (action != null) UnityEventTools.AddPersistentListener(btn.onClick, action);
        return btn;
    }
}
