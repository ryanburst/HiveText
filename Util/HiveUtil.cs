using BepInEx.Configuration;
using System;
using System.Collections.Generic;

namespace HiveText.Util
{
    internal class HiveUtil
    {
        
        public class HiveKeyAttributes
        {
            public string DEFAULT_TEXT = string.Empty;
            public string PropName = string.Empty;
            public string TextKey = string.Empty;
            public ConfigEntry<bool> ConfigName = null!;
        }
        public class HiveKey
        {
            public static bool isDefaultSet = false;

            public static HiveKeyAttributes SLEEP = new HiveKeyAttributes()
                { DEFAULT_TEXT = "", PropName = "m_sleepText", TextKey = "SleepText", ConfigName = HiveTextPlugin.RandomSleepText };
            public static HiveKeyAttributes BIOME = new HiveKeyAttributes()
                { DEFAULT_TEXT = "", PropName = "m_areaText", TextKey = "BiomeText", ConfigName = HiveTextPlugin.RandomBiomeText };
            public static HiveKeyAttributes SPACE = new HiveKeyAttributes()
                { DEFAULT_TEXT = "", PropName = "m_freespaceText", TextKey = "FreespaceText", ConfigName = HiveTextPlugin.RandomSpaceText };
            public static HiveKeyAttributes HAPPY = new HiveKeyAttributes()
                { DEFAULT_TEXT = "", PropName = "m_happyText", TextKey = "HappyText", ConfigName = HiveTextPlugin.RandomHappyText };
            public static HiveKeyAttributes EXTRACTED = new HiveKeyAttributes()
                { TextKey = "ExtractedText", ConfigName = HiveTextPlugin.RandomExtractedText };
            public static HiveKeyAttributes ATTACKED = new HiveKeyAttributes()
                { TextKey = "AttackedText", ConfigName = HiveTextPlugin.RandomAttackedText };
            public static HiveKeyAttributes GENERIC = new HiveKeyAttributes()
                { TextKey = "GenericText" };
        }
  
        // Gets random text from the YML file for a specific group. If that group does
        // not exist, it will attempt to grab from the GENERIC group as a fallback.
        public static string GetRandomTextByKey(string key)
        {
            if (HiveTextPlugin.yamlData == null)
            {
                return string.Empty;
            }
            List<string> options = new List<string>();

            // First try to grab the list that was requested
            if (!string.IsNullOrEmpty(key) && HiveTextPlugin.yamlData.TryGetValue(key, out List<string> keyList))
                options.AddRange(keyList);

            // No items in the yaml data for given key, add generic text instead
            if (options.Count == 0 && HiveTextPlugin.yamlData.TryGetValue((string) HiveKey.GENERIC.TextKey, out List<string> genericList))
                options.AddRange(genericList);

            // Still no options, just return blank
            if (options.Count == 0 )
                return string.Empty;

            var random = new Random();
            int index = random.Next(options.Count);
            return options[index];
        }

        // Returns the default text of a HiveKey by default, but if the HiveKey config
        // option is turned on, it will attempt to grab a random line of text instead
        public static string GetHiveText(HiveKeyAttributes Key)
        {
            string text = "";
            if (Key.ConfigName.Value == true)
            {
                text = GetRandomTextByKey(Key.TextKey);
            }
            return string.IsNullOrEmpty(text) ? Key.DEFAULT_TEXT : text;
        }
    }
}
