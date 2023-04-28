using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateResearchBuilding : MonoBehaviour
{
    GameManager _gameManager;
    Research _research;
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// Amikor megny�lik a kutat� �p�let ablaka kattint�sra, friss�ljenek a ki�rt adatai.
    /// </summary>
    /// <param name="eventData"></param>
    public async void OnMouseDown()
    {
        // Melyik a research �p�let?
        _research = (Research)_gameManager.Buildings["Research"];

        // API H�V�S
        _research.currentScience = await APIHelper.fetchCurrentScienceOfResearch(_research.BuildingID);

        // UI FRISS�T�S
        _research.updateUI();
    }
}
