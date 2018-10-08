using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Graphene.Rhythm
{
    [CreateAssetMenu(menuName = "Rhythm/SongData")]
    public class SongData : ScriptableObject
    {
        [Serializable]
        public struct NoteData
        {
            public int duration;
            public int note;
        }

        public string Name;
        
        public int frame;

        public List<NoteData> Notes;
    }
}