using System.Collections;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class FeedBack : MonoBehaviour
    {
        private Renderer _renderer;
        private Metronome _metronome;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.enabled = false;
            
            _metronome = FindObjectOfType<Metronome>();
            _metronome.BeatSubscribe(Flash);
        }

        private IEnumerator Flash(int i)
        {
            _renderer.enabled = true;

            yield return new WaitForSeconds(60 / (float) _metronome.Bpm / (float) _metronome.Tempo);

            _renderer.enabled = false;
        }
    }
}