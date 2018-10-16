using Graphene.Rhythm.Game;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class CameraSimpleFollow : MonoBehaviour
    {
        public Transform Target;

        public Vector3 Offset;
        //public Vector3 BackOffset;

        private float _speed = 8;
        private MenuManager _menuManager;
        private Vector3 _iniPos;
        private Quaternion _iniRot;

        private void Start()
        {
            if (Target == null)
                Target = FindObjectOfType<Player>().transform;


            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnRestartGame += RestartGame;

            _iniPos = transform.position;
            _iniRot = transform.rotation;
        }

        private void RestartGame()
        {
            transform.position = _iniPos;
            transform.rotation = _iniRot;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, Target.transform.position + Offset, Time.deltaTime * _speed);
            
            var dir = transform.position - Target.transform.position + Offset;

            dir.z = 0;
            dir.Normalize();
            
            //transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
        }
    }
}