using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Csce552
{
    public class Unpause : MonoBehaviour
    {
        public GameObject pauseOverlay;
        
        public void onPointerClick()
        {
            AudioListener.pause = false;
            pauseOverlay.SetActive(false);
            Time.timeScale = 1;
            enabled = true;
        }
    }
}
