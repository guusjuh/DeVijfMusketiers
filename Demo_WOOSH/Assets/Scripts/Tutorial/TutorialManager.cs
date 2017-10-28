using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager
{
    private List<string[]> tutorialText;

    private int counter;

    private Vector2 wolfPos;

    public void Initialize()
    {
        tutorialText = new List<string[]>();
        tutorialText.Add(new string[]
        {
            "Wizards Hat: Hi you there! I can feel some magic around you...",
            "Wizards Hat: I believe we should work together to achieve great things!",
            "Wizards Hat: Travelers often seek the help of wizards to stay safe on the roads.",
            "Wizards Hat: You see that human over there? That's our first training!",
        });
        tutorialText.Add(new string[]
        {
            "Random Traveler: Help, help! I'm in great danger.",
            "Wizards Hat: Quick, clike me to put me on your head and start an epic adventure."
        });
        tutorialText.Add(new string[]
        {
            "Wizards Hat: That's basically all there's to it! Simple right?",
            "Wizards Hat: You can now call yourself a wizard of W.O.O.S.H.",
            "Wizards Hat: That stands for WIZARDS ORDER OF STRATEGIC HUMAN-SAVING ofcourse!"
        });


        counter = 0;
    }

    public void Next()
    {
        if (UIManager.Instance.LevelSelectUI.Dialog.On)
        {
            // continue dialog
            UIManager.Instance.LevelSelectUI.Dialog.Next();
            Debug.Log("next dialog");
            if (UIManager.Instance.LevelSelectUI.Dialog.On) return;
        }

        counter++;

        switch (counter)
        {
            // wizards hat clicked, show first hat dialog
            case 1:
                Step1();
                Debug.Log("Completed clicking on wizards hat");
                break;
            // first dialog with hat finished
            case 2:
                Step2();
                Debug.Log("Completed firt dialog");
                break;
            // first dialog with human/head
            case 3:
                Step3();
                Debug.Log("Clicked human");
                break;
            case 4:
                Step4();
                Debug.Log("Dialog with human & hat over");
                break;
            case 5:
                Step5();
                break;
            // pre game
            case 6:
                Step6();
                break;
            case 7:
                Step7();
                break;
            case 8:
                Step8();
                break;
            case 9:
                Step9();
                break;
            case 10:
                Step10();
                break;
            case 11:
                Step11();
                break;
            case 12:
                Step12();
                break;
            case 13:
                Step13();
                break;
            case 14:
                Step14();
                break;
            case 15:
                Step15();
                break;
            case 16:
                Step16();
                break;
            case 17:
                Step17();
                break;
            case 18:
                Step18();
                break;
            case 19:
                Step19();
                break;
            case 20:
                Step20();
                break;
            case 21:
                Step21();
                break;
            case 22:
                Step22();
                break;
        }
       
    }

    // The first dialog with the hat
    private void Step1()
    {
        // deactivate no click panel and only button (op zn minst kleur uit)
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel();

        // set dialog
        UIManager.Instance.LevelSelectUI.ActivateDialog(tutorialText[0]);
    }

    // The clickable human
    private void Step2()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        Vector2 pos = (Vector2)UIManager.Instance.LevelSelectUI.TutorialPath.Levels[0].GetComponent<RectTransform>().localPosition +
                      (Vector2)UIManager.Instance.LevelSelectUI.TutorialPath.PathObject.GetComponent<RectTransform>().localPosition +
                      new Vector2(-45, 30);

        // human clickable
        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel(pos, UIManager.Instance.LevelSelectUI.TutorialPath.Levels[0].firstContractIndicator.sprite);
    }

    // Dialog with the human
    private void Step3()
    {
        // deactivate no click panel and only button (op zn minst kleur uit)
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel();

        // set dialog
        UIManager.Instance.LevelSelectUI.ActivateDialog(tutorialText[1]);
    }

    // Clicking the hat
    private void Step4()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        // human clickable
        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel(UIManager.Instance.LevelSelectUI.WizardsHat.GetComponent<RectTransform>().anchoredPosition,
                                                              UIManager.Instance.LevelSelectUI.WizardsHat.GetComponentInChildren<Image>().sprite);
    }

    // Clicking the level
    private void Step5()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        float posY = UIManager.Instance.LevelSelectUI.TutorialPath.Levels[0].GetComponent<RectTransform>().localPosition.y +
                      UIManager.Instance.LevelSelectUI.TutorialPath.PathObject.GetComponent<RectTransform>().localPosition.y;

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel(new Vector2(0, posY), UIManager.Instance.LevelSelectUI.TutorialPath.Levels[0].levelSelectButtonImage.sprite);
    }

    // Goto next state and
    // Clicking the selectable contract
    private void Step6()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        // start tutorial level
        UberManager.Instance.PreGameManager.SelectedLevel = 5;
        UberManager.Instance.GotoState(UberManager.GameStates.PreGame);

        UIManager.Instance.PreGameUI.DeactivateNoClickPanel();

        UIManager.Instance.PreGameUI.PreGameInfoPanel.FirstSelectableContract.transform.parent.GetComponent<Button>().onClick.AddListener(Next);

        // set arrow
        UIManager.Instance.PreGameUI.SetArrow(new Vector2(-330, -280), 70.0f, 200.0f, "Select the human for this level.");
    }

    // Clicking the start level button
    private void Step7()
    {
        UIManager.Instance.PreGameUI.PreGameInfoPanel.FirstSelectableContract.transform.parent.GetComponent<Button>().onClick.RemoveAllListeners();

        UIManager.Instance.PreGameUI.DeactivateNoClickPanel();

        UIManager.Instance.PreGameUI.ActivateNoClickPanel(new Vector2(365, -910), Resources.Load<Sprite>("Sprites/UI/Tutorial/StartLevel"), 240, 240);

        UIManager.Instance.PreGameUI.SetArrow(new Vector2(365, -910), 120.0f, 75.0f, "Now start the level.");
    }

    // Goto ingame and
    // Pausing till the player clicks (on the enemy? with a text?)
    private void Step8()
    {
        UIManager.Instance.PreGameUI.StartGame();

        Time.timeScale = 0;

        wolfPos = UIManager.Instance.InGameUI.WorldToCanvas(GameManager.Instance.LevelManager.Enemies[0].transform.position);

        UIManager.Instance.InGameUI.ActivateNoClickPanel(wolfPos, 
            GameManager.Instance.LevelManager.Enemies[0].GetComponent<SpriteRenderer>().sprite);

        UIManager.Instance.InGameUI.SetArrow(wolfPos, 100.0f, 100.0f, "The enemy goes first. Click to start.");
    }

    // do enemy turn
    private void Step9()
    {
        UIManager.Instance.InGameUI.DeactivateNoClickPanel();

        Time.timeScale = 1.0f;
    }

    // select human
    private void Step10()
    {
        Time.timeScale = 0;

        Vector2 humanPos = UIManager.Instance.InGameUI.WorldToCanvas(GameManager.Instance.LevelManager.Humans[0].transform.position);

        GameManager.Instance.LevelManager.Player.StartPlayerTurn();
        UIManager.Instance.InGameUI.BeginPlayerTurn();

        UIManager.Instance.InGameUI.ActivateNoClickPanel(humanPos,
            GameManager.Instance.LevelManager.Humans[0].GetComponent<SpriteRenderer>().sprite);

        UIManager.Instance.InGameUI.OnlyButton.onClick.AddListener(GameManager.Instance.LevelManager.Humans[0].Click);

        UIManager.Instance.InGameUI.SetArrow(humanPos, 110.0f, 100.0f, "Now it's your turn. Click the human first.");
    }

    // select teleport button
    private void Step11()
    {
        UIManager.Instance.InGameUI.DeactivateNoClickPanel();

        Vector2 teleportButtonPos = new Vector2(0, -115);

        UIManager.Instance.InGameUI.ActivateNoClickPanel(teleportButtonPos, Resources.Load<Sprite>("Sprites/UI/Tutorial/TeleportButton"), 125, 125);

        UIManager.Instance.InGameUI.OnlyButton.onClick.AddListener(UIManager.Instance.InGameUI.TeleportButton.Click);

        UIManager.Instance.InGameUI.SetArrow(teleportButtonPos, 70.0f, 100.0f, "To flee, select the teleport spell.");
    }

    // wait for spell visual
    private void Step12()
    {
        UIManager.Instance.InGameUI.DeactivateNoClickPanel();

        Time.timeScale = 1.0f;
    }

    // select surrounding push button
    private void Step13()
    {
        Time.timeScale = 0.0f;

        UIManager.Instance.InGameUI.DeactivateNoClickPanel();

        Vector2 highlightPos = UIManager.Instance.InGameUI.WorldToCanvas(GameManager.Instance.TileManager.GetWorldPosition(new Coordinate(0, 0)));

        UIManager.Instance.InGameUI.ActivateNoClickPanel(highlightPos, Resources.Load<Sprite>("Sprites/UI/Tutorial/HighHex"), 150, 130);

        UIManager.Instance.InGameUI.OnlyButton.onClick.AddListener(TutorialTeleport);

        UIManager.Instance.InGameUI.SetArrow(highlightPos, 80.0f, 100.0f, "Click here to teleport to this tile.");
    }

    // enemy turn
    private void Step14()
    {
        UIManager.Instance.InGameUI.DeactivateNoClickPanel();

        Time.timeScale = 1.0f;
    }

    // select the enemy
    private void Step15()
    {
        Time.timeScale = 0;

        wolfPos = UIManager.Instance.InGameUI.WorldToCanvas(GameManager.Instance.LevelManager.Enemies[0].transform.position);

        GameManager.Instance.LevelManager.Player.StartPlayerTurn();
        UIManager.Instance.InGameUI.BeginPlayerTurn();

        UIManager.Instance.InGameUI.ActivateNoClickPanel(wolfPos,
            GameManager.Instance.LevelManager.Enemies[0].GetComponent<SpriteRenderer>().sprite);

        UIManager.Instance.InGameUI.OnlyButton.onClick.AddListener(GameManager.Instance.LevelManager.Enemies[0].Click);

        UIManager.Instance.InGameUI.SetArrow(wolfPos, 70.0f, 100.0f, "Attack the enemy to save the human.");
    }

    // select the attack spell
    private void Step16()
    {
        UIManager.Instance.InGameUI.DeactivateNoClickPanel();

        Vector2 attackButtonPos = new Vector2(-105, -50);

        UIManager.Instance.InGameUI.ActivateNoClickPanel(attackButtonPos, Resources.Load<Sprite>("Sprites/UI/Tutorial/AttackButton"), 125, 125);

        UIManager.Instance.InGameUI.OnlyButton.onClick.AddListener(UIManager.Instance.InGameUI.AttackButton.Click);

        UIManager.Instance.InGameUI.SetArrow(attackButtonPos, 70.0f, 100.0f, "To attack, select the attack spell.");
    }

    // wait for spell visual
    private void Step17()
    {
        UIManager.Instance.InGameUI.DeactivateNoClickPanel();

        Time.timeScale = 1.0f;
    }

    // apply spell and wait
    private void Step18()
    {
        TutorialAttack();

        Time.timeScale = 0.0f;
    }

    // tell the player he won
    private void Step19()
    {
        UIManager.Instance.InGameUI.ActivateNoClickPanel();

        UIManager.Instance.InGameUI.SetArrow(wolfPos, 260.0f, 100.0f, "You killed him!");
    }

    // goto next state
    private void Step20()
    {
        UberManager.Instance.GotoState(UberManager.GameStates.PostGame);

        UIManager.Instance.PostGameUI.DeactivateNoClickPanel();

        UIManager.Instance.PostGameUI.SetArrow(new Vector2(-105, 345), 110.0f, 100.0f, "Your human made it!");
    }

    // in level select again
    private void Step21()
    {
        UIManager.Instance.PostGameUI.DeactivateNoClickPanel();

        Time.timeScale = 1.0f;

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel();

        // last dialog with hat
        UIManager.Instance.LevelSelectUI.ActivateDialog(tutorialText[2]);
    }

    private void Step22()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        // tutorial = false
        UberManager.Instance.EndTutorial();

        UberManager.Instance.GotoState(UberManager.GameStates.LevelSelection);
    }

    public void TutorialTeleport()
    {
        GameManager.Instance.LevelManager.Humans[0].Teleport(new Coordinate(0, 0));
        GameManager.Instance.LevelManager.CheckForExtraAP();
        GameManager.Instance.LevelManager.EndPlayerMove(3);
        GameManager.Instance.LevelManager.Player.SetCooldown(GameManager.SpellType.Teleport);
    }

    public void TutorialAttack()
    {
        GameManager.Instance.LevelManager.Enemies[0].TryHit(10);

        UIManager.Instance.InGameUI.CastingSpell = -1;

        UIManager.Instance.InGameUI.HideSpellButtons();
    }
}
