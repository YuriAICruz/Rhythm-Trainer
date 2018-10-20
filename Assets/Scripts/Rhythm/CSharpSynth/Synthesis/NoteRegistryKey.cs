using System;

namespace CSharpSynth.Synthesis
{
    //UnitySynth
    //public struct NoteRegistryKey : IEquatable<NoteRegistryKey>
    public struct NoteRegistryKey
    {
        //--Variables
        private readonly int note;
        private readonly int channel;
        //--Public Properties
        public int Note { get { return note; } }
        public int Channel { get { return channel; } }
        //--Public Methods
        public NoteRegistryKey(int channel, int note)
        {
            this.note = note;
            this.channel = channel;
        }
        public override bool Equals(object obj)
        {
            if (obj is NoteRegistryKey)
            {
                NoteRegistryKey r = (NoteRegistryKey)obj;
                return r.channel == this.channel && r.note == this.note;
            }
            return false;
        }
        public bool Equals(NoteRegistryKey obj)
        {
            return obj.channel == this.channel && obj.note == this.note;
        }
    }
}
