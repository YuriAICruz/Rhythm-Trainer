using System;
using System.Collections;
using System.Collections.Generic;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class Metronome : MonoBehaviour
    {
        public int Tempo = 4;
        public uint Bpm = 60;

        public uint TotalBeats;

        public float ElapsedTime { get; private set; }

        public Action<float> SetElapsedTime;

        public delegate IEnumerator RoutineAction(int i);

        public event Action<int> Beat;
        public event Action<int> BeatEvent;
        List<RoutineAction> _routineBeat = new List<RoutineAction>();
        private MenuManager _menuManager;
        private bool _canPlay;

        public void BeatSubscribe(RoutineAction action)
        {
            _routineBeat.Add(action);
        }

        private void Start()
        {
            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnStartGame += StartMetronome;
            _menuManager.OnGameOver += StopMetronome;
            StartCoroutine(Counter());

            SetElapsedTime += f => ElapsedTime = f;
        }

        private void StartMetronome()
        {
            _canPlay = true;
        }

        private void StopMetronome()
        {
            _canPlay = false;
        }

        private void Update()
        {
            if (!_canPlay) return;

            //ElapsedTime += Time.deltaTime;
        }

        IEnumerator Counter()
        {
            var i = 0;
            ElapsedTime = 0;
            while (true)
            {
                if (!_canPlay)
                {
                    i = 0;
                    TotalBeats = 0;
                    ElapsedTime = 0;
                    yield return null;
                    continue;
                }

                yield return new WaitForSeconds(60f / Bpm);

                TotalBeats++;
                //ElapsedTime = TotalBeats * (60f / Bpm);

                Beat?.Invoke(i);

                foreach (var action in _routineBeat)
                {
                    StartCoroutine(action(i));
                }

                i = (i + 1) % Tempo;
            }
        }

        public void PlayEvent(int i)
        {
            BeatEvent?.Invoke(i);
        }

        public float GetLapse()
        {
            var bps = Bpm / 60f;
            return ElapsedTime* bps - Mathf.Floor(ElapsedTime * bps);
        }
    }
}