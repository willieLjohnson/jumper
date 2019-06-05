using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
  public Transform enemies;

  public AudioSource audioSource;
  public AudioClip levelUpClip;

  private bool levelTransitioning = false;
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    if (enemies.childCount == 0 && !levelTransitioning) 
    {
      levelTransitioning = true;
      audioSource.PlayOneShot(levelUpClip);
      Invoke("Win", 1f);
    }
  }

  private void Win()
  {
    SceneManager.LoadScene(0);
  }
}
