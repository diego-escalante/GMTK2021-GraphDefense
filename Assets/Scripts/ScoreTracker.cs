using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class ScoreTracker : MonoBehaviour {
    
    private int score = 0;
    public TMP_Text text;
    public TMP_Text final;
    private bool gameOver = false;

    private void Start() {
        text.text = "Score: " + score;

        GameObject[] gos = GameObject.FindGameObjectsWithTag("Structure");

        gos[0].GetComponent<BaseStructure>().ConnectTo(gos[1].GetComponent<BaseStructure>());
    }

    private void Update() {

        if (gameOver) {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
                SceneManager.LoadScene(0);
            }
            return;
        }

        //Check if the player is dead.
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Structure");

        if (gos.Length == 0) {
            GameOver();
            return;
        }

        if (gos.Where(go => go.GetComponent<BaseStructure>().isBuilt).Count() <= 0) {
            GameOver();
            return;
        }
    }

    public void IncreaseScore() {
        score++;
        text.text = "Score: " + score;
    }

    private void GameOver() {
        GetComponent<EnemySpawner>().enabled = false;
        GetComponent<Controls>().enabled = false;
        GetComponent<BuilderControls>().enabled = false;
        final.text = "Game Over!\n\nFinal Score: " + score + "\n\nClick anywhere to restart";
        text.enabled = false;
        gameOver = true;
    }
}
