using System.Collections;
using System.Net;
using UnityEngine;
using System.IO;
using System;

using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LoginManager : MonoBehaviour
{
    [Header("Variables")]
    [Space(10)]
    public string registerPageLink = "https://houseofswords.hu/register";

    [Space(5)]
    [Header("Login Screen Objects")]
    [Space(5)]
    public TMP_InputField usernameText;
    public TMP_InputField pwdText;
    public TMP_Text errorText;

    [Space(10)]
    [Header("Towns Menu")]
    [Space(5)]
    public GameObject townCreationButton;
    public TMP_InputField townNameInput;
    public TMP_Text townErrorText;
    public GameObject[] townMenuButtons;

    GameObject loginPanel;
    GameObject loggedInPanel;

    GameManager gameManager;
    User user;

    public void Start()
    {
        usernameText.Select();
        pwdText.inputType = TMP_InputField.InputType.Password;

        loginPanel = GameObject.Find("LoginPanel");
        loginPanel.SetActive(true);
        loggedInPanel = GameObject.Find("LoggedInPanel");
        loggedInPanel.SetActive(false);

        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        // Debug.Log(CryptoManager.hashString("wauboikdiekrl�ciasnmvteveiH"));
    }

    /// <summary>
    /// Megnyitja a felhaszn�l� alap�rtelmezett b�ng�sz�j�ben a regisztr�ci�s oldalt.
    /// </summary>
    public void registerNavigation()
    {
        Application.OpenURL(registerPageLink);
    }

    /// <summary>
    /// Ellen�rzi a felhaszn�l� �ltal be�rt adatok helyess�g�t, �s ha minden rendben van, akkor bejelentkezteti a felhaszn�l�t.
    /// </summary>
    public void tryLogin()
    {
        displayError();

        string username = usernameText.text;
        string pwd = pwdText.text;

        // user = fetchUserByName(username);
        user = APIHelper.fetchUserByName(username);
        if (user == null)
        {
            displayError("User does not exist.", usernameText);
            return;
        }

        string saltedPwd = pwd + user.PwdSalt;

        if (isPwdCorrect(saltedPwd, user.PwdHash))
        {
            onLoginSuccess(true);
            return;
        }

        displayError("Passwords don't match.", pwdText);
        return;
    }

    /// <summary>
    /// A be�rt jelsz� ellen�rz�se a titkos�t�si ir�nyelveket k�vetve.
    /// </summary>
    /// <param name="saltedPwd">A be�rt jelsz�, melynek a v�g�re hozz�f�zt�k a s�-l�ncot.</param>
    /// <param name="hash">Az adatb�zisb�l �rkezett titkos�tott jelsz�, amivel egyeznie kell az el�z� jelsz�nak titkos�t�s ut�n.</param>
    /// <returns>Ha igaz, akkor helyes a be�rt jelsz�.</returns>
    public bool isPwdCorrect(string saltedPwd, string hash)
    {
        for (int i = 0; i < 52; i++)
            if (CryptoManager.hashString(CryptoManager.pepperString(saltedPwd)[i]) == hash) return true;

        return false;
    }

    /// <summary>
    /// Ki�r egy hib�t, amibe a bejelentkez�s sor�n �tk�zt�nk, vagy kit�rli a hibamez�ket (ha nem adunk meg param�tereket).
    /// </summary>
    /// <param name="errorValue">A ki�rand� hiba (ha van).</param>
    /// <param name="problemField">A bemeneti mez�, amivel van a probl�ma (ha van).</param>
    public void displayError(string errorValue = "", TMP_InputField problemField = null)
    {
        errorText.text = errorValue;
        errorText.enabled = errorValue != "";
        if (problemField != null) problemField.Select();
    }

    /// <summary>
    /// Ha el�z�leg ki�rtunk egy hib�t, �s a felhaszn�l� elkezdi kijav�tani a be�rt adatokat, akkor elt�ntetj�k a ki�rt hib�t.
    /// </summary>
    public void inputTextChanged()
    {
        displayError();
    }

    /// <summary>
    /// Bejelentkez�ssel, avagy kijelentkez�ssel kapcsolatos logika.
    /// </summary>
    /// <param name="value">True: sikeres bejelentkez�s; False: kijelentkez�s</param>
    public void onLoginSuccess(bool value)
    {
        loginPanel.SetActive(!value);
        loggedInPanel.SetActive(value);
        pwdText.text = "";

        if (value)
        {
            gameManager.User = user;
            int index = 0;
            Town[] townsOfUser = APIHelper.fetchTownsOfUser(user.UID);
            if (townsOfUser.Length > 0)
            {
                foreach (Town town in townsOfUser)
                {
                    townMenuButtons[index].GetComponentInChildren<Text>().text = town.TownName;
                    index++;
                }
            }

            for (int i = index; i < gameManager.maxTownCount; i++)
            {

                townMenuButtons[i].GetComponentInChildren<Text>().text = "Create new town...";

                townMenuButtons[i].GetComponent<Button>().onClick.AddListener(showTownCreationButtons);
            }
        }
        else
        {
            gameManager.User = null;
            townErrorText.text = String.Empty;
            townNameInput.text = String.Empty;
            townErrorText.enabled = false;
            townCreationButton.SetActive(false);
            townNameInput.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �j v�ros k�sz�t�s�n�l a be�rt v�rosn�v hossz�nak ellen�rz�se.
    /// </summary>
    /// <param name="enteredName">A be�rt v�rosn�v</param>
    /// <returns>�res sz�veget, ha minden rendben, ellenkez� esetben pedig a hiba�zenetet.</returns>
    public string validateTownName(string enteredName)
    {
        if (enteredName.Length < 5) return "A v�rosn�v t�l r�vid (min. 5 karakter)!";
        if (enteredName.Length > 20) return "A v�rosn�v t�l hossz� (max. 20 karakter)!";
        return String.Empty;
    }

    /// <summary>
    /// Az �j v�rosn�v beviteli mez�j�nek v�ltoz�sa eset�n automatikus valid�l�s.
    /// </summary>
    public void townNameTextChanged()
    {
        string validationResult = validateTownName(townNameInput.text);

        townErrorText.text = validationResult;
        townErrorText.enabled = validationResult != String.Empty;
    }

    /// <summary>
    /// Ha a felhaszn�l� r�nyomott az �j v�ros gombra, akkor jelenjen meg a v�rosn�v beviteli mez� �s a l�trehoz�s gomb.
    /// </summary>
    public void showTownCreationButtons()
    {
        townNameInput.gameObject.SetActive(true);
        townCreationButton.SetActive(true);
    }

    /// <summary>
    /// A l�trehoz�s gombra kattintva v�gs� ellen�rz�s, majd egy k�r�s k�ld�se a szervernek.
    /// </summary>
    public void tryCreateTown()
    {
        if (validateTownName(townNameInput.text) != String.Empty) return;

        // CREATE TOWN WITH POST REQUEST
        StartCoroutine(APIHelper.postCreateTown(townNameInput.text, gameManager.User.UID));
    }
}