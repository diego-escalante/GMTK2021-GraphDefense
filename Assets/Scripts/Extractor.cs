using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Extractor : MonoBehaviour, IStorage {
    
    [SerializeField]
    private float capacity = 10f;
    [SerializeField]
    private float storage = 0f;
    [SerializeField]
    private float extractionRate = 1f;
    [SerializeField]
    private LayerMask energyDepositLayerMask;

    private List<EnergyDeposit> energyDeposits;

    private void Start() {
        // Detect overlap with any Energy Deposits that have energy and remember them.
        float radius = Mathf.Max(transform.localScale.x, transform.localScale.y) / 2;
        energyDeposits = Physics2D.OverlapCircleAll(transform.position, radius, energyDepositLayerMask).Select(c => c.GetComponent<EnergyDeposit>()).Where(ed => ed.GetStorage() > 0).ToList();
    }

    private void Update() {

        // TODO: Send energy once wires are in place.

        // Don't extract if generator is at capacity.
        if (storage >= capacity) {
            return;
        }

        // Extract from energy deposits and put in storage.
        float extractionPerDeposit = extractionRate/energyDeposits.Count();
        foreach (EnergyDeposit energyDeposit in energyDeposits) {
            
            // Determine the amount of energy to extract from the deposit.
            float amountToExtract = extractionPerDeposit * Time.deltaTime;
            if (energyDeposit.GetStorage() < amountToExtract) {
                amountToExtract = energyDeposit.GetStorage();
            }
            if (capacity - storage < amountToExtract) {
                amountToExtract = capacity - storage;
            }

            // Transfer energy from deposit and place in generator.
            energyDeposit.UpdateStorage(-amountToExtract);
            UpdateStorage(amountToExtract);

            // if the energy Deposit is empty, remove.
            if (energyDeposit.GetStorage() <= 0) {
                energyDeposits.Remove(energyDeposit);
            }

            // if the generator is at capacity, stop extracting.
            if (storage >= capacity) {
                break;
            }
        }
    }

    public float GetCapacity() {
        return capacity;
    }

    public float GetStorage() {
        return storage;
    }

    public void UpdateStorage(float delta) {
        storage += delta;
        if (storage < 0) {
            storage = 0;
            Debug.LogWarning("Tried to remove more than available from Storage!");
        } else if (storage > capacity) {
            storage = capacity;
            Debug.LogWarning("Tried to add more than Storage's capacity allows for!");
        }
    }
}
