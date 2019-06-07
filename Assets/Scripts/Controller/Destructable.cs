using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
  private int health = 100;

  public AudioClip deathClip;
  public AudioClip damagedClip;

  public bool isDead = false;
  // Update is called once per frame
  void Update()
  {
    if (health <= 0)
    {
      isDead = true;
      Invoke("Die", 0.25f);
    }
  }

  public void Damage(int amount)
  {
    health -= amount;
    LevelManager.Instance.audioSource.PlayOneShot(damagedClip);
  }

  private void Die()
  {
    LevelManager.Instance.audioSource.PlayOneShot(deathClip);
    Destroy(gameObject);
  }
}
