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
            "Wizards Hat: Hi you there! I can feel some powerful magic around you...",
            "Wizards Hat: Every hat needs a wizard to be able to work... So I can't do anything.",
            "Wizards Hat: Would you like to be my wizard?",
            "Wizards Hat: Awesome! Thank you so much, we will achieve great things.",
            "Wizards Hat: ...",
            "Wizards Hat: Travelers often seek the help of wizards to stay safe on the roads.",
            "Wizards Hat: I will help you understand the basics of the art of Human-Saving.",
        });
        tutorialText.Add(new string[]
        {
            "Random Traveler: Help, help! I'm in great danger.",
            "Wizards Hat: Quick, click me to put me on your head and start this epic adventure."
        });
        tutorialText.Add(new string[]
        {
            "Wizards Hat: That's basically all there's to it! Simple right?",
            "Wizards Hat: You can now call yourself a wizard of W.O.O.S.H.",
            "Wizards Hat: That stands for WIZARDS ORDER OF STRATEGIC HUMAN-SAVING!"
        });

        counter = 0;
    }

    public void Next()
    {
        if (UIManager.Instance.LevelSelectUI.Dialog.On)
        {
            // continue dialog
            UIManager.Instance.LevelSelectUI.Dialog.Next();
            if (UIManager.Instance.LevelSelectUI.Dialog.On) return;
        }

        counter++;

        switch (counter)
        {
            // wizards hat clicked, show first hat dialog
            case 1:
                StepSelectHat();
                break;
            case 2:
                StepSelectCity();
                break;
            case 3:
                StepSelectContract();
                break;
            case 4:
                StepAcceptContract();
                break;
            case 5:
                StepClickExit();
                break;
            // first dialog with hat finished
            case 6:
                Step2();
                break;
            // first dialog with human/head
            case 7:
                Step3();
                break;
            case 8:
                Step4();
                break;
            case 9:
                Step5();
                break;
            // pre game
            case 10:
                Step6();
                break;
            case 11:
                Step7();
                break;
            case 12:
                Step8();
                break;
            case 13:
                Step9();
                break;
            case 14:
                Step10();
                break;
            case 15:
                Step11();
                break;
            case 16:
                Step12();
                break;
            case 17:
                Step13();
                break;
            case 18:
                Step14();
                break;
            case 19:
                Step15();
                break;
            case 20:
                Step16();
                break;
            case 21:
                Step17();
                break;
            case 22:
                Step18();
                break;
            case 23:
                Step19();
                break;
            case 24:
                Step20();
                break;
            case 25:
                Step21();
                break;
            case 26:
                Step22();
                break;
        }
       
    }

    // The first dialog with the hat
    private void StepSelectHat()
    {
        // deactivate no click panel and only button (op zn minst kleur uit)
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel();

        // set dialog
        UIManager.Instance.LevelSelectUI.ActivateDialog(tutorialText[0]);
    }

    private void StepSelectCity()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel(new Vector2(0, 126.0f), 
            UIManager.Instance.LevelSelectUI.TutorialCity.GetComponent<Image>().sprite, 260, 260);

        //notice: this is special case code!!
        //the selectcontractwindow.activate neeeeds to be called before the next step
        //so we first remove the tutorialmanager.next listener, than add the selectcontractwindow.activate 
        //and THAN add tutorialmanager.next again.. well hope u get the general idea. 

        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.RemoveAllListeners();

        //TODO: on click open select contract window, next step select the contract, next step = 2
        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.AddListener(
            delegate
            {
                UIManager.Instance.LevelSelectUI.SelectContractWindow.Activate(true,
                    UIManager.Instance.LevelSelectUI.TutorialCity, Destination.Tutorial);
            });

        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.AddListener(Next);

        Contract contract = UberManager.Instance.ContractManager.GenerateContract(UIManager.Instance.LevelSelectUI.TutorialPath, 1, 1);

        UIManager.Instance.LevelSelectUI.TutorialCity.RefreshAvailableContracts(contract, Destination.Tutorial);     
    }

    private void StepSelectContract()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel(new Vector2(-260.0f, 130.0f), Resources.Load<Sprite>("Sprites/UI/Tutorial/AvailableContractButton"), false, 225, 225);
        UIManager.Instance.LevelSelectUI.SetArrow(new Vector2(-260.0f, 130.0f), 280.0f, 180.0f, "Click this helpless human.");

        //notice: this is special case code!!
        //the selectcontractwindow.activate neeeeds to be called before the next step
        //so we first remove the tutorialmanager.next listener, than add the selectcontractwindow.activate 
        //and THAN add tutorialmanager.next again.. well hope u get the general idea. 

        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.RemoveAllListeners();

        //TODO: on click open select contract window, next step select the contract, next step = 2
        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.AddListener(UIManager.Instance.LevelSelectUI.SelectContractWindow.AvailableContractIndicators[0].OnClick);

        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.AddListener(Next);
    }

    private void StepAcceptContract()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel(new Vector2(-175.0f, -85.0f), Resources.Load<Sprite>("Sprites/UI/Tutorial/AcceptButton"), false, 155, 105);
        UIManager.Instance.LevelSelectUI.SetArrow(new Vector2(-175.0f, -85.0f), 300.0f, 120.0f, "Accept to protect him on his journey.");

        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.AddListener(UIManager.Instance.LevelSelectUI.SelectContractWindow.AvailableContractIndicators[0].SelectContract);
        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.AddListener(UIManager.Instance.LevelSelectUI.SelectContractWindow.MyAcceptButton.DisableWindow);
    }

    private void StepClickExit()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        UIManager.Instance.LevelSelectUI.ActivateNoClickPanel(new Vector2(300.0f, 355.0f), Resources.Load<Sprite>("Sprites/UI/WorldView/CloseWindow"), false);
        UIManager.Instance.LevelSelectUI.SetArrow(new Vector2(300.0f, 355.0f), 260.0f, 100.0f, "Close the window.");

        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.AddListener(UIManager.Instance.LevelSelectUI.SelectContractWindow.Deactivate);
    }

    // The clickable human
    private void Step2()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        Vector2 pos = (Vector2)UIManager.Instance.LevelSelectUI.TutorialPath.Levels[0].GetComponent<RectTransform>().localPosition +
                      (Vector2)UIManager.Instance.LevelSelectUI.TutorialPath.PathObject.GetComponent<RectTransform>().localPosition +
                      new Vector2(-45, 150);

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

        UIManager.Instance.LevelSelectUI.OnlyButton.onClick.AddListener(
            delegate
            {
                GameObject.Destroy(UIManager.Instance.LevelSelectUI.WizardsHat);
            }); 
    }

    // Clicking the level
    private void Step5()
    {
        UIManager.Instance.LevelSelectUI.DeactivateNoClickPanel();

        float posY =
            UIManager.Instance.LevelSelectUI.TutorialPath.Levels[0].GetComponent<RectTransform>().localPosition.y +
            UIManager.Instance.LevelSelectUI.TutorialPath.PathObject.GetComponent<RectTransform>().localPosition.y + 125;

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

        wolfPos = new Vector2(0, 180);

        UIManager.Instance.InGameUI.ActivateNoClickPanel(wolfPos, Resources.Load<Sprite>("Sprites/UI/Tutorial/Wolf"), 120, 110);

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

        UIManager.Instance.InGameUI.ActivateNoClickPanel(humanPos, GameManager.Instance.LevelManager.Humans[0].ContractRef.InWorldSprite);

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

        wolfPos = new Vector2(-108, -258);

        GameManager.Instance.LevelManager.Player.StartPlayerTurn();
        UIManager.Instance.InGameUI.BeginPlayerTurn();

        UIManager.Instance.InGameUI.ActivateNoClickPanel(wolfPos, Resources.Load<Sprite>("Sprites/UI/Tutorial/Wolf"), 120, 110);

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
        Time.timeScale = 1.0f;

        UberManager.Instance.GotoState(UberManager.GameStates.PostGame);

        UIManager.Instance.PostGameUI.DeactivateNoClickPanel();

        UIManager.Instance.PostGameUI.SetArrow(new Vector2(-110, -605), 110.0f, 100.0f, "You gain reputation if your humans makes it.");
    }

    // in level select again
    private void Step21()
    {
        UIManager.Instance.PostGameUI.DeactivateNoClickPanel();

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
        UberManager.Instance.SoundManager.PlaySoundEffect(GameManager.SpellType.Teleport);
    }

    public void TutorialAttack()
    {
        GameManager.Instance.LevelManager.Enemies[0].TryHit(10);

        UIManager.Instance.InGameUI.CastingSpell = -1;

        UIManager.Instance.InGameUI.HideSpellButtons();
        UberManager.Instance.SoundManager.PlaySoundEffect(GameManager.SpellType.Attack);
    }
}
