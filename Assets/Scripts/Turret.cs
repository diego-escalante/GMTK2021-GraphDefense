using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : BaseStructure {

    [Header("Turret")]
    public float energyPerShot = 1f;
    public float range = 5f;
    public GameObject bulletPrefab;
    public float shotInterval = 0.25f;

    private bool canShoot = true;

    protected override void Update() {
        base.Update();

        if (!isBuilt) {
            return;
        }

        if (canShoot) {
            Shoot();
        }
    }

    private void Shoot() {
        if (energyPerShot > storage) {
            return;
        }

        Transform target = FindTarget();
        if (target == null) {
            return;
        }

        RemoveFromStorage(energyPerShot);
        Transform bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity).transform;

        // Point towards target.
        bullet.up = target.transform.position - bullet.transform.position;

        canShoot = false;
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown() {
        yield return new WaitForSeconds(shotInterval);
        canShoot = true;
    }

    private Transform FindTarget() {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Enemy");

        if (gos.Length == 0) {
            return null;
        }

        Transform result = null;
        float distance = range;
        foreach (GameObject go in gos) {
            float dist = Vector2.Distance(go.transform.position, transform.position);
            if (dist < distance) {
                result = go.transform;
                distance = dist;
            }
        }

        return result;
    }
}
