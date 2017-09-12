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
    private List<GameObject> paths;
    private List<PathNode> pathNodes;
    private int currentTarget = 0;
    private int currentPath = 0;
    private float distToNext = 0;

    private bool waiting = false;

    public void NextPath()
    {
        currentPath++;

        PathNode[] temp = paths[currentPath].GetComponentsInChildren<PathNode>();

        pathNodes.Clear();

        foreach (PathNode p in temp)
        {
            pathNodes.Add(p);
        }

        currentTarget = 0;
    }

    public void Initialize()
    {
        currentTarget = 0;
        currentPath = 0;

        PathNode[] temp = paths[currentPath].GetComponentsInChildren<PathNode>();

        pathNodes = new List<PathNode>();

        foreach (PathNode p in temp)
        {
            pathNodes.Add(p);
        }
    }

    public void Loop()
    {
        if (GameManager.Instance.Player.InSpot || GameManager.Instance.Player.Finished || waiting)
        {
            return;
        }

        // obtain current distance to node
        distToNext = (new Vector2(transform.position.x - pathNodes[currentTarget].transform.position.x, 
            transform.position.z - pathNodes[currentTarget].transform.position.z)).magnitude;

        // while not reached last node
        if (currentTarget < pathNodes.Count)
        {
            // while next not reached
            if (distToNext > moveSpeed / 2.0f)
            {
                Rotate(pathNodes[currentTarget].transform.position);
                Move();
            }
            else
            {
                if (pathNodes[currentTarget].WaitTime > 0.0f)
                {
                    transform.position = new Vector3(pathNodes[currentTarget].transform.position.x, transform.position.y, pathNodes[currentTarget].transform.position.z);

                    StartCoroutine(Waiting(pathNodes[currentTarget].WaitTime));
                }

                // node reached
                currentTarget++;
            
                if(currentTarget >= pathNodes.Count)
                {
                    currentTarget = 0;
                }

                // update dist
                distToNext = (new Vector2(transform.position.x - pathNodes[currentTarget].transform.position.x,
                    transform.position.z - pathNodes[currentTarget].transform.position.z)).magnitude;
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

    private IEnumerator Waiting(float waitTime)
    {
        waiting = true;

        yield return new WaitForSeconds(waitTime);

        waiting = false;
    }

}


