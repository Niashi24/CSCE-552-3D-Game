using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Csce552
{
    public class ScoreManager : MonoBehaviour
    {
        public int score;

        public TMP_Text text;

        private void Start()
        {
            EventManager.addScore += AddScore;
        }

        private void OnDestroy()
        {
            EventManager.addScore -= AddScore;
        }

        public void AddScore(int score)
        {
            this.score += score;
            text.text = "Score: " + this.score;
        }
    }
}
