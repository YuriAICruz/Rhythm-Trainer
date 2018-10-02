using UnityEngine;

namespace DefaultNamespace
{
    public class TimeBar : MonoBehaviour
    {
        public SongData Song;
        private float _initZ;

        private void Awake()
        {
            Metronome.Beat += OnBeat;
            GenerateBar();
            _initZ = transform.position.z;
        }

        private void GenerateBar()
        {
            var obj = Resources.Load("Notes/Note");
            var noteDur = 60 / (float) Metronome.Bpm;
            var time = Metronome.Tempo * noteDur;
            
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
            transform.position = new Vector3(transform.position.x, transform.position.y, _initZ-Metronome.ElapsedTime);
        }

        private void OnBeat(int obj)
        {
        }
    }
}