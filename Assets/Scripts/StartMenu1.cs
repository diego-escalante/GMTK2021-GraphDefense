using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu1 : MonoBehaviour {
    
    private void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) {
            SceneManager.LoadScene(0);
        }
    }

}
