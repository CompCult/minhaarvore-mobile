using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CacheService
{
	private static string CACHE_PREFIX = "MinhaArvore:Cache:";

	public static string Get (string cacheName)
	{
		string cache = CACHE_PREFIX + cacheName,
		       data = PlayerPrefs.GetString(cache, null);
		
		return data;
	}

	public static void Set (string cacheName, string data)
	{
		string cache = CACHE_PREFIX + cacheName;
		PlayerPrefs.SetString(cache, data);
	}
}