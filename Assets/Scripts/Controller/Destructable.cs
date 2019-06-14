using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
  public int health = 100;
  public int value = 0;

  public AudioClip deathClip;
  public AudioClip damagedClip;

  public bool isDead = false;
  public float deathDuration = 0.25f;

  public ParticleSystem deathParticles;

  void Awake()
  {
    // value of 0 means it won't start of with a random value.
    value *= (int)(Mathf.Abs(Random.insideUnitSphere.x) * 10);
    Debug.Log(value);
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
    LevelManager.Instance.audioSource.PlayOneShot(damagedClip);
    CameraFollow.Instance.TriggerShake(0.05f, 0.05f);
  }

  private void Die()
  {
    if (deathParticles)
    {
      ParticleSystem deathPS = Instantiate(deathParticles, transform.position, Quaternion.identity);
      Destroy(deathPS, deathPS.duration);
    }

    GameObject collectible = GameObject.FindWithTag("Collectible");
    CameraFollow.Instance.TriggerShake();
    LevelManager.Instance.audioSource.PlayOneShot(deathClip);

    for (var i = 0; i < value; i++)
    {
      Instantiate(collectible, transform.position, Quaternion.identity);
    }

    Destroy(gameObject);
  }
}
