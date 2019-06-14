using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Collectible : MonoBehaviour
{
  public Transform target;
  public AudioClip collectibleClip;

  Controller2D controller;

  // Range at which the player will collect.
  float collectionRange = 5f;

  float gravity = 12f;
  float moveSpeed = 13f;

  float accelerationTimeGrounded = 0.1f;
  float accelerationTimeAirborne = 0.2f;

  Vector3 velocity;

  float velocityXSmoothing;
  float velocityYSmoothing;

  void Start()
  {
    controller = GetComponent<Controller2D>();

    if (target == null && Player.Instance != null)
    {
      target = Player.Instance.transform;
    }

    velocity = Random.insideUnitSphere * moveSpeed;
  }

  // Update is called once per frame
  void Update()
  {
    CalculateVelocity();
    controller.Move(velocity * Time.deltaTime, false);

    if (controller.collisions.below)
    {
      velocity.y = 0;
    }
  }

  void CalculateVelocity()
  {
    if (target)
    {
      Vector2 distance = (target.position - transform.position);

      // Chase target
      if (Mathf.Abs(distance.x) < collectionRange && Mathf.Abs(distance.y) < collectionRange)
      {
        float targetVelocityX = Mathf.Sign(distance.x) * moveSpeed;
        float targetVelocityY = Mathf.Sign(distance.y) * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, (controller.collisions.left || controller.collisions.right) ? accelerationTimeGrounded : accelerationTimeAirborne);
      }
    }

    velocity += Vector3.down * gravity * Time.deltaTime;
  }

  public void Collect()
  {
    LevelManager.Instance.audioSource.PlayOneShot(collectibleClip);
    GameObject.Destroy(gameObject);
  }
}
