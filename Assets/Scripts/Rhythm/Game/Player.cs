using System;
using Graphene.Grid;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm.Game
{
    public class Player : MonoBehaviour
    {
        public float TurnSpeed;
        public int ViewArea = 3;
        public bool Climbing;
        private LayerMask _coinMask;
        private LayerMask _obstacleMask;

        public Animator Animator;

        private IGridInfo _gr;
        private GridSystem _grid;
        private Vector3 _position;
        private InfiniteHexGrid.FloatFunc GetY;
        private Vector3 _lastPosition;

        private Transform camera;

        private Metronome _metronome;
        private int _iniBpm;
        private float _acceleration;

        private int count;
        private MenuManager _menuManager;
        private bool _playing, _playOk;
        private Vector3 _iniPos;
        private float _movement;
        private float _lastMove;


        private void Start()
        {
            _grid = FindObjectOfType<GridSystem>();
            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnStartGame += StartGame;
            _menuManager.OnRestartGame += RestartGame;

            _coinMask = (LayerMask.GetMask("Coin"));
            _obstacleMask = (LayerMask.GetMask("Obstacle"));

            _iniPos = transform.position;

            _grid.gameObject.GetComponent<CoinGenerator>().SetTarget(transform);
            _grid.gameObject.GetComponent<ObstaclesGenerator>().SetTarget(transform);

            camera = Camera.main.transform;

            if (_grid.Grid == null)
            {
                _grid.GenInfHexGrid();
                var grid = (InfiniteHexGrid) _grid.Grid;
                GetY = grid.YGraph;
            }

            var ray = new Ray(transform.position, Vector3.down);
            _gr = _grid.Grid.GetPos(ray);

            _position = _gr.worldPos;

            transform.position = _position;

            _grid.GenMesh(_grid.Grid.SelectRegion(_gr, ViewArea, false));

            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += Beat;
            _iniBpm = _metronome.Bpm;
        }

        private void StartGame()
        {
            Animator.SetFloat("Speed", 2);
            _playOk = true;
        }

        private void RestartGame()
        {
            Debug.Log(_iniPos);

            transform.position = _iniPos;

            var ray = new Ray(transform.position, Vector3.down);
            _gr = _grid.Grid.GetPos(ray);

            _position = _gr.worldPos;

            _grid.GenMesh(_grid.Grid.SelectRegion(_gr, ViewArea, false));

            StartGame();
        }

        private void Update()
        {
            if (!_playing) return;

            _position.x += _metronome.Bpm / 6f * Time.deltaTime;
            _position.z += _movement * Time.deltaTime;
            _position.y = GetY(_position);

            _movement = Mathf.Lerp(_movement, 0, Time.deltaTime * (_metronome.Bpm / 60f));

            CheckGrid();
            GrabCoin();
            CheckCollision();
            transform.position = _position;

            LookDir();

            if (Climbing)
            {
                //_metronome.Bpm = _iniBpm + (int)(_acceleration);
                _acceleration = Mathf.Max(0, _acceleration - Time.deltaTime);
            }
            else
            {
                _acceleration += Time.deltaTime;
                //_metronome.Bpm = Mathf.Min(_iniBpm*2, _iniBpm + (int)(_acceleration));
            }

            _lastPosition = _position;

            GetInput();
        }

        private void GrabCoin()
        {
            var hits = Physics.SphereCastAll(_position, 1, transform.forward, 1, _coinMask);

            foreach (var hit in hits)
            {
                hit.transform.gameObject.SetActive(false);
                CoinCollected();
            }
        }

        private void CheckCollision()
        {
            var hits = Physics.SphereCastAll(_position, 1, transform.forward, 1, _obstacleMask);

            foreach (var hit in hits)
            {
                Die();
                return;
            }
        }

        private void Die()
        {
            _metronome.PlayEvent(1);
            Animator.SetTrigger("Dead");
            Animator.SetFloat("Speed", 0);
            _playing = false;

            Invoke(nameof(EndGame), 1);
        }

        private void EndGame()
        {
            _menuManager.EndGame();

            var ray = new Ray(_iniPos, Vector3.down);
            _gr = _grid.Grid.GetPos(ray);

            _position = _gr.worldPos;

            transform.position = _iniPos;
        }

        private void CoinCollected()
        {
            _menuManager.CollectCoin(1);
            _metronome.PlayEvent(0);
        }

        private void LookDir()
        {
            var dir = _position - _lastPosition;

            if (dir.y > 0)
            {
                Climbing = true;
            }
            else
            {
                Climbing = false;
            }
            Animator.SetBool("Climbing", Climbing);

            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        private void CheckGrid()
        {
            var gr = _grid.Grid.GetPos(_position);

            if (gr.x == _gr.x && gr.y == _gr.y) return;

            _gr = gr;
            count++;
            if (count < 3) return;
            count = 0;

            var advGr = _grid.Grid.GetPos(_gr.x + (int) (ViewArea - 1), _gr.y);

            _grid.GenMesh(_grid.Grid.SelectRegion(advGr, ViewArea, false));
        }


        private void GetInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                DoAction();
            }
            if (Input.GetButtonDown("Horizontal"))
            {
                var l = _metronome.GetLapse();

                l = Mathf.Sin(l * Mathf.PI);

                if (l < 0.4f)
                {
                    MoveTo(-Input.GetAxis("Horizontal"));
                }
            }
        }

        private void MoveTo(float value)
        {
            if(Time.time - _lastMove < 0.4f) return;
            
            _lastMove = Time.time;
            _movement = Mathf.Sign(value) * TurnSpeed;
        }

        private void Beat(int index)
        {
            if (index != 0 || !_playOk) return;

            _playing = true;
        }

        private void DoAction()
        {
        }
    }
}