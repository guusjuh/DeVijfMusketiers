using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;

using UnityEngine;

public class GooglePlayServices
{
    private const string SAVE_NAME = "Save";
    private bool isSaving;

    private SaveData data;
    public bool GetTutorialFinished { get { return data.tutorialFinsihed; } }
    public int GetGamespeed { get { return data.gameSpeed; } }
    public TimeSpan TotalPlayTime { get { return data.GetTotalPlaytime; } }

    public void Initialize()
    {
        data = new SaveData();
        data.Initialize();

        if (!PlayerPrefs.HasKey(SAVE_NAME)) PlayerPrefs.SetString(SAVE_NAME, DataToString(SaveData.InitialData()));
        if (!PlayerPrefs.HasKey("IsFirstTime")) PlayerPrefs.SetInt("IsFirstTime", 1); // 1 for true since booleans dont exist in playerprefs

        LoadLocal();

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
            .EnableSavedGames()
            .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        Social.localUser.Authenticate(succes => { Load(); });
    }

    private string DataToString(SaveData data)
    {
        if (data == null) return "";

        // create the serializer
        XmlSerializer serizalizer = new XmlSerializer(typeof(SaveData));
        using (MemoryStream m = new MemoryStream())
        {
            serizalizer.Serialize(m, data);
            m.Flush();
            return Encoding.UTF8.GetString(m.GetBuffer());
        }
    }

    private SaveData StringToData(string stringData)
    {
        if (stringData == "" || stringData == string.Empty) return null;

        SaveData tempData = null;

        XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
        using (MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(stringData)))
        {
            tempData = (SaveData)serializer.Deserialize(m);
        }

        return tempData;
    }

    private void OpenSavedGame()
    {
        (Social.Active as PlayGamesPlatform).SavedGame.OpenWithAutomaticConflictResolution(SAVE_NAME,
            DataSource.ReadCacheOrNetwork, ConflictResolutionStrategy.UseLongestPlaytime, OnSavedGameOpened);
    }

    public void Save()
    {
        // so you now everything is up-to-date!
        data.UpdateSaved();

        if (Social.localUser.authenticated)
        {
            isSaving = true;
            OpenSavedGame();
        }
        else
        {
            SaveLocal();
        }
    }

    public void Load()
    {
        if (Social.localUser.authenticated)
        {
            isSaving = false;
            OpenSavedGame();
        }
        else
        {
            LoadLocal();
        }

        //data.UpdateIngame();
    }

    public void UpdateInGame()
    {
        data.UpdateIngame();
    }

    /// <summary>
    /// Called when the saved game is opened, either before reading of writing it.
    /// </summary>
    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (!isSaving) LoadGameData(game);
            else SaveGameData(game);
        }
        else
        {
            if (!isSaving) LoadLocal();
            else SaveLocal();
        }
    }

    private void SaveGameData(ISavedGameMetadata game)
    {
        // obtain save data as byte array, for now simply parse reputation integer
        byte[] saveData = Encoding.ASCII.GetBytes(
            DataToString(data));

        // update local playerprefs to synch 
        SaveLocal();

        // obtain metadata instance with new playtime
        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder()
            .WithUpdatedPlayedTime(data.GetTotalPlaytime)
            .Build();

        // commit update
        (Social.Active as PlayGamesPlatform).SavedGame
            .CommitUpdate(game, update, saveData, OnSavedGameDataWritten);
    }

    private void SaveLocal()
    {
        PlayerPrefs.SetString(SAVE_NAME, DataToString(data));
    }

    private void OnSavedGameDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        //possible to check for succesfully doing save stuff
    }

    private void LoadGameData(ISavedGameMetadata game)
    {
        (Social.Active as PlayGamesPlatform).SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
    }

    private void LoadLocal()
    {
        Debug.Log(PlayerPrefs.GetString(SAVE_NAME));
        data.SetValues(StringToData(PlayerPrefs.GetString(SAVE_NAME)));
        UpdateInGame();
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] savedData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // get a string of both the saved data (cloud) and local to compare 
            // so we can take the most up-to-date version
            string cloudString = savedData.Length == 0 ?
                DataToString(SaveData.InitialData()) :
                Encoding.ASCII.GetString(savedData);
            string localString = PlayerPrefs.GetString(SAVE_NAME);

            SaveData cloudData = StringToData(cloudString);
            SaveData localData = StringToData(localString);

            // if it's the first time on this device (this can also be first time ever),
            // take the cloud data and synch to the player prefs
            if (PlayerPrefs.GetInt("IsFirstTime") == 1)
            {
                PlayerPrefs.SetInt("IsFirstTime", 0);
                PlayerPrefs.SetString(SAVE_NAME, cloudString);
            }

            // compare total play time to choose 
            if (cloudData.GetTotalPlaytime > localData.GetTotalPlaytime)
            {
                // it's logical that the cloud data is the newest
                // just update 
                data.SetValues(cloudData);
            }
            else
            {
                // if something went wrong with saving earlier
                // the local data can be the newest
                // and we need to update the cloud data too!
                data.SetValues(localData);
                Save();
            }

            // call update ingame
            UpdateInGame();
        }
        else
        {
            // handle error
        }
    }

    public static void UnlockAchievement(string id)
    {
        Social.ReportProgress(id, 100, succes => { });
    }
    public static void ShowAchievmentUI()
    {
        Social.ShowAchievementsUI();
    }
}
