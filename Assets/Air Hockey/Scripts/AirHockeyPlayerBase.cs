using System;
using Air_Hockey.Scripts;
using Unity.MLAgents;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AirHockeyPlayerBase : Agent
{
    private Rigidbody _rigidbody;

    public float MoveForce = 20f;
    public float MaxVelocity = 5f;

    public bool IsPlayer1 = false;
    public AirHockeyGameManager GameManager;

    private Vector3 _startPosition;
    private bool _frozen = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public override void Initialize()
    {
        _startPosition = transform.position;
    }

    public override void OnEpisodeBegin()
    {
        if (IsPlayer1)
            GameManager.Restart();
        
        transform.position = _startPosition;
        _rigidbody.velocity = Vector3.zero;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        float vertical = (IsPlayer1 ? -vectorAction[0] : vectorAction[0]) * MoveForce;
        float horizontal = (IsPlayer1 ? vectorAction[1] : -vectorAction[1]) * MoveForce;
        _rigidbody.AddForce(vertical, 0, horizontal);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Vertical");
        actionsOut[1] = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (_rigidbody.velocity.magnitude > MaxVelocity)
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * MaxVelocity;
        }
    }

    public virtual void Score(bool myVictory)
    {
        AddReward(myVictory ? 1f : -0.2f); // Feel free to override the Score method in your own version for a better score to give the agents
    }

    public void Restart()
    {
        _rigidbody.velocity = Vector3.zero;
        transform.position = _startPosition;
    }

    public void FreezeAgent()
    {
        _frozen = true;
        _rigidbody.Sleep();

    }

    public void UnfreezeAgent()
    {
        _frozen = false;
        _rigidbody.WakeUp();
    }

    public bool isFreezon()
    {
        return _frozen;
    }
}
