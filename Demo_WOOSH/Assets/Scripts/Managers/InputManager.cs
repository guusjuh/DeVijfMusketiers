﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager {
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
                        hit.transform.gameObject.GetComponent<WorldObject>().Click();
                    }
                }
                else
                {
                    UIManager.Instance.InGameUI.HideSpellButtons();
                    UIManager.Instance.InGameUI.ActivatePushButtons(false);
                }
            }
        }
    }
}
