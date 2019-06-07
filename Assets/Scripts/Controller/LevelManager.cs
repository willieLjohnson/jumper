using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; private set; }

  public AudioClip levelUpClip;

  private int currentScene = 0;

  private bool levelTransitioning = false;
  private bool newLevel = false;

  Transform enemies;
  GameObject player;

  [HideInInspector]
  public AudioSource audioSource;

  void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  private void Start()
  {
    SetupLevel();
  }

  private void SetupLevel()
  {
    enemies = GameObject.Find("Enemies").transform;
    player = GameObject.FindGameObjectWithTag("Player");

    levelTransitioning = false;
    newLevel = false;

    if (audioSource == null)
    {
      audioSource = GetComponent<AudioSource>();
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (newLevel)
    {
      SetupLevel();
      return;
    }

    if (enemies.childCount == 0 && !levelTransitioning)
    {
      levelTransitioning = true;
      audioSource.PlayOneShot(levelUpClip);
      Invoke("Win", 1f);
    }

    if (player == null && !levelTransitioning)
    {
      levelTransitioning = true;
      // audioSource.PlayOneShot(levelUpClip);
      Invoke("Lose", 1f);
    }
  }

  public void Win()
  {
    if (currentScene + 1 > SceneManager.sceneCount)
    {
      GoToFirstScene();
    }
    else
    {
      GoToNextScene();
    }
  }

  public void Lose()
  {
    GoToFirstScene();
  }


  public void GoToFirstScene()
  {
    currentScene = 0;
    newLevel = true;
    SceneManager.LoadScene(currentScene);
  }

  public void GoToNextScene()
  {
    currentScene++;
    newLevel = true;
    SceneManager.LoadScene(currentScene);
  }
}
