using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PostGameInfoReputation : MonoBehaviour {
    private ReputationBar repBar;
    private Text star1;
    private Text star2;


	// Use this for initialization
    public void Initialize(float start, float end)
    {
        star1 = transform.Find("Star1").Find("Text").GetComponent<Text>();
        star2 = transform.Find("Star2").Find("Text").GetComponent<Text>();

        repBar = transform.Find("ReputationBar").GetComponent<ReputationBar>();
        repBar.Initialize();

        UberManager.Instance.StartCoroutine(gainReputation(start, end));
        FloatingIndicator fi = new FloatingIndicator();
        fi.Initialize((end - start).ToString(), (end > start)? Color.green:Color.red, 0.5f, 5.0f, repBar.transform.position, false);
    }

    public void Restart(float start, float end)
    {
        UberManager.Instance.StartCoroutine(gainReputation(start, end));
    }

    private IEnumerator gainReputation(float start, float end)
    {

        while(Mathf.Abs(start - end) > 1){
            if(start < end){
                start++;
            }
            else if (start > end)
            {
                start--;
            }
            setElements(start);

            yield return new WaitForSeconds(0.1f);
        } 
        setElements(end);

        yield return null;
    }

    private void setElements(float value)
    {
        int level = (int)(value / 100.0f);
        star1.text = "" + level;
        star2.text = "" + (level + 1);

        repBar.SetBar(value % 100);
    }
}
