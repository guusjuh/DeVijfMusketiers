using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Security.Policy;

public static class MetricsDataClass
{
    private static int acceptedContracts = 0;
    public static int AcceptedContracts { get { return acceptedContracts; } set { acceptedContracts = value; } }

    private static int wins = 0;
    public static int Wins { get { return wins; } }
    private static int loses = 0;
    public static int Loses { get { return loses; } }

    private static int[] timesStartedLevel = new int[3];
    public static void StartedLevel(int id)
    {
        if (id > timesStartedLevel.Length) return;
        timesStartedLevel[id]++;
    }

    private static int humansStartedLevel = 0;
    public static int HumansStartedLevel { get { return humansStartedLevel; } }
    private static int humansContinued;
    public static int HumansContinued { get { return humansContinued; } }
    private static int humansStayed = 0;
    public static int HumansStayed { get { return humansStayed; } }
    private static int humansDied = 0;
    public static int HumansDied { get { return humansDied; } }

    private static List<float> changesInReputation = new List<float>();
    public static void AddChangeInReputation(float changeInReputation)
    { 
        changesInReputation.Add(changeInReputation);
    }

    private static Dictionary<GameManager.SpellType, int> usedSpells = new Dictionary<GameManager.SpellType, int>()
    {
        {GameManager.SpellType.Attack, 0},
        {GameManager.SpellType.Teleport, 0},
        {GameManager.SpellType.FrostBite, 0},
        {GameManager.SpellType.Fireball, 0}
    };
    public static void UsedSpell(GameManager.SpellType usedSpell)
    {
        usedSpells[usedSpell]++;
    }

    private static Dictionary<int, int> selectedEnemyPerTurn = new Dictionary<int, int>();
    public static void EnemySelected()
    {
        int turnNumber = GameManager.Instance.LevelManager.AmountOfTurns;
        if (selectedEnemyPerTurn.ContainsKey(turnNumber)) selectedEnemyPerTurn[turnNumber]++;
        else selectedEnemyPerTurn.Add(turnNumber, 1);
    }

    private static Dictionary<Contract, float> changeInHappinessPerContract = new Dictionary<Contract, float>();
    public static void ChangeInHappiness(Contract changedContract, float changeInHappiness)
    {
        if (changeInHappinessPerContract.ContainsKey(changedContract)) changeInHappinessPerContract[changedContract] += changeInHappiness;
        else changeInHappinessPerContract.Add(changedContract, changeInHappiness);
    }

    private static int humansDiedByTiles = 0;
    public static int HumansDiedByTiles { get { return humansDiedByTiles; } }
    private static int humansDiedByEnemy = 0;
    public static int HumansDiedByEnemy { get { return humansDiedByEnemy; } }

    private static int activatedShrines = 0;
    public static int ActivatedShrines { get { return activatedShrines; } }

    private static int skipTurnUsed = 0;
    public static int SkipTurnUsed { get { return skipTurnUsed; } }

    private static List<KeyValuePair<int, TimeSpan>> timePerLevel = new List<KeyValuePair<int, TimeSpan>>();
    private static int currentLevelID = 0;
    private static DateTime currentStartTimeLevel = DateTime.MinValue;
    public static void StartLevel()
    {
        currentLevelID = GameManager.Instance.CurrentLevel;
        currentStartTimeLevel = System.DateTime.Now;
    }
    public static void EndLevel()
    {
        timePerLevel.Add(new KeyValuePair<int, TimeSpan>(currentLevelID, currentStartTimeLevel - System.DateTime.Now));
    }

    private static List<TimeSpan> timeInBetweenAction = new List<TimeSpan>();
    private static DateTime currentStartTimeAction = DateTime.MinValue;
    public static void StartAbleToDoAction()
    {
        currentStartTimeAction = System.DateTime.Now;
    }
    public static void ActionExecuted()
    {
        timeInBetweenAction.Add(currentStartTimeAction - System.DateTime.Now);
    }

    public static void WriteDataToCSV()
    {
        
    }
}
