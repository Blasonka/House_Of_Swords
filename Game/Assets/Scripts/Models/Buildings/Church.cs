using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Church
{
    // "�R�K�LT" V�LTOZ�K
    public int BuildingID;
    public int Towns_TownID;
    public string BuildingType;
    public int BuildingLvl;

    // AZ �P�LET SZINTJ�T�L F�GG� V�LTOZ�K
    public TimeSpan MassLength;
    public int HappinessBoost;

    public struct churchLevelVariables
    {
        public int Lvl;
        public string MassLength;
        public int HappinessBoost;
    }

    public async void updateLevelVariables(int newLevel)
    {
        churchLevelVariables? fetchedVariables = await APIHelper.fetchChurchStats(BuildingLvl);

        if (fetchedVariables == null)
        {
            Debug.LogError("Church updateLevelVariables failed");
        }

        fetchedVariables = fetchedVariables.Value;

        string[] massLengthArray = fetchedVariables.Value.MassLength.Split(':');
        MassLength = new TimeSpan(int.Parse(massLengthArray[0]), int.Parse(massLengthArray[1]), int.Parse(massLengthArray[2]));
        HappinessBoost = fetchedVariables.Value.HappinessBoost;

        GameObject.Find("GameManager").GetComponent<GameManager>().updateLabels();
    }

    // PARAM�TEREK
    public DateTime lastMassDate;
    //struct paramsStruct
    //{
    //    public string lastMassDate;
    //}

    // SZ�M�TOTT MEZ�K
    public TimeSpan massCooldown => lastMassDate.Add(MassLength).Subtract(DateTime.Now);

    // KONSTRUKTOR
    public Church(Building building)
    {
        Initialize(building);
        updateLevelVariables(BuildingLvl);
    }

    private void Initialize(Building building)
    {
        BuildingID = building.BuildingID;
        Towns_TownID = building.Towns_TownID;
        BuildingType = building.BuildingType;
        BuildingLvl = building.BuildingLvl;

        lastMassDate = Convert.ToDateTime(building.lastMassDate);

        //paramsStruct churchParams = JsonUtility.FromJson<paramsStruct>(building.Params);

        //lastMassDate = Convert.ToDateTime(churchParams.lastMassDate);
    }

    // AKCI�K
    public async void StartMass()
    {
        Building changesAfterMass = await APIHelper.postStartMass(BuildingID);

        if (changesAfterMass == null)
        {
            GameObject.Find("StartButton").GetComponent<Button>().interactable = true;
            return;
        }

        Initialize(changesAfterMass);
    }
}
