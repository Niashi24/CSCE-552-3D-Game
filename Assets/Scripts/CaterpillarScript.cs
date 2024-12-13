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

        private Animator animator;

        private void Awake()
        {
            pauseHandler = GameObject.Find("PauseHandler");
            loseOverlay = GameObject.Find("Canvas").transform.GetChild(1).gameObject;
            animator = model.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            
            if(animator.GetCurrentAnimatorStateInfo(0).length > animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
            {
                return;
            }
            else
            {
                transform.Rotate(Vector3.back, 40.0f);
                animator.Play(0);
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            pauseHandler.GetComponent<PauseScript>().PauseNooverlay();
            loseOverlay.SetActive(true);
        }
    }
}
