using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
  public int health = 100;

  public AudioClip deathClip;
  public AudioClip damagedClip;

  public bool isDead = false;
  public float deathDuration = 0.25f;

  public ParticleSystem deathParticles;

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
    CameraFollow.Instance.TriggerShake();
    LevelManager.Instance.audioSource.PlayOneShot(deathClip);
    Destroy(gameObject);
  }
}
