using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager
{
    private static InputManager instance = null;
    public static InputManager Instance
    {
        get
        {
            if (instance == null) instance = UberManager.Instance.InputManager;
            return instance;
        }
    }

    private const int LEFT = 0;
    private Vector2 previousPosition;

    private Vector2 dragVelocity;
    public Vector2 DragVelocity { get { return dragVelocity; } }

    private float zoomVelocity;
    public float ZoomVelocity { get { return zoomVelocity; } }

    // vars for pinch code


    private bool stillTouching = false;

    public void CatchInput()
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

        if (Input.GetMouseButtonDown(LEFT))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Input.mousePosition;
                // use the position from controller as start of raycast instead of mousePosition.

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            // if no ui objects are hit
            // or if the ui objects were tagged as status icon
            bool noUIClicked = results.Count <= 0;
            bool onlyStatusIconClicked = (results.Count > 0 &&
                                          results.FindAll(r => r.gameObject.transform.tag == "StatusIcon").Count != 0);
            if (noUIClicked || onlyStatusIconClicked)
            {
                if (!GameManager.Instance.LevelManager.PlayersTurn) return;

                //gather all hits, instead of one
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition),
                    Vector2.zero);
                List<WorldObject> worldObjects = new List<WorldObject>();

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].transform.gameObject.GetComponent<WorldObject>() != null)
                    {
                        worldObjects.Add(hits[i].transform.gameObject.GetComponent<WorldObject>());
                    }
                }

                for (int i = 0; i < worldObjects.Count; i++)
                {
                    if (worldObjects[i].Type != TileManager.ContentType.BrokenBarrel)
                    {
                        UIManager.Instance.InGameUI.ActivateTeleportButtons(false);
                        GameManager.Instance.TileManager.HidePossibleRoads();

                        hits[i].transform.gameObject.GetComponent<WorldObject>().Click();
                        break;
                    }
                    else
                    {
                        if (worldObjects.Count > 1)
                        {
                            continue;
                        }
                        else
                        {
                            UIManager.Instance.InGameUI.ActivateTeleportButtons(false);
                            GameManager.Instance.TileManager.HidePossibleRoads();

                            hits[i].transform.gameObject.GetComponent<WorldObject>().Click();
                            break;
                        }
                    }
                }
                if(worldObjects.Count <= 0 && !UIManager.Instance.InGameUI.CastingSpell){
                    NoRaycastHit();

                }
            }
        }
        if (Input.GetMouseButton(LEFT))
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

        if (Input.GetMouseButtonUp(LEFT))
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
    }

    private void NoRaycastHit()
    {
        UIManager.Instance.InGameUI.HideSpellButtons();
        UIManager.Instance.InGameUI.ActivateTeleportButtons(false);
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();
        GameManager.Instance.TileManager.HidePossibleRoads();

        if (!stillTouching)
        {
            stillTouching = true;
            previousPosition = Input.mousePosition;
        }
    }
}
