using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : RaycastController
{
  public int damage = 110;
  public Vector2 pushForce = new Vector2(1f, 0.5f);
  public float range = 2;

  public float attackDuration = .05f;

  public float attackDelay = 0;

  public ParticleSystem forceParticles;
  public GameObject attackPivot;

  public AttackInfo attack;
  float timeToFinishAttacking;
  float timeToNextAttack;

  Vector2 moveAmount;

  public override void Start()
  {
    base.Start();
    range *= collider.bounds.size.x;
  }

  private void Update()
  {
    if (attack.isAttacking && timeToNextAttack < 0)
    {
      if (timeToFinishAttacking > 0)
      {
        timeToNextAttack = attackDelay;
        timeToFinishAttacking -= Time.deltaTime;

        if (attack.horizontal)
        {
          HorizontalAttack();
        }

        if (attack.vertical)
        {
          VerticalAttack();
        }
      }
      else
      {
        timeToFinishAttacking = attackDuration;
        attack.Reset();
      }
    }
    else
    {
      timeToNextAttack -= Time.deltaTime;
    }
  }

  public void Attack(ref Vector3 moveAmount, Vector2 direction, bool isStanding, Vector3 targetPoint)
  {
    direction = direction.normalized;

    attack.horizontal = direction.x != 0;
    attack.vertical = direction.y != 0;
    attack.isAttacking = true;
    attack.direction = direction;

    float pushForceX = pushForce.x * direction.x * ((isStanding) ? 1 : 0.5f);
    float pushForceY = pushForce.y * direction.y * ((isStanding) ? 1 : 0.5f);

    moveAmount = new Vector2(moveAmount.x + pushForceX, moveAmount.y + pushForceY);
    attack.moveAmount = moveAmount;

    if (forceParticles)
    {
      ParticleSystem attackPS = Instantiate(forceParticles, transform.position, Quaternion.identity);
      GameObject.Destroy(attackPS.gameObject, attackPS.main.duration);

      GameObject attackSP = Instantiate(attackPivot, transform.position, Quaternion.identity);
      attackSP.transform.parent = transform;

      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
      attackSP.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

      GameObject.Destroy(attackSP, attackPS.main.duration);
    }
  }

  public void HorizontalAttack()
  {
    UpdateRaycastOrigins();
    Collider2D otherCollider = null;
    HashSet<Collider2D> attackedEnemies = new HashSet<Collider2D>();

    float directionX = Mathf.Sign(attack.direction.x);

    for (int i = 0; i < horizontalRayCount; i++)
    {
      Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
      rayOrigin += Vector2.up * (horizontalRaySpacing * i);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, range, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.right * directionX * range, Color.yellow);

      if (hit)
      {
        if (hit.distance == 0)
        {
          continue;
        }

        otherCollider = hit.collider;
      }

      if (otherCollider != null && otherCollider.gameObject != this.gameObject && !attackedEnemies.Contains(otherCollider)) // && otherCollider.tag == "Pushable"
      {
        attackedEnemies.Add(otherCollider);
        Destructable destructable = otherCollider.gameObject.GetComponent<Destructable>();
        if (destructable)
        {
          destructable.Damage(damage);
        }

        if (otherCollider.tag == "Pushable")
        {
          PushableObject pushable = otherCollider.gameObject.GetComponent<PushableObject>();
          Vector2 pushAmount = pushable.Launch(new Vector2(pushForce.x * directionX, pushForce.y));
        }
      }

    }
  }

  public void VerticalAttack()
  {
    UpdateRaycastOrigins();
    // float originalMoveAmountX = moveAmount.x;
    Collider2D otherCollider = null;
    HashSet<Collider2D> attackedEnemies = new HashSet<Collider2D>();

    float directionY = Mathf.Sign(attack.direction.y);

    for (int i = 0; i < verticalRayCount; i++)
    {
      Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
      rayOrigin += Vector2.right * (verticalRaySpacing * i);
      RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, range, collisionMask);

      Debug.DrawRay(rayOrigin, Vector2.up * directionY * range, Color.yellow);

      if (hit)
      {
        if (hit.distance == 0)
        {
          continue;
        }

        otherCollider = hit.collider;
      }

      if (otherCollider != null && otherCollider.gameObject != this.gameObject && !attackedEnemies.Contains(otherCollider)) // && otherCollider.tag == "Pushable"
      {
        attackedEnemies.Add(otherCollider);
        Destructable destructable = otherCollider.gameObject.GetComponent<Destructable>();
        if (destructable)
        {
          destructable.Damage(damage);
        }


        if (otherCollider.tag == "Pushable")
        {
          PushableObject pushable = otherCollider.gameObject.GetComponent<PushableObject>();
          pushable.Launch(new Vector2(pushForce.x, pushForce.y * directionY));
        }
      }
    }
  }

  public struct AttackInfo
  {
    public bool horizontal;
    public bool vertical;
    public bool isAttacking;
    public Vector2 moveAmount;
    public Vector2 direction;

    public void Reset()
    {
      horizontal = false;
      vertical = false;
      isAttacking = false;
      direction = Vector2.zero;
      moveAmount = Vector2.zero;
    }
  }
}