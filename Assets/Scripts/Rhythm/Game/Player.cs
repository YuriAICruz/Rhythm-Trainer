using Graphene.Grid;
using UnityEngine;

namespace Graphene.Rhythm.Game
{
    public class Player : MonoBehaviour
    {
        public float TurnSpeed;
        public int ViewArea = 3;
        public bool Climbing;
        public LayerMask Mask;

        private IGridInfo _gr;
        private GridSystem _grid;
        private Vector3 _position;
        private InfiniteHexGrid.FloatFunc GetY;
        private Vector3 _lastPosition;
        private Vector2 _direction;

        private Transform camera;

        private Metronome _metronome;
        private int _iniBpm;
        private float _acceleration;

        private int count;


        private void Start()
        {
            _grid = FindObjectOfType<GridSystem>();

            _grid.gameObject.GetComponent<CoinGenerator>().SetTarget(transform);

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


        private void Update()
        {
            _position.x += _metronome.Bpm / 6f * Time.deltaTime;
            _position.z += _direction.x * TurnSpeed * Time.deltaTime;
            _position.y = GetY(_position);
            
            CheckGrid();
            GrabCoin();
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
            var hits = Physics.SphereCastAll(_position, 1, transform.forward, 1, Mask);

            foreach (var hit in hits)
            {
                hit.transform.gameObject.SetActive(false);
                CoinCollected();
            }
        }

        private void CoinCollected()
        {
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

            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        private void CheckGrid()
        {
            var gr = _grid.Grid.GetPos(_position);

            if (gr.x == _gr.x && gr.y == _gr.y) return;

            _gr = gr;
            count++;
            if(count < 3) return;
            count = 0;

            var advGr = _grid.Grid.GetPos(_gr.x + (int) (ViewArea-1), _gr.y);

            _grid.GenMesh(_grid.Grid.SelectRegion(advGr, ViewArea, false));
        }


        private void GetInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                DoAction();
            }

            _direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            _direction.x *= -1;
        }

        private void Beat(int index)
        {
            //throw new System.NotImplementedException();
        }

        private void DoAction()
        {
        }
    }
}