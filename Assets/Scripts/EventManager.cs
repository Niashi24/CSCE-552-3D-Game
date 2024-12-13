using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Csce552
{
    public class EventManager : MonoBehaviour
    {
        public static event Action<int> addScore;

        public static void AddScore(int score)
        {
            addScore?.Invoke(score);
        }
    }
}
