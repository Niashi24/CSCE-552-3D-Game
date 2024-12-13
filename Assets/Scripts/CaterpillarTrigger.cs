using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Csce552
{
    public class CaterpillarTrigger : MonoBehaviour
    {
        public CaterpillarScript caterpillar;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                caterpillar.SpawnIn();
            }
        }
    }
}
