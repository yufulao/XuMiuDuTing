// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    public class PauseButton : MonoBehaviour
    {
        private void Start()
        {
            var button = GetComponent<UnityEngine.UI.Button>();
            if (button == null || button.onClick.GetPersistentEventCount() > 0) return;
            button.onClick.AddListener(TogglePauseMenu);
        }

        public void TogglePauseMenu()
        {
            var pause = FindObjectOfType<Pause>();
            if (pause == null || pause.pausePanel == null) return;
            if (pause.pausePanel.isOpen) pause.pausePanel.Close(); else pause.pausePanel.Open();
        }
    }
}
