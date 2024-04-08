using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using JetBrains.Annotations;
using ServerSync;
using UnityEngine;
using HiveText.Util;

namespace HiveText
{
    [BepInPlugin(ModGUID, ModName, ModVersion)]
    public class HiveTextPlugin : BaseUnityPlugin
    {
        // 
        internal const string ModName = "HiveText";
        internal const string ModVersion = "0.1.0";
        internal const string Author = "WildGrue";
        private const string ModGUID = Author + "." + ModName;
        private static string ConfigFileName = ModGUID + ".cfg";
        private static string ConfigFileFullPath = Paths.ConfigPath + Path.DirectorySeparatorChar + ConfigFileName;
        internal static string ConnectionError = "";
        private readonly Harmony _harmony = new(ModGUID);

        public static readonly ManualLogSource HiveTextLogger =
            BepInEx.Logging.Logger.CreateLogSource(ModName);

        private static readonly ConfigSync ConfigSync = new(ModGUID)
        { DisplayName = ModName, CurrentVersion = ModVersion, MinimumRequiredVersion = ModVersion };

        // Use a .yml file instead of the cfg to hold all of the random text
        internal static readonly string yamlFileName = SanitizeFileName($"{Author}.{ModName}.yml");
        internal static readonly string yamlPath = Paths.ConfigPath + Path.DirectorySeparatorChar + yamlFileName;
        internal static readonly CustomSyncedValue<string> HiveText_yamlData = new(ConfigSync, "HiveText_yamlData", "");

        // Stores random text as a list of options in a dictionary with the group as the key
        internal static Dictionary<string, List<string>>? yamlData;
        internal static HiveTextPlugin self = null!;
        

        public enum Toggle
        {
            On = 1,
            Off = 0
        }

        public void Awake()
        {
            _serverConfigLocked = config("1 - General", "Lock Configuration", Toggle.On,new ConfigDescription(
                "If on, the configuration is locked and can be changed by server admins only.", null, new ConfigurationManagerAttributes() { Order = 8 }));
            _ = ConfigSync.AddLockingConfigEntry(_serverConfigLocked);

            // Enables different random text groups
            RandomSleepText = config("2 - Enable Random Text", "SleepText", true, new ConfigDescription("When the bees sleep, use random text to tell the player. Or just shit the hell up.", null, new ConfigurationManagerAttributes() { Order = 7 }));
            RandomSpaceText = config("2 - Enable Random Text", "FreespaceText", true, new ConfigDescription("The bees need more room? Tell them to drink less coffee so they can afford it. Or use random text.", null, new ConfigurationManagerAttributes() { Order = 6 }));
            RandomBiomeText = config("2 - Enable Random Text", "BiomeText", true, new ConfigDescription("When the bees whine about the biome they're in, use random text to complain.", null, new ConfigurationManagerAttributes() { Order = 5 }));
            RandomHappyText = config("2 - Enable Random Text", "HappyText",true, new ConfigDescription("They don't need to be happy, but use random text to let them express themselves.", null, new ConfigurationManagerAttributes() { Order = 4 }));
            RandomExtractedText = config("2 - Enable Random Text", "ExtractedText",true, new ConfigDescription("Someone stole from the bees, use random text to shout at them.", null, new ConfigurationManagerAttributes() { Order = 3 }));
            RandomAttackedText = config("2 - Enable Random Text", "AttackedText",true, new ConfigDescription("Someone has entered their safe space, use this random text when attacked.", null, new ConfigurationManagerAttributes() { Order = 2 }));
            
            if (!File.Exists(yamlPath))
            {
                YamlUtil.WriteConfigFileFromResource(yamlPath);
            }

            HiveText_yamlData.ValueChanged += OnValChangedUpdate; // check for file changes
            HiveText_yamlData.AssignLocalValue(File.ReadAllText(yamlPath));

            Assembly assembly = Assembly.GetExecutingAssembly();
            _harmony.PatchAll(assembly);
            SetupWatcher();
        }

        public static string SanitizeFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }


        private void OnDestroy()
        {
            Config.Save();
        }

        private void SetupWatcher()
        {
            // Watch for config file changes
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;

            // Watch for yml file changes
            FileSystemWatcher yamlwatcher = new(Paths.ConfigPath, yamlFileName);
            yamlwatcher.Changed += ReadYamlFiles;
            yamlwatcher.Created += ReadYamlFiles;
            yamlwatcher.Renamed += ReadYamlFiles;
            yamlwatcher.IncludeSubdirectories = true;
            yamlwatcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            yamlwatcher.EnableRaisingEvents = true;
        }

        private void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                HiveTextLogger.LogDebug("ReadConfigValues called");
                Config.Reload();
            }
            catch
            {
                HiveTextLogger.LogError($"There was an issue loading your {ConfigFileName}");
                HiveTextLogger.LogError("Please check your config entries for spelling and format!");
            }
        }

        private void ReadYamlFiles(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(yamlPath)) return;
            try
            {
                HiveTextLogger.LogDebug("ReadConfigValues called");
                HiveText_yamlData.AssignLocalValue(File.ReadAllText(yamlPath));
            }
            catch
            {
                HiveTextLogger.LogError($"There was an issue loading your {yamlFileName}");
                HiveTextLogger.LogError("Please check your entries for spelling and format!");
            }
        }

        private static void OnValChangedUpdate()
        {
            HiveTextLogger.LogDebug("OnValChanged called");
            try
            {
                YamlUtil.ReadYaml(HiveText_yamlData.Value);
            }
            catch (Exception e)
            {
                HiveTextLogger.LogError($"Failed to deserialize {yamlFileName}: {e}");
            }
        }



        #region ConfigOptions

        private static ConfigEntry<Toggle> _serverConfigLocked = null!;
        internal static ConfigEntry<bool> RandomSleepText = null!;
        internal static ConfigEntry<bool> RandomSpaceText = null!;
        internal static ConfigEntry<bool> RandomBiomeText = null!;
        internal static ConfigEntry<bool> RandomHappyText = null!;
        internal static ConfigEntry<bool> RandomExtractedText = null!;
        internal static ConfigEntry<bool> RandomAttackedText = null!;

        private ConfigEntry<T> config<T>(string group, string name, T value, ConfigDescription description,
            bool synchronizedSetting = true)
        {
            ConfigDescription extendedDescription =
                new(
                    description.Description +
                    (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]"),
                    description.AcceptableValues, description.Tags);
            ConfigEntry<T> configEntry = Config.Bind(group, name, value, extendedDescription);

            SyncedConfigEntry<T> syncedConfigEntry = ConfigSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        private class ConfigurationManagerAttributes
        {
            [UsedImplicitly] public int? Order = null!;
            [UsedImplicitly] public bool? Browsable = null!;
            [UsedImplicitly] public string? Category = null!;
            [UsedImplicitly] public Action<ConfigEntryBase>? CustomDrawer = null!;
        }

        class AcceptableShortcuts : AcceptableValueBase
        {
            public AcceptableShortcuts() : base(typeof(KeyboardShortcut))
            {
            }

            public override object Clamp(object value) => value;
            public override bool IsValid(object value) => true;

            public override string ToDescriptionString() =>
                "# Acceptable values: " + string.Join(", ", UnityInput.Current.SupportedKeyCodes);
        }

        #endregion
    }
    
    public static class KeyboardExtensions
    {
        public static bool IsKeyDown(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKeyDown(shortcut.MainKey) && shortcut.Modifiers.All(Input.GetKey);
        }

        public static bool IsKeyHeld(this KeyboardShortcut shortcut)
        {
            return shortcut.MainKey != KeyCode.None && Input.GetKey(shortcut.MainKey) && shortcut.Modifiers.All(Input.GetKey);
        }
    }
}