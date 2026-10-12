using UnityEngine;
using Game.Core.Managers;

namespace Game.UI
{
    public class GameNavigation : MonoBehaviour
    {
        public GameObject MainMenuPanel;
        //public GameObject LearningPanel;
        //public GameObject TracingPanel;
        public GameObject FillLetterPanel;
        //public GameObject SettingsPanel;
        //public GameObject MiniGamesPanel;

        private void Start()
        {
            ShowMainMenu();
        }

        private void HideAll()
        {
            if (MainMenuPanel != null) MainMenuPanel.SetActive(false);
            //if (LearningPanel != null) LearningPanel.SetActive(false);
            //if (TracingPanel != null) TracingPanel.SetActive(false);
            if (FillLetterPanel != null) FillLetterPanel.SetActive(false);
            //if (SettingsPanel != null) SettingsPanel.SetActive(false);
            //if (MiniGamesPanel != null) MiniGamesPanel.SetActive(false);
        }

        public void ShowMainMenu()
        {

            HideAll();
            CheckCpAd.instance.CallAd();
            if (MainMenuPanel) MainMenuPanel.SetActive(true);
        }

        //public void ShowLearning()
        //{
        //    HideAll();
        //    if (LearningPanel) LearningPanel.SetActive(true);
        //}

        //public void ShowTracing()
        //{
        //    HideAll();
        //    if (TracingPanel) TracingPanel.SetActive(true);
        //}

        public void ShowFillLetter()
        {
            CheckCpAd.instance.CallAd();
            HideAll();
            if (FillLetterPanel) FillLetterPanel.SetActive(true);
        }

        //public void ShowSettings()
        //{
        //    HideAll();
        //    if (SettingsPanel) SettingsPanel.SetActive(true);
        //}

        //public void ShowMiniGames()
        //{
        //    HideAll();
        //    if (MiniGamesPanel) MiniGamesPanel.SetActive(true);
        //}

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
