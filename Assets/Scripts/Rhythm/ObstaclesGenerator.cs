using System.Linq;
using Graphene.Grid;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class ObstaclesGenerator : MonoBehaviour
    {
        private Transform _target;
        public int PoolSize;
        private GameObject[] _pool; 
        private TrailSystem _trail;
        private InfiniteHexGrid _infGrid;
        private GridSystem _gridSystem;
        public float Space = 4;
        private Metronome _metronome;

        private int _lastPos;
        private int _currentCoin;
        private bool _baseLineGenerated;

        private readonly int _mul = 6;
        private GameObject[] _obstacles;

        private void Awake()
        {
            _pool = new GameObject[PoolSize * _mul];

            _obstacles = Resources.LoadAll<GameObject>("Pool/Obstacles");

            for (int i = 0; i < PoolSize * _mul; i++)
            {
                _pool[i] = Instantiate(_obstacles[i%_obstacles.Length]);
                _pool[i].transform.position = Vector3.one * -1000;
            }

            _trail = FindObjectOfType<TrailSystem>();

            if (_gridSystem == null)
                _gridSystem = GetComponent<GridSystem>();

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;
            _metronome = FindObjectOfType<Metronome>();

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            if (_target == null || _gridSystem == null || _gridSystem.Grid == null) return;

            if (_infGrid == null)
                _infGrid = (InfiniteHexGrid) _gridSystem.Grid;

            if (_infGrid == null) return;

            var p = (int) ((_target.position.x + Space * (PoolSize / 4f)) / Space);

            if (p <= _lastPos)
                return;

            _lastPos = p;

            Draw(new Vector3(_lastPos * Space, 0, _target.position.z));
        }


        void Draw(Vector3 p)
        {
            var pos = new Vector3[]
            {
                new Vector3(p.x, 0, Mathf.Floor(p.z / Space) * Space),
                new Vector3(p.x, 0, Mathf.Floor(p.z / Space) * Space + _trail.Step),
                new Vector3(p.x, 0, Mathf.Floor(p.z / Space) * Space - _trail.Step),
            };

            var offset = Random.Range(-1, 1);

            for (int i = 0; i < pos.Length; i++)
            {
                var outPos = _trail.CoinMath(pos[i]);
                var split = Mathf.Abs(outPos[0].z - outPos[1].z) > 1f;

                outPos[0].z += offset * Space;
                outPos[0].y = _infGrid.YGraph(outPos[0]);

                
                _pool[_currentCoin + i * 2].transform.position = outPos[0];
                _pool[_currentCoin + i * 2].gameObject.SetActive(true);

                if (split)
                {
                    outPos[1].z += offset * Space;
                    outPos[1].y = _infGrid.YGraph(outPos[1]);
                    _pool[_currentCoin + i * 2 + 1].transform.position = outPos[1];
                    _pool[_currentCoin + i * 2 + 1].gameObject.SetActive(true);
                }
            }
            _currentCoin = (_currentCoin + _mul) % PoolSize * _mul;
        }
    }
}