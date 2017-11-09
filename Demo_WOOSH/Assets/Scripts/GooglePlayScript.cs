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

    public static void UnlockAchievemnt(string id)
    {
        Social.ReportProgress(id, 1.0d, succes => { UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.Victory); });
    }

    public static void ShowAchievmentUI()
    {
        UberManager.Instance.SoundManager.PlaySoundEffect(SoundManager.SoundEffect.Victory);
        Social.ShowAchievementsUI();
    }
    #endregion
}
