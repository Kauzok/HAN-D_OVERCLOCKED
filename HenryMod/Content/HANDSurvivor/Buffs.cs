﻿using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using R2API;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.Networking;
using RoR2.Audio;
using RMORMod.Modules;

namespace RMORMod.Content.HANDSurvivor
{
    public class Buffs
    {
        private static NetworkSoundEventDef platingSound = LegacyResourcesAPI.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseArmorPlateBlock");
        public static BuffDef DronePassive;

        public static void Init()
        {
            if (!Buffs.DronePassive)
            {
                Buffs.DronePassive = Modules.Buffs.CreateBuffDef(
                       "SWARM_ASSAULT",
                       true,
                       false,
                       false,
                       new Color(255f / 255f, 0f / 255f, 84f / 255f),
                       Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSwarmAssault.png")
                       );
                R2API.RecalculateStatsAPI.GetStatCoefficients += RMORPassiveHook;
            }
        }
        private static void RMORPassiveHook(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {
            args.attackSpeedMultAdd += (sender.GetBuffCount(DronePassive) * 0.05f);
            //args.damageMultAdd += (sender.GetBuffCount(DronePassive) * 0.1f);
        }
    }
}
