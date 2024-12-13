using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Csce552
{
    public class PauseScript : MonoBehaviour
    {
        public GameObject pauseOverlay;
        public GameObject loseOverlay;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log(Time.timeScale);
                if(loseOverlay.activeInHierarchy == false)
                {
                    if(Time.timeScale != 0)   
                    {   
                        Pause();
                    }
                    else
                    {
                        Unpause();
                    }
                }
            }
        }

        public void Pause()
        {
            AudioListener.pause = true;
            pauseOverlay.SetActive(true);
            Time.timeScale = 0;
            //enabled = false;
        }

        public void PauseNooverlay()
        {
            AudioListener.pause = true;
            Time.timeScale = 0;
            //enabled = false;
        }

        public void Unpause()
        {
            AudioListener.pause = false;
            pauseOverlay.SetActive(false);
            Time.timeScale = 1;
            //enabled = true;
        }
    }
}
