using Graphene.UiGenerics;
using UnityEngine;

namespace Graphene.Rhythm.Presentation
{
    public class TimeLapseImage : ImageView
    {
        private Metronome _metronome;

        public float Dist;
        public Vector3 Dir;
        private Vector3 _iniPos;
        private Vector3 _scale;
        private Vector3 _iniScale;
        private float _lastTime;

        private void Start()
        {
            _metronome = FindObjectOfType<Metronome>();

            _iniPos = transform.position;
            _iniScale = transform.localScale;
            _scale = _iniScale  + Vector3.up * 1.4f;
        }

        private void Update()
        {
            var l = _metronome.GetLapse();
            l = Mathf.Sin(l * Mathf.PI);
            
            //Image.color = new Color(1, 1, 1, l);

            transform.localScale = Vector3.Lerp(_scale, _iniScale, l);
            
            transform.position = _iniPos + Dir * Dist * l;
        }
    }
}