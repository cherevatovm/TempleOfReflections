using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float pursuitSpeed = 3f;
    [SerializeField] private float maxPursuitDistance = 7f;
    private int waypointIndex = 0;

    private void Start() => transform.position = waypoints[waypointIndex].position;

    private void Update()
    {
        if (Dist(PlayerMovement.instance.transform.position, transform.position) > maxPursuitDistance)
            MoveAlongThePath();
        else
            FollowPlayer(PlayerMovement.instance.rigidBody);
    }

    private float Dist(Vector3 p1, Vector3 p2) => math.sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y));

    private void MoveAlongThePath()
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
    private void FollowPlayer(Rigidbody2D rigidBody) => transform.position = Vector2.MoveTowards(transform.position, rigidBody.position, pursuitSpeed * Time.deltaTime);

    public Transform GetFirstWaypointsTransform() => waypoints[0];
}
