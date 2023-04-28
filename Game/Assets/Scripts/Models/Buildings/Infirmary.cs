using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Infirmary
{
    #region GameObject.Find-ok
    GameObject StartCureButton;
    GameObject FinishCureButton;
    #endregion

    #region �P�LET V�LTOZ�K
    public int BuildingID;
    public int Towns_TownID;
    public string BuildingType;
    public int BuildingLvl;
    #endregion

    #region PARAM�TEREK
    public DateTime lastCureDate;
    public int currentCure;
    public int injuredUnits;
    public int healedUnits;
    #endregion

    #region AZ �P�LET SZINTJ�T�L F�GG� V�LTOZ�K
    public infirmaryLevelVariables levelVariables;
    public struct infirmaryLevelVariables
    {
        public int Lvl;
        public string HealingTime;
        public int Effectivity;
        public int MaxInjuredUnits;
        public int MaxHealedUnits;
    }

    public async void updateLevelVariables(int newLevel)
    {
        infirmaryLevelVariables? fetchedVariables = await APIHelper.fetchInfirmaryStats(newLevel);
        
        if (fetchedVariables == null)
        {
            Debug.Log("Infirmary updateLevelVariables failed");
        }

        levelVariables = fetchedVariables.Value;

        updateUI();
    }
    #endregion

    #region SZ�M�TOTT MEZ�K
    public TimeSpan cureCoolDown => lastCureDate.Add(TimeSpan.Parse(levelVariables.HealingTime)).Subtract(DateTime.Now);
    #endregion

    #region KONSTRUKTOR
    public Infirmary(Building building)
    {
        StartCureButton = GameObject.Find("StartCureButton");
        FinishCureButton = GameObject.Find("FinishCureButton");
        Initialize(building);
        updateLevelVariables(BuildingLvl);
    }

    private void Initialize(Building building)
    {
        BuildingID = building.BuildingID;
        Towns_TownID = building.Towns_TownID;
        BuildingType = building.BuildingType;
        BuildingLvl = building.BuildingLvl;

        Debug.Log("Infirmary building:\n" + JsonUtility.ToJson(building));

        lastCureDate = DateTime.Parse(building.lastCureDate);
        currentCure = (int)building.currentCure;
        injuredUnits = (int)building.injuredUnits;
        healedUnits = (int)building.healedUnits;

        updateLevelVariables(BuildingLvl);
    }
    #endregion

    #region AKCI�K
    public async void StartCure()
    {
        Building changesAfterCure = await APIHelper.postStartCure(BuildingID);

        if(changesAfterCure == null)
        {
            StartCureButton.SetActive(true);
            return;
        }

        Debug.LogWarning(changesAfterCure.lastCureDate);

        Initialize(changesAfterCure);
    }

    public async void FinishCure()
    {
        Building changesAfterCure = await APIHelper.postFinishCure(BuildingID);

        if (changesAfterCure == null)
        {
            FinishCureButton.SetActive(true);
            return;
        }
        StartCureButton.SetActive(true);

        Initialize(changesAfterCure);
    }
    #endregion

    #region UI FRISS�T�S
    public void updateUI()
    {
        GameObject.Find("HealedUnitsQuantityLabel").GetComponent<TMP_Text>().text = $"{healedUnits}/{levelVariables.MaxHealedUnits}";
        GameObject.Find("InjuredUnitsQuantityLabel").GetComponent<TMP_Text>().text = $"{injuredUnits}/{levelVariables.MaxInjuredUnits}";
        
        if (cureCoolDown > TimeSpan.Zero + TimeSpan.FromMinutes(1))
        {
            GameObject.Find("CurrentCureLabel").GetComponent<TMP_Text>().text = $"Folyamatban l�v� gy�gy�t�s:\n{currentCure} egys�g";
            GameObject.Find("CureCountDownLabel").GetComponent<TMP_Text>().text = $"H�tral�v� id�: {cureCoolDown.Hours} �ra {cureCoolDown.Minutes} perc";
            StartCureButton.SetActive(false);
            FinishCureButton.SetActive(false);
        }
        else
        {
            GameObject.Find("CureCountDownLabel").GetComponent<TMP_Text>().text = "Nincs folyamatban gy�gy�t�s";
            if (currentCure > 0)
            {
                GameObject.Find("CurrentCureLabel").GetComponent<TMP_Text>().text = $"Utols� gy�gy�t�s:\n{currentCure} egys�g,\nebb�l sikeresen meggy�gy�tott: {(int)(currentCure * levelVariables.Effectivity / 100)}";
                StartCureButton.SetActive(false);
                FinishCureButton.SetActive(true);
            }
            else
            {
                if (injuredUnits == 0)
                    StartCureButton.SetActive(false);
                else
                    StartCureButton.SetActive(true);

                FinishCureButton.SetActive(false);
                GameObject.Find("CurrentCureLabel").GetComponent<TMP_Text>().text = "";
            }

        }
    }
    #endregion
}
