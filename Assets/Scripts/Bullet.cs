using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    public float ttl = 5f;
    public float speed = 10f;
    public LayerMask enemyLayer;

    private static ScoreTracker scoreTracker;

    private void Start() {
        if (scoreTracker == null) {
            scoreTracker = GameObject.FindWithTag("GameController").GetComponent<ScoreTracker>();
        }
        Destroy(gameObject, ttl);
    }

    private void Update() {
        transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));

        Collider2D coll = Physics2D.OverlapCircle(transform.position, 0.1f, enemyLayer);
        if (coll != null) {
            Destroy(coll.gameObject);
            Destroy(gameObject);
            scoreTracker.IncreaseScore();
        }
    }

}
