﻿using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace HANDMod.Content.HANDSurvivor.Achievements
{
    [RegisterAchievement("MoffeinHANDOverclockedClearGameMonsoon", "Skins.HANDOverclocked.Mastery", null, 10u, null)]
    public class HandMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("HANDOverclockedBody");
        }
    }
}
