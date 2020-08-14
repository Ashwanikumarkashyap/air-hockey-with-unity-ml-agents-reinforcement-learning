using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class QWOPController : Agent
{
	// Controls the legs
	public GameObject LeftThigh;
	public GameObject RightThigh;
	public GameObject LeftCalf;
	public GameObject RightCalf;

	// Movement speed of the legs
	public float MoveSpeed = 200.0f;

	// Whether we are in training
	public bool Training = false;

	// Keep track of the current legs state
	private float currentThighRotation = 45.0f;
	private float currentCalfRotation = 45.0f;

	// Whether we are frozen
	private bool frozen = false;

	// Rigidbody of the actor
	new private Rigidbody2D rigidbody;

	public override void Initialize()
	{
		rigidbody = gameObject.GetComponent<Rigidbody2D>();
	}

	public override void OnEpisodeBegin()
	{
		// Reset position and orientation
		gameObject.transform.localPosition = new Vector3(0.0f, 6.5f, 0.0f);
		gameObject.transform.localRotation = new Quaternion();
		// Set random position for the legs
		currentThighRotation = Random.Range(-45.0f, 45.0f);
		currentCalfRotation = Random.Range(0.0f, 90.0f);
		UpdateLegsRotations();

		// Reset the rigidbody state
		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0.0f;
		rigidbody.WakeUp();

		// Unfreeze the actor
		frozen = false;
	}

	// Update is called once per frame
	public override void Heuristic(float[] actionsOut)
	{
		// If nothing is pressed, keep actions at 0.0
		actionsOut[0] = 0.0f;
		actionsOut[1] = 0.0f;

		// If frozen, ignore all input
		if(frozen)
		{
			return;
		}

		// Control Thighs
        if(Input.GetKey(KeyCode.Q))
		{
			actionsOut[0] = -1.0f;
		}
		else if(Input.GetKey(KeyCode.W))
		{
			actionsOut[0] = 1.0f;
		}

		// Control Calves
		if (Input.GetKey(KeyCode.O))
		{
			actionsOut[1] = 1.0f;
		}
		else if (Input.GetKey(KeyCode.P))
		{
			actionsOut[1] = -1.0f;
		}

		// If not in training, reset
		if(!Training && Input.GetKey(KeyCode.R))
		{
			EndEpisode();
		}
	}

	public override void OnActionReceived(float[] vectorAction)
	{
		// Update the rotation of the legs based on inputs
		currentThighRotation = Mathf.Clamp(currentThighRotation + MoveSpeed * Time.deltaTime * vectorAction[0], -45.0f, 45.0f);
		currentCalfRotation = Mathf.Clamp(currentCalfRotation + MoveSpeed * Time.deltaTime * vectorAction[1], 0.0f, 90.0f);

		// Update their actual orientation
		UpdateLegsRotations();
	}

	public override void CollectObservations(VectorSensor sensor)
	{
		sensor.AddObservation(new float[1]);
	}

	private void UpdateLegsRotations()
	{
		// Update the rotations of the legs based on the current rotations
		LeftThigh.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, currentThighRotation);
		RightThigh.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -currentThighRotation);

		LeftCalf.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -90.0f + currentCalfRotation);
		RightCalf.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -currentCalfRotation);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Floor"))
		{
			// If the body hits the floor, game over
			frozen = true;
			rigidbody.Sleep();
			EndEpisode();
		}
	}
}
