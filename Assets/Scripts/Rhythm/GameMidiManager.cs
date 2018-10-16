using System;
using System.Collections;
using CSharpSynth.Midi;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm
{
    [RequireComponent(typeof(AudioSource))]
    public class GameMidiManager : MonoBehaviour
    {
        public event Action<MidiFile> OnMidiSet;
        
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
        
        [SerializeField] private int[] _beetInstrument;
        [SerializeField] private int[] _beepNote;
        [SerializeField] private float[] _beepDuration;
        private MenuManager _menuManager;
        private MidiFile _midi;

        void Awake()
        {
            midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
            sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

            midiStreamSynthesizer.LoadBank(bankFilePath);

            midiSequencer = new MidiSequencer(midiStreamSynthesizer);
            midiSequencer.Looping = true;

            ShouldPlayFile = false;
            
            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnStartGame += StartMetronome;
            _menuManager.OnGameOver += StopMetronome;

            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += StartMusic;
            _metronome.BeatSubscribe(DoBeep);
            _metronome.BeatEvent += Play;
        }

        private void StopMetronome()
        {
            ShouldPlayFile = false;
        }

        private void StartMetronome()
        {
            ShouldPlayFile = true;
        }

        private void StartMusic(int i)
        {
            if (i != 0) return;

            if (midiSequencer.isPlaying || !ShouldPlayFile) return;
            
            LoadSong(midiFilePath);
            //StartCoroutine(ProceduralMusic());
        }


        private void LoadSong(string midiPath)
        {
            _midi = new MidiFile(midiPath);
            OnMidiSet?.Invoke(_midi);
            
            Debug.Log(_midi.BeatsPerMinute);
            Debug.Log(_midi.Tracks.Length);

            midiSequencer.LoadMidi(midiPath, false);
            midiSequencer.Play();
        }

        private void Play(int i)
        {
            StartCoroutine(DoBeep(0, midiNote + i, 0.4f, midiNoteVolume, midiInstrument));
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
            var beat = _metronome.Bpm / 60f;

            int loop = 0;
            
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
            midiStreamSynthesizer.NoteOn(0, _beepNote[i], midiNoteVolume, _beetInstrument[i]);
            yield return new WaitForSeconds(_beepDuration[i]);
            midiStreamSynthesizer.NoteOff(0, _beepNote[i]);
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