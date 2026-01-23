using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostGamePanelUI : MonoBehaviour
{
    [SerializeField] private Image _bossImage;
    [SerializeField] private TMP_Text _postGameText;
    private void Start()
    {
        GameEvents.OnPostGame.AddListener(UpdatePostGame);
    }
    public void UpdatePostGame(bool cond)
    {
        // On Victory
        if (cond == true)
        {
            _postGameText.text = "Victory. You may have save the galaxy..." +
                "... for now, ihr Haribobaerchen";
        }
        // On Defeat
        else
        {
            _postGameText.text = "Haha, ihr Haribobaerchen"+"You are defeated!" +
                "Now the domination of the galaxy is ours!";
        }
    }
    public void OnCLickRestart()
    {
        OwnLobbyManager.Instance.RpcRequestStartGame();
    }
    public void OnClickLobby()
    {
        OwnLobbyManager.Instance.SetGlobalState(GameState.Lobby);
    }
    public void OnClickQuit()
    {
        SaveManager.Instance.SavePlayerProfile(GameSystem.Instance.ActiveProfile);
        Application.Quit();
    }
}
