﻿using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using R2API;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.Networking;
using RoR2.Audio;
using HANDMod.Modules;

namespace HANDMod.Content.HANDSurvivor
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
                       "HANDMod_DronePassive",
                       true,
                       false,
                       false,
                       new Color(74f / 255f, 170f / 255f, 198f / 255f),
                       Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBuffSwarmArmor.png")
                       );
                On.RoR2.HealthComponent.TakeDamageProcess += DronePassiveHook;
            }
        }

        private static void DronePassiveHook(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                if ((damageInfo.damageType & DamageType.BypassArmor) == 0 && self.body)
                {
                    int buffCount = self.body.GetBuffCount(Buffs.DronePassive);
                    if (buffCount > 0 && damageInfo.damage > 1f)
                    {
                        float totalDamageReduction = buffCount * 0.003f * self.fullCombinedHealth;
                        damageInfo.damage = Mathf.Max(1f, damageInfo.damage - totalDamageReduction);
                        EntitySoundManager.EmitSoundServer(platingSound.index, self.gameObject);
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}
