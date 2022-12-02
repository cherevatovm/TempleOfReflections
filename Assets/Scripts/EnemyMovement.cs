using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float moveSpeed = 3f;
    int waypointIndex = 0;

    void Start()
    {
        transform.position = waypoints[waypointIndex].position;
    }

    void Update()
    {
        MoveAlongThePath();
    }

    void MoveAlongThePath()
    {
        if (waypointIndex < waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[waypointIndex].position, moveSpeed * Time.deltaTime);
            if (transform.position == waypoints[waypointIndex].position)
                waypointIndex++;
        }
        else
            waypointIndex = 0;
    }
}
