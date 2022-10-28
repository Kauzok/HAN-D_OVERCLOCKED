﻿using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RoR2;
using System.Linq;

namespace HANDMod.Content.HANDSurvivor.Components.Body
{
    public class TargetingController : MonoBehaviour
    {
        public void Awake()
        {
            this.enemyIndicator = new Indicator(base.gameObject, enemyIndicatorPrefab);
            this.allyIndicator = new Indicator(base.gameObject, allyIndicatorPrefab);
            this.characterBody = base.GetComponent<CharacterBody>();
            this.inputBank = base.GetComponent<InputBankTest>();
            this.teamComponent = base.GetComponent<TeamComponent>();
        }

        public void FixedUpdate()
        {
            this.trackerUpdateStopwatch += Time.fixedDeltaTime;
            if (this.trackerUpdateStopwatch >= 1f / this.trackerUpdateFrequency)
            {
                this.trackerUpdateStopwatch -= 1f / this.trackerUpdateFrequency;
                HurtBox hurtBox = this.trackingTarget;
                Ray aimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
                this.SearchForTarget(aimRay);
                Transform targetTransform = (this.trackingTarget ? this.trackingTarget.transform : null);
                this.enemyIndicator.targetTransform = targetTransform;
                this.allyIndicator.targetTransform = targetTransform;
            }

            if (characterBody.skillLocator.special.stock <= 0)
            {
                this.enemyIndicator.active = false;
                this.allyIndicator.active = false;
            }
            else
            {
                bool targetingEnemy = true;
                if (this.teamComponent && this.trackingTarget && this.trackingTarget.teamIndex == this.teamComponent.teamIndex)
                {
                    targetingEnemy = false;
                }

                if (targetingEnemy)
                {
                    this.enemyIndicator.active = true;
                    this.allyIndicator.active = false;
                }
                else
                {
                    this.enemyIndicator.active = false;
                    this.allyIndicator.active = true;
                }
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.all;
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.maxDistanceFilter = this.maxTrackingDistance;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            this.trackingTarget = this.search.GetResults().FirstOrDefault<HurtBox>();
        }

        public HurtBox GetTrackingTarget()
        {
            return this.trackingTarget;
        }

        public static GameObject enemyIndicatorPrefab;
        public static GameObject allyIndicatorPrefab;

        public float maxTrackingDistance = 160f;
        public float maxTrackingAngle = 120f;
        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;

        private CharacterBody characterBody;
        private TeamComponent teamComponent;
        private InputBankTest inputBank;
        private float trackerUpdateStopwatch;
        private Indicator enemyIndicator;
        private Indicator allyIndicator;
        private readonly BullseyeSearch search = new BullseyeSearch();
    }
}
