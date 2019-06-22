using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
  public static Player Instance { get; private set; }
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
  GameObject jumpee;
  Animator jumpeeAnimator;
  ParticleSystem lifeForceParticleSystem;
  ParticleSystem daemonParticleSystem;

  Vector2 directionalInput;
  bool wallSliding;
  int wallDirX;

  public AudioClip jumpClip;
  public AudioClip attackClip;
  public AudioClip walkClip;
  public AudioClip daemonAudio;

  AudioSource audioSource;

  public bool inDestructable = false;
  public bool isDaemonMode = false;
  const float maxLifeTimer = 20;
  const float daemonMode = 10;
  float gemTimerRefrechAmount;
  float lifeTimer;
  float daemonModeTimer;
  Color camBackground;

  void Awake()
  {
    Instance = this;
  }

  void Start()
  {
    controller = GetComponent<Controller2D>();
    attackController = GetComponent<AttackController>();
    destructable = GetComponent<Destructable>();
    audioSource = GetComponent<AudioSource>();
    jumpee = GameObject.Find("Jumpee");
    jumpeeAnimator = jumpee.GetComponent<Animator>();
    lifeForceParticleSystem = jumpee.GetComponent<ParticleSystem>();
    daemonParticleSystem = GameObject.Find("Daemon Particles").GetComponent<ParticleSystem>();

    CalculateJumpWithGravity();

    lifeTimer = maxLifeTimer * 0.2f;
    gemTimerRefrechAmount = maxLifeTimer * 0.03f;

    if (inDestructable)
    {
      lifeTimer = maxLifeTimer;
      lifeForceParticleSystem.startLifetime = 1f;
    }
  }

  private void CalculateJumpWithGravity()
  {
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
      if (!audioSource.isPlaying)
      {
        audioSource.clip = walkClip;
        audioSource.pitch = Random.Range(0.5f, 1.3f);
        audioSource.Play();
      }
      else
      {
        audioSource.pitch = 1f;
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

      jumpeeAnimator.SetBool("isJumping", false);
    }

    if (!controller.collisions.below)
    {
      jumpeeAnimator.SetBool("isJumping", velocity.y > 0);
      jumpeeAnimator.SetBool("isFalling", velocity.y <= 0);
    }
    else
    {
      jumpeeAnimator.SetBool("isJumping", false);
      jumpeeAnimator.SetBool("isFalling", false);
    }

    Vector3 scale = transform.localScale;
    scale.x = controller.collisions.faceDir;
    jumpee.transform.localScale = scale;

    if (!inDestructable)
    {
      if (daemonModeTimer <= 0)
      {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer < 0)
        {
          lifeTimer = 0;
        }

        if (lifeTimer <= 0)
        {
          destructable.Damage(9000);
        }

        jumpee.GetComponent<MeshRenderer>().enabled = true;
        moveSpeed = 13;
      }
      else
      {
        daemonModeTimer -= Time.deltaTime;
        if (daemonModeTimer <= 0)
          ToggleDaemonMode();
      }
      lifeForceParticleSystem.startLifetime = lifeTimer / maxLifeTimer;
    }
  }

  public void ToggleDaemonMode(bool wasDamaged = false)
  {
    if (!isDaemonMode)
    {
      jumpee.GetComponent<MeshRenderer>().enabled = false;
      moveSpeed = 20;
      maxJumpHeight = 10;
      minJumpHeight = 5;
      isDaemonMode = true;
      daemonParticleSystem.Play();
      audioSource.PlayOneShot(daemonAudio);
      daemonModeTimer = daemonMode;
    }
    else
    {
      jumpee.GetComponent<MeshRenderer>().enabled = true;
      moveSpeed = 13;
      maxJumpHeight = 4;
      minJumpHeight = 1;
      isDaemonMode = false;
      daemonModeTimer = 0;
    }

    if (wasDamaged)
    {
      lifeTimer *= 0.25f;
    }
  }

  public void ToggleAnimator(bool enabled)
  {
    jumpeeAnimator.enabled = enabled;
  }

  public void SetDirectionalInput(Vector2 input)
  {
    directionalInput = input;
  }

  public void OnJumpInputDown()
  {
    if (wallSliding)
    {
      audioSource.PlayOneShot(jumpClip);
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
      audioSource.pitch = Random.Range(0.7f, 1.5f);
      audioSource.PlayOneShot(jumpClip);
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
      audioSource.pitch = 1f;
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
    Vector3 attackDirection = mousePos - transform.position;
    attackController.Attack(ref velocity, attackDirection, controller.collisions.below, mousePos);
    audioSource.PlayOneShot(attackClip);
  }

  public void OnGemCollected()
  {
    if (daemonModeTimer > 0)
    {
      daemonModeTimer += gemTimerRefrechAmount / 2f;
    }
    else
    {
      lifeTimer += gemTimerRefrechAmount;
      if (lifeTimer > maxLifeTimer && !isDaemonMode)
      {
        lifeTimer = maxLifeTimer;
        ToggleDaemonMode();
      }
    }
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
