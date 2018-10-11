using UnityEngine;

namespace Graphene.Rhythm
{
    public class TimeBar : MonoBehaviour
    {
        public SongData Song;
        private float _initZ;
        private Metronome _metronome;

        private void Awake()
        {
            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += OnBeat;
            GenerateBar();
            _initZ = transform.position.z;
        }

        private void GenerateBar()
        {
            var obj = Resources.Load("Notes/Note");
            var noteDur = 60 / (float) _metronome.Bpm;
            var time = _metronome.Tempo * noteDur;

            foreach (var note in Song.Notes)
            {
                var tmp = (GameObject) Instantiate(obj);
                tmp.transform.SetParent(transform);

                tmp.transform.localPosition = new Vector3(0, 0, time);

                time += note.duration * noteDur;
            }
        }

        void Update()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, _initZ - _metronome.ElapsedTime);
        }

        private void OnBeat(int obj)
        {
        }
    }
}