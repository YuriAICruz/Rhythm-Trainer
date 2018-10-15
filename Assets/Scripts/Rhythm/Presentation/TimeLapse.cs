using Graphene.UiGenerics;

namespace Graphene.Rhythm.Presentation
{
    public class TimeLapse : TextView
    {
        private Metronome _metronome;

        private void Start()
        {
            _metronome = FindObjectOfType<Metronome>();
        }

        private void Update()
        {
            Text.text = _metronome.GetLapse().ToString("0.000");
        }
    }
}