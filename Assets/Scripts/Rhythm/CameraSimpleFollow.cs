using Graphene.Rhythm.Game;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class CameraSimpleFollow : MonoBehaviour
    {
        public Player Target;
        public Vector3 Offset;
        private Quaternion _rotation;
        private Quaternion _backRotation;
        private float _speed = 5;
        private Vector3 _backOffset;

        private void Start()
        {
            if (Target == null)
                Target = FindObjectOfType<Player>();

            _rotation = transform.rotation;
            _backRotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x*0.5f, -transform.eulerAngles.y, transform.eulerAngles.z));
            
            _backOffset = new Vector3(-Offset.x, Offset.y, Offset.z);
        }

        private void Update()
        {
            Vector3 dir;
            if (Target.Climbing)
            {
                transform.position = Vector3.Lerp(transform.position, Target.transform.position + _backOffset, Time.deltaTime*_speed);
                dir = transform.position- Target.transform.position + _backOffset;
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