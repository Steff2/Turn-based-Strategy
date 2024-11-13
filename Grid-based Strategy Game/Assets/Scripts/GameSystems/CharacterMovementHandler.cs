using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementHandler : MonoBehaviour
{
    private const float speed = 30f;

    private List<Vector3> pathVectorList;
    private int currentPathIndex;

    private bool isWalking = false;

    private void Start()
    {
        transform.position = new Vector3(5, 5, 0);
    }

    private void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        if (pathVectorList != null)
        {

            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f)
            {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + speed * Time.deltaTime * moveDir;

                if(!isWalking) isWalking = true;
            }
            else
            {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count)
                {
                    StopMoving();
                }
            }
        }
    }
    private void StopMoving()
    {
        pathVectorList = null;

        isWalking = false;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public void SetTargetPosition(Vector3 targetPosition)
    {
        currentPathIndex = 0;
        Pathfinding.Instance.GetPath(GetPosition(), targetPosition);
        pathVectorList = Pathfinding.Instance.pathVectorList;

        if (pathVectorList != null && pathVectorList.Count > 1) 
        {
            pathVectorList.RemoveAt(0);
        }
    }
}
