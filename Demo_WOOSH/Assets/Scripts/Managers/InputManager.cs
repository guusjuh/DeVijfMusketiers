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

    private bool stillTouching = false;

    public void CatchInput()
    {
        zoomVelocity = Input.GetAxis("Mouse ScrollWheel");

        if (Input.GetMouseButtonDown(LEFT))
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Input.mousePosition;
                // use the position from controller as start of raycast instead of mousePosition.

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count <= 0)
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
                        UIManager.Instance.InGameUI.ActivatePushButtons(false);
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
                            UIManager.Instance.InGameUI.ActivatePushButtons(false);
                            GameManager.Instance.TileManager.HidePossibleRoads();

                            hits[i].transform.gameObject.GetComponent<WorldObject>().Click();
                            break;
                        }
                    }
                }
                if(worldObjects.Count <= 0){
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
        UIManager.Instance.InGameUI.ActivatePushButtons(false);
        UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();
        GameManager.Instance.TileManager.HidePossibleRoads();

        if (!stillTouching)
        {
            stillTouching = true;
            previousPosition = Input.mousePosition;
        }
    }
}
