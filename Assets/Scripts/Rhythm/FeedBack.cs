using System.Collections;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class FeedBack : MonoBehaviour
    {
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _renderer.enabled = false;
            Metronome.BeatSubscribe(Flash);
        }

        private IEnumerator Flash(int i)
        {
            _renderer.enabled = true;

            yield return new WaitForSeconds(60 / (float) Metronome.Bpm / (float) Metronome.Tempo);

            _renderer.enabled = false;
        }
    }
}