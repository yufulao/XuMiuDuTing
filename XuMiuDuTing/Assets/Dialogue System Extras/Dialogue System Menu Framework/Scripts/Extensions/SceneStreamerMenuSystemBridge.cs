using UnityEngine;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    public class SceneStreamerMenuSystemBridge : MonoBehaviour
    {

#if USE_SCENESTREAMER
        private SaveHelper saveHelper;
        private bool isLoadingGame = false;

        private void Awake()
        {
            saveHelper = FindObjectOfType<SaveHelper>();
        }

        private void OnEnable()
        {
            saveHelper.returningToTitleMenu += OnReturnToTitleMenu;
            SaveSystem.loadStarted += OnLoadStarted;
            SaveSystem.loadEnded += OnLoadEnded;
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            saveHelper.returningToTitleMenu += OnReturnToTitleMenu;
            SaveSystem.loadStarted += OnLoadStarted;
            SaveSystem.loadEnded += OnLoadEnded;
            SaveSystem.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(string sceneName, int sceneIndex)
        {
            if (isLoadingGame)
            {
                var setStartScene = FindObjectOfType<SceneStreamer.SetStartScene>();
                if (setStartScene != null) setStartScene.enabled = false;
            }
        }

        private void OnLoadStarted()
        {
            isLoadingGame = true;
        }

        private void OnLoadEnded()
        {
            isLoadingGame = false;
        }

        private void OnReturnToTitleMenu()
        {
            SceneStreamer.SceneStreamer.UnloadAll();
        }
#endif

    }
}
