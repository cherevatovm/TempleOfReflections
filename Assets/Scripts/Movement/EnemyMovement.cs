using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;
    [SerializeField] float moveSpeed = 3f;
    int waypointIndex = 0;

    void Start()
    {
        transform.position = waypoints[waypointIndex].position;
    }

    void Update(){
        if (dist(PlayerMovement.instance.transform.position, transform.position) > 7)
        MoveAlongThePath();
        else
             FollowPlayer(PlayerMovement.instance.rigidBody);

    }
    float dist(Vector3 p1, Vector3 p2)
    {
        return math.sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y));

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
    void FollowPlayer(Rigidbody2D rigidBody) {

        transform.position = Vector2.MoveTowards(transform.position,rigidBody.position , moveSpeed * Time.deltaTime*2);


    }

}
