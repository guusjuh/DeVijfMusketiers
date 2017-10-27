using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class FloatingIndicator
{
    // The offset in y-axis, how much the number should appear above it's unit. 
    private float offsetYaxis;

    // The speed of the number floating up.
    private float moveSpeed = 3.5f;

    private RectTransform parentGO;

    // The text element. 
    private Text floatingText;

    // The time the number will be shown. 
    private float lifeTime;

    // The coroutint that handles floating up. 
    private Coroutine floatUp;

    /// <summary>
    /// Initializes the floating number.
    /// </summary>
    public void Initialize(string text, Color color, float moveSpeed, float lifeTime, Vector3 startPos)
    {
        this.moveSpeed = moveSpeed;
        this.lifeTime = lifeTime;

        // Load the floating text and instantiate it. 
        parentGO = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/FloatingIndicator")).GetComponent<RectTransform>();

        floatingText = parentGO.transform.Find("Child").GetComponent<Text>();
        floatingText.color = color;

        parentGO.transform.SetParent(UIManager.Instance.InGameUI.AnchorCenter);
        parentGO.anchoredPosition = Vector3.zero;

        // Set the object to non-active. 
        parentGO.gameObject.SetActive(false);

        // Set the offset to a hardcoded value for now. 
        offsetYaxis = 0.05f;

        Activate(text, color, startPos);
    }

    /// <summary>
    /// Activates the floating number.
    /// </summary>
    /// <param name="text">The taken damage.</param>
    /// <param name="startPos">The position the number starts floating up. </param>
    public void Activate(string text, Color color, Vector3 startPos)
    {
        // Calculate the position based on the camera. 
        Vector3 uiPosition = UIManager.Instance.InGameUI.WorldToCanvas(startPos);
        uiPosition.y += offsetYaxis;
        uiPosition.z = 0;

        // Set it's transform. 
        parentGO.anchoredPosition = uiPosition;
        parentGO.localScale = new Vector3(1, 1, 1);

        // Set damage text. 
        floatingText.text = text;

        // Set the gameobject to active. 
        parentGO.gameObject.SetActive(true);

        // Start the coroutine to make the number float up. 
        floatUp = UberManager.Instance.StartCoroutine(FloatUp());
    }

    /// <summary>
    /// Make the number float up. 
    /// </summary>
    /// <returns></returns>
	private IEnumerator FloatUp()
    {
        // While lifetime is not passed. 
        while (lifeTime > 0)
        {
            // Get delta time. 
            float deltaTime = Time.deltaTime;

            // Decrease lifetime by delta time. 
            lifeTime -= deltaTime;

            // Go up at the speed of moveSpeed.
            parentGO.anchoredPosition = new Vector3(parentGO.anchoredPosition.x, parentGO.anchoredPosition.y + moveSpeed);

            // Wait delta time. 
            yield return new WaitForSeconds(deltaTime);
        }

        // After lifetime has passed, set the object to non-active. 
        parentGO.gameObject.SetActive(false);
        Destroy();

        yield break;
    }

    /// <summary>
    /// Destorys the object. 
    /// </summary>
    public void Destroy()
    {
        // If the coroutine reference is not empty.
        if (floatUp != null)
        {
            // Stop the coroutine and remove the reference. 
            UberManager.Instance.StopCoroutine(floatUp);
            floatUp = null;
        }

        // Destrory the gameobject. 
        GameObject.Destroy(parentGO.gameObject);
    }
}
