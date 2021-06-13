using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyBehavior : MonoBehaviour {
    
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float attackInterval = 0.5f;
    [SerializeField]
    private float attackDamage = 1f;

    private BaseStructure target;
    private CamShake camShake;

    private Coroutine attacking;

    private void Awake() {
        camShake = GetComponent<CamShake>();
    }

    private void Start() {
        StartCoroutine(RefreshTarget());
    }

    private void Update() {
        if (target == null || attacking != null) {
            return;
        }

        // Point towards target.
        transform.up = target.transform.position - transform.position;

        if (Vector2.Distance(transform.position, target.transform.position)> 0.65f) {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        } else {
            Attack();
        }
    }

    private void Attack() {
        attacking = StartCoroutine(AttackCoroutine());
    }

    private void PickTarget() {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Structure");

        if (gos.Length == 0) {
            target = null;
            return;
        }

        Transform result = null;
        float distance = 1000f;
        foreach (GameObject go in gos) {
            float dist = Vector2.Distance(go.transform.position, transform.position);
            if (result == null || dist < distance) {
                result = go.transform;
                distance = dist;
            }
        }

        target = result.GetComponent<BaseStructure>();
    }

    private IEnumerator RefreshTarget() {
        PickTarget();
        yield return new WaitForSeconds(2f);
        StartCoroutine(RefreshTarget());
    }

    private IEnumerator AttackCoroutine() {
        camShake.TinyShake();
        target.LoseHealth(attackDamage);
        yield return new WaitForSeconds(attackInterval);
        attacking = null;
    }
}
