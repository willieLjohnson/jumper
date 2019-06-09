using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
  public float maxJumpHeight = 4;
  public float minJumpHeight = 1;
  public float timeToJumpApex = .4f;
  float accelerationTimeAirborne = .15f;
  float accelerationTimeGrounded = .1f;
  float moveSpeed = 13;

  public Vector2 wallJumpClimb;
  public Vector2 wallJumpOff;
  public Vector2 wallLeap;

  public float wallSlideSpeedMax = 3;
  public float wallStickTime = .25f;
  float timeToWallUnstick;

  float gravity;
  float maxJumpVelocity;
  float minJumpVelocity;
  Vector3 velocity;
  float velocityXSmoothing;

  Controller2D controller;
  AttackController attackController;
  Destructable destructable;

  Vector2 directionalInput;
  bool wallSliding;
  int wallDirX;

  public AudioSource audioSource;
  public AudioClip jumpClip;
  public AudioClip attackClip;
  public AudioClip walkClip;

  void Start()
  {
    controller = GetComponent<Controller2D>();
    attackController = GetComponent<AttackController>();
    destructable = GetComponent<Destructable>();

    gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
    maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
    minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
  }

  void Update()
  {
    CalculateVelocity();
    HandleWallSliding();

    controller.Move(velocity * Time.deltaTime, directionalInput);
    if (directionalInput.x != 0 && controller.collisions.below)
    {
      if (!LevelManager.Instance.audioSource.isPlaying)
      {
        LevelManager.Instance.audioSource.clip = walkClip;
        LevelManager.Instance.audioSource.pitch = Random.Range(0.5f, 1.3f);
        LevelManager.Instance.audioSource.Play();
      }
      else
      {
        LevelManager.Instance.audioSource.pitch = 1f;
      }
    }

    if (controller.collisions.above || controller.collisions.below)
    {
      if (controller.collisions.slidingDownMaxSlope)
      {
        velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
      }
      else
      {
        velocity.y = 0;
      }
    }
  }

  public void SetDirectionalInput(Vector2 input)
  {
    directionalInput = input;
  }

  public void OnJumpInputDown()
  {
    if (wallSliding)
    {
      LevelManager.Instance.audioSource.PlayOneShot(jumpClip);
      if (wallDirX == directionalInput.x)
      {
        velocity.x = -wallDirX * wallJumpClimb.x;
        velocity.y = wallJumpClimb.y;
      }
      else if (directionalInput.y < 0)
      {
        velocity.x = -wallDirX * wallJumpOff.x;
        velocity.y = wallJumpOff.y;
      }
      else
      {
        velocity.x = -wallDirX * wallLeap.x;
        velocity.y = wallLeap.y;
      }
    }
    if (controller.collisions.below)
    {
      LevelManager.Instance.audioSource.pitch = Random.Range(0.7f, 1.5f);
      LevelManager.Instance.audioSource.PlayOneShot(jumpClip);
      if (controller.collisions.slidingDownMaxSlope)
      {
        if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
        { // not jumping against max slope
          velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
          velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
        }
      }
      else
      {
        velocity.y = maxJumpVelocity;
      }
    }
    else
    {
      LevelManager.Instance.audioSource.pitch = 1f;
    }
  }

  public void OnJumpInputUp()
  {
    if (velocity.y > minJumpVelocity)
    {
      velocity.y = minJumpVelocity;
    }
  }

  public void OnAttackButtonDown(Vector3 mousePos)
  {
    Vector3 attackPoint = mousePos - transform.position;
    attackController.Attack(ref velocity, attackPoint, controller.collisions.below);
    LevelManager.Instance.audioSource.PlayOneShot(attackClip);
  }


  void HandleWallSliding()
  {
    wallDirX = (controller.collisions.left) ? -1 : 1;
    wallSliding = false;
    if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
    {
      wallSliding = true;

      if (velocity.y < -wallSlideSpeedMax)
      {
        velocity.y = -wallSlideSpeedMax;
      }

      if (timeToWallUnstick > 0)
      {
        velocityXSmoothing = 0;
        velocity.x = 0;

        if (directionalInput.x != wallDirX && directionalInput.x != 0)
        {
          timeToWallUnstick -= Time.deltaTime;
        }
        else
        {
          timeToWallUnstick = wallStickTime;
        }
      }
      else
      {
        timeToWallUnstick = wallStickTime;
      }

    }

  }

  void CalculateVelocity()
  {
    float targetVelocityX = directionalInput.x * moveSpeed;
    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
    velocity.y += gravity * Time.deltaTime;
  }
}
