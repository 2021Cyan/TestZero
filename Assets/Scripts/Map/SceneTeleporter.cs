using UnityEngine;

namespace EasyTransition
{
    public class SceneTeleporter : MonoBehaviour
    {
        public TransitionSettings transition;
        public float startDelay;
        private PlayerController playerController;

        void Awake()
        {
            playerController = PlayerController.Instance;
        }

        public void LoadScene(string _sceneName)
        {
            TransitionManager.Instance().Transition(_sceneName, transition, startDelay);
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

