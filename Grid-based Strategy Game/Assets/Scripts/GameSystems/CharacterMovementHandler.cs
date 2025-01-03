using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementHandler : MonoBehaviour
{
    [SerializeField] Vector3 startPos;

    Action onReachedPosition;

    private const float speed = 30f;

    private List<Vector3> pathVectorList;
    private int currentPathIndex = -1;

    private void Awake()
    {
        transform.position = startPos;
    }
    private void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        if (currentPathIndex != -1)
        {

            Vector3 targetPosition = pathVectorList[currentPathIndex]; 
            Vector3 moveDir = (targetPosition - transform.position).normalized;

            transform.position = transform.position + speed * Time.deltaTime * moveDir;

            if (Vector3.Distance(transform.position, targetPosition) < 1f)
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
        currentPathIndex = -1;
        onReachedPosition();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public void SetTargetPosition(Vector3 targetPosition, Action onReachedPosition)
    {
        try
        {
            this.onReachedPosition = onReachedPosition;
            var path = Pathfinding.Instance.ShortcutPath(GetPosition(), targetPosition);
            pathVectorList = path.vectorPathList;
        }
        catch(System.Exception e) 
        { 
            Debug.Log(e);
        }

        if (pathVectorList.Count > 0)
        {
            currentPathIndex = 0;
        }
        else
        {
            currentPathIndex = -1;
        }
    }
}
