using System;
using System.Net;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class UtilsService
{
	public static T[] GetJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
        return wrapper.array;
    }
	
    public static string GetDate(string date)
	{
		// Transforms yyyy-mm-ddT000:000 to yyyy/mm/ddT000:000 and get the date only
		date = date.Replace("-", "/");
		date = date.Split('T')[0];

		DateTime _date = DateTime.ParseExact(date, "yyyy/M/d", CultureInfo.InvariantCulture);
		string aux = _date.ToString("dd/MM/yyyy");

		return aux;
	}

	public static string GetInverseDate (string date)
	{
		if (date.Length != 10) // DD/MM/AAAA
			return "";

		DateTime _date = DateTime.ParseExact(date, "d/M/yyyy", CultureInfo.InvariantCulture);
		string aux = _date.ToString("yyyy/MM/dd");

		return aux;
	}

	public static bool CheckName (string name)
	{
		return (name.Length > 3);
	}

	public static bool CheckEmail (string email)
	{
		return (email.Length > 5) && (email.Contains("@")) && (email.Split('@').Length > 1) && (email.Split('.').Length > 1);
	}

	public static bool CheckPassword (string password)
	{
		return (password.Length >= 6) && (password.Length <= 64);
	}
}