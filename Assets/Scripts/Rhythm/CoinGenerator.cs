using Graphene.Grid;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class CoinGenerator : MonoBehaviour
    {
        private Transform _target;
        public int CoinPool;
        private GameObject[] _coins;
        private TrailSystem _trail;
        private InfiniteHexGrid _infGrid;
        private GridSystem _gridSystem;
        public float Space = 4;
        private Metronome _metronome;

        private int _lastPos;
        private int _currentCoin;
        private bool _baseLineGenerated;

        private readonly int _mul = 6;
        private MenuManager _menuManager;

        private void Awake()
        {
            _coins = new GameObject[CoinPool * _mul];

            var go = Resources.Load<GameObject>("Pool/Coin");
            
            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnRestartGame += RestartGame;

            for (int i = 0; i < CoinPool * _mul; i++)
            {
                _coins[i] = Instantiate(go);
                _coins[i].transform.position = Vector3.one * -1000;
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

            var p = (int) ((_target.position.x + Space * (CoinPool / 4f)) / Space);

            if (p <= _lastPos)
                return;

            _lastPos = p;

            DrawCoins(new Vector3(_lastPos * Space, 0, _target.position.z));
        }

        private void RestartGame()
        {
            _lastPos = 0;
        }


        void DrawCoins(Vector3 p)
        {
            var pos = new Vector3[]
            {
                new Vector3(p.x, 0, Mathf.Floor(p.z / Space) * Space),
                new Vector3(p.x, 0, Mathf.Floor(p.z / Space) * Space + _trail.Step),
                new Vector3(p.x, 0, Mathf.Floor(p.z / Space) * Space - _trail.Step),
            };

            for (int i = 0; i < pos.Length; i++)
            {
                var outPos = _trail.CoinMath(pos[i]);
                var split = Mathf.Abs(outPos[0].z - outPos[1].z) > 3f;

                outPos[0].y = _infGrid.YGraph(outPos[0]);
                _coins[_currentCoin + i * 2].transform.position = outPos[0];
                _coins[_currentCoin + i * 2].gameObject.SetActive(true);

                if (split)
                {
                    outPos[1].y = _infGrid.YGraph(outPos[1]);
                    _coins[_currentCoin + i * 2 + 1].transform.position = outPos[1];
                    _coins[_currentCoin + i * 2 + 1].gameObject.SetActive(true);
                }
            }
            _currentCoin = (_currentCoin + _mul) % CoinPool * _mul;
        }
    }
}