using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameInfoReputation : MonoBehaviour
{
    private RectTransform maskPanel;

    private float maxWidth = 900;
    private float growSpeed = 0.05f;
	// Use this for initialization
    public void Initialize(float start, float end)
    {
        maskPanel = transform.Find("Panel").GetComponent<RectTransform>();

        UberManager.Instance.StartCoroutine(gainReputation(start, end));

        FloatingIndicator fi = new FloatingIndicator();
        fi.Initialize((end >= start ? "+" : "-") + (end - start), (end >= start)? Color.green : Color.red, 0.0f, 10.0f, maskPanel.transform.position + new Vector3(maxWidth / 2.0f, 125, 0), false);
    }

    public void Restart(float start, float end)
    { 
        UberManager.Instance.StartCoroutine(gainReputation(start, end));

        FloatingIndicator fi = new FloatingIndicator();
        fi.Initialize((end >= start ? "+" : "-") + (end - start), (end >= start) ? Color.green : Color.red, 0.0f, 10.0f, maskPanel.transform.position + new Vector3(maxWidth / 2.0f, 125, 0), false);
    }

    private IEnumerator gainReputation(float start, float end)
    {
        while(Mathf.Abs(start - end) > 1){
            if(start < end){
                start += growSpeed;
            }
            else if (start > end)
            {
                start -= growSpeed;
            }
            setElements(start);

            yield return new WaitForEndOfFrame();
        } 
        setElements(end);

        yield return null;
    }

    private void setElements(float value)
    {
        maskPanel.sizeDelta = new Vector2((900.0f / 500.0f) * value, maskPanel.sizeDelta.y);
        //repBar.SetBar(value % 100);
    }
}
