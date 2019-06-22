using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
  public static LevelManager Instance { get; private set; }

  public AudioClip levelUpClip;

  private int currentScene = 0;

  private bool levelTransitioning = false;
  private bool newLevel = false;

  Text enemiesLeftText;
  Text pauseMenu;
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
    enemiesLeftText = GameObject.Find("Enemies Left").GetComponent<Text>();
    pauseMenu = GameObject.Find("Pause").GetComponent<Text>();
    pauseMenu.enabled = false;

    player = Player.Instance.gameObject;

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

    enemiesLeftText.text = enemies.childCount.ToString();

    if (enemies.childCount == 0 && !levelTransitioning)
    {
      levelTransitioning = true;
      audioSource.PlayOneShot(levelUpClip);
      Invoke("Win", 2f);
    }

    if (player == null && !levelTransitioning)
    {
      levelTransitioning = true;
      CameraFollow.Instance.TriggerShake(1f, 1f, 0.2f);
      Invoke("Lose", 1f);
    }
  }

  public void Win()
  {
    if (currentScene + 1 > SceneManager.sceneCount)
    {
      GoToFirstLevel();
    }
    else
    {
      GoToNextLevel();
    }
  }

  public void Lose()
  {
    RestartLevel();
  }


  public void GoToFirstLevel()
  {
    currentScene = 1;
    newLevel = true;
    SceneManager.LoadScene(currentScene);
  }

  public void GoToNextLevel()
  {
    currentScene++;
    newLevel = true;
    SceneManager.LoadScene(currentScene);
  }

  public void RestartLevel()
  {
    newLevel = true;
    SceneManager.LoadScene(currentScene);
  }

  public void HandlePause()
  {
    pauseMenu.enabled = !pauseMenu.enabled;
    if (Time.timeScale == 0)
    {
      Time.timeScale = 1;
    }
    else
    {
      Time.timeScale = 0;
    }
  }
}
