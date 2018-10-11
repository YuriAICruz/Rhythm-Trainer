using Graphene.Grid;
using UnityEngine;

namespace Graphene.Rhythm.Game
{
    public class Player : MonoBehaviour
    {
        public float Speed = 3;
        public float TurnSpeed;
        public int ViewArea = 3;
        public bool Climbing;

        private IGridInfo _gr;
        private GridSystem _grid;
        private Vector3 _position;
        private InfiniteHexGrid.FloatFunc GetY;
        private Vector3 _lastPosition;
        private Vector2 _direction;

        private Transform camera;
        private float _iniSpeed;


        private void Start()
        {
            _grid = FindObjectOfType<GridSystem>();
            
            _grid.gameObject.GetComponent<TrailSystem>().SetTarget(transform);

            _iniSpeed = Speed;

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

            Metronome.Beat += Beat;
        }


        private void Update()
        {
            _position.x += Speed * Time.deltaTime;
            _position.z += _direction.x * TurnSpeed * Time.deltaTime;
            _position.y = GetY(_position);
            CheckGrid();
            transform.position = _position;

            LookDir();

            if (Climbing)
            {
                Speed = _iniSpeed;
            }
            else
            {
                Speed += Time.deltaTime;
            }

            _lastPosition = _position;

            GetInput();
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

            var advGr = _grid.Grid.GetPos(_gr.x+(int)(ViewArea/2), _gr.y);

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