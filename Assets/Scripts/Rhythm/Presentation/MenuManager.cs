using System;
using Graphene.UiGenerics;
using UnityEngine;
using UnityEngine.UI;

namespace Graphene.Rhythm.Presentation
{
    public class MenuManager : MonoBehaviour
    {
        public event Action OnStartGame, OnRestartGame;

        public CanvasGroupView MainMenu;
        public CanvasGroupView GameOver;
        public CanvasGroupView ScoreScreen;

        public Text[] Coins;

        int _coins;

        private void Start()
        {
            _coins = 0;
            MainMenu.Show();
            GameOver.Hide();
            ScoreScreen.Hide();
        }

        public void EndGame()
        {
            MainMenu.Hide();
            GameOver.Hide();
            ScoreScreen.Show();

            Invoke(nameof(RestartScreen), 3);
        }

        void RestartScreen()
        {
            MainMenu.Hide();
            GameOver.Show();
            ScoreScreen.Hide();
        }

        private void UpdateCoins()
        {
            foreach (var coin in Coins)
            {
                coin.text = _coins.ToString("000");
            }
        }

        public void RestartGame()
        {
            _coins = 0;
            MainMenu.Hide();
            GameOver.Hide();
            ScoreScreen.Hide();

            OnStartGame?.Invoke();
        }

        public void StartGame()
        {
            UpdateCoins();
            MainMenu.Hide();
            GameOver.Hide();
            ScoreScreen.Hide();

            OnRestartGame?.Invoke();
        }

        public void CollectCoin(int value)
        {
            _coins += value;
            UpdateCoins();
        }
    }
}