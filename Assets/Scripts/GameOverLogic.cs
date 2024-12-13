using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Csce552
{
    public class GameOverLogic : MonoBehaviour
    {
        public void Restart()
        {
            AudioListener.pause = false;
            Time.timeScale = 1;
            //enabled = true;
            SceneManager.LoadScene("GameScene");
        }
        /*
        public void RestartCheckpoint() {
            AudioListener.pause = false;
            Time.timeScale = 1;
            enabled = true;
            SceneManager.LoadScene("GameScene");
        }*/
        public void ToMainMenu()
        {
            AudioListener.pause = false;
            Time.timeScale = 1;
            //enabled = true;
            SceneManager.LoadScene("MenuScene");
        }
    }
}
