using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Csce552
{
    public class CaterpillarScript : MonoBehaviour
    {
        public GameObject pauseHandler;
        public GameObject loseOverlay;
        public GameObject model;

        public Animator animator;
        public string animationToPlay;

        private void Awake()
        {
            pauseHandler = GameObject.Find("PauseHandler");
            loseOverlay = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
            animator.speed = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            pauseHandler.GetComponent<PauseScript>().PauseNooverlay();
            loseOverlay.SetActive(true);
        }

        public void SpawnIn()
        {
            animator.speed = 2.5f;
            animator.Play(animationToPlay);
        }
    }
}
