using System;
using UnityEngine;

public class ArrowHazard : MonoBehaviour
{
    [SerializeField] private ArrowPool arrowPool;

    [SerializeField] float shootInterval;
    private float shootIntervalLeft;

    void Start()
    {
        shootIntervalLeft = shootInterval;
    }

    void Update()
    {
        shootIntervalLeft -= Time.deltaTime;

        if (shootIntervalLeft <= 0)
        {
            GameObject arrow = arrowPool.GetArrow();

            arrow.transform.position = transform.position;
            arrow.transform.rotation = Quaternion.identity;

            arrow.transform.Rotate(0, 180, 0);

            shootIntervalLeft = shootInterval;
        }
    }
}
