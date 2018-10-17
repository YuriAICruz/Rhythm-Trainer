using System;
using Graphene.UiGenerics;
using UnityEngine;
using UnityEngine.UI;

namespace Graphene.Rhythm.Presentation
{
    public class MenuManager : MonoBehaviour
    {
        public event Action OnStartGame, OnRestartGame, OnGameOver;

        public CanvasGroupView MainMenu;
        public CanvasGroupView GameOver;
        public CanvasGroupView ScoreScreen;

        public Transform FeedBack;

        public Text[] Coins;

        int _coins;
        private bool _gameover;
        private float _t;
        private Vector3 _scale;

        private void Start()
        {
            _coins = 0;
            MainMenu.Show();
            GameOver.Hide();
            ScoreScreen.Hide();
        }

        public void EndGame()
        {
            OnGameOver?.Invoke();

            MainMenu.Hide();
            GameOver.Hide();
            ScoreScreen.Show();

            _gameover = true;
        }

        private void Update()
        {
            AnimateFeedback();
            
            if (!_gameover) return;

            if (Input.GetMouseButtonDown(0))
            {
                RestartScreen();
            }
        }

        private void AnimateFeedback()
        {
            FeedBack.localScale = Vector3.Lerp(_scale, Vector3.one, _t);

            _t += Time.deltaTime;
        }

        void RestartScreen()
        {
            _gameover = false;
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
            _gameover = false;

            _coins = 0;
            MainMenu.Hide();
            GameOver.Hide();
            ScoreScreen.Hide();

            OnRestartGame?.Invoke();
        }

        public void StartGame()
        {
            _gameover = false;

            UpdateCoins();
            MainMenu.Hide();
            GameOver.Hide();
            ScoreScreen.Hide();

            OnStartGame?.Invoke();
        }

        public void CollectCoin(int value)
        {
            _coins += value;
            UpdateCoins();
        }

        public void HitFeedBack(float time)
        {
            _scale = Vector3.one * (2 - time);
            _t = 0;
        }
    }
}