using CSharpSynth.Midi;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class MidiChannel : MonoBehaviour
    {
        private float gain = 1f;
        private int bufferSize = 1024;
        private MidiSequencer midiSequencer;
        private StreamSynthesizer midiStreamSynthesizer;
        private float[] sampleBuffer;
        
        public void SetMidi(string midiPath, string bankFilePath)
        {
            midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
            sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

            midiStreamSynthesizer.LoadBank(bankFilePath);

            midiSequencer = new MidiSequencer(midiStreamSynthesizer);
            midiSequencer.Looping = true;
            
            midiSequencer.LoadMidi(new MidiFile(midiPath), false, (uint)transform.GetSiblingIndex()+1);
            midiSequencer.Play();
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