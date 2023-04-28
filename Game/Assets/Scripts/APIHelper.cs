using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public static class APIHelper
{
    // HTTP CLIENT FOR COMMUNICATION
    private static readonly HttpClient client = new HttpClient();

    private static string _gameSessionToken;
    [SerializeField] public static string gameSessionToken {
        private get => _gameSessionToken;
        set
        {
            _gameSessionToken = value;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", value);
        }
    }

    #region GLOBAL API ROUTES
    [SerializeField] private static string usersAPIRoute = "https://houseofswords.hu/api/users";
    [SerializeField] private static string townsAPIRoute = "https://houseofswords.hu/api/towns";
    [SerializeField] private static string buildingsAPIRoute = "https://houseofswords.hu/api/buildings";

    [SerializeField] private static string unitStatsAPIRoute = "http://houseofswords.hu/api/stats/units";

    [SerializeField] private static string churchStatsAPIRoute = "https://houseofswords.hu/api/stats/church";
    [SerializeField] private static string researchStatsAPIRoute = "https://houseofswords.hu/api/stats/research";
    [SerializeField] private static string warehouseStatsAPIRoute = "https://houseofswords.hu/api/stats/warehouse";
    [SerializeField] private static string marketStatsAPIRoute = "https://houseofswords.hu/api/stats/market";        // NOT FUNCTIONAL YET
    [SerializeField] private static string diplomacyStatsAPIRoute = "https://houseofswords.hu/api/stats/diplomacy";     // NOT FUNCTIONAL YET
    [SerializeField] private static string barrackStatsAPIRoute = "https://houseofswords.hu/api/stats/barrack";       // NOT FUNCTIONAL YET
    [SerializeField] private static string infirmaryStatsAPIRoute = "https://houseofswords.hu/api/stats/infirmary";

    [SerializeField] private static string actionsAPIRoute = "https://houseofswords.hu/api/actions";

    [SerializeField] private static string loginRoute = "https://houseofswords.hu/api/createGameSession";
    [SerializeField] private static string logoutRoute = "https://houseofswords.hu/api/terminateGameSession";
    #endregion

    #region API ROUTES FOR LOCAL TESTING
    //[SerializeField] private static string usersAPIRoute = "http://localhost:8000/api/users";
    //[SerializeField] private static string townsAPIRoute = "http://localhost:8000/api/towns";
    //[SerializeField] private static string buildingsAPIRoute = "http://localhost:8000/api/buildings";

    //[SerializeField] private static string unitStatsAPIRoute = "http://localhost:8000/api/stats/units";

    //[SerializeField] private static string churchStatsAPIRoute = "http://localhost:8000/api/stats/church";
    //[SerializeField] private static string researchStatsAPIRoute = "http://localhost:8000/api/stats/research";      // NOT FUNCTIONAL YET
    //[SerializeField] private static string warehouseStatsAPIRoute = "http://localhost:8000/api/stats/warehouse";     // NOT FUNCTIONAL YET
    //[SerializeField] private static string marketStatsAPIRoute = "http://localhost:8000/api/stats/market";        // NOT FUNCTIONAL YET
    //[SerializeField] private static string diplomacyStatsAPIRoute = "http://localhost:8000/api/stats/diplomacy";     // NOT FUNCTIONAL YET
    //[SerializeField] private static string barrackStatsAPIRoute = "http://localhost:8000/api/stats/barrack";       // NOT FUNCTIONAL YET
    //[SerializeField] private static string infirmaryStatsAPIRoute = "http://localhost:8000/api/stats/infirmary";     // NOT FUNCTIONAL YET

    //[SerializeField] private static string actionsAPIRoute = "http://localhost:8000/api/actions";

    //[SerializeField] private static string loginRoute = "http://localhost:8000/api/createGameSession";
    //[SerializeField] private static string logoutRoute = "http://localhost:8000/api/terminateGameSession";
    #endregion

    // NETWORK COMMUNICATION

    #region LOGIN SYSTEM

    /// <summary>
    /// Megpr�b�l bejelentkezni a felhaszn�l� �ltal be�rt adatokkal.
    /// Ha ez sikeres, az api el�r�s�hez sz�ks�ges tokent be�ll�tja mag�nak,
    /// �s visszaadja a lek�rt felhaszn�l� objektumot.
    /// </summary>
    /// <param name="username">A be�rt felhaszn�l�n�v.</param>
    /// <param name="password">A be�rt jelsz�.</param>
    /// <returns></returns>
    public static async Task<User> postTryLogin(string username, string password)
    {
        try
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "Username", username },
                { "Password", password }
            });

            HttpResponseMessage response = await client.PostAsync(loginRoute, content);
            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            Debug.Log(responseString);

            User user = JsonConvert.DeserializeObject<User>(responseString);
            APIHelper.gameSessionToken = user.GameSessionToken;

            return user;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return null;
        }
    }

    /// <summary>
    /// Kijelentkezteti a bejelentkezett felhaszn�l�t, �s kit�rli az azonos�t� tokent.
    /// </summary>
    public static async Task postLogout()
    {
        try
        {
            HttpResponseMessage response = await client.PostAsync(logoutRoute, null);
            response.EnsureSuccessStatusCode();

            APIHelper.gameSessionToken = null;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    /// <summary>
    /// A felhaszn�l� v�rosainak lek�r�se az adatb�zisb�l.
    /// </summary>
    /// <param name="UID">A felhaszn�l� azonos�t�ja.</param>
    /// <returns></returns>
    public static async Task<Town[]> fetchTownsOfUser(int UID)
    {
        try
        {
            string responseString = await client.GetStringAsync(usersAPIRoute + "/" + UID.ToString() + "/towns");

            if (String.IsNullOrEmpty(responseString)) return null;

            if (!responseString.StartsWith('[')) responseString = '[' + responseString + ']';

            Debug.Log("{\"fetchedTowns\":" + responseString + "}");

            return JsonConvert.DeserializeObject<Towns>("{\"fetchedTowns\":" + responseString + "}").fetchedTowns;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Az �sszes v�ros lek�r�se az adatb�zisb�l.
    /// </summary>
    /// <returns></returns>
    public static async Task<Town[]> fetchTowns()
    {
        try
        {
            string responseString = await client.GetStringAsync(townsAPIRoute);

            if (String.IsNullOrEmpty(responseString)) return null;

            if (!responseString.StartsWith('[')) responseString = '[' + responseString + ']';

            return JsonConvert.DeserializeObject<Towns>("{\"fetchedTowns\":" + responseString + "}").fetchedTowns;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// A megadott Town_ID-j� v�ros lek�r�se az adatb�zisb�l.
    /// </summary>
    /// <param name="TID"></param>
    /// <returns></returns>
    public static async Task<Town> fetchTownByID(int TID)
    {
        try
        {
            string responseString = await client.GetStringAsync(townsAPIRoute + "/" + TID.ToString());

            Debug.Log(responseString);

            if (String.IsNullOrEmpty(responseString)) return null;

            return JsonConvert.DeserializeObject<Town>(responseString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            return null;
        }
    }

    /// <summary>
    /// Aszinkron POST k�r�s, melyben a szerver hozzon l�tre a megadott n�vvel a felhaszn�l�nak egy �j v�rost.
    /// </summary>
    /// <param name="townName">A megadott v�rosn�v.</param>
    /// <param name="UID">A felhaszn�l� ID-je, aki� lesz a v�ros.</param>
    /// <returns></returns>
    public static async Task<Town> postCreateTown(string townName, int UID)
    {
        try
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "TownName", townName },
                { "Users_UID", UID.ToString() }
            });

            //HttpResponseMessage response = await client.PostAsync(townsAPIRoute + "/create/" + UID.ToString(), content);
            HttpResponseMessage response = await client.PostAsync(townsAPIRoute, content);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                Debug.Log(responseString);

                return JsonConvert.DeserializeObject<Town>(responseString);
            }

            Debug.LogError("Error: Town creation request");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }
    #endregion

    #region GAMEPLAY

    /// <summary>
    /// Az �sszes �p�let lek�r�se az adatb�zisb�l.
    /// </summary>
    /// <returns></returns>
    public static async Task<Building[]> fetchBuildings()
    {
        try
        {
            string responseString = await client.GetStringAsync(buildingsAPIRoute);

            Debug.Log(responseString);

            if (String.IsNullOrEmpty(responseString)) return null;

            if (!responseString.StartsWith('[')) responseString = '[' + responseString + ']';

            return JsonConvert.DeserializeObject<Buildings>("{\"fetchedBuildings\":" + responseString + "}").fetchedBuildings;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Egy megadott azonos�t�j� �p�let lek�r�se az adatb�zisb�l.
    /// </summary>
    /// <param name="BuildingID">Az �p�let azonos�t�ja</param>
    /// <returns></returns>
    public static async Task<Building> fetchBuildingById(int BuildingID)
    {
        try
        {
            string responseString = await client.GetStringAsync(buildingsAPIRoute + "/" + BuildingID.ToString());

            Debug.Log(responseString);

            if (String.IsNullOrEmpty(responseString)) return null;

            return JsonConvert.DeserializeObject<Building>(responseString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Egy v�roshoz tartoz� �p�letek lek�r�se az adatb�zisb�l.
    /// </summary>
    /// <param name="town">A v�ros, amelyben az �p�letek vannak.</param>
    /// <returns></returns>
    public static async Task<Building[]> fetchBuildingsOfTown(Town town)
    {
        try
        {
            string responseString = await client.GetStringAsync(townsAPIRoute + "/" + town.TownID.ToString() + "/buildings");

            if (!responseString.StartsWith('[')) responseString = '[' + responseString + ']';

            //Debug.Log(DateTime.Now.ToString("yyyy - MM - dd HH:mm:ss => fff"));
            return JsonConvert.DeserializeObject<Buildings>("{\"fetchedBuildings\":" + responseString + "}").fetchedBuildings;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }
    #endregion

    #region TESTING PHASE
    /// <summary>
    /// Lek�ri az adatb�zisb�l a templom �p�letek szintj�hez tartoz� v�ltoz�kat.
    /// </summary>
    /// <param name="churchLvl">A templom �p�let szintje, ha nincs, akkor lek�ri az �sszes szint v�ltoz�it.</param>
    /// <returns></returns>
    public static async Task<Church.churchLevelVariables?> fetchChurchStats(int churchLvl)
    {
        try
        {
            string responseString = await client.GetStringAsync(churchStatsAPIRoute + "/" + churchLvl.ToString());

            return JsonConvert.DeserializeObject<Church.churchLevelVariables>(responseString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Post k�r�s a szervernek, melyben a templom �p�let ind�tson egy istentiszteletet.
    /// </summary>
    /// <param name="ChurchID">A templom �p�let azonos�t�ja.</param>
    /// <returns></returns>
    public static async Task<Building> postStartMass(int ChurchID)
    {
        try
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "BuildingID", ChurchID.ToString() }
            });

            HttpResponseMessage response = await client.PostAsync(actionsAPIRoute + "/church/startMass", content);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                Debug.Log(responseString);

                return JsonConvert.DeserializeObject<Building>(responseString);
            }

            Debug.LogError("Error: Church Start Mass Request");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    // INFIRMARY
    public static async Task<Infirmary.infirmaryLevelVariables?> fetchInfirmaryStats(int infirmaryLvl)
    {
        try
        {
            string responseString = await client.GetStringAsync(infirmaryStatsAPIRoute + "/" + infirmaryLvl.ToString());

            return JsonConvert.DeserializeObject<Infirmary.infirmaryLevelVariables>(responseString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    public static async Task<Building> postStartCure(int InfirmaryID)
    {
        try
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "BuildingID", InfirmaryID.ToString() }
            });

            HttpResponseMessage response = await client.PostAsync(actionsAPIRoute + "/infirmary" + "/startCure", content); 

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Building>(responseString);
            }

            Debug.LogError("Error: Infirmary Start Cure Request");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    public static async Task<Building> postFinishCure(int InfirmaryID)
    {
        try
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "BuildingID", InfirmaryID.ToString() }
            });

            HttpResponseMessage response = await client.PostAsync(actionsAPIRoute + "/infirmary" + "/finishCure", content);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Building>(responseString);
            }

            Debug.LogError("Error: Infirmary Finish Cure Request");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    // Warehouse
    public static async Task<Warehouse.warehouseLevelVariables?> fetchWarehouseStats(int warehouseLvl)
    {
        try
        {
            string responseString = await client.GetStringAsync(warehouseStatsAPIRoute + "/" + warehouseLvl.ToString());

            return JsonConvert.DeserializeObject<Warehouse.warehouseLevelVariables>(responseString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    public static async Task<Building> postAddBrigade(int WarehouseID, string currentSlotName)
    {
        try
        {
            FormUrlEncodedContent content;
            content = null;
            switch (currentSlotName)
            {
                case "StoneResourceCollectorSlot":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "addStone", "1" }
                    });
                    break;
                case "WoodResourceCollectorSlot":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "addWood", "1" }
                    });
                    break;
                case "MetalResourceCollectorSlot":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "addMetal", "1" }
                    });
                    break;
                case "GoldResourceCollectorSlot":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "addGold", "1" }
                    });
                    break;

                default:
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() }
                    });
                    break;
            }

            HttpResponseMessage response = await client.PostAsync(actionsAPIRoute + "/warehouse" + "/addbrigade", content);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Building>(responseString);
            }

            Debug.LogError("Error: Warehouse AddBrigade Request");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    public static async Task<Building> postRemoveBrigade(int WarehouseID, string removeFrom)
    {
        try
        {
            FormUrlEncodedContent content;
            content = null;
            switch (removeFrom)
            {
                case "stone":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "removeStone", "1" }
                    });
                    break;
                case "wood":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "removeWood", "1" }
                    });
                    break;
                case "metal":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "removeMetal", "1" }
                    });
                    break;
                case "gold":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "removeGold", "1" }
                    });
                    break;
                case "all":
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() },
                        { "removeAll", "1" }
                    });
                    break;
                default:
                    content = new FormUrlEncodedContent(new Dictionary<string, string>{
                        { "BuildingID", WarehouseID.ToString() }
                    });
                    break;
            }
            
            HttpResponseMessage response = await client.PostAsync(actionsAPIRoute + "/warehouse" + "/removebrigade", content);

            response.EnsureSuccessStatusCode();

            string responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Building>(responseString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }


    /// <summary>
    /// Lek�ri az adatb�zisb�l a kutat� �p�letek szintj�hez tartoz� v�ltoz�kat.
    /// </summary>
    /// <param name="researchLvl">Az �p�let szintje, ha nincs, akkor az �sszes szintre vonatkoz� adatot lek�ri.</param>
    /// <returns></returns>
    public static async Task<Research.researchLevelVariables?> fetchResearchStats(int researchLvl = 0)
    {
        try
        {
            string responseString = await client.GetStringAsync(researchStatsAPIRoute + (researchLvl == 0 ? "" : ("/" + researchLvl.ToString())));

            return JsonConvert.DeserializeObject<Research.researchLevelVariables>(responseString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Lek�ri az adatb�zisb�l a megadott kutat� �p�letben kifejlesztett egys�geket.
    /// </summary>
    /// <param name="ResearchBuildingID">A kutat� �p�let azonos�t�ja.</param>
    /// <returns></returns>
    public static async Task<Research.ResearchedUnitStruct[]> fetchResearchedUnits(int ResearchBuildingID)
    {
        try
        {
            string responseString = await client.GetStringAsync(researchStatsAPIRoute + "/researchedUnits/" + ResearchBuildingID);

            Debug.Log("Request route: " + $"{researchStatsAPIRoute}/researchedUnits/{ResearchBuildingID}");
            Debug.Log("Researched Units Response: " + responseString);

            Debug.Log("{\"fetchedUnits\":" + responseString + "}");

            var faszhiba = JsonConvert.DeserializeObject<Research.ManyResearchedUnitsStruct>("{\"fetchedUnits\":" + responseString + "}").fetchedUnits;
            Debug.Log(faszhiba);
            return faszhiba;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    //public static async Task<Research.researchLevelVariables[]> fetchResearchStatsUntil(int researchLvl)
    //{
    //    try
    //    {
    //        Debug.Log("Attempting request: " + researchStatsAPIRoute + "/until/" + researchLvl.ToString());
    //        string responseString = await client.GetStringAsync(researchStatsAPIRoute + "/until/" + researchLvl.ToString());
    //        Debug.Log("Request results: " + responseString);

    //        if (!responseString.StartsWith('[')) responseString = '[' + responseString + ']';
    //        Debug.Log("Request results 2: " + responseString);

    //        return JsonConvert.DeserializeObject<Research.manyResearchLevelVariables>("{\"fetchedVariables\":" + responseString + "}").fetchedVariables;
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError(ex.Message);
    //        return null;
    //    }
    //}

    /// <summary>
    /// Post k�r�s a szervernek, melyben a megadott kutat� �p�let t�rolja el az �sszegy�jt�tt tud�st.
    /// </summary>
    /// <param name="ResearchID">A kutat� �p�let azonos�t�ja.</param>
    /// <returns></returns>
    public static async Task<Building> postCollectScience(int ResearchID)
    {
        try
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "BuildingID", ResearchID.ToString() }
            });

            HttpResponseMessage response = await client.PostAsync(actionsAPIRoute + "/research/collectScience", content);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                Debug.Log(responseString);

                return JsonConvert.DeserializeObject<Building>(responseString);
            }

            Debug.LogError("Error: Research Collect Science Request");
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Post k�r�s a szervernek, melyben a kutat� �p�let fejlessze ki az egys�get.
    /// </summary>
    /// <param name="ResearchBuildingID">A kutat� �p�let azonos�t�ja.</param>
    /// <param name="UnitID">A fejlesztend� egys�g azonos�t�ja.</param>
    /// <returns></returns>
    public static async Task<HttpStatusCode> postResearchUnit(int ResearchBuildingID, int UnitID)
    {
        try
        {
            FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "ResearchBuildingID", ResearchBuildingID.ToString() },
                { "UnitID", UnitID.ToString() }
            });

            HttpResponseMessage response = await client.PostAsync(actionsAPIRoute + "/research/researchUnit", content);

            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();

                Debug.Log(responseString);

                //return JsonConvert.DeserializeObject<Building>(responseString);
                return response.StatusCode;
            }

            Debug.LogError("Error: Research New Unit Request");
            return response.StatusCode;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return (HttpStatusCode)500;
        }
    }

    /// <summary>
    /// Lek�ri az adatb�zisb�l az egys�gek adatait.
    /// </summary>
    /// <param name="UnitID">A k�v�nt egys�g azonos�t�ja, ha nincs, akkor az �sszes egys�gre vonatkozik a k�r�s.</param>
    /// <returns></returns>
    public static async Task<Unit[]> fetchUnitStats(int UnitID = 0)
    {
        try
        {
            string responseString = await client.GetStringAsync(unitStatsAPIRoute + (UnitID == 0 ? "" : UnitID.ToString()));

            if (!responseString.StartsWith('[')) responseString = '[' + responseString + ']';

            return JsonConvert.DeserializeObject<Units>("{\"fetchedUnits\":" + responseString + "}").fetchedUnits;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }
    }

    public static async Task<int> fetchCurrentScienceOfResearch(int BuildingID)
    {
        try
        {
            string responseString = await client.GetStringAsync(buildingsAPIRoute + "/" + BuildingID.ToString() + "/currentScience");

            return Convert.ToInt32(responseString);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return 0;
        }
    }
    #endregion

    #region DEPRECATED

    // DEPRECATED

    /// <summary>
    /// Aszinkron POST k�r�s, melyben a szerver hozzon l�tre a megadott n�vvel a felhaszn�l�nak egy �j v�rost.
    /// </summary>
    /// <param name="townName">A megadott v�rosn�v.</param>
    /// <param name="UID">A felhaszn�l� ID-je, aki� lesz a v�ros.</param>
    /// <returns></returns>
    //public static async Task<HttpStatusCode> postCreateTown(string townName, int UID)
    //{
    //    string responseString = await response.Content.ReadAsStringAsync();

    //    UnityWebRequest request;

    //    WWWForm data = new WWWForm();
    //    data.AddField("TownName", townName);
    //    //string json = JsonUtility.ToJson(new TownToCreate(townNameInput.text));
    //    //Debug.Log(json);
    //    request = UnityWebRequest.Post(townsAPIRoute + "/create/" + UID, data);

    //    yield return request.SendWebRequest();

    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.LogError("NINCS NET");
    //    }
    //    else
    //    {
    //        Debug.Log(request.downloadHandler.text);
    //    }

    //    request.Dispose();
    //}

    /// <summary>
    /// Lek�ri az adatb�zisb�l a felhaszn�l� adatait.
    /// </summary>
    /// <param name="username">A felhaszn�l� neve, ami alapj�n t�rt�nik a keres�s.</param>
    /// <returns>A keresett felhaszn�l�.</returns>
    //public static User fetchUserByName(string username)
    //{
    //    HttpWebRequest request;
    //    HttpWebResponse response;
    //    try
    //    {
    //        request = (HttpWebRequest)WebRequest.Create(usersAPIRoute + "?Username=" + username);
    //        request.Method = "GET";
    //        response = (HttpWebResponse)request.GetResponse();
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError(ex.Message);
    //        return null;
    //    }

    //    StreamReader sr = new StreamReader(response.GetResponseStream());
    //    string json = sr.ReadToEnd();

    //    return JsonConvert.DeserializeObject<User>(json.TrimStart('[').TrimEnd(']'));
    //}

    /// <summary>
    /// A felhaszn�l� v�rosainak lek�r�se az adatb�zisb�l.
    /// </summary>
    /// <param name="UID">A felhaszn�l� azonos�t�ja.</param>
    /// <returns></returns>
    //public static Town[] fetchTownsOfUser(int UID)
    //{
    //    HttpWebRequest request;
    //    HttpWebResponse response;
    //    try
    //    {
    //        request = (HttpWebRequest)WebRequest.Create(townsAPIRoute + "?Users_UID=" + UID);
    //        request.Method = "GET";
    //        response = (HttpWebResponse)request.GetResponse();

    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.LogError(ex.Message);
    //        return null;
    //    }

    //    // Debug.Log(response.GetResponseStream());
    //    StreamReader sr = new StreamReader(response.GetResponseStream());
    //    string json = sr.ReadToEnd();

    //    // Debug.Log(json);

    //    if (!json.StartsWith('[') && !json.EndsWith(']')) json = '[' + json + ']';

    //    return JsonConvert.DeserializeObject<Towns>("{\"fetchedTowns\":" + json + "}").fetchedTowns;
    //}
    #endregion
}