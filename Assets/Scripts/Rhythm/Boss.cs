using System;
using System.Collections;
using System.Xml;
using Graphene.Grid;
using Graphene.Rhythm.Game;
using Graphene.Rhythm.Presentation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Graphene.Rhythm
{
    public class Boss : MonoBehaviour
    {
        public event Action OnHit, OnDie;

        public int MaxHp;

        [HideInInspector] public int Hp;

        public float StartOffset = 100;

        public int ProjectilesCount;
        public float ProjectileSpeed;
        private float _iniProjectileSpeed;

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

        private int _count;

        [SerializeField] private Animator Animator;


        private float _colisionRadius = 1;
        private LayerMask _obstacleMask;
        private float _hitOffset;
        private bool _hited;
        private bool _isdead;

        private void Awake()
        {
            _iniProjectileSpeed = ProjectileSpeed;
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

            _obstacleMask = LayerMask.GetMask("Player");

            _projectiles = new Transform[ProjectilesCount];

            for (int i = 0; i < ProjectilesCount; i++)
            {
                _projectiles[i] = Instantiate(go).transform;
                _projectiles[i].position = Vector3.one * -1000;
            }

            _lastTime = _metronome.ElapsedTime;
            Hp = MaxHp;
        }

        private void Shoot(int index)
        {
            if(_isdead) return;
            
            _count++;
            
            Animator.SetInteger("BeatTempo", index);
            Animator.SetTrigger("Beat");
                
            if (index == 3)
                Animator.SetTrigger("Attack");
            
            if (_hited || _count < 6)
            {
                _hited = false;
                return;
            }

            if (Hp / (float) MaxHp < 0.4f)
            {
                Animator.SetTrigger("Attack");
                
                _projectiles[_current].gameObject.SetActive(true);
                _projectiles[_current].position = transform.position;

                _current = (_current + 1) % ProjectilesCount;
            }
            
            if (index != 0) return;

            _projectiles[_current].gameObject.SetActive(true);
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
            if(_isdead) return;
            if (_infGrid == null)
                _infGrid = (InfiniteHexGrid) _gridSystem.Grid;

            UpdatePosition();
            UpdateProjectilesPosition();
            CheckCollision();

            if (Input.GetKeyDown(KeyCode.Q))
            {
                Hit();
            }
        }

        private void UpdateProjectilesPosition()
        {
            var delta = _metronome.ElapsedTime - _lastTime;
            for (int i = 0; i < ProjectilesCount; i++)
            {
                if (!_projectiles[i].gameObject.activeSelf) continue;

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

            _position.x = (_metronome.ElapsedTime + StartOffset) * _gridSystem.Widith * _metronome.Bpm / 60f + _hitOffset; //Mathf.Lerp(_position.x, (_metronome.TotalBeats+20) * _grid.Widith, _t);
            _position.y = _infGrid.YGraph(_position);
            _position.z = Mathf.Lerp(_position.z, _lastPlayerZ + _side * _gridSystem.Widith, _t);

            var mul = (Hp / (float) MaxHp) < 0.4f ? 2 : 1;
            mul = (Hp / (float) MaxHp) < 0.05f ? 4: 1;
            _t += Time.deltaTime *mul;

            transform.position = _position;
        }

        private void CheckCollision()
        {
            var hits = Physics.SphereCastAll(_position, _colisionRadius, transform.forward, _colisionRadius, _obstacleMask);

            foreach (var hit in hits)
            {
                Hit();

                hit.transform.parent.gameObject.SetActive(false);

                return;
            }
        }

        private void Hit()
        {
            _hited = true;
            
            Hp--;

            if (Hp / (float) MaxHp < 0.4f)
                ProjectileSpeed = _iniProjectileSpeed*2; 
            if (Hp / (float) MaxHp < 0.1f)
                ProjectileSpeed = _iniProjectileSpeed*4; 
            
            OnHit?.Invoke();
            
            StartCoroutine(Hited());

            if (Hp <= 0)
            {
                Die();
            }
            else
            {
                Animator.SetTrigger("Hit");
                _metronome.PlayEvent(2);
            }
        }

        private void Die()
        {
            OnDie?.Invoke();

            _isdead = true;

            Animator.SetTrigger("Die");
            _metronome.PlayEvent(6);
            //_animator.SetTrigger("Dead");
            
            _menuManager.CollectCoin(100);

            Invoke(nameof(EndGame), 3);
        }

        private void EndGame()
        {
            _menuManager.EndGame(true);
            
            for (int i = 0; i < ProjectilesCount; i++)
            {
                _projectiles[i].gameObject.SetActive(false);
            }
        }

        private IEnumerator Hited()
        {
            var t = 0f;

            while (t < 1)
            {
                _hitOffset = Mathf.Lerp(0, 1, Mathf.Pow(Mathf.Sin(t * Mathf.PI), 2)) * 4;

                yield return null;

                t += Time.deltaTime * 2.5f;
            }
        }
    }
}