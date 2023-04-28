using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloseMapScene : MonoBehaviour
{
    /// <summary>
    /// Bez�rja a t�rk�pet.
    /// </summary>
    public void CloseMap()
    {
        SceneManager.UnloadSceneAsync("Map");
    }
}
