using Graphene.Rhythm.Game;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class CameraSimpleFollow : MonoBehaviour
    {
        public Player Target;
        public Vector3 Offset;
        //public Vector3 BackOffset;
        
        private float _speed = 2;

        private void Start()
        {
            if (Target == null)
                Target = FindObjectOfType<Player>();
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, Target.transform.TransformPoint(Offset), Time.deltaTime*_speed);
            var dir = transform.position- Target.transform.position + Offset;

            dir.z = 0;
            dir.Normalize();
            transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
        }
    }
}