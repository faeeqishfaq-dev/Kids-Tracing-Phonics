using UnityEngine;
using Game.Core.Managers;
using System.Collections;

namespace Game.Core.Managers
{
    public class LoadingAutoStarter : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(LoadNextScene());
        }

        private IEnumerator LoadNextScene()
        {
            yield return new WaitForSeconds(1.5f);
            if (SceneLoader.Instance != null)
            {
                SceneLoader.Instance.LoadScene("1_Gameplay");
            }
        }
    }
}
