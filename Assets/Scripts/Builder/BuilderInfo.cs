using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BuilderInfo : MonoBehaviour
{
    private BuildInteractable currentBuilder;

    [SerializeField] private TextMeshProUGUI goldCount;
    [SerializeField] private TextMeshProUGUI copperCount;
    [SerializeField] private TextMeshProUGUI requirementText;

    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button destroyButton;

    [SerializeField] private Image buildingImage;

    [SerializeField] private GameObject[] buildingPrefabs = new GameObject[3];
    [SerializeField] private Sprite[] buildingSprites = new Sprite[3];
    public void UpdateUI(BuildInteractable builder)
    {
        currentBuilder = builder;

        RefreshUI();
    }

    private void RefreshUI()
    {
        copperCount.text = "Copper: " + GameManager.GetPlayer().copperCount.ToString();
        goldCount.text = "Gold: " + GameManager.GetPlayer().goldCount.ToString();
        requirementText.text = currentBuilder.GetBuildingLevel() < currentBuilder.maxLevel ? currentBuilder.GetRequirementText()  : "The building is at max level";

        if (currentBuilder.GetBuildingLevel() < currentBuilder.maxLevel && currentBuilder.HasSufficientFund())
        {
            upgradeButton.gameObject.SetActive(true);
        }
        else
        {
            upgradeButton.gameObject.SetActive(false);
        }

        destroyButton.gameObject.SetActive(currentBuilder.GetBuildingLevel() > 0);

        Color imageColour = buildingImage.color;
        imageColour.a = currentBuilder.GetBuildingLevel() == 0 ? 0.0f : (150.0f/255.0f);
        buildingImage.color = imageColour;
    }

    public void Upgrade()
    {
        buildingImage.sprite = buildingSprites[currentBuilder.GetBuildingLevel()];
        GameObject newBuilding = Instantiate(buildingPrefabs[currentBuilder.GetBuildingLevel()]);
        currentBuilder.Upgrade(newBuilding);

        RefreshUI();
    }

    public void DestroyBuilding()
    {
        currentBuilder.DestroyBuilding();
        buildingImage.sprite = null;

        RefreshUI();
    }
}
