using System;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class AirHockeyPlayerMiley : AirHockeyPlayerBase
{
    public AirHockeyPuck Puck;
    public Transform _goalP1, _goalP2, _oppPad;

    private bool _skip = false;
    private int _currentDifficulty;
    private bool _firstHitDone = false;

    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        _skip = false;
        _firstHitDone = false;
        _currentDifficulty = (int) Academy.Instance.EnvironmentParameters.GetWithDefault("track", 0f);
        if (_currentDifficulty == 0)
        {
            if (!IsPlayer1)
            {
                //_skip = true;
            }
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (_skip)
            return;

        if (sensor == null)
        {
            Debug.LogWarning("Sensor is null!");
            return;
        }

        // 2 observations
        Vector2 toPuck = new Vector2(Puck.transform.position.x - transform.position.x, Puck.transform.position.z - transform.position.z);
        sensor.AddObservation(toPuck.normalized);

        // 4 observations
        Vector2 toGoalP1 = new Vector2(_goalP1.position.x - transform.position.x, _goalP1.position.z - transform.position.z);
        Vector2 toGoalP2 = new Vector2(_goalP2.position.x - transform.position.x, _goalP2.position.z - transform.position.z);
        sensor.AddObservation(toGoalP1);
        sensor.AddObservation(toGoalP2);

        // 6 observations
        Vector2 oppPadPos = new Vector2(_oppPad.position.x, _oppPad.position.z);
        Vector2 goalP1Pos = new Vector2(_goalP1.position.x, _goalP1.position.z);
        Vector2 goalP2Pos = new Vector2(_goalP2.position.x, _goalP2.position.z);
        sensor.AddObservation(oppPadPos);
        sensor.AddObservation(goalP1Pos);
        sensor.AddObservation(goalP2Pos);

        // 5 observations
        var myPos2D = new Vector2(transform.position.x, transform.position.z);
        var puckPos2D = new Vector2(Puck.transform.position.x, Puck.transform.position.z);
        sensor.AddObservation(myPos2D);
        sensor.AddObservation(puckPos2D);
        sensor.AddObservation(Vector2.Distance(myPos2D, puckPos2D));
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (_skip || isFreezon())
            return;
        base.OnActionReceived(vectorAction);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_skip)
            return;

        if (other.gameObject.CompareTag("AirHockeyPuck"))
        {
            if (!_firstHitDone)
                StartCoroutine(WaitABit(0.2f, () => { _firstHitDone = true; }));
            AddReward(0.2f);
        }
    }

    private IEnumerator WaitABit(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    private void FixedUpdate()
    {
        if (_currentDifficulty == 0)
        {
            if (_firstHitDone && Puck.GetSpeed() < 0.05f)
            {
                AddReward(-1f);
                Puck.Restart();
                //EndEpisode();
            }
        }
    }

    public override void Score(bool myVictory)
    {
        if (_skip)
            return;

        if (myVictory)
            AddReward(1f);
        //else
        //    AddReward(-0.2f);
    }
}
