using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Game.Gameplay.FillLetter;
using Game.Core.Managers;
using Game.Gameplay.FillLetter;
namespace Game.UI
{
    public class FillLetterUIController : MonoBehaviour
    {

        public AlphabetDatabase alphabetDatabase;
        [Header("Guide")]
        [SerializeField] private GuideLoader guideLoader;

        [Header("Popup Animation")]
        public CanvasGroup popupCanvasGroup;
        public GameObject dimBackground;
        public float popupDuration = 0.25f;


        [Header("Panels")]
        public RectTransform selectionMenu;
        public GameObject paintingCanvas;
        public GameObject winPanel;

        [Header("Painting Dependencies")]
        public Image outlineImage;
        public SpriteMask outlineMask;
        public BrushPainter brushPainter;
        public FillLetterManager fillManager;

        //[Header("Data")]
        //public List<Sprite> alphabetSprites = new List<Sprite>();
        //public List<Sprite> alphabetMaskSprites = new List<Sprite>();
        private int _currentIndex = 0;

        public System.Action<int> OnLetterSelectedEvent;

        private void OnEnable()
        {
            if (alphabetDatabase == null)
            {
                Debug.LogError("Alphabet Database is missing on FillLetterUIController.");
                enabled = false;
                return;
            }
            if (fillManager != null) fillManager.OnLetterCompleted += HandleLetterCompleted;

            // Initial state
            paintingCanvas.SetActive(false);
            if (winPanel != null) winPanel.SetActive(false);

            // Slide in menu
            if (selectionMenu != null)
            {
                //selectionMenu.gameObject.SetActive(true);
                ShowLetterPopup();
            }
        }

        private void OnDisable()
        {
            if (fillManager != null) fillManager.OnLetterCompleted -= HandleLetterCompleted;
        }

        public void SelectLetter(int index)
        {
            Debug.Log("SELECTED INDEX = " + index);

            Debug.Log("TOTAL LETTERS = " + alphabetDatabase.letters.Count);

            Debug.Log("SPRITE = " + alphabetDatabase.letters[index].letterSprite.name);
            if (alphabetDatabase == null)
            {
                Debug.LogError("Alphabet Database is not assigned!");
                return;
            }

            if (index < 0 || index >= alphabetDatabase.letters.Count)
                return;

            LetterData data = alphabetDatabase.letters[index];
            Sprite letterSprite = data.letterSprite;
            Sprite maskSprite = data.maskSprite;

            _currentIndex = index;

            if (guideLoader != null)
            {
                char letter = (char)('A' + index);
                guideLoader.LoadLetterGuides(letter);
            }

            OnLetterSelectedEvent?.Invoke(index);

            // Update the visuals
            if (outlineImage != null) outlineImage.sprite = letterSprite;

            if (brushPainter != null && outlineImage != null)
            {
                brushPainter.drawArea = outlineImage.rectTransform;
                brushPainter.SetCurrentLetter(outlineImage);
            }

            // Fix the SpriteMask scaling without breaking the UI Image
            if (outlineImage != null)
            {
                // If SpriteMask is on the same object as Image, it will cause the Image to scale to infinity (black screen)
                if (outlineMask != null && outlineMask.gameObject == outlineImage.gameObject)
                {
                    Destroy(outlineMask);

                    GameObject maskObj = new GameObject("DynamicMask");
                    maskObj.transform.SetParent(outlineImage.transform, false);
                    outlineMask = maskObj.AddComponent<SpriteMask>();
                }

                if (outlineMask != null && maskSprite != null)
                {
                    outlineMask.sprite = maskSprite;
                    RectTransform rt = outlineImage.GetComponent<RectTransform>();
                    //if (rt != null && letterSprite != null)
                    {
                        // Calculate ratio of UI Canvas size vs Sprite world size
                        //float widthScale = rt.rect.width / letterSprite.bounds.size.x;
                        //float heightScale = rt.rect.height / letterSprite.bounds.size.y;

                        // Apply this scale ONLY to the mask child object, so the visual Image remains normal!
                        //outlineMask.transform.localScale = new Vector3(widthScale, heightScale, 1f);
                    }
                }
            }

            // Slide out menu, show canvas
            if (selectionMenu != null)
            {
                //StartCoroutine(SlideMenu(selectionMenu, Vector2.zero, new Vector2(0, -1000), 0.4f, () => 
                //{
                //    selectionMenu.gameObject.SetActive(false);
                //    StartPainting();
                //}));
                HideLetterPopup(StartPainting);
            }
            else
            {
                StartPainting();
            }
        }

        private void StartPainting()
        {
            if (brushPainter != null) brushPainter.ClearAll();
            paintingCanvas.SetActive(true);
            if (winPanel != null) winPanel.SetActive(false);
            if (fillManager != null) fillManager.InitializePainting();
        }

        public void ReturnToMenu()
        {
            CheckCpAd.instance.CallAd();
            paintingCanvas.SetActive(false);
            if (winPanel != null) winPanel.SetActive(false);

            if (brushPainter != null) brushPainter.ClearAll();

            if (selectionMenu != null)
            {
                selectionMenu.gameObject.SetActive(true);
                //StartCoroutine(SlideMenu(selectionMenu, new Vector2(0, -1000), Vector2.zero, 0.4f));
                ShowLetterPopup();
            }
        }

        private void HandleLetterCompleted()
        {
            CheckCpAd.instance.CallAd();
            ShowWinPanel();
        }

        private void ShowWinPanel()
        {
            if (winPanel == null)
                return;

            if (paintingCanvas != null)
                paintingCanvas.SetActive(false);

            winPanel.SetActive(true);

            winPanel.transform.SetAsLastSibling();

            CanvasGroup cg = winPanel.GetComponent<CanvasGroup>();

            if (cg == null)
                cg = winPanel.AddComponent<CanvasGroup>();

            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;

            Canvas canvas = winPanel.GetComponent<Canvas>();

            if (canvas == null)
                canvas = winPanel.AddComponent<Canvas>();

            canvas.overrideSorting = true;
            canvas.sortingOrder = 1000;

            GraphicRaycaster raycaster = winPanel.GetComponent<GraphicRaycaster>();

            if (raycaster == null)
                winPanel.AddComponent<GraphicRaycaster>();

            winPanel.transform.localScale = Vector3.zero;

            StartCoroutine(ScaleUp(winPanel.transform, 0.25f));
        }

        private IEnumerator AutoAdvanceLetter()
        {
            // The stroke guide component handles completion delay, but we add a small buffer here just in case.
            // Wait for 1.5 seconds before transitioning.
            yield return new WaitForSeconds(1.5f);

            int nextIndex = _currentIndex + 1;

            // If all letters are completed
            //if (nextIndex >= alphabetDatabase.alphabetSprites.Count)
            //{
            if (nextIndex >= alphabetDatabase.letters.Count)
            {
                winPanel.SetActive(true);
                //}

                //if (winPanel != null)
                //{
                //winPanel.SetActive(true);
                // Pull winPanel extremely close to camera to ensure it passes Z-test over LineRenderer
                Vector3 pos = winPanel.transform.localPosition;
                pos.z = -500f;
                winPanel.transform.localPosition = pos;

                // Add a Canvas override to force sorting natively without breaking UI layout
                Canvas canvas = winPanel.GetComponent<Canvas>();
                if (canvas == null) canvas = winPanel.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = 200; // Force it above LineRenderer's 100

                winPanel.transform.localScale = Vector3.zero;
                StartCoroutine(ScaleUp(winPanel.transform, 0.3f));
            }
            //}
            else
            {
                // Go to next letter automatically
                if (winPanel != null) winPanel.SetActive(false);
                SelectLetter(nextIndex);
            }
        }

        public void NextLetter()
        {
            CheckCpAd.instance.CallAd();
            if (winPanel != null)
                winPanel.SetActive(false);

            if (paintingCanvas != null)
                paintingCanvas.SetActive(true);

            if (brushPainter != null)
                brushPainter.ClearAll();
            int nextIndex = _currentIndex + 1;

            if (nextIndex >= alphabetDatabase.letters.Count)
            {
                ReturnToMenu();
                return;
            }

            SelectLetter(nextIndex);

            //SelectLetter(nextIndex);
        }

        private IEnumerator SlideMenu(RectTransform target, Vector2 start, Vector2 end, float duration, System.Action onComplete = null)
        {
            float elapsed = 0f;
            target.anchoredPosition = start;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                target.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
                yield return null;
            }
            target.anchoredPosition = end;
            onComplete?.Invoke();
        }

        private IEnumerator ScaleUp(Transform target, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                target.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, elapsed / duration);
                yield return null;
            }
            target.localScale = Vector3.one;
        }

        private void ShowLetterPopup()
        {
            selectionMenu.gameObject.SetActive(true);

            if (dimBackground != null)
                dimBackground.SetActive(true);

            selectionMenu.localScale = Vector3.zero;

            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = 0;
                popupCanvasGroup.interactable = false;
                popupCanvasGroup.blocksRaycasts = false;
            }

            StartCoroutine(PopupIn());
        }

        private void HideLetterPopup(System.Action onComplete = null)
        {
            StartCoroutine(PopupOut(onComplete));
        }

        private IEnumerator PopupIn()
        {
            float t = 0;

            while (t < popupDuration)
            {
                t += Time.deltaTime;

                float p = t / popupDuration;

                selectionMenu.localScale =
                    Vector3.Lerp(Vector3.zero, Vector3.one, p);

                if (popupCanvasGroup != null)
                    popupCanvasGroup.alpha = p;

                yield return null;
            }

            selectionMenu.localScale = Vector3.one;

            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = 1;
                popupCanvasGroup.interactable = true;
                popupCanvasGroup.blocksRaycasts = true;
            }
        }

        private IEnumerator PopupOut(System.Action onComplete)
        {
            float t = 0;

            while (t < popupDuration)
            {
                t += Time.deltaTime;

                float p = t / popupDuration;

                selectionMenu.localScale =
                    Vector3.Lerp(Vector3.one, Vector3.zero, p);

                if (popupCanvasGroup != null)
                    popupCanvasGroup.alpha = 1 - p;

                yield return null;
            }

            selectionMenu.gameObject.SetActive(false);

            if (dimBackground != null)
                dimBackground.SetActive(false);

            onComplete?.Invoke();
        }

    }
}
