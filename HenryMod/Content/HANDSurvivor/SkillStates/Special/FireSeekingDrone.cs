﻿using HANDMod.Content.HANDSurvivor;
using HANDMod.Content.HANDSurvivor.Components.Body;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace EntityStates.HAND_Overclocked.Special
{
    public class FireSeekingDrone : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            hasFired = false;
            Transform modelTransform = base.GetModelTransform();
            targetingController = base.GetComponent<HANDTargetingController>();
            Util.PlaySound("Play_HOC_Drone", base.gameObject);
            if (base.isAuthority && targetingController)
            {
                this.initialOrbTarget = targetingController.GetTrackingTarget();
                //handController.CmdHeal();
            }
            this.duration = baseDuration; /// this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(this.duration + 1f);
            }
            this.isCrit = base.RollCrit();
        }

        public override void OnExit()
        {
            if (!hasFired && base.isAuthority)
            {
                FireProjectile(this.initialOrbTarget, base.inputBank.aimOrigin);
            }
            base.OnExit();
        }

        private void FireProjectile(HurtBox target, Vector3 position)
        {
            hasFired = true;
            FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
            fireProjectileInfo.position = position;

            fireProjectileInfo.rotation = Util.QuaternionSafeLookRotation(base.GetAimRay().direction);
            fireProjectileInfo.crit = base.RollCrit();
            fireProjectileInfo.damage = this.damageStat * FireSeekingDrone.damageCoefficient;
            fireProjectileInfo.damageColorIndex = DamageColorIndex.Default;
            fireProjectileInfo.owner = base.gameObject;
            fireProjectileInfo.force = FireSeekingDrone.force;
            fireProjectileInfo.projectilePrefab = GetProjectile();
            if (target)
            {
                fireProjectileInfo.target = target.gameObject;
            }
            ProjectileManager.instance.FireProjectile(fireProjectileInfo);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!hasFired && base.isAuthority)
            {
                FireProjectile(this.initialOrbTarget, base.inputBank.aimOrigin);
            }
            if (base.fixedAge > this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public virtual GameObject GetProjectile()
        {
            return FireSeekingDrone.projectilePrefab;
        }

        private bool hasFired;


        public static float damageCoefficient = 2.7f;
        public static GameObject projectilePrefab;
        public static string muzzleString;
        public static GameObject muzzleflashEffectPrefab;
        public static float baseDuration = 0.25f;
        public static float force = 250f;

        private float duration;
        protected bool isCrit;
        private HurtBox initialOrbTarget = null;
        private HANDTargetingController targetingController;
    }
}
