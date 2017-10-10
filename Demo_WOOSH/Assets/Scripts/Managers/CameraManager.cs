using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private const int X_AXIS = 0, Y_AXIS = 1;
    private bool[] lockedAxis = { false, false };
    private Transform target;

    [SerializeField]
    private float speedScalar = 0.001f;

    [SerializeField]
    private Vector2 bordersMin;

    [SerializeField]
    private Vector2 bordersMax;

    [SerializeField]
    private Vector2 curViewportMin;

    [SerializeField]
    private Vector2 curViewportMax;

    private Rect viewportRect;

    //clamps camera between minimum (in world position) and maximum (in world position).
    public void SetBorderRange(Vector2 min, Vector2 max)
    {
        bordersMin = min;
        bordersMax = max;
    }

    public void LockLocation(Vector2 position)
    {
        lockedAxis[X_AXIS] = true;
        lockedAxis[Y_AXIS] = true;
        transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
    }

    public void LockTarget(Transform targetTransform)
    {
        lockedAxis[X_AXIS] = true;
        lockedAxis[Y_AXIS] = true;
        target = targetTransform;
    }

    public void UnlockAxis()
    {
        lockedAxis[X_AXIS] = false;
        lockedAxis[Y_AXIS] = false;
    }

    public void LockAxis(bool x, bool y, Vector2 lockPosition)
    {
        Vector2 translation = Vector2.zero;
        if (x)
        {
            translation.x = lockPosition.x - transform.position.x;
            lockedAxis[X_AXIS] = true;
        }
        if (y)
        {
            translation.y = lockPosition.y - transform.position.y;
            lockedAxis[Y_AXIS] = true;
        }

        transform.Translate(translation);
    }


    public void Initialize()
    {
        UnlockAxis();

        viewportRect = Camera.main.pixelRect;
        Vector2 min = GameManager.Instance.TileManager.GetWorldPosition(new Coordinate(-2, -3));
        Vector2 max = GameManager.Instance.TileManager.GetWorldPosition(new Coordinate(GameManager.Instance.TileManager.Rows + 1, GameManager.Instance.TileManager.Columns + 2));

        speedScalar = Camera.main.orthographicSize * 0.001f;

        SetBorderRange(min, max);

        Vector2 position = bordersMin + ((bordersMax - bordersMin) * 0.5f);
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void MoveCamera(Vector2 desiredVelocity)
    {
        Vector2 velocity = Vector2.zero;
        if (bordersMin == Vector2.zero && bordersMax == Vector2.zero)
        {
            Debug.LogError("Borders have not been set");
            LockLocation(transform.position);
        }

        curViewportMin = (Vector3)desiredVelocity + Camera.main.ScreenToWorldPoint(viewportRect.min);
        curViewportMax = (Vector3)desiredVelocity + Camera.main.ScreenToWorldPoint(viewportRect.max);
        if (!lockedAxis[X_AXIS] || target != null)
        {
            if (curViewportMin.x <= bordersMin.x && curViewportMax.x >= bordersMax.x)
            {
                LockAxis(true, false, bordersMin + ((bordersMax - bordersMin) * 0.5f));
                //split difference
            }
            else if (curViewportMax.x >= bordersMax.x)
            {
                velocity.x = bordersMax.x - Camera.main.ScreenToWorldPoint(viewportRect.max).x;
                //move to bordersmax
            }
            else if (curViewportMin.x <= bordersMin.x)
            {
                velocity.x = bordersMin.x - Camera.main.ScreenToWorldPoint(viewportRect.min).x;
                //move to bordermin
            }
            else
            {
                velocity.x = desiredVelocity.x;
                //normal translation
            }
        }
        if (!lockedAxis[Y_AXIS] || target != null)
        {
            if (curViewportMin.y <= bordersMin.y && curViewportMax.y >= bordersMax.y)
            {
                LockAxis(false, true, bordersMin + ((bordersMax - bordersMin) * 0.5f));
                //split difference
            }
            else if (curViewportMax.y >= bordersMax.y)
            {
                velocity.y = bordersMax.y - Camera.main.ScreenToWorldPoint(viewportRect.max).y;
                //move to bordersmax
            }
            else if (curViewportMin.y <= bordersMin.y)
            {
                velocity.y = bordersMin.y - Camera.main.ScreenToWorldPoint(viewportRect.min).y;
                //move to bordermin
            }
            else
            {
                velocity.y = desiredVelocity.y;
                //normal translation
            }
        }

        transform.Translate(velocity);
    }

    // Update is called once per frame
    public void UpdatePosition()
    {
        if (lockedAxis[X_AXIS] && lockedAxis[Y_AXIS])
        {
            if (target != null)
            {
                MoveCamera(new Vector3(target.position.x, target.position.y, transform.position.z) - transform.position);
            }
        }
        else
        {
            if (target != null)
            {
                target = null;
            }
            //implement drag movement
            MoveCamera(-UberManager.Instance.InputManager.DragVelocity * speedScalar);
        }
    }
}