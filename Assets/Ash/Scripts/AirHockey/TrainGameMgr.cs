using System;
using Unity.MLAgents;
using UnityEngine;
using System.Collections;


namespace Air_Hockey.Scripts
{
    public class TrainingGameManager : MonoBehaviour
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
    }
}
