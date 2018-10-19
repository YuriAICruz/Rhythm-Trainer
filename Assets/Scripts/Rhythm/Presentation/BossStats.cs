using Graphene.Rhythm.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Graphene.Rhythm.Presentation
{
    public class BossStats : MonoBehaviour
    {
        public Image Hp;
        private Boss _boss;

        private void Start()
        {
            _boss = FindObjectOfType<Boss>();
            _boss.OnHit += UpdateData;

            UpdateData();
        }

        private void UpdateData()
        {
            Hp.fillAmount = _boss.Hp / (float) _boss.MaxHp;
        }
    }
}