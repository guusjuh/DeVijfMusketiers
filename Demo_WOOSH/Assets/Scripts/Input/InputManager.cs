using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    private const int LEFT_MOUSE_BUTTON = 0;

    private Vector2 previousPosition;
    private bool stillTouching = false;
    public bool StillTouching { get { return stillTouching; } }

    private Vector2 dragVelocity;
    public Vector2 DragVelocity { get { return dragVelocity; } }

    private float zoomVelocity;
    public float ZoomVelocity { get { return zoomVelocity; } }

    private Vector2 teleportClick;
    private DateTime clickTime;
    public bool highlightsActivated = false;

    public void CatchInput()
    {
        if (UberManager.Instance.SpellManager.CastingSpell >= SpellManager.SpellType.Attack && UberManager.Instance.SpellManager.CastingSpell != SpellManager.SpellType.Teleport) return;

        if (CatchZoomInput()) return;
        if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
        {
            if (!CatchUIClicks())
            {
                if (!GameManager.Instance.LevelManager.PlayersTurn) return;

                List<WorldObject> worldObjects = ObtainClickedObjects();

                HandleActionOnClickedObjects(worldObjects);

                if (worldObjects.Count <= 0)
                {
                    StartDrag();
                }
            }
            else
            {
                PointerEventData pointerData = new PointerEventData(EventSystem.current);

                // use the position from controller as start of raycast instead of mousePosition.
                pointerData.position = Input.mousePosition;

                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointerData, results);

                bool onlyStatusIconClicked = (results.Count > 0 &&
                                      results.FindAll(r => r.gameObject.transform.tag == "SurroundingPushButton").Count == results.Count);

                if(onlyStatusIconClicked) StartDrag();
            }
        }
        else if (Input.GetMouseButton(LEFT_MOUSE_BUTTON))
        {
            Drag();
            teleportClick = Input.mousePosition;
            clickTime = DateTime.Now;
        }
        else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
        {
            Vector2 worldMouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Coordinate gridPosition = UberManager.Instance.GameManager.TileManager.GetGridPosition(worldMouse);

            if (UberManager.Instance.SpellManager.CastingSpell == SpellManager.SpellType.Teleport && highlightsActivated)
            {
                if ((teleportClick - (Vector2)Input.mousePosition).magnitude <= GameManager.Instance.TileManager.HexagonScale / 2.0f
                    && clickTime.Subtract(DateTime.Now).TotalSeconds <= 0.2f)
                {
                    UberManager.Instance.SpellManager.SetSelectedTile();
                    teleportClick = new Vector2(-100, -100);
                }

            } else if ((UberManager.Instance.GameManager.TileManager.GetWorldPosition(gridPosition) - worldMouse).magnitude > GameManager.Instance.TileManager.HexagonScale / 2.0f)
            {
                ClearOnClick();
                highlightsActivated = false;
            }
            EndDrag();
        }
    }

    private void StartDrag()
    {
        GameManager.Instance.CameraManager.SetBouncyness();

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
                if (!(worldObjects[i].IsBarrel() &&
                    worldObjects[i].GetComponent<Rock>().Destroyed))
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
        UberManager.Instance.SpellManager.HideSpellButtons();
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
                                      results.FindAll(r => r.gameObject.transform.tag == "StatusIcon").Count == results.Count);
        bool onlyFloatingIndicatorsClicked = (results.Count > 0 &&
                                              results.FindAll(r => r.gameObject.transform.tag == "FloatingIndicator").Count == results.Count);
        bool closeSkipButton = (results.FindAll(r => r.gameObject.transform.tag == "APSkip-indicator")).Count == 0;
        if (closeSkipButton) UberManager.Instance.UiManager.InGameUI.PlayerActionPoints.CloseSkipButton();

        return !(noUIClicked || onlyStatusIconClicked || onlyFloatingIndicatorsClicked);
    }

    private bool CatchZoomInput()
    {
        if (UberManager.Instance.Tutorial) return false;

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
