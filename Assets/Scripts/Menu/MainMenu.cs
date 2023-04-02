using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject chooseSceneMenu;

    public void SetActiveChooseSceneMenu() => chooseSceneMenu.SetActive(!chooseSceneMenu.activeSelf);

    public void LoadFirst() => SceneManager.LoadScene(1);

    public void LoadSecond() => SceneManager.LoadScene(2);

    public void ExitGame() => Application.Quit();
}
