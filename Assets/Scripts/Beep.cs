using System.Collections;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(AudioSource))]
    public class Beep : MonoBehaviour
    {
        private AudioSource _audioSource;

        public int position = 0;
        public int samplerate = 44100;
        public float frequency = 440;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = AudioClip.Create("", 1000, 1, samplerate, false, OnAudioRead, OnAudioSetPosition);

            Metronome.Beat += DoBeep;
        }

        private void OnAudioSetPosition(int i)
        {
            position = i;
        }

        private void OnAudioRead(float[] data)
        {
            int count = 0;
            while (count < data.Length)
            {
                data[count] = Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate);
                position++;
                count++;
            }
        }

        private void DoBeep(int i)
        {
                _audioSource.Play();
        }
    }
}