using System.Collections;
using UnityEngine;

namespace Graphene.Rhythm
{
    [RequireComponent(typeof(AudioSource))]
    public class Beep : MonoBehaviour
    {
        private AudioSource _audioSource;

        public int position = 0;
        public int samplerate = 44100;
        public float frequency = 440;
        private AudioClip[] _beeps;
        private Metronome _metronome;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _beeps = new AudioClip[]
            {
                AudioClip.Create("", 1000, 1, samplerate, true, d => OnAudioRead(d, 0), OnAudioSetPosition),
                AudioClip.Create("", 1000, 1, samplerate, true, d => OnAudioRead(d, 1), OnAudioSetPosition),
                AudioClip.Create("", 1000, 1, samplerate, true, d => OnAudioRead(d, 2), OnAudioSetPosition),
                AudioClip.Create("", 1000, 1, samplerate, true, d => OnAudioRead(d, 3), OnAudioSetPosition),
                AudioClip.Create("", 1000, 1, samplerate, true, d => OnAudioRead(d, 4), OnAudioSetPosition),
                AudioClip.Create("", 1000, 1, samplerate, true, d => OnAudioRead(d, 5), OnAudioSetPosition),
                AudioClip.Create("", 1000, 1, samplerate, true, d => OnAudioRead(d, 6), OnAudioSetPosition),
                AudioClip.Create("", 1000, 1, samplerate, true, d => OnAudioRead(d, 7), OnAudioSetPosition)
            };

            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += DoBeep;
        }

        private void OnAudioSetPosition(int i)
        {
            position = i;
        }

        private void OnAudioRead(float[] data, int i)
        {
            int count = 0;
            while (count < data.Length)
            {
//                if (i % 2 == 0)
//                    data[count] = Mathf.Sin(2 * Mathf.PI * frequency * (i + 1) * position / samplerate);
//                else
                data[count] = (Mathf.PingPong(frequency * (i + 1) * position / samplerate, 0.5f));
                position++;
                count++;
            }
        }

        private void DoBeep(int i)
        {
            _audioSource.clip = _beeps[i];
            _audioSource.Play();
        }
    }
}