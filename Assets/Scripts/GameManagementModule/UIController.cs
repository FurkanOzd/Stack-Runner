using DG.Tweening;
using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace GameManagementModule
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _tapToPlayText;

        [SerializeField]
        private Button _tapToPlayButton;

        [SerializeField]
        private GameObject _levelSuccessPanel;
        [SerializeField]
        private GameObject _levelFailPanel;

        [SerializeField]
        private float _tapToPlayFadeDuration;

        [Inject]
        private readonly SignalBus _signalBus;

        private void Start()
        {
            ListenEvents();
            ToggleTapToPlay(true, GameState.ReadyToPlay);
        }

        private void ListenEvents()
        {
            _signalBus.Subscribe<GameStateChangedSignal>(OnGameStateChanged);
            _tapToPlayButton.onClick.AddListener(OnTapToPlayButtonClick);
        }

        private void OnTapToPlayButtonClick()
        {
            gameObject.SetActive(false);
        }

        private void OnGameStateChanged(GameStateChangedSignal gameStateChangedSignal)
        {
            GameState gameState = gameStateChangedSignal.GameState;
            
            bool toggleUI = gameState == GameState.Fail || gameState == GameState.Success;
            
            switch (gameStateChangedSignal.GameState)
            {
                case GameState.Fail:
                    ToggleLevelFailedPanel(true);
                    ToggleTapToPlay(true, gameStateChangedSignal.GameState);
                    break;
                case GameState.Success:
                    ToggleLevelSuccessPanel(true);
                    ToggleTapToPlay(true, gameStateChangedSignal.GameState);
                    break;
                case GameState.Playing:
                    ToggleLevelFailedPanel(false);
                    ToggleLevelSuccessPanel(false);
                    ToggleTapToPlay(false, gameStateChangedSignal.GameState);
                    break;
            }
            
            gameObject.SetActive(toggleUI);
        }

        private void ToggleTapToPlay(bool toggle, GameState gameState)
        {
            const string LevelFailedButtonText = "Tap To Restart";
            const string LevelSuccessButtonText = "Tap To Play";

            _tapToPlayText.text = gameState == GameState.Fail 
                ? LevelFailedButtonText 
                : LevelSuccessButtonText;
            
            _tapToPlayText.DOKill();
            
            if (toggle)
            {
                _tapToPlayText.DOFade(1f, _tapToPlayFadeDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
            }
            
            _tapToPlayButton.gameObject.SetActive(toggle);
        }

        private void ToggleLevelSuccessPanel(bool toggle)
        {
            _levelSuccessPanel.SetActive(toggle);
        }

        private void ToggleLevelFailedPanel(bool toggle)
        {
            _levelFailPanel.SetActive(toggle);
        }

        private void UnsubscribeFromEvents()
        {
            _signalBus.TryUnsubscribe<GameStateChangedSignal>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }
    }
}