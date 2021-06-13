using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Extractor : BaseStructure {

    [Header("Extraction")]
    [SerializeField]
    private float extractionRate = 1f;
    [SerializeField]
    private LayerMask energyDepositLayerMask;

    private List<EnergyDeposit> energyDeposits;

    private void Start() {
        // Detect overlap with any Energy Deposits that have energy and remember them.
        float radius = Mathf.Max(transform.localScale.x, transform.localScale.y) / 2;
        energyDeposits = Physics2D.OverlapCircleAll(transform.position, radius, energyDepositLayerMask).Select(c => c.GetComponent<EnergyDeposit>()).Where(ed => !ed.IsDepleted()).ToList();
    }

    protected override void Update() {
        base.Update();
        
        if (!isBuilt) {
            return;
        }

        Extract();
    }

    private void Extract() {
         // Don't extract if generator is at capacity.
        if (storage >= capacity) {
            return;
        }

        // Extract from energy deposits and put in storage.
        float extractionPerDeposit = extractionRate/energyDeposits.Count;
        foreach (EnergyDeposit energyDeposit in energyDeposits.ToArray()) {
            
            // Determine the amount of energy to extract from the deposit.
            float amountToExtract = extractionPerDeposit * Time.deltaTime;
            if (energyDeposit.GetEnergy() < amountToExtract) {
                amountToExtract = energyDeposit.GetEnergy();
            }
            if (capacity - storage < amountToExtract) {
                amountToExtract = capacity - storage;
            }

            // Transfer energy from deposit and place in generator.
            energyDeposit.Remove(amountToExtract);
            AddToStorage(amountToExtract);

            // if the energy Deposit is empty, remove.
            if (energyDeposit.IsDepleted()) {
                energyDeposits.Remove(energyDeposit);
            }

            // if the generator is at capacity, stop extracting.
            if (storage >= capacity) {
                break;
            }
        }
    }
}
