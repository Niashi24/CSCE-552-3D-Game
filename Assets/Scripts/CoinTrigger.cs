using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Csce552
{
    public class CoinTrigger : MonoBehaviour
    {
        public Coin coin;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                coin.SpawnIn();
            }
        }
    }
}
