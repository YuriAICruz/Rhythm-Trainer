using System.Collections;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using UnityEngine;

namespace Graphene.Rhythm
{
    [RequireComponent(typeof(AudioSource))]
    public class GameMidiManager : MonoBehaviour
    {
        public string bankFilePath = "GM Bank/gm";

        public string midiFilePath = "Midis/Groove.mid";

        public int bufferSize = 1024;
        public int midiNote = 60;
        public int midiNoteVolume = 100;

        [Range(0, 127)] public int midiInstrument = 0;

        public bool ShouldPlayFile;

        private float[] sampleBuffer;

        private float gain = 1f;
        private MidiSequencer midiSequencer;
        private StreamSynthesizer midiStreamSynthesizer;
        private Metronome _metronome;

        [SerializeField] private int _pianoNote;

        void Awake()
        {
            midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
            sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

            midiStreamSynthesizer.LoadBank(bankFilePath);

            midiSequencer = new MidiSequencer(midiStreamSynthesizer);

            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += StartMusic;
            //_metronome.BeatSubscribe(DoBeep);
            _metronome.BeatEvent += Play;
        }

        private void StartMusic(int i)
        {
            if (i != 0) return;

            _metronome.Beat -= StartMusic;

            LoadSong(midiFilePath);
            //StartCoroutine(ProceduralMusic());
        }


        void LoadSong(string midiPath)
        {
            midiSequencer.LoadMidi(midiPath, false);
            midiSequencer.Play();
        }

        private void Play(int i)
        {
            StartCoroutine(DoBeep(i + 7));
        }

        private void Update()
        {
            if (midiSequencer.isPlaying && !ShouldPlayFile)
            {
                midiSequencer.Stop(true);
            }
        }

        IEnumerator ProceduralMusic()
        {
            var t = 0f;
            var beat = _metronome.Bpm / 60f / (float) _metronome.Tempo;

            int loop = 0;
            Debug.Log(beat);
            while (true)
            {
                if (loop % 5 == 0)
                    StartCoroutine(DoBeep(0, 60 + loop / 5, beat * 5, 200, 102));

                StartCoroutine(DoBeep(0, (int) (_pianoNote + Mathf.Sin(t * _metronome.Bpm) * 6), beat, 300, midiInstrument));

                yield return new WaitForSeconds(beat / 2);

                StartCoroutine(DoBeep(0, (int) (_pianoNote + 1 + Mathf.Sin(t * _metronome.Bpm) * 6), beat, 300, midiInstrument));

                yield return new WaitForSeconds(beat / 2);

                loop++;
                if ((int) (loop / 5) > 4)
                    loop = 0;
                t += Time.deltaTime;
            }
        }


        IEnumerator DoBeep(int i)
        {
            midiStreamSynthesizer.NoteOn(0, midiNote + i, midiNoteVolume, midiInstrument);
            yield return new WaitForSeconds(0.4f);
            midiStreamSynthesizer.NoteOff(0, midiNote + i);
        }

        IEnumerator DoBeep(int channel, int note, float duration, int volume, int instrument)
        {
            midiStreamSynthesizer.NoteOn(channel, note, volume, instrument);
            yield return new WaitForSeconds(duration);
            midiStreamSynthesizer.NoteOff(channel, note);
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