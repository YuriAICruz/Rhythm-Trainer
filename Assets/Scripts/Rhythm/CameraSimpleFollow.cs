using Graphene.Rhythm.Game;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class CameraSimpleFollow : MonoBehaviour
    {
        public Player Target;
        public Vector3 Offset;
        public Vector3 BackOffset;
        public Vector3 Rotation;
        public Vector3 BackRotation;
        
        private Quaternion _rotation;
        private Quaternion _backRotation;
        private float _speed = 2;

        private void Start()
        {
            if (Target == null)
                Target = FindObjectOfType<Player>();

            _rotation = Quaternion.Euler(Rotation);
            _backRotation = Quaternion.Euler(BackRotation);
        }

        private void Update()
        {
            Vector3 dir;
            if (Target.Climbing)
            {
                transform.position = Vector3.Lerp(transform.position, Target.transform.position + BackOffset, Time.deltaTime*_speed);
                dir = transform.position- Target.transform.position + BackOffset;
                transform.rotation = Quaternion.Lerp(transform.rotation, _backRotation, Time.deltaTime*_speed);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, Target.transform.position + Offset, Time.deltaTime*_speed);
                dir = transform.position- Target.transform.position + Offset;
                transform.rotation = Quaternion.Lerp(transform.rotation, _rotation, Time.deltaTime*_speed);
            }
        }
    }
}