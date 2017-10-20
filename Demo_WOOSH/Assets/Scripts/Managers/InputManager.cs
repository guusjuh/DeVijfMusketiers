﻿using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager
{
    private const int LEFT_MOUSE_BUTTON = 0;

    private Vector2 previousPosition;
    private bool stillTouching = false;

    private Vector2 dragVelocity;
    public Vector2 DragVelocity { get { return dragVelocity; } }

    private float zoomVelocity;
    public float ZoomVelocity { get { return zoomVelocity; } }

    public void CatchInput()
    {
        if (UIManager.Instance.InGameUI.CastingSpell) return;

        if (CatchZoomInput()) return;
        if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) {
            if (!CatchUIClicks()) {
                if (!GameManager.Instance.LevelManager.PlayersTurn) return;

                List<WorldObject> worldObjects = ObtainClickedObjects();

                HandleActionOnClickedObjects(worldObjects);

                if (worldObjects.Count <= 0) {
                    StartDrag();
                }
            }
        }
        else if (Input.GetMouseButton(LEFT_MOUSE_BUTTON))
        {
            Drag();
        }
        else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
        {
            EndDrag();
        }
    }

    private void StartDrag()
    {
        ClearOnClick();

        if (!stillTouching)
        {
            stillTouching = true;
            previousPosition = Input.mousePosition;
        }
    }

    private void Drag()
    {
        //drag implementation
        if (stillTouching)
        {
            dragVelocity = -(previousPosition - (Vector2)Input.mousePosition);
            if (dragVelocity.magnitude > 1000.0f) // normal magnitude is between 100 and 500
            {
                Debug.LogWarning("InputManager: too high dragvelocity");
            }
            previousPosition = Input.mousePosition;
        }
    }

    private void EndDrag()
    {
        if (stillTouching)
        {
            stillTouching = false;
        }
        if (dragVelocity.magnitude > 0.0f)
        {
            dragVelocity = Vector2.zero;
        }
    }

    private void HandleActionOnClickedObject(WorldObject worldObject)
    {
        ClearOnClick();

        worldObject.Click();
    }

    private void HandleActionOnClickedObjects(List<WorldObject> worldObjects)
    {
        for (int i = 0; i < worldObjects.Count; i++)
        {
            if (worldObjects.Count > 1)
            {
                if (worldObjects[i].Type != TileManager.ContentType.BrokenBarrel)
                {
                    HandleActionOnClickedObject(worldObjects[i]);
                    break;
                }
            }
            else
            {
                HandleActionOnClickedObject(worldObjects[i]);
                break;
            }
        }
    }

    private List<WorldObject> ObtainClickedObjects()
    {
        //gather all hits, instead of one
        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        List<WorldObject> worldObjects = new List<WorldObject>();

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.GetComponent<WorldObject>() != null)
                worldObjects.Add(hits[i].transform.gameObject.GetComponent<WorldObject>());
        }

        return worldObjects;
    }

    private void ClearOnClick()
    {
        UIManager.Instance.InGameUI.HideSpellButtons();
        UIManager.Instance.InGameUI.ActivateTeleportButtons(false);
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();
        GameManager.Instance.TileManager.HidePossibleRoads();
    }

    private bool CatchUIClicks()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

        // use the position from controller as start of raycast instead of mousePosition.
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        // if no ui objects are hit
        // or if the ui objects were tagged as status icon
        bool noUIClicked = results.Count <= 0;
        bool onlyStatusIconClicked = (results.Count > 0 &&
                                      results.FindAll(r => r.gameObject.transform.tag == "StatusIcon").Count != 0);

        return !(noUIClicked || onlyStatusIconClicked);
    }

    private bool CatchZoomInput()
    {
        zoomVelocity = Input.GetAxis("Mouse ScrollWheel");

        if (Input.touchCount == 2)
        {
            // obtain touches
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // calculate position touches last frame
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // store the difference between the positions of this and last frame
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // calculate the difference between the touches
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            zoomVelocity = Mathf.Clamp(deltaMagnitudeDiff, -1.0f, 1.0f);
        }

        if (zoomVelocity > 0.1f) return true;
        return false;
    }
}
