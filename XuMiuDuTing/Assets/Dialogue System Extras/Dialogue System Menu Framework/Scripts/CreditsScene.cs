// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEngine.SceneManagement;
using Yu;

namespace PixelCrushers.DialogueSystem.MenuSystem
{

    /// <summary>
    /// This script handles the credits scene. It just provides a method to
    /// return to the title menu scene.
    /// </summary>
    public class CreditsScene : MonoBehaviour
    {

        public void ReturnToTitleScene()
        {
            var titleMenu = FindObjectOfType<TitleMenu>();
            GameManager.Instance.ReturnToTitle();
            FindObjectOfType<TitleMenu>().titleMenuPanel.Open();
        }
    }
}
