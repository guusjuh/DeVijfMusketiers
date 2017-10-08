using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager
{
    private const int LEFT = 0;
    private Vector2 previousPosition;

    private Vector2 dragVelocity;
    public Vector2 DragVelocity { get { return dragVelocity; } }

    private bool stillTouching = false;

    public void CatchInput() {
        if (Input.GetMouseButtonDown(0)) {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = Input.mousePosition; // use the position from controller as start of raycast instead of mousePosition.

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if(results.Count <= 0)
            {
                if (!GameManager.Instance.LevelManager.PlayersTurn) return;

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.transform.gameObject.GetComponent<WorldObject>() != null)
                    {
                        UIManager.Instance.InGameUI.ActivatePushButtons(false);
                        hit.transform.gameObject.GetComponent<WorldObject>().Click();
                    }
                }
                else
                {
                    UIManager.Instance.InGameUI.HideSpellButtons();
                    UIManager.Instance.InGameUI.ActivatePushButtons(false);
                    UIManager.Instance.InGameUI.EnemyInfoUI.OnChange();

                    if (!stillTouching)
                    {
                        stillTouching = true;
                        previousPosition = Input.mousePosition;
                    }
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
}
