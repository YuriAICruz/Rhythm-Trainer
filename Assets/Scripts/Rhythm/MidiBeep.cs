using System.Collections;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using UnityEngine;

namespace Graphene.Rhythm
{
    [RequireComponent(typeof(AudioSource))]
    public class MidiBeep : MonoBehaviour
    {
        public string bankFilePath = "GM Bank/gm";

        public int bufferSize = 1024;
        public int midiNote = 60;
        public int midiNoteVolume = 100;

        [Range(0, 127)]
        public int midiInstrument = 0;

        private float[] sampleBuffer;

        private float gain = 1f;
        private MidiSequencer midiSequencer;
        private StreamSynthesizer midiStreamSynthesizer;
        private Metronome _metronome;

        void Awake()
        {
            midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
            sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

            midiStreamSynthesizer.LoadBank(bankFilePath);

            midiSequencer = new MidiSequencer(midiStreamSynthesizer);		
            
            _metronome = FindObjectOfType<Metronome>();
            _metronome.BeatSubscribe(DoBeep);
            _metronome.BeatEvent += Play;
        }

        private void Play(int i)
        {
            StartCoroutine(DoBeep(i+7));
        }

        IEnumerator DoBeep(int i)
        {
            midiStreamSynthesizer.NoteOn(0, midiNote+i, midiNoteVolume, midiInstrument);
            yield return new WaitForSeconds(0.4f);
            midiStreamSynthesizer.NoteOff(0, midiNote+i);
        }
        
        private void OnAudioFilterRead(float[] data, int channels)
        {
            midiStreamSynthesizer.GetNext(sampleBuffer);

            for (int i = 0; i < data.Length; i++)
            {
                data[i] = sampleBuffer[i] * gain;
            }
        }
    }
}