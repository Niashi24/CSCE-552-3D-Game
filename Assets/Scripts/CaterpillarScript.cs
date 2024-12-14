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
        public float animationSpeed = 1f;

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
            animator.speed = animationSpeed;
            animator.Play(animationToPlay);
        }
    }
}
