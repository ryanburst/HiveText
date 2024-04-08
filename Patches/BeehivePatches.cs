using HarmonyLib;
using HiveText.Util;

namespace HiveText.Patches;

[HarmonyPatch(typeof(Beehive), nameof(Beehive.Interact))]
public static class BeehiveAwakePatch
{
    // I think this is the  best place for this logic?
    public static void Prefix(Beehive __instance)
    {
        // If we haven't stored the game's default prop values so we can later
        // restore them if a config gets turned off, set it now.
        if(!HiveUtil.HiveKey.isDefaultSet)
        {
            // This will actually store the localized key, so localization will
            // remain intact for the default values. 
            HiveUtil.HiveKey.SLEEP.DEFAULT_TEXT = __instance.m_sleepText;
            HiveUtil.HiveKey.BIOME.DEFAULT_TEXT = __instance.m_areaText;
            HiveUtil.HiveKey.SPACE.DEFAULT_TEXT = __instance.m_freespaceText;
            HiveUtil.HiveKey.HAPPY.DEFAULT_TEXT = __instance.m_happyText;

            HiveUtil.HiveKey.isDefaultSet = true;
        }
        
    }
}

[HarmonyPatch(typeof(Beehive), nameof(Beehive.Interact))]
public static class BeehiveInteractTextPatch
{
    // Rather than duplicating the checks to find out which message should
    // be shown, just set the prop and let the base class do the work.
    public static void Prefix(Beehive __instance, Humanoid character)
    {
        // Since config can turn on/off between interactions, always set the
        // prop by lettting HiveUtil.GetHiveText to do logic checking
        __instance.m_sleepText = HiveUtil.GetHiveText(HiveUtil.HiveKey.SLEEP);
        __instance.m_areaText = HiveUtil.GetHiveText(HiveUtil.HiveKey.BIOME);
        __instance.m_freespaceText = HiveUtil.GetHiveText(HiveUtil.HiveKey.SPACE);
        __instance.m_happyText = HiveUtil.GetHiveText(HiveUtil.HiveKey.HAPPY);

        // They can extract honey
        if (__instance.GetHoneyLevel() > 0)
        {
            // Only message the hud if there is extracted text
            string text = HiveUtil.GetHiveText(HiveUtil.HiveKey.EXTRACTED);
            if (!string.IsNullOrEmpty(text))
            {
                character.Message(MessageHud.MessageType.Center, text);
            }
        }
    }
}

[HarmonyPatch(typeof(WearNTear), nameof(WearNTear.RPC_Damage))]
public class BeehiveDamagedPatch
{
    // Something that can take wear and tear got hit, if it's a Beehive, someone
    // attacked the poor things! Maybe send random text telling them to fuck off.
    public static void Postfix(WearNTear __instance, long sender, HitData hit)
    {
        if (__instance.m_piece.m_name == "$piece_beehive")
        {
            // There is no default message, so if it's empty, don't even send it.
            // It will return empty if the config is not enabled.
            string text = HiveUtil.GetHiveText(HiveUtil.HiveKey.ATTACKED);
            if (!string.IsNullOrEmpty(text))
            {
                Character attacker = hit.GetAttacker();
                attacker.Message(MessageHud.MessageType.Center, text);
            }
        }
            
    }
}