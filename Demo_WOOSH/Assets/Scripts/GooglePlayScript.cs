using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class GooglePlayScript : MonoBehaviour {
    void Start()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();

        SignIn();
    }

    void SignIn()
    {
        Social.localUser.Authenticate(succes => { ProcessAuthentication(succes); });
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
}
