using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using UnityEditor.SceneManagement;
using UnityEngine;

public class GooglePlayScript : MonoBehaviour {
    public static GooglePlayScript Instance { get; private set; }

    private const string SAVE_NAME = "Save";
    private bool isSaving;
    private bool isCloudDataLoaded = false;

    public void Initialize()
    {
        Instance = this;

        if(!PlayerPrefs.HasKey(SAVE_NAME)) PlayerPrefs.SetString(SAVE_NAME, "112"); //TODO: replace string 0 with playerdata
        if(!PlayerPrefs.HasKey("IsFirstTime")) PlayerPrefs.SetInt("IsFirstTime", 1); // 1 for true since booleans dont exist in playerprefs

        LoadLocal();

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        Debug.Log("Loaded rep = " +UberManager.Instance.PlayerData.Reputation);


        SignIn();
    }

    void SignIn()
    {
        Social.localUser.Authenticate(succes => { LoadData(); });
    }

    void ProcessAuthentication(bool success)
    {
        if (!success)
            Debug.LogWarning("Social Services not initiated");
        else
        {
            Debug.Log(Social.localUser.userName);
        }
    }

    #region Achievements

    public static void UnlockAchievement(string id)
    {
        //note: if all is right, there shouldnt be a boolean check

        Social.ReportProgress(id, 100, succes => {  });
    }

    public static void ShowAchievmentUI()
    {
        Social.ShowAchievementsUI();
    }
    #endregion

    #region Saves
    public string PlayerDataToString()
    {
        return UberManager.Instance.PlayerData.Reputation.ToString();
    }

    public void StringToPlayerData(string cloudData, string localData)
    {
        // first time playing on this device?
        if (PlayerPrefs.GetInt("IsFirstTime") == 1)
        {
            PlayerPrefs.SetInt("IsFirstTime", 0);

            //TODO: add timestamp to playerdata and make sure the clouddata is more up to date
            PlayerPrefs.SetString(SAVE_NAME, cloudData);
        }

        //TODO: add timestamp to playerdata and make sure the localdata is more up to date
        PlayerPrefs.SetString(SAVE_NAME, cloudData);
        SaveData();

        //TODO: update playerdata according to 
        UberManager.Instance.PlayerData.Reputation = int.Parse(cloudData);
        isCloudDataLoaded = true;
    }

    public void StringToPlayerData(string localData)
    {
        UberManager.Instance.PlayerData.Reputation = float.Parse(localData);
    }

    public void LoadData()
    {
        if (Social.localUser.authenticated)
        {
            isSaving = false;

            (Social.Active as PlayGamesPlatform).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
                DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }
        else
        {
            LoadLocal();
        }
    }

    void LoadLocal()
    {
        StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME));
    }

    public void SaveData()
    {
        if (!isCloudDataLoaded)
        {
            SaveLocal();
            return;
        }
        if (Social.localUser.authenticated)
        {
            isSaving = true;

            (Social.Active as PlayGamesPlatform).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
                DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }
        else
        {
            SaveLocal();
        }
    }

    private void SaveLocal()
    {
        PlayerPrefs.SetString(SAVE_NAME, PlayerDataToString());
    }

    // compare data and select the most up-to-date one (TODO: add timestamp to playerdata saves)
    private void ResolveConflict(IConflictResolver resolver, ISavedGameMetadata origin, byte[] originData, ISavedGameMetadata unmerged, byte[] unmergedData)
    {
        if(origin == null) resolver.ChooseMetadata(unmerged);
        else if (unmerged == null) resolver.ChooseMetadata(origin);
        else
        {
            //TODO: compare timestamps
            resolver.ChooseMetadata(unmerged);
            return;
        }
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (!isSaving)
                LoadGame(game);
            else
                SaveGame(game);
        }
        else
        {
            if (!isSaving)
                LoadLocal();
            else
                SaveLocal();
        }
    }

    private void LoadGame(ISavedGameMetadata game)
    {
        (Social.Active as PlayGamesPlatform).SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
    }

    private void SaveGame(ISavedGameMetadata game)
    {
        string stringToSave = PlayerDataToString();
        SaveLocal();

        byte[] dataToSave = Encoding.ASCII.GetBytes(stringToSave);

        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
        (Social.Active as PlayGamesPlatform).SavedGame.CommitUpdate(game, update, dataToSave, OnSavedGameDataWritten);
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] savedData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string cloudDataString;
            if (savedData.Length == 0)
                cloudDataString = "0"; // TODO
            else
                cloudDataString = Encoding.ASCII.GetString(savedData);

            string localDataString = PlayerPrefs.GetString(SAVE_NAME);

            StringToPlayerData(cloudDataString, localDataString);
        }
    }

    private void OnSavedGameDataWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        //possible to check for succesfully doing save stuff
    }
    #endregion
}
