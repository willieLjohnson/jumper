﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{

  public LayerMask collisionMask;

  public const float skinWidth = 0.015f;
  const float distBetweenRays = 0.25f;


  [HideInInspector]
  public int horizontalRayCount;

  [HideInInspector]
  public int verticalRayCount;

  [HideInInspector]
  public float horizontalRaySpacing;
  [HideInInspector]
  public float verticalRaySpacing;

  [HideInInspector]
  public BoxCollider2D collider;
  public RaycastOrigins raycastOrigins;

  public virtual void Awake()
  {
    collider = GetComponent<BoxCollider2D>();
  }

  public virtual void Start()
  {
    CalculateRaySpacing();
  }

  public void UpdateRayCastOrigins()
  {
    Bounds bounds = collider.bounds;
    bounds.Expand(skinWidth * -2);

    raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
    raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
    raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
  }

  public void CalculateRaySpacing()
  {
    Bounds bounds = collider.bounds;
    bounds.Expand(skinWidth * -2);

    float boundsWidth = bounds.size.x;
    float boundsHeight = bounds.size.y;

    horizontalRayCount = Mathf.RoundToInt(boundsHeight / distBetweenRays);
    verticalRayCount = Mathf.RoundToInt(boundsWidth / distBetweenRays);

    horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
    verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

    horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
    verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
  }

  public struct RaycastOrigins
  {
    public Vector2 topLeft, topRight;
    public Vector2 bottomLeft, bottomRight;
  }

}
