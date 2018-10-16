using System.Collections;
using Graphene.UiGenerics;
using UnityEditor;
using UnityEngine;

namespace Graphene.Rhythm.Presentation
{
    public class TimeFeedback : ImageView
    {
        private Metronome _metronome;

        private void Start()
        {
            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += Beat;
        }

        private void Beat(int index)
        {
            Image.color = new Color(1,1,1,0);

            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            var dur = _metronome.Bpm/60f;
            var t = 0f;
            while (t<=dur)
            {
                Image.color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), t / dur);

                yield return null;

                t += Time.deltaTime;
            }
        }
    }
}