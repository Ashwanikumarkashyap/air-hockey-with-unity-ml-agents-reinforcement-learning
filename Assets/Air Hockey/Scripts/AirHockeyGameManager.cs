using System;
using Unity.MLAgents;
using UnityEngine;
using System.Collections;


namespace Air_Hockey.Scripts
{
    public class AirHockeyGameManager : MonoBehaviour
    {
        public AirHockeyPlayerBase P1;
        public AirHockeyPlayerBase P2;
        public AirHockeyPuck Puck;

        private int _scoreP1;
        private int _scoreP2;

        public void PlayerScored(bool isP1)
        {
            P1.Score(isP1);
            P2.Score(!isP1);

            _scoreP1 += isP1 ? 1 : 0;
            _scoreP2 += isP1 ? 0 : 1;
            
            // Restart without ending any episode
            P1.Restart();
            P2.Restart();
            Puck.Restart();
        }

        public void Restart()
        {
            Academy.Instance.StatsRecorder.Add("Highscore Player 1", _scoreP1);
            Academy.Instance.StatsRecorder.Add("Highscore Player 2", _scoreP2);
            
            _scoreP1 = 0;
            _scoreP2 = 0;
            Puck.Restart();
        }

        /*
         * UI Code 
         * 
         */

        [Tooltip("Game ends after this many seconds have elapsed")]
        public float timerAmount = 60f;

        [Tooltip("The UI Controller")]
        public UIController uiController;

        [Tooltip("The main camera for the scene")]
        public Camera mainCamera;

        // When the game timer started
        private float gameTimerStartTime;

        public float maxScore = 10f;

        /// <summary>
        /// All possible game states
        /// </summary>
        public enum GameState
        {
            Default,
            MainMenu,
            Preparing,
            Playing,
            Gameover
        }

        /// <summary>
        /// The current game state
        /// </summary>
        public GameState State { get; private set; } = GameState.Default;

        ///// <summary>
        ///// Gets the time remaining in the game
        ///// </summary>
        public float TimeRemaining
        {
            get
            {
                if (State == GameState.Playing)
                {
                    float timeRemaining = timerAmount - (Time.time - gameTimerStartTime);
                    return Mathf.Max(0f, timeRemaining);
                }
                else
                {
                    return 0f;
                }
            }
        }

        /// <summary>
        /// Handles a button click in different states
        /// </summary>
        public void ButtonClicked()
        {
            if (State == GameState.Gameover)
            {
                // In the Gameover state, button click should go to the main menu
                MainMenu();

            }
            else if (State == GameState.MainMenu)
            {
                // In the MainMenu state, button click should start the game
                StartCoroutine(StartGame());
            }
            else
            {
                Debug.LogWarning("Button clicked in unexpected state: " + State.ToString());
            }
        }

        ///// <summary>
        ///// Called when the game starts
        ///// </summary>
        private void Start()
        {
            // Subscribe to button click events from the UI
            uiController.OnButtonClicked += ButtonClicked;

            // Start the main menu
            MainMenu();
        }

        ///// <summary>
        ///// Called on destroy
        ///// </summary>
        private void OnDestroy()
        {
            // Unsubscribe from button click events from the UI
            uiController.OnButtonClicked -= ButtonClicked;
        }

        ///// <summary>
        ///// Shows the main menu
        ///// </summary>
        private void MainMenu()
        {
            // Set the state to "main menu"
            State = GameState.MainMenu;

            // Update the UI
            uiController.ShowBanner("");
            uiController.ShowButton("Start");

            // Use the main camera, disable agent cameras
            //mainCamera.gameObject.SetActive(true);
            //player.agentCamera.gameObject.SetActive(false);
            //opponent.agentCamera.gameObject.SetActive(false); // Never turn this back on

            // Reset the agents
            P1.OnEpisodeBegin();
            P2.OnEpisodeBegin();

            // Freeze the agents
            P1.FreezeAgent();
            P2.FreezeAgent();
            uiController.setScoreP1(0);
            uiController.setScoreP2(0);
        }

        ///// <summary>
        ///// Starts the game with a countdown
        ///// </summary>
        ///// <returns>IEnumerator</returns>
        private IEnumerator StartGame()
        {
            // Set the state to "preparing"
            State = GameState.Preparing;

            // Update the UI (hide it)
            uiController.ShowBanner("");
            uiController.HideButton();

            // Use the player camera, disable the main camera
            //mainCamera.gameObject.SetActive(false);
            //player.agentCamera.gameObject.SetActive(true);

            // Show countdown
            uiController.ShowBanner("3");
            yield return new WaitForSeconds(1f);
            uiController.ShowBanner("2");
            yield return new WaitForSeconds(1f);
            uiController.ShowBanner("1");
            yield return new WaitForSeconds(1f);
            uiController.ShowBanner("Go!");
            yield return new WaitForSeconds(1f);
            uiController.ShowBanner("");

            // Set the state to "playing"
            State = GameState.Playing;

            // Start the game timer
            gameTimerStartTime = Time.time;

            // Unfreeze the agents
            P1.UnfreezeAgent();
            P2.UnfreezeAgent();
        }

        ///// <summary>
        ///// Ends the game
        ///// </summary>
        private void EndGame()
        {
            // Set the game state to "game over"
            State = GameState.Gameover;

            // Freeze the agents
            P1.FreezeAgent();
            P2.FreezeAgent();
            uiController.setScoreP1(0);
            uiController.setScoreP2(0);

            // Update banner text depending on win/lose
            if (_scoreP1 >= _scoreP2)
            {
                uiController.ShowBanner("ML Agent Wins!");
            }
            else
            {
                uiController.ShowBanner("You Win!");
            }

            // Update button text
            uiController.ShowButton("Main Menu");
        }

        ///// <summary>
        ///// Called every frame
        ///// </summary>
        private void Update()
        {
            Debug.Log("P1 Score: " + _scoreP1);
            Debug.Log("P2 Score: " + _scoreP2);
            if (State == GameState.Playing)
            {
                // Check to see if time has run out or either agent got the max nectar amount
                if (TimeRemaining <= 0f ||
                    _scoreP1 >= maxScore ||
                    _scoreP2 >= maxScore)
                {
                    EndGame();
                }

                // Update the timer and nectar progress bars
                uiController.SetTimer(TimeRemaining);
                uiController.SetPlayerNectar(_scoreP1 / maxScore);
                uiController.setScoreP1(_scoreP1);
                uiController.setScoreP2(_scoreP2);
                uiController.SetOpponentNectar(_scoreP2 / maxScore);
            }
            else if (State == GameState.Preparing || State == GameState.Gameover)
            {
                // Update the timer
                uiController.SetTimer(TimeRemaining);
            }
            else
            {
                // Hide the timer
                uiController.SetTimer(-1f);

                // Update the progress bars
                uiController.SetPlayerNectar(0f);
                uiController.SetOpponentNectar(0f);
            }

        }

    }
}
