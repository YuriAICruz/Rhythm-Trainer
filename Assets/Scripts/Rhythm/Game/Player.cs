using UnityEngine;

namespace DefaultNamespace.Game
{
    public class Player : MonoBehaviour
    {
        private void Awake()
        {
            
        }

        private void Start()
        {
            Metronome.Beat += Beat;
        }

        private void Update()
        {
            GetInput();
        }

        private void GetInput()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                DoAction();
            }
        }

        private void Beat(int index)
        {
            throw new System.NotImplementedException();
        }

        private void DoAction()
        {
            
        }
    }
}