using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private Difficulty difficulty;

    private Button button;
    private GameManager gameManager;

    public void OnClick() {
        // Disable titleObject onClick so that it doesn't get called in every DisableGameObjects call, which
        // is called for every difficulty button (i.e. easy, medium, hard). 
        // Effecively just saves 2 `Find` & `SetActive` operations
        GameObject titleObject = GameObject.Find("Title Text");
        titleObject.SetActive(false);

        DisableGameObjects();
        gameManager.RunGame(difficulty);
    }

    public void OnGameStart() {
        DisableGameObjects();
    }

    private void DisableGameObjects() {
        button.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        button = GetComponent<Button>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
}

public enum Difficulty : byte {
    EASY,
    MEDIUM,
    HARD,
};
