using Graphene.Grid;
using Graphene.Rhythm.Game;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class Boss : MonoBehaviour
    {
        public float StartOffset = 100;

        public int ProjectilesCount;
        public float ProjectileSpeed;

        [SerializeField] [Range(0, 1)] private float _probability = 0.5f;

        private MenuManager _menuManager;
        private TrailSystem _trail;
        private GridSystem _gridSystem;
        private InfiniteHexGrid _infGrid;
        private Metronome _metronome;

        private Vector3 _position;
        private int _side;
        private Player _player;
        private float _t;
        private float _lastPlayerZ;

        private Transform[] _projectiles;

        private int _current;
        private float _lastTime;


        private void Awake()
        {
            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnStartGame += StartGame;
            _menuManager.OnRestartGame += RestartGame;

            _gridSystem = FindObjectOfType<GridSystem>();

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;

            _metronome = _gridSystem.GetComponent<Metronome>();
            _metronome.Beat += Shoot;

            _trail = _gridSystem.GetComponent<TrailSystem>();

            _position = transform.position;

            _player = FindObjectOfType<Player>();

            var go = Resources.Load<GameObject>("Boss/Projectile");

            _projectiles = new Transform[ProjectilesCount];

            for (int i = 0; i < ProjectilesCount; i++)
            {
                _projectiles[i] = Instantiate(go).transform;
                _projectiles[i].position = Vector3.one * -1000;
            }

            _lastTime = _metronome.ElapsedTime;
        }

        private void Shoot(int index)
        {
            if (index != 0) return;
            
            if (Random.Range(0, 100) / 100f < _probability) return;

            _projectiles[_current].position = transform.position;

            _current = (_current + 1) % ProjectilesCount;
        }


        private void RestartGame()
        {
        }

        private void StartGame()
        {
            _current = 0;
        }

        private void Update()
        {
            if (_infGrid == null)
                _infGrid = (InfiniteHexGrid) _gridSystem.Grid;

            UpdatePosition();
            UpdateProjectilesPosition();
        }

        private void UpdateProjectilesPosition()
        {
            var delta = _metronome.ElapsedTime - _lastTime;
            for (int i = 0; i < ProjectilesCount; i++)
            {
                _projectiles[i].position = new Vector3(_projectiles[i].position.x - delta * ProjectileSpeed, _infGrid.YGraph(_projectiles[i].position), _projectiles[i].position.z);
            }
            _lastTime = _metronome.ElapsedTime;
        }

        private void UpdatePosition()
        {
            if (_t > 1)
            {
                _t = 0;
                _lastPlayerZ = _player.transform.position.z;
                _side = (int) Mathf.Sign(Random.Range(-1, 1));
            }

            _position.x = (_metronome.ElapsedTime + StartOffset) * _gridSystem.Widith * _metronome.Bpm / 60f; //Mathf.Lerp(_position.x, (_metronome.TotalBeats+20) * _grid.Widith, _t);
            _position.y = _infGrid.YGraph(_position);
            _position.z = Mathf.Lerp(_position.z, _lastPlayerZ + _side * _gridSystem.Widith, _t);

            _t += Time.deltaTime;

            transform.position = _position;
        }
    }
}