using Air_Hockey.Scripts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AirHockeyPuck : MonoBehaviour
{
    public AirHockeyGameManager GameManager;

    private Rigidbody _rigidbody;
    private Vector3 _startPosition;

    private void Awake()
    {
        Physics.bounceThreshold = 0f;

        _rigidbody = GetComponent<Rigidbody>();
        _startPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.PlayerScored(other.CompareTag("AirHockeyGoalP2"));
    }

    public void Restart()
    {
        _rigidbody.velocity = Vector3.zero;
        transform.position = _startPosition;
    }

    public double GetSpeed()
    {
        return _rigidbody.velocity.magnitude;
    }
}
