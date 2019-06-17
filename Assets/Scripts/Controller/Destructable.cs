using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
  public int health = 100;
  public int value = 0;

  public bool isDead = false;
  public float deathDuration = 0.25f;

  public ParticleSystem deathParticles;

  public AudioClip damageAudio;
  public AudioClip deathAudio;

  AudioSource audioSource;

  Transform meshTransform;

  private Vector2 initialPosition;
  private float shakeTimer = 0f;
  private float shakeMagnitude = 0.2f;
  private float dampingSpeed = 1.0f;
  public bool shaking = false;


  void Awake()
  {
    // value of 0 means it won't start of with a random value.
    value *= (int)(Mathf.Abs(Random.insideUnitSphere.x) * 10);
  }

  void Start()
  {
    audioSource = gameObject.GetComponent<AudioSource>();
    meshTransform = gameObject.transform.Find("Mesh");
    if (tag == "Player")
    {
      meshTransform = gameObject.transform.Find("Jumpee");
    }
    initialPosition = meshTransform.localPosition;
    Debug.Log("Tag: " + tag + " Source: " + audioSource);
  }

  // Update is called once per frame
  void Update()
  {
    if (shaking)
      UpdateShake();
    else
    {
      meshTransform.localPosition = initialPosition;
    }


    if (health <= 0 || isDead)
    {
      isDead = true;
      Invoke("Die", deathDuration);
    }
  }

  public void Damage(int amount)
  {
    health -= amount;
    audioSource.PlayOneShot(damageAudio);
    CameraFollow.Instance.TriggerShake(0.05f, 0.05f);
    TriggerShake();
  }

  /// Shakes camera by moving it around randomly during shake timer.
  private void UpdateShake()
  {
    if (shakeTimer > 0)
    {
      meshTransform.localPosition = initialPosition + (Vector2)Random.insideUnitSphere * shakeMagnitude;
      shakeTimer -= Time.deltaTime * dampingSpeed;
    }
    else
    {
      shakeTimer = 0f;
      shaking = false;
    }
  }

  /// Triggers camera shake.
  public void TriggerShake(float magnitude = 0.05f, float duration = 0.25f, float damp = 1.0f)
  {
    shakeTimer = duration;
    shakeMagnitude = magnitude;
    dampingSpeed = damp;
    shaking = true;
  }


  private void Die()
  {
    if (deathParticles)
    {
      ParticleSystem deathPS = Instantiate(deathParticles, transform.position, Quaternion.identity);
      deathPS.Play();
      Destroy(deathPS, deathPS.main.duration);
    }

    GameObject collectible = GameObject.FindWithTag("Collectible");
    CameraFollow.Instance.TriggerShake();

    GameObject carcass = new GameObject();
    audioSource = carcass.AddComponent<AudioSource>();
    if (gameObject.tag == "Player")
    {
      audioSource.pitch = 0.25f;
    }
    audioSource.PlayOneShot(deathAudio);
    Destroy(carcass, 1f);

    for (var i = 0; i < value; i++)
    {
      Instantiate(collectible, transform.position, Quaternion.identity);
    }

    Destroy(gameObject);
  }
}
