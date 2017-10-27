using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private Camera camRef;

    private const int X_AXIS = 0, Y_AXIS = 1;
    private bool[] lockedAxis = { false, false };
    private Transform target;

    private Vector2 bordersMin;
    private Vector2 bordersMax;

    private Vector2 curViewportMin;
    private Vector2 curViewportMax;

    private Rect viewportRect;

    private float minSize = 3.0f, maxSize = 8.0f;

    private Vector2 velocity = Vector2.zero;
    private float speedScalar = 0.001f;
    private float zoomSpeed = 0.3f;

    private float currentSize;
    public float CurrentSize { get { return currentSize; } }
    public float InvCurrentSize { get { return (maxSize - currentSize) + 1; } }

    public void Initialize()
    {
        camRef = GetComponent<Camera>();

        UnlockAxis();

        viewportRect = Camera.main.pixelRect;
        Vector2 min = GameManager.Instance.TileManager.GetWorldPosition(new Coordinate(-3, -3));
        Vector2 max = GameManager.Instance.TileManager.GetWorldPosition(new Coordinate(GameManager.Instance.TileManager.Rows + 2,
                                                                                       GameManager.Instance.TileManager.Columns + 2));
        SetBorderRange(min, max);

        currentSize = camRef.orthographicSize;

        zoomSpeed = Application.isEditor ? 3.0f : 0.1f;
        speedScalar = Camera.main.orthographicSize * 0.001f;

        Vector2 position = bordersMin + ((bordersMax - bordersMin) * 0.5f);
        transform.position = new Vector3(position.x, position.y, transform.position.z);
    }

    public void ResetDEVMODE()
    {
        if (!UberManager.Instance.DevelopersMode) return;

        Vector2 min = GameManager.Instance.TileManager.GetWorldPosition(new Coordinate(-3, -3));
        Vector2 max = GameManager.Instance.TileManager.GetWorldPosition(new Coordinate(GameManager.Instance.TileManager.Rows + 2,
                                                                                       GameManager.Instance.TileManager.Columns + 2));
        SetBorderRange(min, max);

        Vector2 newPos = (((min + max) / 2.0f));
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);

        Vector2 newSize = (max - min);

        float maxY =  newSize.y / 2;
        float maxX = (newSize.x / 2) / Camera.main.aspect;

        maxSize = maxX > maxY ? maxX : maxY;
    }

    //clamps camera between minimum (in world position) and maximum (in world position).
    private void SetBorderRange(Vector2 min, Vector2 max) {
        bordersMin = min;
        bordersMax = max;
    }

    private void LockLocation(Vector2 position) {
        lockedAxis[X_AXIS] = true;
        lockedAxis[Y_AXIS] = true;
        transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
    }

    public void LockTarget(Transform targetTransform) {
        lockedAxis[X_AXIS] = true;
        lockedAxis[Y_AXIS] = true;
        target = targetTransform;
    }

    public void UnlockAxis() {
        lockedAxis[X_AXIS] = false;
        lockedAxis[Y_AXIS] = false;
    }

    private void LockAxis(bool x, bool y, Vector2 lockPosition) {
        Vector2 translation = Vector2.zero;

        if (x) {
            translation.x = lockPosition.x - transform.position.x;
            lockedAxis[X_AXIS] = true;
        }
        if (y) {
            translation.y = lockPosition.y - transform.position.y;
            lockedAxis[Y_AXIS] = true;
        }

        transform.Translate(translation);
    }

    private void MoveCamera(Vector2 desiredVelocity, bool instant) {
        if (bordersMin == Vector2.zero && bordersMax == Vector2.zero) {
            Debug.LogError("Borders have not been set");
            LockLocation(transform.position);
        }

        CalculateVelocity(desiredVelocity);

        if (instant) transform.Translate(velocity);
        else transform.position += new Vector3(velocity.x, velocity.y, 0) * Time.deltaTime * 10; 
    }

    private void CalculateVelocity(Vector2 desiredVelocity)
    {
        velocity = Vector2.zero;

        curViewportMin = (Vector3)desiredVelocity + Camera.main.ScreenToWorldPoint(viewportRect.min);
        curViewportMax = (Vector3)desiredVelocity + Camera.main.ScreenToWorldPoint(viewportRect.max);

        if (!lockedAxis[X_AXIS] || target != null)
            CalculateXVelocity(desiredVelocity.x);

        if (!lockedAxis[Y_AXIS] || target != null)
            CalculateYVelocity(desiredVelocity.y);
    }

    private void CalculateXVelocity(float xVelocity) {
        if (curViewportMin.x <= bordersMin.x && curViewportMax.x >= bordersMax.x) {
            LockAxis(true, false, bordersMin + ((bordersMax - bordersMin) * 0.5f)); //split difference
        }
        else if (curViewportMax.x >= bordersMax.x) {
            velocity.x = bordersMax.x - Camera.main.ScreenToWorldPoint(viewportRect.max).x; //move to bordersmax
        }
        else if (curViewportMin.x <= bordersMin.x) {
            velocity.x = bordersMin.x - Camera.main.ScreenToWorldPoint(viewportRect.min).x; //move to bordermin
        }
        else {
            velocity.x = xVelocity; //normal translation
        }
    }

    private void CalculateYVelocity(float yVelocity) {
        if (curViewportMin.y <= bordersMin.y && curViewportMax.y >= bordersMax.y) {
            LockAxis(false, true, bordersMin + ((bordersMax - bordersMin) * 0.5f)); //split difference
        }
        else if (curViewportMax.y >= bordersMax.y) {
            velocity.y = bordersMax.y - Camera.main.ScreenToWorldPoint(viewportRect.max).y; //move to bordersmax
        }
        else if (curViewportMin.y <= bordersMin.y) {
            velocity.y = bordersMin.y - Camera.main.ScreenToWorldPoint(viewportRect.min).y; //move to bordermin
        }
        else {
            velocity.y = yVelocity; //normal translation
        }
    }

    // Update is called once per frame
    public void UpdatePosition() {
        if (UberManager.Instance.InputManager.DragVelocity.magnitude > 10) {
            target = null;
            UnlockAxis();
            MoveCamera(-UberManager.Instance.InputManager.DragVelocity * speedScalar, true);
            return;
        }
        else if (Mathf.Abs(UberManager.Instance.InputManager.ZoomVelocity) > 0.01f) {
            Zoom(-UberManager.Instance.InputManager.ZoomVelocity);
        }

        // locked means either 
        // enemy turn OR 
        // having a target OR 
        // too big for the level
        if (lockedAxis[X_AXIS] && lockedAxis[Y_AXIS]) {
            // we have a target, go follow it
            if (target != null) {
                // keep moving
                Vector3 toPosition = new Vector3(target.position.x, target.position.y, transform.position.z) -
                                     transform.position;
                MoveCamera(toPosition, false);
            }
        }
    }

    // Update is called once per frame
    public void UpdateDEVMODE()
    {
        Vector2 mousePosition = Input.mousePosition;
        float zoomVelocity = Input.GetAxis("Mouse ScrollWheel");

        if (mousePosition.x < 0 || mousePosition.y < 0 || mousePosition.x > Camera.main.pixelWidth ||
            mousePosition.y > Camera.main.pixelHeight)
        {
            zoomVelocity = 0.0f;
        }

        Vector2 moveVelocity = Vector2.zero;
        float speed = 3.0f;

        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveVelocity.y = 1.0f;
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            moveVelocity.y = -1.0f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveVelocity.x = 1.0f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveVelocity.x = -1.0f;
        }

        if (moveVelocity != Vector2.zero)
        {
            target = null;
            UnlockAxis();
            MoveCamera(-moveVelocity * speed * Time.deltaTime, true);
            return;
        }
        else if (Mathf.Abs(zoomVelocity) > 0.01f)
        {
            Zoom(-zoomVelocity);
        }
    }

    private void Zoom(float zoom) {
        // adjust currentsize based on zoom
        currentSize += zoom * zoomSpeed;

        // clamp the currentsize
        currentSize = Mathf.Clamp(currentSize, minSize, maxSize);

        camRef.orthographicSize = currentSize;
    }
}