using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject chooseSceneMenu;

    private void Start() => chooseSceneMenu.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(delegate { SaveSystem.Load(); });

    public void SetActiveChooseSceneMenu() => chooseSceneMenu.SetActive(!chooseSceneMenu.activeSelf);

    public void LoadFirst() => SceneManager.LoadScene(1);

    public void LoadSecond() => SceneManager.LoadScene(2);

    public void ExitGame() => Application.Quit();
}
