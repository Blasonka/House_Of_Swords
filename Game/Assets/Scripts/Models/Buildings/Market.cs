using System;
using System.Collections.Generic;
using UnityEngine;

public class Market
{
    // "�R�K�LT" V�LTOZ�K
    public int BuildingID;
    public int Towns_TownID;
    public string BuildingType;
    public int BuildingLvl;

    // AZ �P�LET SZINTJ�T�L F�GG� V�LTOZ�K
    public int MaxTax = 5;
    public int HappinesModifierPerTax = -5;

    public async void updateLevelVariables(int newLevel)
    {
        // ...
    }

    // PARAM�TEREK
    public int taxAmount;

    struct paramsStruct
    {
        public int taxAmount;
    }

    // SZ�M�TOTT MEZ�K
    public int HappinesModifier => taxAmount * HappinesModifierPerTax;

    // KONSTRUKTOR

    public Market(Building building)
    {
        BuildingID = building.BuildingID;
        Towns_TownID = building.Towns_TownID;
        BuildingType = building.BuildingType;
        BuildingLvl = building.BuildingLvl;


        //paramsStruct marketParams = JsonUtility.FromJson<paramsStruct>(building.Params);
        //taxAmount = marketParams.taxAmount;        
        //taxAmount = building.taxAmount;        

        updateLevelVariables(BuildingLvl);
    }
}
