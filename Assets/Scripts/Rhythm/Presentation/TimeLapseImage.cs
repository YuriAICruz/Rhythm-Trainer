using Graphene.UiGenerics;
using UnityEngine;

namespace Graphene.Rhythm.Presentation
{
    public class TimeLapseImage : ImageView
    {
        private Metronome _metronome;

        public bool OneMinus;
        public bool Absolut;

        public float Dist;
        public Vector3 Dir;
        private Vector3 _iniPos;

        private void Start()
        {
            _metronome = FindObjectOfType<Metronome>();

            _iniPos = transform.position;
        }

        private void Update()
        {
            var l = _metronome.GetLapse();
            if (!Absolut)
            {
                l = Mathf.Sin(l * Mathf.PI);
            }

            if (OneMinus)
                l = 1 - l;
            
            //Image.color = new Color(1, 1, 1, l);
            
            

            transform.position = _iniPos + Dir * Dist * l;
        }
    }
}