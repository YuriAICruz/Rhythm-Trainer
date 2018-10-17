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
        private GridSystem _gridSystem;
        private InfiniteHexGrid _infGrid;
        private float[] _events;

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
            _metronome.BeatSubscribe(DoBeep);
            _metronome.BeatEvent += Play;

            _gridSystem = GetComponent<GridSystem>();

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;
            if (_infGrid != null)
                _infGrid.SetYGraph = YGraph;
        }

        private float YGraph(Vector3 pos)
        {
//            if (!midiSequencer.isPlaying || _events == null) return 0;

//            var i = (int) (pos.x * 0.5f % _events.Length);
//            var i2 = Mathf.Sign(pos.x - _events[i]) > 0 ? (i + 1) % _events.Length : (i - 1) % _events.Length;
//            return Mathf.Lerp(_events[i], _events[i2], Mathf.Abs(pos.x - _events[i])) + Mathf.Cos(pos.z * 0.2f) * 1.5f;

            return Mathf.Sin(pos.x * 0.05f) * 2.6f + Mathf.Cos(pos.z * 0.1f) * 1.2f;
        }

        private void StopMetronome()
        {
            ShouldPlayFile = false;
        }

        private void StartMetronome()
        {
            ShouldPlayFile = true;
            StartMusic();
        }

        private void StartMusic()
        {
            if (midiSequencer.isPlaying || !ShouldPlayFile) return;

            LoadSong(midiFilePath);
        }


        private void LoadSong(string midiPath)
        {
            _midi = new MidiFile(midiPath);
            OnMidiSet?.Invoke(_midi);
            midiSequencer.LoadMidi(_midi, false, 0);

//            var totalTime = (int) _midi.Tracks[0].TotalTime;
//            _events = new float[(int) (totalTime / _gridSystem.Widith)];
//
//            var frame = midiStreamSynthesizer.samplesperBuffer;
//
//            var list = _midi.Tracks[0].MidiEvents.ToList();
//
//            var sampleTime = 0;
//            var eventIndex = 0;
//
//            for (int i = 0; i < (totalTime / frame); i++)
//            {
//                var j = 1;
//                while (eventIndex < _midi.Tracks[0].EventCount && _midi.Tracks[0].MidiEvents[eventIndex].deltaTime < (sampleTime + frame))
//                {
//                    if (_midi.Tracks[0].MidiEvents[eventIndex].midiChannelEvent == MidiHelper.MidiChannelEvent.Note_On)
//                    {
//                        _events[i] = j;
//                        j++;
//                    }
//
//                    eventIndex++;
//                }
//
//                sampleTime += frame;
//            }

            Debug.Log("BeatsPerMinute: " + _midi.BeatsPerMinute);
            Debug.Log("Tracks.Length: " + _midi.Tracks.Length);

            midiSequencer.Play();
        }

        private void Play(int i)
        {
            StartCoroutine(DoBeep(0, midiNote + i, 0.4f, midiNoteVolume, midiInstrument));
        }

        private void Update()
        {
            if (_infGrid == null)
                _infGrid = (InfiniteHexGrid) _gridSystem.Grid;

            if (_infGrid != null && _infGrid.SetYGraph == null)
                _infGrid.SetYGraph = YGraph;

            _metronome.SetElapsedTime((midiSequencer.SampleTime / (float) midiStreamSynthesizer.samplesperBuffer) * 0.02f);

            if (midiSequencer.isPlaying && !ShouldPlayFile)
            {
                midiSequencer.Stop(true);
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