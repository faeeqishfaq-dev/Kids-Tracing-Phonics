using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Game.Gameplay.FillLetter
{
    public class GuideLoader : MonoBehaviour
    {
        [SerializeField] private RectTransform guideContainer;

        public void LoadLetterGuides(char letter)
        {
            ClearGuides();

            Sprite[] sprites = Resources.LoadAll<Sprite>("alphabet/" + letter);

            if (sprites == null || sprites.Length == 0)
            {
                Debug.LogWarning("No guides found for " + letter);
                return;
            }

            List<Sprite> guides = new List<Sprite>();

            foreach (Sprite s in sprites)
            {
                if (s.name.ToLower().Contains("guide"))
                {
                    guides.Add(s);
                }
            }

            guides.Sort((a, b) => a.name.CompareTo(b.name));

            LoadGuides(guides.ToArray());
        }

        private readonly List<Image> activeGuides = new();

        public void LoadGuides(Sprite[] guideSprites)
        {

            ClearGuides();

            foreach (Sprite sprite in guideSprites)
            {
                GameObject go = new GameObject(sprite.name);

                go.transform.SetParent(guideContainer, false);

                RectTransform rt = go.AddComponent<RectTransform>();

                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;

                Image img = go.AddComponent<Image>();

                img.sprite = sprite;

                img.raycastTarget = false;

                activeGuides.Add(img);
            }
        }

        public void ClearGuides()
        {
            foreach (Image img in activeGuides)
            {
                if (img != null)
                    Destroy(img.gameObject);
            }

            activeGuides.Clear();
        }
    }
}