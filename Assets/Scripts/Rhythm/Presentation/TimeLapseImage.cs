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
        private Vector3 _scale;
        private Vector3 _iniScale;
        private float _lastTime;

        private void Start()
        {
            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += Resize;

            _iniPos = transform.position;
            _iniScale = transform.localScale;
        }

        private void Resize(int obj)
        {
            _lastTime = Time.time;
            _scale = _iniScale  + Vector3.up * 1.4f;
            transform.localScale = _iniScale;
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

            transform.localScale = Vector3.Lerp(transform.localScale, _scale, Time.deltaTime*4);
            
            transform.position = _iniPos + Dir * Dist * l;
        }
    }
}