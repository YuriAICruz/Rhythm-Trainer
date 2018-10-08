using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class Metronome : MonoBehaviour
    {
        public static int Tempo = 4;
        public static int Bpm = 60;

        public static float ElapsedTime { get; private set; }

        public delegate IEnumerator RoutineAction(int i);

        public static event Action<int> Beat;
        static List<RoutineAction> _routineBeat = new List<RoutineAction>();

        public static void BeatSubscribe(RoutineAction action)
        {
            _routineBeat.Add(action);
        }

        private void Start()
        {
            StartCoroutine(Counter());
        }

        private void Update()
        {
            ElapsedTime += Time.deltaTime;
        }

        private uint _totalBeats;

        IEnumerator Counter()
        {
            var i = 0;
            ElapsedTime = 0;
            while (true)
            {
                yield return new WaitForSeconds(60 / (float) Bpm);
                _totalBeats++;
                ElapsedTime = _totalBeats * (60 / (float) Bpm);
                if (Beat != null) Beat(i);
                foreach (var action in _routineBeat)
                {
                    StartCoroutine(action(i));
                }
                i = (i + 1) % Tempo;
            }
        }
    }
}