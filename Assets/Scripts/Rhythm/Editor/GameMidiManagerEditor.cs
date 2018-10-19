using System.Collections;
using System.Collections.Generic;
using Graphene.Rhythm;
using UnityEditor;
using UnityEngine;

namespace Graphene.Rhythm
{
    [CustomEditor(typeof(GameMidiManager))]
    public class GameMidiManagerEditor : Editor
    {
        private GameMidiManager _self;

        private bool _showPlayer;

        private void Awake()
        {
            _self = (GameMidiManager) target;
        }

        public override void OnInspectorGUI()
        {
            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Show Player"))
                {
                    _showPlayer = !_showPlayer;
                }
                if(_showPlayer)
                    DynamicInspectorGUI();
                else
                    base.OnInspectorGUI();
            }
            else
            {
                base.OnInspectorGUI();
            }
        }

        public SoundEvent[] GameEvents;
        public SoundEvent[] Beep;

        private void DynamicInspectorGUI()
        {
            EditorGUILayout.LabelField("GameEvents");
            for (int i = 0; i < _self.GameEvents.Length; i++)
            {
                DrawSoundEvent(_self.GameEvents[i]);
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField("Beep");
            for (int i = 0; i < _self.Beep.Length; i++)
            {
                DrawSoundEvent(_self.Beep[i]);
            }
        }

        private void DrawSoundEvent(SoundEvent snd)
        {
            snd.Note = EditorGUILayout.IntField("Note", snd.Note);
            snd.Instrument = EditorGUILayout.IntField("Instrument", snd.Instrument);
            snd.Volume = EditorGUILayout.IntField("Volume", snd.Volume);
            snd.Duration = EditorGUILayout.FloatField("Duration", snd.Duration);

            if (GUILayout.Button("Play"))
            {
                _self.StartCoroutine(_self.DoBeep(0, snd.Note, snd.Duration, snd.Volume, snd.Instrument));
            }
        }
    }
}