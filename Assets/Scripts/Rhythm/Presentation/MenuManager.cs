using System;
using System.Collections;
using Graphene.UiGenerics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Graphene.Rhythm.Presentation
{
    public class MenuManager : MonoBehaviour
    {
        public event Action OnStartGame, OnRestartGame, OnGameOver, OnCoinEvent;

        public CanvasGroupView MainMenu;
        public CanvasGroupView GameOver;
        public CanvasGroupView ScoreScreen;

        public GameObject Win, Lose;

        public Transform FeedBack;

        public Text[] Coins;
        public Text[] Combos;

        public int _coins;
        public int _totalCoins;
        private bool _gameover;
        private float _t;
        private Vector3 _scale;
        private int _combo;

        private void Start()
        {
            _coins = 0;
            MainMenu.Show();
            GameOver.Hide();
            ScoreScreen.Hide();
        }

        public void EndGame(bool win = false)
        {
            Win.SetActive(win);
            Lose.SetActive(!win);
            
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
                SceneManager.LoadScene(0);
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
            StartCoroutine(Popup(Coins[0]));
            Coins[0].text = _coins.ToString("000");
            
            StartCoroutine(Popup(Coins[1]));
            Coins[1].text = _totalCoins.ToString("000");
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
            _coins += value * ((int) (1 + _combo / 4f));
            _totalCoins += value * ((int) (1 + _combo / 4f));

            if (_coins >= 100)
            {
                _coins -= 100;
                OnCoinEvent?.Invoke();
            }
            
            UpdateCoins();
        }

        public void HitFeedBack(float time)
        {
            _scale = Vector3.one * (2 - time);
            _t = 0;
        }

        public void Combo(int combo)
        {
            _combo = combo;

            UpdateCombos();
        }

        private void UpdateCombos()
        {
            foreach (var combo in Combos)
            {
                StartCoroutine(Popup(combo));
                combo.text = "x " + ((int) (1 + _combo / 4f)).ToString("00");
            }
        }

        IEnumerator Popup(Text combo)
        {
            var t = 0f;
            while (t < 1)
            {
                combo.transform.localScale = Vector3.Lerp(Vector3.one * 1.4f, Vector3.one, t);
                yield return null;
                t += Time.deltaTime;
            }
        }

        public void ComboReset()
        {
            _combo = 0;

            UpdateCombos();
        }
    }
}