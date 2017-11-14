using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameInfoReputation : MonoBehaviour
{
    private RectTransform maskPanel;

    private float maxWidth = 900;
    float sizeOneStar = 100.0f;

    private float growSpeed = 0.2f;

	// Use this for initialization
    public void Initialize(float start, float end)
    {      
        maskPanel = transform.Find("Panel").GetComponent<RectTransform>();

        UberManager.Instance.StartCoroutine(gainReputation(start, end));

        FloatingIndicator fi = new FloatingIndicator();
        fi.Initialize((end >= start ? "+" : "") + (end - start), (end >= start)? Color.green : Color.red, 0.0f, 10.0f, new Vector2(0.0f, 125.0f), false, transform);
    }

    public void Restart(float start, float end)
    {
        UberManager.Instance.StartCoroutine(gainReputation(start, end));

        FloatingIndicator fi = new FloatingIndicator();
        fi.Initialize((end >= start ? "+" : "") + (end - start), (end >= start) ? Color.green : Color.red, 0.0f, 10.0f, new Vector2(0.0f, 125.0f), false, transform);
    }

    private IEnumerator gainReputation(float start, float end)
    {
        int startCompletedStars = UberManager.Instance.PlayerData.LevelForRep(start);

        float startNextRep = UberManager.Instance.PlayerData.ReqRep(startCompletedStars + 1);
        float startLastRep = UberManager.Instance.PlayerData.ReqRep(startCompletedStars);

        float startPercentInThis = ((start - startLastRep) / (startNextRep - startLastRep)) * 100.0f;
        float startSize = (startCompletedStars*sizeOneStar) + startPercentInThis;

        // -----

        int endCompletedStars = UberManager.Instance.PlayerData.LevelForRep(end);

        float endNextRep = UberManager.Instance.PlayerData.ReqRep(endCompletedStars + 1);
        float endLastRep = UberManager.Instance.PlayerData.ReqRep(endCompletedStars);

        float endPercentInThis = ((end - endLastRep) / (endNextRep - endLastRep)) * 100.0f;
        float endSize = (endCompletedStars * sizeOneStar) + endPercentInThis;

        // -----

        while (Mathf.Abs(startSize - endSize) > 1){
            if(startSize < endSize){
                startSize += growSpeed;
            }
            else if (startSize > endSize)
            {
                startSize -= growSpeed;
            }
            SetElements(startSize);

            yield return new WaitForEndOfFrame();
        }
        SetElements(startSize);

        yield return null;
    }

    private void SetElements(float value)
    {
        maskPanel.sizeDelta = new Vector2((900 / 500.0f) * value, maskPanel.sizeDelta.y);
    }
}
