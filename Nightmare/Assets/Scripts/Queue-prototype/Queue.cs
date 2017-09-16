using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Queue : MonoBehaviour
{
    protected int queueLength = 4;
    protected int amountOfSpells = 3;
    [SerializeField]
    protected GameObject containerParent;
    protected List<Image> containers = new List<Image>();
    [SerializeField]
    protected Sprite[] sprites; //fill in editor
    [SerializeField]
    protected Sprite emptySprite = new Sprite();
    protected Queue<int> queue = new Queue<int>();

    public virtual void Start()
    {
        //sprites = new Sprite[amountOfSpells];
        for (int i = 0; i < containerParent.transform.childCount; i++)
        {
            containers.Add(containerParent.transform.GetChild(i).gameObject.GetComponent<Image>());
        }
    }

    public void Refresh()
    {
        for (int i = 0; i < containers.Count; i++)
        {
            containers[i].sprite = getSprite(i);
        }
    }

    public void Add(int spell)
    {
        if (queue.Count < queueLength)
        {
            queue.Enqueue(spell);
            Refresh();
        }
    }

    public void Remove()
    {
        queue.Dequeue();
        Refresh();
    }

    public PlayerSpell Peek()
    {
        if (queue.Count > 0)
        {
            return (PlayerSpell)queue.Peek();
        }
        return ((PlayerSpell)(-1));
    }

    public void RemoveAt(int id)
    {
        if (id < queue.Count)
        {
            int count = queue.Count;
            for (int i = 0; i < count; i++)
            {
                if (i == id)
                {
                    queue.Dequeue();
                }
                else
                {
                    queue.Enqueue(queue.Dequeue());
                }
            }
            Refresh();
        }
    }

    /// <summary>
    /// returns sprite based on the id of the corresponding container
    /// </summary>
    /// <param name="id">container id</param>
    /// <returns></returns>
    public Sprite getSprite(int id)
    {
        if (id >= 0 && id < queue.Count)
        {
            int value = 0;
            for (int i = 0; i < queue.Count; i++)
            {
                if (i == id)
                {
                    value = queue.Peek();
                }
                queue.Enqueue(queue.Dequeue());
            }
            return (sprites[value]);
        }
        else
        {
            return emptySprite;
        }
    }
}
