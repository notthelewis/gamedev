using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public List<GameObject> targets;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public Button restartButton;
    public bool isGameOver = false;

    private int score = 0;
    private float spawnRate = 1f;
    private Difficulty difficulty;

    private Coroutine mainLoop;

    public void AddScore(int toAdd) {
        score += toAdd;
        scoreText.SetText("Score: {0}", score);
    }

    public void GameOver(GameOverReason reason) {
        Debug.Log($"Ended for: {reason}");
        isGameOver = true;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        StopCoroutine(mainLoop);
    }

    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RunGame(Difficulty d) {
        difficulty = d;
        spawnRate = 6 - ((float)difficulty + 1);
        mainLoop = StartCoroutine(SpawnTarget());
        scoreText.SetText("Score: {0}", score);
    }

    IEnumerator SpawnTarget() {
        while (!isGameOver) {
            yield return new WaitForSeconds(spawnRate);

            int range = difficulty == Difficulty.EASY
                ? (int)difficulty
                : Random.Range(1, (int)difficulty+1)
            ;

            for (int i = 0; i <= range; i++) {
                int index = Random.Range(0, targets.Count);
                Instantiate(targets[index]);
            }
        }
    }
}

public enum GameOverReason {
    MISS_GOOD,
    HIT_BAD,
}
