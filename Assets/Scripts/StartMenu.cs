using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {
    
    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(2);
        } else if (Input.GetMouseButtonDown(1)) {
            SceneManager.LoadScene(1);
        }
    }

}
