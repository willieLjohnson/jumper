using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
  private int health = 100;

  public AudioSource audioSource;
  public AudioClip deathClip;
  public AudioClip damagedClip;
  // Update is called once per frame
  void Update()
  {
    if (health <= 0)
      Invoke("Die", 0.25f);
  }

  public void Damage(int amount)
  {
    health -= amount;
    audioSource.PlayOneShot(damagedClip);
  }

  private void Die()
  {
    audioSource.PlayOneShot(deathClip);
    Destroy(gameObject);
  }
}
