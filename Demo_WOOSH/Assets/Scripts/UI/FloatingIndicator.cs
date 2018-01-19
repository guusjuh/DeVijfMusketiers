using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class FloatingIndicator
{
    // The offset in y-axis, how much the number should appear above it's unit. 
    private float offsetYaxis;
    private Vector3 startPos;

    // The speed of the number floating up.
    private float moveSpeed = 3.5f;

    private RectTransform parentGO;

    private Text floatingText;

    // The time the number will be shown. 
    private float lifeTime;

    // The coroutint that handles floating up. 
    private Coroutine floatUp;

    /// <summary>
    /// Initializes the floating number.
    /// </summary>
    public void Initialize(string text, Color color, float moveSpeed, float lifeTime, Vector3 startPos, bool inGame = true, Transform parent = null)
    {
        this.moveSpeed = moveSpeed;
        this.lifeTime = lifeTime;
        this.startPos = startPos;

        // Load the floating text and instantiate it. 
        parentGO = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/FloatingIndicator")).GetComponent<RectTransform>();

        floatingText = parentGO.transform.Find("Child").GetComponent<Text>();
        floatingText.color = color;

        if (parent == null) parent = UIManager.Instance.InGameUI.AnchorCenter;
        parentGO.transform.SetParent(parent);

        parentGO.anchoredPosition = Vector3.zero;

        // Set the object to non-active. 
        parentGO.gameObject.SetActive(false);

        // Start at only a tiny bit offset, increase over time. 
        offsetYaxis = 0.05f;

        if (inGame)
            Activate(text, color);
        else
            ActivateUI(text, color);
    }

    /// <summary>
    /// Activates the floating number.
    /// </summary>
    /// <param name="text">The taken damage.</param>
    /// <param name="startPos">The position the number starts floating up. </param>
    public void ActivateUI(string text, Color color)
    {
        // Calculate the position based on the camera. 
        Vector3 uiPosition = startPos;
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
        floatUp = UberManager.Instance.StartCoroutine(FloatUpUI());
    }

    /// <summary>
    /// Make the number float up. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator FloatUpUI()
    {
        // While lifetime is not passed. 
        while (lifeTime > 0)
        {
            // Get delta time. 
            float deltaTime = Time.deltaTime;

            // Decrease lifetime by delta time. 
            lifeTime -= deltaTime;

            offsetYaxis += moveSpeed;

            Vector3 uiPosition = startPos;
            uiPosition.y += offsetYaxis;
            uiPosition.z = 0;

            // Go up at the speed of moveSpeed.
            parentGO.anchoredPosition = new Vector3(uiPosition.x, uiPosition.y);

            // Wait delta time. 
            yield return new WaitForSeconds(deltaTime);
        }

        // After lifetime has passed, set the object to non-active. 
        parentGO.gameObject.SetActive(false);
        Destroy();

        yield break;
    }

    /// <summary>
    /// Activates the floating number.
    /// </summary>
    /// <param name="text">The taken damage.</param>
    /// <param name="startPos">The position the number starts floating up. </param>
    public void Activate(string text, Color color)
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

            offsetYaxis += moveSpeed;

            Vector3 uiPosition = UIManager.Instance.InGameUI.WorldToCanvas(startPos);
            uiPosition.y += offsetYaxis;
            uiPosition.z = 0;

            // Go up at the speed of moveSpeed.
            parentGO.anchoredPosition = new Vector3(uiPosition.x, uiPosition.y);

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
