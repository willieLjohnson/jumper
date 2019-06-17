using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
  public int health = 100;
  public float value = 0;

  public bool isDead = false;
  public float deathDuration = 0.25f;

  public ParticleSystem deathParticles;

  public AudioClip damageAudio;
  public AudioClip deathAudio;

  AudioSource audioSource;

  Transform meshTransform;
  MeshRenderer meshRenderer;

  private Vector3 meshInitialPosition;
  private Color meshInitialColor;
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
    foreach (Transform child in transform)
    {
      if (child.tag == "Mesh")
      {
        meshTransform = child;
        meshRenderer = child.GetComponent<MeshRenderer>();
        meshInitialPosition = child.localPosition;
        meshInitialColor = meshRenderer.material.color;
      }
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (meshTransform)
      if (shaking)
        UpdateShake();
      else
      {
        meshTransform.localPosition = meshInitialPosition;
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
    if (meshTransform)
      TriggerShake();
  }

  /// Shakes camera by moving it around randomly during shake timer.
  private void UpdateShake()
  {
    if (shakeTimer > 0)
    {
      Vector3 shakeAmount = Random.insideUnitSphere * shakeMagnitude;
      shakeAmount.x /= transform.localScale.x;
      shakeAmount.y /= transform.localScale.y;

      meshTransform.localPosition = meshInitialPosition + shakeAmount;

      shakeTimer -= Time.deltaTime * dampingSpeed;
    }
    else
    {
      meshRenderer.material.color = meshInitialColor;
      shakeTimer = 0f;
      shaking = false;
    }
  }

  /// Triggers camera shake.
  public void TriggerShake(float magnitude = 0.25f, float duration = 0.2f, float damp = 1.0f)
  {
    meshRenderer.material.color = Color.red + meshInitialColor;
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
