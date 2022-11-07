using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.IO;
using System.ComponentModel;
using System;

using Newtonsoft.Json;
using System.Security.Cryptography;
using TMPro;
using System.Text;
using System.Linq.Expressions;
using System.Web;

public class LoginManager : MonoBehaviour
{
    public string usersAPIRoute = "https://houseofswords.hu/api/users";

    public TMP_InputField usernameText;
    public TMP_InputField pwdText;

    public TMP_Text errorText;

    public UserClass[] getAllUsers()
    {
        HttpWebRequest request;
        HttpWebResponse response;
        try
        {
            request = (HttpWebRequest)WebRequest.Create(usersAPIRoute);
            response = (HttpWebResponse)request.GetResponse();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return null;
        }

        StreamReader sr = new StreamReader(response.GetResponseStream());
        string json = sr.ReadToEnd();

        Debug.Log(json);

        return JsonUtility.FromJson<Users>("{\"requestedUsers\":" + json + "}").requestedUsers;

        //Users userArray = JsonUtility.FromJson<Users>("{\"requestedUsers\":" + json + "}");

        //foreach (UserClass user in userArray.requestedUsers)
        //{
        //    Debug.Log($"{user.UID}: {user.Username} - {user.EmailAddress} | {user.PwdHash} | {user.LastLoginDate}");
        //}
    }

    public void tryLogin()
    {
        errorText.enabled = false;
        errorText.text = "";

        string username = usernameText.text;
        string pwd = pwdText.text;

        UserClass[] users = getAllUsers();
        if (users == null)
        {
            errorText.text = "Something went wrong. Please try again later.";
            errorText.enabled = true;
            return;
        }

        foreach (UserClass user in users)
        {
            Debug.Log(user.Username);
            Debug.Log(username);

            // HIBA: A "user.Username" �s a sima "username" valami�rt nem egyezik, f�ggetlen�l a be�rt �rt�kt�l.
            // MEGOLD�S: A "usernameText" �s a "pwdText" v�ltoz�k t�pus�t TMP_InputField-re kellett �t�rni, TMP_Text helyett.
            if (user.Username == username)
            {
                Debug.Log("ANY�DKURVAVOLTKISKOR�T�LKEZDVEBAZDMEG");
                if (isPwdRight(pwd, user.PwdHash))
                {
                    // FASZA MINDEN
                    errorText.text = "LOGGED IN!";
                    errorText.enabled = true;
                    return;
                }
                else
                {
                    errorText.text = "Passwords don't match.";
                    errorText.enabled = true;
                    return;
                }
            }
        }

        errorText.text = "User does not exist.";
        errorText.enabled = true;
        return;
    }

    public bool isPwdRight(string pwd, string hash)
    {
        // A titkos�t� oszt�ly el�k�sz�t�se
        SHA512 hasher = SHA512.Create();
        hasher.Initialize();

        // A be�rt jelsz�t egy hexadecim�lis byte t�mbb� alak�tjuk, majd ezt a t�mb�t a titkos�t� oszt�ly seg�ts�g�vel k�doljuk
        byte[] data = hasher.ComputeHash(Encoding.Default.GetBytes(pwd));

        // Az eredm�nyt byte t�mbb�l visszaalak�tjuk karakterekk�
        string result = BitConverter.ToString(data);

        // Debug.Log("Before: " + result);

        // A karakterl�nc form�z�sa, hogy az adatb�zisban l�v�vel egyez� legyen
        // (nincsenek benne k�t�jelek, �s kisbet�s)
        string reductedResult = "";
        for (int i = 0; i < result.Length; i++)
            if (result[i] != '-') reductedResult += result[i];
        reductedResult = reductedResult.ToLower();

        // Debug.Log("After: " + reductedResult);

        // Ha a frissen titkos�tott hash egyezik az adatb�zisban l�v�vel, akkor j� a jelsz�
        return reductedResult == hash;
    }
}