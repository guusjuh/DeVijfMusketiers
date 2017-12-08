using System.IO;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public class GooglePlayScript : MonoBehaviour {
    public static GooglePlayScript Instance { get; private set; }

    private const string SAVE_NAME = "Save";
    private bool isSaving;
    private bool isCloudDataLoaded = false;

    public void Initialize()
    {
        Instance = this;

        if(!PlayerPrefs.HasKey(SAVE_NAME)) PlayerPrefs.SetString(SAVE_NAME, InitialPlayerDataToString()); 
        if(!PlayerPrefs.HasKey("IsFirstTime")) PlayerPrefs.SetInt("IsFirstTime", 1); // 1 for true since booleans dont exist in playerprefs

        ResetData();
        LoadLocal();

        //PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        //PlayGamesPlatform.InitializeInstance(config);
        //PlayGamesPlatform.Activate();

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

    public string InitialPlayerDataToString()
    {
        // SavedPlayerData needs to be converted to string
        XmlSerializer serizalizer = new XmlSerializer(typeof(SavedPlayerData));
        using (MemoryStream m = new MemoryStream())
        {
            serizalizer.Serialize(m, SavedPlayerData.InitialData());
            m.Flush();

            return Encoding.UTF8.GetString(m.GetBuffer());
        }
    }

    public string PlayerDataToString()
    {
        // SavedPlayerData needs to be converted to string
        XmlSerializer serizalizer = new XmlSerializer(typeof(SavedPlayerData));
        using (MemoryStream m = new MemoryStream())
        {
            serizalizer.Serialize(m, SavedPlayerData.Instance);

            Debug.Log(Encoding.UTF8.GetString(m.GetBuffer()));
            m.Flush();

            return Encoding.UTF8.GetString(m.GetBuffer());
        }
    }

    public void StringToPlayerData(string cloudData, string localData)
    {
        SavedPlayerData cloudPlayerData = null;
        SavedPlayerData localPlayerData = null;

        if (cloudData != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SavedPlayerData));
            using (MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(cloudData)))
            {
                cloudPlayerData = (SavedPlayerData)serializer.Deserialize(m);
            }
        }

        if (localData != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SavedPlayerData));
            using (MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(localData)))
            {
                localPlayerData = (SavedPlayerData)serializer.Deserialize(m);
            }
        }

        // first time playing on this device?
        if (PlayerPrefs.GetInt("IsFirstTime") == 1)
        {
            PlayerPrefs.SetInt("IsFirstTime", 0);

            if (cloudPlayerData != null && localPlayerData != null &&
                cloudPlayerData.timeStamp > localPlayerData.timeStamp)
            {
                PlayerPrefs.SetString(SAVE_NAME, cloudData);
            }
        }
        else
        {
            // update ingame to local data if its the newst
            if (localPlayerData != null && cloudPlayerData != null &&
                localPlayerData.timeStamp > cloudPlayerData.timeStamp)
            {
                SavedPlayerData.Instance = localPlayerData;
                if (!UberManager.Instance.DoingSetup) SavedPlayerData.Instance.UpdateIngame();

                isCloudDataLoaded = true;

                SaveData();
                return;
            }
        }

        if (cloudPlayerData != null)
        {
            SavedPlayerData.Instance = cloudPlayerData;
            if (!UberManager.Instance.DoingSetup) SavedPlayerData.Instance.UpdateIngame();
        }

        isCloudDataLoaded = true;
    }

    public void StringToPlayerData(string localData)
    {
        SavedPlayerData tempData = null; 

        if (localData != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SavedPlayerData));
            using (MemoryStream m = new MemoryStream(Encoding.UTF8.GetBytes(localData)))
            {
                tempData = (SavedPlayerData) serializer.Deserialize(m);
            }
        }

        if (tempData != null)
        {
            SavedPlayerData.Instance = tempData;
            if(!UberManager.Instance.DoingSetup) SavedPlayerData.Instance.UpdateIngame();
        }

        //UberManager.Instance.PlayerData.Reputation = float.Parse(localData);
    }

    public void LoadData()
    {
        if (Social.localUser.authenticated)
        {
            isSaving = false;

            //(Social.Active as PlayGamesPlatform).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
            //    DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }
        else
        {
            LoadLocal();
        }
    }

    private void LoadLocal()
    {
        StringToPlayerData(PlayerPrefs.GetString(SAVE_NAME));
    }

    public void SaveData()
    {
        SavedPlayerData.Instance.UpdateSaved();

        if (!isCloudDataLoaded)
        {
            SaveLocal();
            return;
        }
        if (Social.localUser.authenticated)
        {
            isSaving = true;

            //(Social.Active as PlayGamesPlatform).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
            //    DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }
        else
        {
            SaveLocal();
        }
    }

    public void DeleteProgress()
    {
        PlayerPrefs.SetString(SAVE_NAME, InitialPlayerDataToString());

        if (Social.localUser.authenticated)
        {
            isSaving = true;

            //(Social.Active as PlayGamesPlatform).SavedGame.OpenWithManualConflictResolution(SAVE_NAME,
            //    DataSource.ReadCacheOrNetwork, true, ResolveConflict, OnSavedGameOpened);
        }

        LoadData();
    }

    private void SaveLocal()
    {
        PlayerPrefs.SetString(SAVE_NAME, PlayerDataToString());
    }

    private void ResetData()
    {
        PlayerPrefs.SetString(SAVE_NAME, InitialPlayerDataToString());
    }

    // compare data and select the most up-to-date one 
    private void ResolveConflict(IConflictResolver resolver, ISavedGameMetadata origin, byte[] originData, ISavedGameMetadata unmerged, byte[] unmergedData)
    {
        if(origin == null) resolver.ChooseMetadata(unmerged);
        else if (unmerged == null) resolver.ChooseMetadata(origin);
        else
        {
            resolver.ChooseMetadata(unmerged);
            return;
        }
    }

    private void OnSavedGameOpened(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            if (!isSaving) LoadGame(game);
            else SaveGame(game);
        }
        else
        {
            if (!isSaving) LoadLocal();
            else SaveLocal();
        }
    }

    private void LoadGame(ISavedGameMetadata game)
    {
        //(Social.Active as PlayGamesPlatform).SavedGame.ReadBinaryData(game, OnSavedGameDataRead);
    }

    private void SaveGame(ISavedGameMetadata game)
    {
        string stringToSave = PlayerDataToString();
        SaveLocal();

        byte[] dataToSave = Encoding.ASCII.GetBytes(stringToSave);

        SavedGameMetadataUpdate update = new SavedGameMetadataUpdate.Builder().Build();
        //(Social.Active as PlayGamesPlatform).SavedGame.CommitUpdate(game, update, dataToSave, OnSavedGameDataWritten);
    }

    private void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] savedData)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            string cloudDataString;
            if (savedData.Length == 0)
                cloudDataString = InitialPlayerDataToString(); 
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
