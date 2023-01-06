using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

public static class CryptoManager
{
    /// <summary>
    /// Titkos�t egy karakterl�ncot SHA512-es form�tumba.
    /// </summary>
    /// <param name="s">Az �talak�tand� karakterl�nc.</param>
    /// <returns></returns>
    public static string hashString(string s)
    {
        // A titkos�t� oszt�ly el�k�sz�t�se
        SHA512 hasher = SHA512.Create();
        hasher.Initialize();

        // A be�rt karakterl�ncot egy hexadecim�lis byte t�mbb� alak�tjuk,
        // majd ezt a t�mb�t a titkos�t� oszt�ly seg�ts�g�vel k�doljuk
        byte[] data = hasher.ComputeHash(Encoding.Default.GetBytes(s));

        // Az eredm�nyt byte t�mbb�l visszaalak�tjuk karakterekk�
        string result = BitConverter.ToString(data);

        // A karakterl�nc form�z�sa, hogy az adatb�zisban l�v�vel egyez� legyen
        // (nincsenek benne k�t�jelek, �s kisbet�s)
        string reductedResult = "";
        for (int i = 0; i < result.Length; i++)
            if (result[i] != '-') reductedResult += result[i];
        reductedResult = reductedResult.ToLower();

        // A titkos�t� oszt�ly p�ld�ny�t t�r�lj�k a biztons�g �rdek�ben.
        hasher.Dispose();

        return reductedResult;
    }

    /// <summary>
    /// Visszaadja a karakterl�nc �sszes lehets�ges vari�ci�j�t borsoz�s ut�n.
    /// </summary>
    /// <param name="s">A borsozand� karakterl�nc.</param>
    /// <returns></returns>
    public static List<string> pepperString(string s)
    {
        List<string> pepperedStrings = new List<string>();

        for (char c = 'a'; c <= 'z'; c++)
            pepperedStrings.Add(s + c);
        for (char c = 'A'; c <= 'Z'; c++)
            pepperedStrings.Add(s + c);

        return pepperedStrings;
    }
}

