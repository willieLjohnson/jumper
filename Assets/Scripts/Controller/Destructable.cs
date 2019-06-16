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

  void Awake()
  {
    // value of 0 means it won't start of with a random value.
    value *= (int)(Mathf.Abs(Random.insideUnitSphere.x) * 10);
  }

  void Start()
  {
    audioSource = gameObject.GetComponent<AudioSource>();
    Debug.Log("Tag: " + tag + " Source: " + audioSource);
  }

  // Update is called once per frame
  void Update()
  {
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
