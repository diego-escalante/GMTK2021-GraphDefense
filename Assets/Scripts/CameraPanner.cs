using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPanner : MonoBehaviour {

    public float maxDistanceFromCenter = 100f;
    public int Boundary  = 50;
    public float speed  = 5;
    private int screenWidth, screenHeight;
    
    private void Start() {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        Cursor.lockState = CursorLockMode.Confined;
    }
    
    private void Update() {
        if (Input.mousePosition.x > screenWidth - Boundary) {
            transform.Translate(speed * Time.deltaTime, 0, 0); 
        }
        if (Input.mousePosition.x < 0 + Boundary) {
            transform.Translate(-speed * Time.deltaTime, 0, 0);
        }
        if (Input.mousePosition.y > screenHeight - Boundary) {
            transform.Translate(0, speed * Time.deltaTime, 0); 
        }
        if (Input.mousePosition.y < 0 + Boundary) {
            transform.Translate(0, -speed * Time.deltaTime, 0); 
        }

        transform.position = new Vector3(Mathf.Clamp(transform.position.x, -maxDistanceFromCenter, maxDistanceFromCenter), Mathf.Clamp(transform.position.y, -maxDistanceFromCenter, maxDistanceFromCenter), transform.position.z);
    }      

}
