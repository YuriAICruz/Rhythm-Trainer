using System;
using Graphene.Grid;
using Graphene.Rhythm.Presentation;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Graphene.Rhythm.Game
{
    public class Player : MonoBehaviour
    {
        public event Action OnHit, OnDie;

        public int MaxHp = 3;
        [HideInInspector] public int Hp;

        public float ColisionRadius = 0.6f;
        public int ViewArea = 3;
        public bool Climbing;

        public Animator Animator;

        [Space] public int ProjectilesCount;
        public float ProjectileSpeed;


        private LayerMask _coinMask;
        private LayerMask _obstacleMask;

        private IGridInfo _gr;
        private GridSystem _grid;
        private MenuManager _menuManager;
        private Metronome _metronome;

        private InfiniteHexGrid.FloatFunc GetY;

        private Vector3 _lastPosition;
        private Vector3 _position;

        private float _acceleration;

        private int count;
        private bool _playing, _playOk;
        private Vector3 _iniPos;
        private float _travelZ;
        private float _lastMove;
        private float _t = 0;
        private bool _hit;

        private Transform[] _projectiles;
        private int _current;
        private float _lastTime;
        private Collider _lastHit;
        private float _coinCollected;
        private int _combo;
        private float _inivicible;


        private void Awake()
        {
            var go = Resources.Load<GameObject>("Player/Projectile");
            _projectiles = new Transform[ProjectilesCount];

            for (int i = 0; i < ProjectilesCount; i++)
            {
                _projectiles[i] = Instantiate(go).transform;
                _projectiles[i].position = Vector3.one * -1000;
            }

            _coinMask = (LayerMask.GetMask("Coin"));
            _obstacleMask = (LayerMask.GetMask("Obstacle"));

            Hp = MaxHp;
        }

        private void Start()
        {
            _grid = FindObjectOfType<GridSystem>();
            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnStartGame += StartGame;
            _menuManager.OnRestartGame += RestartGame;
            _menuManager.OnCoinEvent += AddLife;

            _iniPos = transform.position;

            _grid.gameObject.GetComponent<CoinGenerator>().SetTarget(transform);
            _grid.gameObject.GetComponent<ObstaclesGenerator>().SetTarget(transform);

            if (_grid.Grid == null)
            {
                _grid.GenInfHexGrid();
                var grid = (InfiniteHexGrid) _grid.Grid;
                GetY = grid.YGraph;
            }

            var ray = new Ray(transform.position, Vector3.down);
            _gr = _grid.Grid.GetPos(ray);

            SetPos();

            transform.position = _position;

            _grid.GenMesh(_grid.Grid.SelectRegion(_gr, ViewArea, false));

            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += Beat;
        }

        private void AddLife()
        {
            if (Hp == MaxHp) return;

            Hp++;
            OnHit?.Invoke();
        }

        private void SetPos()
        {
            _position = _gr.worldPos;
            _travelZ = Mathf.Floor(_position.z / _grid.Widith) * _grid.Widith;
        }

        private void StartGame()
        {
            if (_metronome != null)
                _lastTime = _metronome.ElapsedTime;
            _playOk = true;
        }

        private void RestartGame()
        {
            Debug.Log(_iniPos);

            transform.position = _iniPos;

            var ray = new Ray(transform.position, Vector3.down);
            _gr = _grid.Grid.GetPos(ray);

            SetPos();

            _grid.GenMesh(_grid.Grid.SelectRegion(_gr, ViewArea, false));

            StartGame();
        }

        private void Update()
        {
            if (!_playing) return;

            _position.x = (_metronome.ElapsedTime + 20) * _grid.Widith * _metronome.Bpm / 60f; //Mathf.Lerp(_position.x, (_metronome.TotalBeats+20) * _grid.Widith, _t);
            _position.z = Mathf.Lerp(_position.z, _travelZ, _t);
            _position.y = GetY(_position);

            _t += _metronome.Bpm / 60f * Time.deltaTime;

            CheckGrid();
            GrabCoin();
            CheckCollision();
            UpdateProjectilesPosition();
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

        private void UpdateProjectilesPosition()
        {
            var delta = _metronome.ElapsedTime - _lastTime;
            for (int i = 0; i < ProjectilesCount; i++)
            {
                if (!_projectiles[i].gameObject.activeSelf) continue;

                _projectiles[i].position = new Vector3(_projectiles[i].position.x + delta * ProjectileSpeed, GetY(_projectiles[i].position), _projectiles[i].position.z);
            }
            _lastTime = _metronome.ElapsedTime;
        }

        private void GrabCoin()
        {
            var hits = Physics.SphereCastAll(_position, ColisionRadius, transform.forward, ColisionRadius, _coinMask);

            foreach (var hit in hits)
            {
                hit.transform.gameObject.SetActive(false);
                CoinCollected();
            }
        }

        private void CheckCollision()
        {
            var hits = Physics.SphereCastAll(_position, ColisionRadius, transform.forward, ColisionRadius, _obstacleMask);

            foreach (var hit in hits)
            {
                if (hit.collider == _lastHit) continue;

                _lastHit = hit.collider;

                if (hit.transform.CompareTag("bullet"))
                    hit.transform.parent.gameObject.SetActive(false);

                Hit();
                return;
            }
        }

        private void Hit()
        {
            if(Time.time - _inivicible < 1) return;
            
            Hp -= 1;
            
            ResetCombo();

            _inivicible = Time.time;
            
            OnHit?.Invoke();

            if (Hp <= 0)
                Die();
            else
            {
                Animator.SetTrigger("Hit");
                _metronome.PlayEvent(1);
            }
        }

        private void Die()
        {
            OnDie?.Invoke();

            _metronome.PlayEvent(5);
            Animator.SetTrigger("Dead");
            _playing = false;
            _playOk = false;

            Invoke(nameof(EndGame), 1);
        }

        private void EndGame()
        {
            _menuManager.EndGame();

            var ray = new Ray(_iniPos, Vector3.down);
            _gr = _grid.Grid.GetPos(ray);

            SetPos();

            transform.position = _iniPos;
        }

        private void CoinCollected()
        {
            _combo += 1;

            _coinCollected = Time.time;
            _menuManager.Combo(_combo);
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

            // transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        private void CheckGrid()
        {
            var gr = _grid.Grid.GetPos(_position);

            if (gr.x == _gr.x && gr.y == _gr.y) return;

            _gr = gr;
            count++;
            if (count < 3) return;
            count = 0;

            var advGr = _grid.Grid.GetPos(_gr.x + (int) (ViewArea - 2), _gr.y);

            _grid.GenMesh(_grid.Grid.SelectRegion(advGr, ViewArea, false));
        }


        private void GetInput()
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                var l = _metronome.GetLapse();

                l = Mathf.Sin(l * Mathf.PI);

                if (l < 0.6f)
                {
                    _menuManager.HitFeedBack(l+1);
                    MoveTo(-Input.GetAxis("Horizontal"));
                    _hit = true;
                }
                else if (l > 0.6f)
                {
                    ResetCombo();
                }
            }
        }

        private void MoveTo(float value)
        {
            if (Time.time - _lastMove < 0.4f) return;

            _t = 0;

            _lastMove = Time.time;
            _travelZ = _position.z + Mathf.Sign(value) * _grid.Widith;

            Shoot();
        }

        private void Shoot()
        {
            _projectiles[_current].gameObject.SetActive(true);
            _projectiles[_current].position = _position;

            _current = (_current + 1) % ProjectilesCount;
        }

        private void Beat(int index)
        {
            Animator.SetInteger("BeatTempo", index);
            Animator.SetTrigger("Beat");

            if (Time.time - _coinCollected > 0.5f)
            {
                ResetCombo();
                _coinCollected = Time.time;
            }

            _t = 0;

            if (!_hit)
                _menuManager.HitFeedBack(0);

            _hit = false;

            if (index != 0 || !_playOk) return;

            _playing = true;
        }

        private void ResetCombo()
        {
            _combo = 0;
            _menuManager.ComboReset();
        }
    }
}