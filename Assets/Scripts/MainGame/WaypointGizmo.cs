using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGizmo : MonoBehaviour
{
    [SerializeField] private float gizmoRadius = 1;
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}
