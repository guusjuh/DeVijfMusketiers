using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellVisual : MonoBehaviour
{
    private Image image;
    private RectTransform rect;

    private const float MAX_SIZE = 2500.0f;
    private const float MIN_SIZE = 10.0f;

    // per second
    private float shrinkSpeed = 3500.0f;
    private float rotationSpeed = 270.0f;
    
    private Vector2 worldPos;

    public void Initialize()
    {
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();

        gameObject.SetActive(false);
    }

    public IEnumerator Activate(GameManager.SpellType type, Vector2 worldPos)
    {
        this.worldPos = worldPos;

        rect.sizeDelta = new Vector2(MAX_SIZE, MAX_SIZE);

        gameObject.SetActive(true);
        image.color = UberManager.Instance.UiManager.InGameUI.SpellColor(type);

        yield return StartCoroutine(Adjust());
    }

    private IEnumerator Adjust()
    {
        while (rect.sizeDelta.x > MIN_SIZE)
        {
            Shrink();
            Rotate();
            CenterOnTarget();

            yield return null;
        }

        yield return null;
    }

    private void Shrink()
    {
        rect.sizeDelta = new Vector2(rect.sizeDelta.x - shrinkSpeed * Time.deltaTime, 
                                     rect.sizeDelta.y - shrinkSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        rect.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    private void CenterOnTarget()
    {
        rect.anchoredPosition = UIManager.Instance.InGameUI.WorldToCanvas(worldPos);
    }
}
