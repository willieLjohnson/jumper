using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
  float lastPlayTime = 0;
  float playTimer;
  float pitchIncreaseDelay = 0.5f;
  AudioSource audioSource;
  // Start is called before the first frame update
  void Start()
  {
    audioSource = GetComponent<AudioSource>();
    lastPlayTime = Time.deltaTime;
  }

  // Update is called once per frame
  void Update()
  {
    playTimer += Time.deltaTime;
  }

  public void PlayWithIncreasingPitch(AudioClip clip)
  {
    float playTimeDelta = playTimer - lastPlayTime;
    Debug.Log(playTimeDelta);

    if (playTimeDelta > 1f)
    {
      audioSource.pitch = 1;
    }
    if (playTimeDelta < 0.5f && playTimeDelta > 0.0005f)
    {
      audioSource.pitch += 0.1f;
    }
    else if (playTimeDelta <= 0.0005f)
    {
      return;
    }

    audioSource.PlayOneShot(clip);
    lastPlayTime = playTimer;
  }
}
