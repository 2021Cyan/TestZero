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
            playerController = PlayerController.Instance;
            podScript = PodScript.Instance;
        }

        public void LoadScene(string _sceneName)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {

            if (playerController != null)
            {
                playerController.transform.position = new Vector3(-5.5f, -2.5f, 0f);
            }

            if (podScript != null)
            {
                podScript.transform.position = new Vector3(-5.5f, -2.5f, 0f);
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
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

