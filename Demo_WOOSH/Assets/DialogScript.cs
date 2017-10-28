using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogScript : MonoBehaviour
{
    private bool on;
    public bool On { get { return on; } }

    private string currentText;
    private string[] currentConversation;
    private int counter;

    private Text textElement;

    public void Initialize()
    {
        on = false;

        textElement = GetComponentInChildren<Text>();
        gameObject.SetActive(false);
    }

    public void Activate(string[] conversation)
    {
        gameObject.SetActive(true);

        on = true;

        currentConversation = conversation;
        currentText = currentConversation[0];
        textElement.text = currentText;
    }

    public void Next()
    {
        counter++;

        if (counter >= currentConversation.Length)
        {
            Deactivate();
            return;
        }

        currentText = currentConversation[counter];
        textElement.text = currentText;
    }

    public void Deactivate()
    {
        currentConversation = null;
        on = false;
        counter = 0;
        gameObject.SetActive(false);
    }
}
