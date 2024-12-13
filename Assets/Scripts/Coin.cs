using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Csce552
{
    public class Coin : MonoBehaviour
    {
        public Animator animator;
        public bool collected;
        public AudioClip collectSfx;
        public int score = 100;

        public void SpawnIn()
        {
            animator.Play("Spawn");
        }

        private void OnTriggerExit(Collider other)
        {
            if (collected)
                return;
            
            if (other.CompareTag("Player"))
            {
                EventManager.AddScore(score);
                collected = true;
                animator.Play("Collected");
                AudioSource.PlayClipAtPoint(collectSfx, this.gameObject.transform.position);
                Destroy(this.gameObject);
            }
        }
    }
}
