using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Csce552
{
    public class MenuLogic : MonoBehaviour
    {
        public void LoadGame()
        {
            SceneManager.LoadScene("GameScene");
        }

        public void QuitGame()
        {
            #if UNITY_STANDALONE
                Application.Quit();
            #endif

            //this is just so the quit button works while in the editor
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
