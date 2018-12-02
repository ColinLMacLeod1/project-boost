using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;
    private RigidbodyConstraints rigidbodyConstraints;
    [SerializeField] float rotationSpeed = 200;
    [SerializeField] float thrustSpeed = 4;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        rigidbodyConstraints = rigidBody.constraints;
        audioSource = GetComponent<AudioSource>();

	}
	
	// Update is called once per frame
	void Update () {
        handleInput();
    }

    private void handleInput()
    {
        Thrust();
        Rotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                print("OK");
                break;
            default:
                print("Dead");
                break;
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true; // take manual control
        float rotationThisFrame = Time.deltaTime * rotationSpeed;
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward* rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward* rotationThisFrame);
        }
        rigidBody.freezeRotation = false; // release manual control
        rigidBody.constraints = rigidbodyConstraints;
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * thrustSpeed);
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
