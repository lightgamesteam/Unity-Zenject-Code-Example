using UnityEngine;

public static class PlayerPrefsExtension
{
   public static void SetBool(string key, bool value, bool save = true)
   {
      PlayerPrefs.SetInt(key, value ? 1 : 0);
      
      if(save)
         PlayerPrefs.Save();
   }
 
   public static bool GetBool(string key)
   {
      if (!PlayerPrefs.HasKey(key))
         return false;
      
      return PlayerPrefs.GetInt(key) == 1;
   }
   
   public static void SetString(string key, string value, bool save = true)
   {
      PlayerPrefs.SetString(key, value);
      
      if(save)
         PlayerPrefs.Save();
   }
 
   public static string GetString(string key)
   {
      if (!PlayerPrefs.HasKey(key))
         return string.Empty;
      
      return PlayerPrefs.GetString(key);
   }
   
   public static int GetInt(string key)
   {
      if (!PlayerPrefs.HasKey(key))
         return 1;
      
      return PlayerPrefs.GetInt(key);
   }
   
   public static void SetInt(string key, int value, bool save = false)
   {
      PlayerPrefs.SetInt(key, value);
      
      if(save)
         PlayerPrefs.Save();
   }
}