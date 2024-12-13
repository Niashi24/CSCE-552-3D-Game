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
                // TODO: Increase score
                collected = true;
                animator.Play("Collected");
            }
        }
    }
}
