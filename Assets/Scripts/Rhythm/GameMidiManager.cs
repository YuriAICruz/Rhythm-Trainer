using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CSharpSynth.Midi;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using Graphene.Grid;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm
{
    [Serializable]
    public class SoundEvent
    {
        public int Note;
        public int Volume;
        public int Instrument;
        public float Duration = 0.4f;
    }

    [RequireComponent(typeof(AudioSource))]
    public class GameMidiManager : MonoBehaviour
    {
        [Header("Paths")] public string bankFilePath = "GM Bank/gm";
        public string midiFilePath = "Midis/Groove.mid";

        [Header("Stream Options")]
        public int bufferSize = 1024;
        
        private float[] sampleBuffer;
        public int samplerate = 44100;

        private bool _shouldPlayFile;

        public float _BeatOffset = 0f;
        
        [Header("DinamicNotes")] public SoundEvent[] GameEvents;
        public SoundEvent[] Beep;

        #region Private Params

        private float gain = 1f;
        private MidiSequencer midiSequencer;
        private StreamSynthesizer midiStreamSynthesizer;
        private Metronome _metronome;

        private MenuManager _menuManager;
        private MidiFile _midi;
        private GridSystem _gridSystem;
        private InfiniteHexGrid _infGrid;
        private float[] _events;
        private int _position;
        private float _loopOffset;

        #endregion

        void Awake()
        {
            midiStreamSynthesizer = new StreamSynthesizer(samplerate, 2, bufferSize, 40);
            sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

            midiStreamSynthesizer.LoadBank(bankFilePath);

            midiSequencer = new MidiSequencer(midiStreamSynthesizer);
            midiSequencer.Looping = true;
            midiSequencer.OnLoop += AddLoopOffset;

            _shouldPlayFile = false;

            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnStartGame += StartMetronome;
            _menuManager.OnGameOver += StopMetronome;

            _metronome = FindObjectOfType<Metronome>();
            _metronome.BeatSubscribe(DoBeep);
            _metronome.BeatEvent += Play;

            _gridSystem = GetComponent<GridSystem>();

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;

#if UNITY_WEBGL
            AudioSettings.outputSampleRate = samplerate;
            var aud = GetComponent<AudioSource>();
            aud.clip = AudioClip.Create("Proc", midiStreamSynthesizer.BufferSize, 1, samplerate, true, Reader, SetAudioPosition);
            aud.Play();
#endif
        }

        int _count;

        private void AddLoopOffset()
        {
            _count++;
            _loopOffset = (midiSequencer.SampleTime / (float) midiStreamSynthesizer.samplesperBuffer) *0.02f * _count;
            Debug.Log("Reset: " + _loopOffset);
        }

        private void Start()
        {
#if UNITY_WEBGL
#endif
        }

        private void StopMetronome()
        {
            _shouldPlayFile = false;
        }

        private void StartMetronome()
        {
            _shouldPlayFile = true;
            StartMusic();
        }

        private void StartMusic()
        {
            if (midiSequencer.isPlaying || !_shouldPlayFile) return;

            LoadSong(midiFilePath);
        }


        private void LoadSong(string midiPath)
        {
            _midi = new MidiFile(midiPath);
            Debug.Log("BeatsPerMinute: " + _midi.BeatsPerMinute);
            Debug.Log("Tracks.Length: " + _midi.Tracks.Length);
            
            midiSequencer.LoadMidi(_midi, false, 0);


            midiSequencer.Play();
        }


        // 0 - Coin
        // 1 - Player Hit
        // 2 - Boss Hit
        // 3 - Player Projectile
        // 4 - Boss Projectile
        // 5 - Player Die
        // 6 - Boss Die
        private void Play(int i)
        {
            StartCoroutine(DoBeep(0, GameEvents[i].Note, GameEvents[i].Duration, GameEvents[i].Volume, GameEvents[i].Instrument));
        }

        private void Update()
        {
            if (_infGrid == null)
                _infGrid = (InfiniteHexGrid) _gridSystem.Grid;

            _metronome.SetElapsedTime((midiSequencer.SampleTime / (float) midiStreamSynthesizer.samplesperBuffer) * 0.02f + _loopOffset + _BeatOffset);

            if (midiSequencer.isPlaying && !_shouldPlayFile)
            {
                midiSequencer.Stop(true);
            }
        }

        IEnumerator DoBeep(int i)
        {
            midiStreamSynthesizer.NoteOn(0, Beep[i].Note, Beep[i].Volume, Beep[i].Instrument);
            yield return new WaitForSeconds(Beep[i].Duration);
            midiStreamSynthesizer.NoteOff(0, Beep[i].Note);
        }

        public IEnumerator DoBeep(int channel, int note, float duration, int volume, int instrument)
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