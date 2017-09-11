using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoScript : MonoBehaviour {
    // movement variables
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float turnSpeed;

    [SerializeField]
    private List<GameObject> targetPath;
    private int currentTarget = 0;
    private float distToNext = 0;

    public void Initialize()
    {
        currentTarget = 0;
    }

    public void Loop()
    {
        if (GameManager.Instance.Player.InSpot || GameManager.Instance.Player.Finished)
        {
            return;
        }

        // obtain current distance to node
        distToNext = (new Vector2(transform.position.x - targetPath[currentTarget].transform.position.x, 
            transform.position.z - targetPath[currentTarget].transform.position.z)).magnitude;

        // while not reached last node
        if (currentTarget < targetPath.Count)
        {
            // while next not reached
            if (distToNext > moveSpeed / 2.0f)
            {
                Rotate(targetPath[currentTarget].transform.position);
                Move();
            }
            else
            {
                // node reached
                currentTarget++;
            
                if(currentTarget >= targetPath.Count)
                {
                    currentTarget = 0;
                }

                // update dist
                distToNext = (new Vector2(transform.position.x - targetPath[currentTarget].transform.position.x,
                    transform.position.z - targetPath[currentTarget].transform.position.z)).magnitude;
            }
        }
    }

    protected void Rotate(Vector3 targetPosition)
    {
        // Calculate the target direction.
        Vector3 targetDirection = targetPosition - transform.position;

        // Calculate the angle.
        float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;

        // rotate the transform with the speed
        transform.Rotate(0f, Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle) * turnSpeed * Time.deltaTime, 0f, Space.Self);
    }

    protected void Move()
    {
        // Move forward. 
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime, Space.World);
    }
}
