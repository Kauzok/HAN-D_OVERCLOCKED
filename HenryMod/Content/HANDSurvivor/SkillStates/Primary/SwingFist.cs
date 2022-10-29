﻿using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.HANDSurvivor.Components.Body;
using HANDMod.SkillStates.BaseStates;
using RoR2;
using R2API;
using UnityEngine;

namespace EntityStates.HANDMod.Primary
{
    public class SwingFist : BaseMeleeAttack
    {
        public static GameObject swingEffect;

        private bool hitEnemy = false;
        public override void OnEnter()
        {
            this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;

            //this.muzzleString = swingIndex % 2 == 0 ? "SwingLeft" : "SwingRight";
            this.swingEffectPrefab = null;
            this.hitEffectPrefab = null;


            this.damageType = DamageType.Generic;
            this.hitHopVelocity = 10f;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "Play_MULT_shift_hit";
            this.swingSoundString = "Play_HOC_Punch";
            this.hitboxName = "FistHitbox";
            this.damageCoefficient = 3.9f;
            this.procCoefficient = 1f;
            this.baseDuration = 1.25f;
            this.baseEarlyExitTime = 0.25f;
            this.attackStartTime = 0.4f;
            this.attackEndTime = 0.55f;
            this.pushForce = 1400f;

            Util.PlaySound("Play_HOC_StartPunch", base.gameObject);

            if (base.characterBody && base.characterBody.HasBuff(Buffs.Overclock) && this.swingIndex == 1)
            {
                this.damageType |= DamageType.Stun1s;
            }

            base.OnEnter();

            if (this.swingIndex != 0)
            {
                base.characterBody.OnSkillActivated(base.skillLocator.primary);
            }

            if (this.attack != null)
            {
                this.attack.AddModdedDamageType(global::HANDMod.Content.DamageTypes.HANDPrimaryPunch);
                this.attack.AddModdedDamageType(global::HANDMod.Content.DamageTypes.ResetVictimForce);
            }
        }


        protected override void PlayAttackAnimation()
        {
            //Uncomment when updated punch anims are in
            /*switch (this.swingIndex)
            {
                case 0:
                    base.PlayCrossfade("Gesture, Override", "PunchL", "Punch.playbackRate", this.duration, 0.2f);
                    break;
                case 1:
                    //base.PlayCrossfade("Gesture, Override", "PunchR", "Punch.playbackRate", this.duration, 0.2f);
                    base.PlayCrossfade("Gesture, Override", "PunchLR", "Punch.playbackRate", this.duration, 0.2f);
                    break;
                case 2:
                    //base.PlayCrossfade("Gesture, Override", "PunchL", "Punch.playbackRate", this.duration, 0.2f);
                    base.PlayCrossfade("Gesture, Override", "PunchRL", "Punch.playbackRate", this.duration, 0.2f);
                    break;
            }*/

            if (this.swingIndex == 1)
            {
                base.PlayCrossfade("Gesture, Override", "PunchR", "Punch.playbackRate", this.duration, 0.2f);
            }
            else
            {
                base.PlayCrossfade("Gesture, Override", "PunchL", "Punch.playbackRate", this.duration, 0.2f);
            }
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (!hitEnemy)
            {
                hitEnemy = true;
                OverclockController hc = base.gameObject.GetComponent<OverclockController>();
                if (hc)
                {
                    hc.MeleeHit();
                    if (base.characterBody && base.characterBody.HasBuff(Buffs.Overclock)) hc.ExtendOverclock(0.8f);
                }
            }
        }


        protected override void SetNextState()
        {
            int index = this.swingIndex;
            switch (index)
            {
                case 0:
                    index = 1;
                    break;
                case 1:
                    index = 2;
                    break;
                case 2:
                    index = 1;
                    break;
            }
            //0 - PunchR
            //1 - PunchLR
            //2 - PunchRL

            this.outer.SetNextState(new SwingFist
            {
                swingIndex = index
            });
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}