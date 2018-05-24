using System;
using System.Text;
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
		if (date.Split('/').Length == 3)
			return date;

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

	public static string GetBase64 (byte[] data)
	{
		string base64string = Convert.ToBase64String (data);
		return base64string;
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

	public static Texture2D GetDefaultPhoto ()
	{
		var texture = Resources.Load("Icons/pot_seeding") as Texture2D;
		texture.Apply();

		return texture;
	}

	public static Texture2D GetDefaultProfilePhoto ()
	{
		var texture = Resources.Load("Icons/seeding_icon") as Texture2D;
		texture.Apply();

		return texture;
	}
	
	public static Texture2D ResizeTexture(Texture2D pSource, string pFilterMode, float pScale)
	{
	    //*** Variables
	    int i;
	 
	    //*** Get All the source pixels
	    Color[] aSourceColor = pSource.GetPixels(0);
	    Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);
	 
	    //*** Calculate New Size
	    float xWidth = Mathf.RoundToInt((float)pSource.width * pScale);                     
	    float xHeight = Mathf.RoundToInt((float)pSource.height * pScale);
	 
	    //*** Make New
	    Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);
	 
	    //*** Make destination array
	    int xLength = (int)xWidth * (int)xHeight;
	    Color[] aColor = new Color[xLength];
	 
	    Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);
	 
	    //*** Loop through destination pixels and process
	    Vector2 vCenter = new Vector2();
	    for(i=0; i<xLength; i++){
	 
	        //*** Figure out x&y
	        float xX = (float)i % xWidth;
	        float xY = Mathf.Floor((float)i / xWidth);
	 
	        //*** Calculate Center
	        vCenter.x = (xX / xWidth) * vSourceSize.x;
	        vCenter.y = (xY / xHeight) * vSourceSize.y;
	 
	        //*** Do Based on mode
	        //*** Nearest neighbour (testing)
	        if(pFilterMode == "Nearest"){
	 
	            //*** Nearest neighbour (testing)
	            vCenter.x = Mathf.Round(vCenter.x);
	            vCenter.y = Mathf.Round(vCenter.y);
	 
	            //*** Calculate source index
	            int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);
	 
	            //*** Copy Pixel
	            aColor[i] = aSourceColor[xSourceIndex];
	        }
	 
	        //*** Bilinear
	        else if(pFilterMode == "Biliner"){
	 
	            //*** Get Ratios
	            float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
	            float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);
	 
	            //*** Get Pixel index's
	            int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
	            int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
	            int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
	            int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
	 
	            //*** Calculate Color
	            aColor[i] = Color.Lerp(
	                Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
	                Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
	                xRatioY
	            );
	        }
	 
	        //*** Average
	        else if(pFilterMode == "Average"){
	 
	            //*** Calculate grid around point
	            int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
	            int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
	            int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
	            int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);
	 
	            //*** Loop and accumulate
	            //Vector4 oColorTotal = new Vector4();
	            Color oColorTemp = new Color();
	            float xGridCount = 0;
	            for(int iy = xYFrom; iy < xYTo; iy++){
	                for(int ix = xXFrom; ix < xXTo; ix++){
	 
	                    //*** Get Color
	                    oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];
	 
	                    //*** Sum
	                    xGridCount++;
	                }
	            }
	 
	            //*** Average Color
	            aColor[i] = oColorTemp / (float)xGridCount;
	        }
	    }
	 
	    //*** Set Pixels
	    oNewTex.SetPixels(aColor);
	    oNewTex.Apply();
	 
	    //*** Return
	    return oNewTex;
	}
}