using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public static class MetricsDataClass
{
    public static int AcceptedContracts { get; set; }

    public static int Wins { get; set; }
    public static int Loses { get; set; }

    private static int[] timesStartedLevel = new int[]{0, 0, 0};
    public static void StartedLevel(int id)
    {
        if (id > timesStartedLevel.Length) return;
        timesStartedLevel[id - 1]++;
    }

    public static int HumansStartedLevel { get; set; }
    public static int HumansContinued { get; set; }
    public static int HumansStayed { get; set; }
    public static int HumansDied { get; set; }

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

    public static int HumansDiedByTiles { get; set; }
    public static int HumansDiedByEnemy { get; set; }

    public static int ActivatedShrines { get; set; }
    public static int SkipTurnUsed { get; set; }

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
        timePerLevel.Add(new KeyValuePair<int, TimeSpan>(currentLevelID, System.DateTime.Now - currentStartTimeLevel));
    }

    private static List<TimeSpan> timeInBetweenAction = new List<TimeSpan>();
    private static DateTime currentStartTimeAction = DateTime.MinValue;
    public static void StartAbleToDoAction()
    {
        currentStartTimeAction = System.DateTime.Now;
    }
    public static void ActionExecuted()
    {
        timeInBetweenAction.Add(System.DateTime.Now - currentStartTimeAction);
    }

    public static void WriteDataToCSV()
    {
        float changeInRep = 0;
        changesInReputation.HandleAction(r => changeInRep += r);
        changeInRep /= (float)changesInReputation.Count;

        float averageEnemySelectionPerTurn = 0;
        foreach (KeyValuePair<int, int> entry in selectedEnemyPerTurn)
        {
            averageEnemySelectionPerTurn += entry.Value;
        }
        averageEnemySelectionPerTurn /= (float)selectedEnemyPerTurn.Count;

        float changeInHappiness = 0;
        foreach (KeyValuePair<Contract, float> entry in changeInHappinessPerContract)
        {
            changeInHappiness += entry.Value;
        }
        changeInHappiness /= (float) changeInHappinessPerContract.Count;

        float averageTimePerLevel = 0;
        foreach (KeyValuePair<int, TimeSpan> entry in timePerLevel)
        {
            averageTimePerLevel += entry.Value.Seconds;
        }
        averageTimePerLevel /= timePerLevel.Count;

        float averageTimeInBetweenActions = 0;
        foreach (TimeSpan entry in timeInBetweenAction)
        {
            averageTimeInBetweenActions += entry.Seconds;
        }
        averageTimeInBetweenActions /= timeInBetweenAction.Count;

        string columnNames = "TutorialPlayed" + ", " + 
                             "AcceptedContracts" + ", " +
                             "Wins" + ", " +
                             "Loses" + ", " +
                             "StartedLevel1" + ", " +
                             "StartedLevel2" + ", " +
                             "StartedLevel3" + ", " +
                             "HumansStarted" + ", " +
                             "HumansContinued" + ", " +
                             "HumansStayed" + ", " +
                             "HumansDied" + ", " +
                             "ChangeInReputation" + ", " +
                             "AttackSpellUsed" + ", " +
                             "TeleportSpellUsed" + ", " +
                             "FireballSpellUsed" + ", " +
                             "FrostbiteSpellUsed" + ", " +
                             "AverageSelectedEnemiesPerTurn" + ", " +
                             "AverageChangeInHappiness" + ", " +
                             "HumansDiedByTiles" + ", " +
                             "HumansDiedByEnemy" + ", " +
                             "ActivatedShrines" + ", " +
                             "SkipTurnUsed" + ", " +
                             "AverageTimePerLevel" + ", " +
                             "AverageTimeInBetweenActions";

        string generalData = UberManager.Instance.TestWithTutorial.ToString() + ", " + 
                             AcceptedContracts.ToString() + ", " +
                             Wins.ToString() + ", " +
                             Loses.ToString() + ", " +
                             timesStartedLevel[0].ToString() + ", " +
                             timesStartedLevel[1].ToString() + ", " +
                             timesStartedLevel[2].ToString() + ", " +
                             HumansStartedLevel.ToString() + ", " +
                             HumansContinued.ToString() + ", " +
                             HumansStayed.ToString() + ", " +
                             HumansDied.ToString() + ", " +
                             changeInRep.ToString() + ", " +
                             usedSpells[GameManager.SpellType.Attack].ToString() + ", " +
                             usedSpells[GameManager.SpellType.Teleport].ToString() + ", " +
                             usedSpells[GameManager.SpellType.Fireball].ToString() + ", " +
                             usedSpells[GameManager.SpellType.FrostBite].ToString() + ", " +
                             averageEnemySelectionPerTurn.ToString() + ", " +
                             changeInHappiness.ToString() + ", " +
                             HumansDiedByTiles.ToString() + ", " +
                             HumansDiedByEnemy.ToString() + ", " +
                             ActivatedShrines.ToString() + ", " +
                             SkipTurnUsed.ToString() + ", " +
                             averageTimePerLevel.ToString() + ", " +
                             averageTimeInBetweenActions.ToString();

        DirectoryInfo dir = new DirectoryInfo("Assets/CSV");
        FileInfo[] allFiles = dir.GetFiles("*.csv");
        List<int> validFiles = new List<int>();

        foreach (FileInfo f in allFiles)
        {
            Match match = Regex.Match(f.Name, @"\d+");//"[0-9]*");

            validFiles.Add(int.Parse(match.Value));
        }

        string fileName = "test" + validFiles.Count + ".csv";

        string filePath = Application.dataPath + "/CSV/" + fileName;
        StreamWriter writer = new StreamWriter(filePath);
        writer.WriteLine(columnNames);
        writer.WriteLine(generalData);
        writer.Flush();
        writer.Close();
    }
}
