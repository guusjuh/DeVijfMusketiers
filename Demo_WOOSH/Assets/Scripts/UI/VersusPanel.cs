using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VersusPanel : MonoBehaviour
{
    private Image bossImg;

    private const int MAX_HUMANS_IN_GRID = 3;
    private List<Image> humanGridFront;
    private List<Image> humanGridBack;

    private const int MAX_ENEMIES_IN_GRID = 2;
    private List<Image> enemyGridFront;
    private List<Image> enemyGridBack;

    private Dictionary<SecContentType, Sprite> enemySprites = new Dictionary<SecContentType, Sprite>();

    public void Initialize()
    {
        GetComponent<Button>().onClick.AddListener(Deactivate);

        bossImg = transform.Find("Enemies").Find("BossImg").GetComponent<Image>();

        humanGridFront = new List<Image>(transform.Find("Humans").Find("HumanGridFront").GetComponentsInChildren<Image>());
        humanGridBack = new List<Image>(transform.Find("Humans").Find("HumanGridBack").GetComponentsInChildren<Image>());

        enemyGridFront = new List<Image>(transform.Find("Enemies").Find("EnemyGridFront").GetComponentsInChildren<Image>());
        enemyGridBack = new List<Image>(transform.Find("Enemies").Find("EnemyGridBack").GetComponentsInChildren<Image>());

        enemySprites.Add(SecContentType.Arnest, Resources.Load<Sprite>("Sprites/Enemies/InWorld/Arnest"));
        enemySprites.Add(SecContentType.Dodin, Resources.Load<Sprite>("Sprites/Enemies/InWorld/Dodin"));
        enemySprites.Add(SecContentType.Sketta, Resources.Load<Sprite>("Sprites/Enemies/InWorld/Sketta"));
        enemySprites.Add(SecContentType.Wolf, Resources.Load<Sprite>("Sprites/Enemies/InWorld/Wolf"));

        enemyGridFront.HandleAction(i => i.gameObject.SetActive(false));
        enemyGridBack.HandleAction(i => i.gameObject.SetActive(false));
        humanGridFront.HandleAction(i => i.gameObject.SetActive(false));
        humanGridBack.HandleAction(i => i.gameObject.SetActive(false));

        gameObject.SetActive(false);
    }

    public void Activate()
    {
        LevelData leveldataRef = UberManager.Instance.ContentManager.LevelData(UberManager.Instance.PreGameManager.SelectedLevel);

        // set contracts
        List<Contract> contracts = UIManager.Instance.PreGameUI.PreGameInfoPanel.GetSelectedContracts();
        for (int i = 0; i < contracts.Count; i++)
        {
            if (i < MAX_HUMANS_IN_GRID)
            {
                humanGridFront[i].sprite = contracts[i].InWorldSprite;
                humanGridFront[i].gameObject.SetActive(true);
            }
            else if (i < MAX_HUMANS_IN_GRID * 2)
            {
                humanGridBack[i - MAX_HUMANS_IN_GRID].sprite = contracts[i].InWorldSprite;
                humanGridBack[i - MAX_HUMANS_IN_GRID].gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("You have to code more to have that amount of humans in your level.");
            }
        }

        // set boss
        bool arnest = leveldataRef.spawnNodes.FindAll(s => s.secType == SecContentType.Arnest).Count > 0;
        bool dodin = leveldataRef.spawnNodes.FindAll(s => s.secType == SecContentType.Dodin).Count > 0;
        bool sketta = leveldataRef.spawnNodes.FindAll(s => s.secType == SecContentType.Sketta).Count > 0;

        //note: assumed that there can be only one boss!
        if (arnest)
            bossImg.sprite = enemySprites[SecContentType.Arnest];
        else if (dodin)
            bossImg.sprite = enemySprites[SecContentType.Dodin];
        else if (sketta)
            bossImg.sprite = enemySprites[SecContentType.Sketta];

        // find amount of minions
        int amountOfMinions = leveldataRef.spawnNodes.FindAll(s => s.secType == SecContentType.Wolf).Count;
        for (int i = 0; i < amountOfMinions; i++)
        {
            if (i < MAX_ENEMIES_IN_GRID)
            {
                enemyGridFront[i].gameObject.SetActive(true);
            }
            else if (i < MAX_ENEMIES_IN_GRID * 2)
            {
                enemyGridBack[i].gameObject.SetActive(true);
            }
            else
            {
                Debug.LogError("You have to code more to have that amount of enemies in your level.");
            }
        }

        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        enemyGridFront.HandleAction(i => i.gameObject.SetActive(false));
        enemyGridBack.HandleAction(i => i.gameObject.SetActive(false));
        humanGridFront.HandleAction(i => i.gameObject.SetActive(false));
        humanGridBack.HandleAction(i => i.gameObject.SetActive(false));

        gameObject.SetActive(false);

        UIManager.Instance.PreGameUI.StartGame();
    }
}
