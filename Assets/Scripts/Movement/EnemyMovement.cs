using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float moveSpeed = 3f;
    private int waypointIndex = 0;

    private void Start() => transform.position = waypoints[waypointIndex].position;

    private void Update()
    {
        if (Dist(PlayerMovement.instance.transform.position, transform.position) > 7)
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
    private void FollowPlayer(Rigidbody2D rigidBody) => transform.position = Vector2.MoveTowards(transform.position, rigidBody.position, moveSpeed * Time.deltaTime * 2);

    public Transform GetFirstWaypointsTransform() => waypoints[0];
}
