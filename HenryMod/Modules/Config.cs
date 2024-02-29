﻿using BepInEx.Configuration;
using EntityStates.HAND_Overclocked.Primary;
using RiskOfOptions;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace HANDMod.Modules
{
    public static class Config
    {
        public static bool forceUnlock = false;
        public static bool allowPlayerRepair = false;
        public static float screenshakeScale = 0.2f;
        public static ConfigEntry<KeyboardShortcut> KeybindEmoteCSS;
        public static ConfigEntry<KeyboardShortcut> KeybindEmote1;
        public static ConfigEntry<KeyboardShortcut> KeybindEmote2;
        public static float sortPosition = 4.5f;

        public static void ReadConfig()
        {
            sortPosition = HandPlugin.instance.Config.Bind("General", "Survivor Sort Position", 4.5f, "Controls where HAN-D is placed in the character select screen.").Value;
            forceUnlock = HandPlugin.instance.Config.Bind("General", "Force Unlock", false, "Automatically unlock HAN-D and his skills by default.").Value;
            allowPlayerRepair = HandPlugin.instance.Config.Bind("General", "Allow Player Repair", false, "HAN-D teammates can be revived as an NPC ally for $150.").Value;

            screenshakeScale = HandPlugin.instance.Config.Bind("General", "Screenshake Scale", 0.2f, "Multiplies HURT and FORCED_REASSEMBLY screenshake by this amount.").Value;

            SwingHammer.useForwardLunge = HandPlugin.instance.Config.Bind("Skills", "SMASH - Forward Lunge", true, "Lunge forward when attacking with this skill.");

            KeybindEmote1 = HandPlugin.instance.Config.Bind("Keybinds", "Emote - Sit", new KeyboardShortcut(KeyCode.Alpha1), "Button to play this emote.");
            KeybindEmote2 = HandPlugin.instance.Config.Bind("Keybinds", "Emote - Malfunction", new KeyboardShortcut(KeyCode.Alpha2), "Button to play this emote.");
            KeybindEmoteCSS = HandPlugin.instance.Config.Bind("Keybinds", "Emote - Hat Tip", new KeyboardShortcut(KeyCode.Alpha3), "Button to play this emote.");

            if (HandPlugin.RiskOfOptionsLoaded)
            {
                RiskOfOptionsCompat();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void RiskOfOptionsCompat()
        {
            ModSettingsManager.SetModIcon(Assets.mainAssetBundle.LoadAsset<Sprite>("modicon"));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(SwingHammer.useForwardLunge));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmote1));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmote2));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(KeybindEmoteCSS));
        }

        // this helper automatically makes config entries for disabling survivors
        public static ConfigEntry<bool> CharacterEnableConfig(string characterName, string description = "Set to false to disable this character", bool enabledDefault = true) {

            return HandPlugin.instance.Config.Bind<bool>("General",
                                                          "Enable " + characterName,
                                                          enabledDefault,
                                                          description);
        }

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }
}