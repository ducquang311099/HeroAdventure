using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollower : MonoBehaviour
{
    [SerializeField] private GameObject[] waypoints;
    [SerializeField] private float moveSpeed = 3f;

    private int currentWaypointNumber = 0;

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(waypoints[currentWaypointNumber].transform.position, transform.position) < .1f)
            if (++currentWaypointNumber >= waypoints.Length)
                currentWaypointNumber = 0;           
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointNumber].transform.position, Time.deltaTime * moveSpeed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            Debug.Log("");
    }

}
