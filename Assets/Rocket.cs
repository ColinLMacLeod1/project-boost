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
    [SerializeField] float rotationSpeed = 250;
    [SerializeField] float thrustSpeed = 10000;
    [SerializeField] float waitTime = 1;

    [SerializeField] AudioClip thrustAudio;
    [SerializeField] AudioClip explosionAudio;
    [SerializeField] AudioClip successAudio;

    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] ParticleSystem successParticles;

    //Dev
    [SerializeField] bool collisions = true;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        rigidbodyConstraints = rigidBody.constraints;
        audioSource = GetComponent<AudioSource>();

    }
	
	// Update is called once per frame
	void Update () {
        if(state == State.Alive) HandleInput();
    }

    private void HandleInput()
    {
        Thrust();
        Rotate();
        if(Debug.isDebugBuild) RespondToDebugKeys();
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

    private void Rotate()
    {

        float rotationThisFrame = Time.deltaTime * rotationSpeed;
        if (Input.GetKey(KeyCode.A))
        {
            RotateManually(rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            RotateManually(-rotationThisFrame);
        }
        rigidBody.constraints = rigidbodyConstraints;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !collisions) return;
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                state = State.Alive;
                print("OK");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }


    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.C)) collisions = !collisions;
        else if (Input.GetKeyDown(KeyCode.L)) StartSuccessSequence();
    }

    private void StartSuccessSequence()
    {
        state = State.Transitioning;
        audioSource.Stop();
        audioSource.PlayOneShot(successAudio);
        successParticles.Play();
        print(SceneManager.sceneCountInBuildSettings);
        if(SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings-1)
        {
            Invoke("LoadNextScene", waitTime);
        }
        else
        {
            Invoke("Restart", waitTime);
        }

    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        thrustParticles.Stop();
        audioSource.PlayOneShot(explosionAudio);
        explosionParticles.Play();
        Invoke("ReloadScene", waitTime);
    }

    private void Restart()
    {
        SceneManager.LoadScene(0);
        state = State.Alive;
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        state = State.Alive;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void RotateManually(float rotationThisFrame)
    {
        rigidBody.freezeRotation = true; // take manual control
        transform.Rotate(Vector3.forward * rotationThisFrame);
        rigidBody.freezeRotation = false; // release manual control
    }


}
