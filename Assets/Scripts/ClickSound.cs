using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Csce552
{
    public class ClickSound : MonoBehaviour
    {
        public AudioSource audioSource;
        public AudioClip clickSfx;
        
        public void PlaySound()
        {
            audioSource.PlayOneShot(clickSfx);
        }
    }
}
