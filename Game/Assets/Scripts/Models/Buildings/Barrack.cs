using System;
using System.Collections.Generic;
using UnityEngine;

public class Barrack
{
    // "�R�K�LT" V�LTOZ�K
    public int BuildingID;
    public int Towns_TownID;
    public string BuildingType;
    public int BuildingLvl;

    // AZ �P�LET SZINTJ�T�L F�GG� V�LTOZ�K
    //public int MaxAllyCount = 3;
    //public int MaxAllyRange = 15;
    //public TimeSpan FightLength = new TimeSpan(14, 0, 0);

    //public async void updateLevelVariables(int newLevel)
    //{
    //    // ...
    //}

    // PARAM�TEREK
    public DateTime fightStartDate;
    public int enemyTownId;

    struct paramsStruct
    {
        public string fightStartDate;
        public int enemyTownID;
    }

    // SZ�M�TOTT MEZ�K
    //public TimeSpan fightCooldown => fightStartDate.Add(FightLength).Subtract(DateTime.Now);

    // KONSTRUKTOR
    public Barrack(Building building)
    {
        BuildingID = building.BuildingID;
        Towns_TownID = building.Towns_TownID;
        BuildingType = building.BuildingType;
        BuildingLvl = building.BuildingLvl;

        //paramsStruct barrackParams = JsonUtility.FromJson<paramsStruct>(building.Params);

        //fightStartDate = Convert.ToDateTime(barrackParams.fightStartDate);
        //enemyTownId = barrackParams.enemyTownID;
        // ...

        //updateLevelVariables(BuildingLvl);
    }
}
