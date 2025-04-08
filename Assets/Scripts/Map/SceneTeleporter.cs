using UnityEngine;
using UnityEngine.SceneManagement;

namespace EasyTransition
{
    public class SceneTeleporter : MonoBehaviour
    {
        public TransitionSettings transition;
        public float startDelay;
        private PlayerController playerController;
        private PodScript podScript;

        void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            // playerController = PlayerController.Instance;
            // podScript = PodScript.Instance;
        }

        public void LoadScene(string _sceneName)
        {

            TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            if (playerController != null)
            {
                playerController.transform.position = new Vector3(-5.5f, -2.5f, 0f);
            }

            if (podScript != null)
            {
                podScript.transform.position = new Vector3(-5.5f, -2.5f, 0f);
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            playerController = PlayerController.Instance;
            podScript = PodScript.Instance;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (playerController != null && other.CompareTag("Player"))
            {
                LoadScene("MainGame");
            }
        }
    }

}