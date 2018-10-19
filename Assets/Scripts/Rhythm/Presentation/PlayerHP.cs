using Graphene.Rhythm.Game;
using UnityEngine;

namespace Graphene.Rhythm.Presentation
{
    public class PlayerHP : MonoBehaviour
    {
        public GameObject[] Hearts;
        private Player _player;

        private void Start()
        {
            _player = FindObjectOfType<Player>();
            _player.OnHit += UpdateData;

            UpdateData();
        }

        private void UpdateData()
        {
            for (int i = 0; i < Hearts.Length; i++)
            {
                Hearts[i].SetActive(_player.Hp > i);
            }
        }
    }
}