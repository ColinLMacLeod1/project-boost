using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Rocket : MonoBehaviour {

    enum State { Alive, Dying, Transitioning}
    State state = State.Alive;

    Rigidbody rigidBody;
    AudioSource audioSource;
    private RigidbodyConstraints rigidbodyConstraints;
    [SerializeField] float rotationSpeed = 200;
    [SerializeField] float thrustSpeed = 4;
    [SerializeField] float waitTime = 1;
    [SerializeField] AudioClip thrustAudio;
    [SerializeField] AudioClip explosionAudio;
    [SerializeField] AudioClip successAudio;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] ParticleSystem successParticles;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        rigidbodyConstraints = rigidBody.constraints;
        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        if(state == State.Alive) handleInput();  //TODO stop audio
    }

    private void handleInput()
    {
        Thrust();
        Rotate();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return;
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                state = State.Alive;
                print("OK");
                break;
            case "Fuel":
                print("Fuel");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        state = State.Transitioning;
        audioSource.Stop();
        audioSource.PlayOneShot(successAudio);
        successParticles.Play();
        Invoke("LoadNextScene", waitTime);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        thrustParticles.Stop();
        audioSource.PlayOneShot(explosionAudio);
        explosionParticles.Play();
        Invoke("Restart", waitTime);
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
        state = State.Alive;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1);
        state = State.Alive;
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
            rigidBody.AddRelativeForce(Vector3.up * thrustSpeed * Time.deltaTime);
            if (!audioSource.isPlaying) audioSource.PlayOneShot(thrustAudio);
            thrustParticles.Play();
        }
        else
        {
            audioSource.Stop();
            thrustParticles.Stop();

        }
    }
}
