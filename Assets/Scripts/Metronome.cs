using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class Metronome : MonoBehaviour
    {
        public int Tempo = 4;
        public int Bpm = 60;

        public delegate IEnumerator RoutineAction(int i);

        public static event Action<int> Beat;
        public static event RoutineAction RoutineBeat;

        private void Start()
        {
            StartCoroutine(Counter());
        }

        IEnumerator Counter()
        {
            var i = 0;
            while (true)
            {
                yield return new WaitForSeconds(60 / (float) Bpm);
                if (Beat != null) Beat(i);
                if (RoutineBeat != null) StartCoroutine(RoutineBeat(i));
                i = (i + 1) % Tempo;
            }
        }
    }
}