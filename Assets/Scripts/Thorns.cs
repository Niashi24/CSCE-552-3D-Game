using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Csce552
{
    public class Thorns : MonoBehaviour
    {
        public GameObject pauseHandler;
        public GameObject loseOverlay;
        private void Awake() {
            pauseHandler = GameObject.Find("PauseHandler");
            loseOverlay = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
        }
        private void OnTriggerEnter(Collider other)
            {
                pauseHandler.GetComponent<PauseScript>().PauseNooverlay();
                loseOverlay.SetActive(true);
            }
    }
}
