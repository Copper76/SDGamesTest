using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildInteractable : Interactable
{
    public int maxLevel = 3;
    [SerializeField] Vector3 buildingOffset;

    private int buildingLevel = 0;
    private int[] buildCostCopper = new int[3] { 1, 2, 3 };
    private int[] buildCostGold = new int[3] { 0, 0, 1 };

    private GameObject building;
    public override void Interact()
    {
        GameManager.GetPlayer().ToggleBuildingMenu(this);
    }

    public int GetBuildingLevel()
    {
        return buildingLevel;
    }

    public string GetRequirementText()
    {
        return "You need " + buildCostCopper[buildingLevel] + " Copper ore and " + buildCostGold[buildingLevel] + " ore for the next level";
    }

    public bool HasSufficientFund()
    {
        return GameManager.GetPlayer().copperCount >= buildCostCopper[buildingLevel] && GameManager.GetPlayer().goldCount >= buildCostGold[buildingLevel];
    }

    public void Upgrade(GameObject newBuilding)
    {
        GameManager.GetPlayer().copperCount -= buildCostCopper[buildingLevel];
        GameManager.GetPlayer().goldCount -= buildCostGold[buildingLevel];

        GameManager.GetPlayer().ConsumeItem(1, buildCostCopper[buildingLevel]);
        GameManager.GetPlayer().ConsumeItem(2, buildCostGold[buildingLevel]);

        if (building)
        {
            Destroy(building);
        }

        building = newBuilding;
        building.transform.parent = transform;
        building.transform.position = transform.position + buildingOffset;

        buildingLevel++;
    }

    public void DestroyBuilding()
    {
        if (building)
        {
            Destroy(building);
        }

        buildingLevel = 0;
    }
}
