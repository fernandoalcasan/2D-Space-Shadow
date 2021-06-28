using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _credits;
    [SerializeField]
    private GameObject _controls;

    public void LoadScene(int index)
    {
        StartCoroutine(LoadSceneWithDelay(index));
    }

    private IEnumerator LoadSceneWithDelay(int index)
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(index);
    }

    public void DisplayCredits()
    {
        _credits.SetActive(!_credits.activeSelf);
        EventSystem.current.SetSelectedGameObject(null);
    }
    
    public void DisplayControls()
    {
        _controls.SetActive(!_controls.activeSelf);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
