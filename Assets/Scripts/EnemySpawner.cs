using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public float spawnInterval = 5f;
    public float spawnIntervalDecay = 0.05f;
    public GameObject enemyPrefab;

    private Coroutine s;

    private void Start() {
        s = StartCoroutine(Spawn());
    }

    private void OnDisable() {
        if (s != null) {
            StopCoroutine(s);
        }
    }

    private IEnumerator Spawn() {
        Instantiate(enemyPrefab, new Vector3(Random.Range(-30, 30f), Random.Range(-30, 30f), -1), Quaternion.identity);
        spawnInterval = Mathf.Max(0.1f, spawnInterval - spawnIntervalDecay);
        yield return new WaitForSeconds(spawnInterval);
        s = StartCoroutine(Spawn());
    }

}
