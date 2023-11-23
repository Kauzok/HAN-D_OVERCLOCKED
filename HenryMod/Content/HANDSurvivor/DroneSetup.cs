﻿using RoR2;
using UnityEngine;
using RoR2.Projectile;
using R2API;
using EntityStates.HAND_Overclocked.Special;
using HANDMod.Content.HANDSurvivor.Components.Body;
using UnityEngine.AddressableAssets;

namespace HANDMod.Content.HANDSurvivor
{
    //Copypasted the code from the original HAN-D Overclocked.
    public class DroneSetup
    {
        public static GameObject droneProjectileGhost;

        public static void Init()
        {
            //Currently hardcoded so that Drone Ghost is made in CreateDroneProjectile. Should be separated now that there are multiple things that rely on it.
            if (!FireSeekingDrone.projectilePrefab) FireSeekingDrone.projectilePrefab = CreateDroneProjectile();
            if (!FireSpeedDrone.speedDroneProjectile) FireSpeedDrone.speedDroneProjectile = CreateDroneSpeedProjectile();

            if (!DroneFollowerController.dronePrefab) DroneFollowerController.dronePrefab = CreateDroneFollower();
            if (!HANDTargetingController.allyIndicatorPrefab) HANDTargetingController.allyIndicatorPrefab = CreateAllyIndicator();
            if (!HANDTargetingController.enemyIndicatorPrefab) HANDTargetingController.enemyIndicatorPrefab = CreateEnemyIndicator();
        }

        private static GameObject CreateAllyIndicator()
        {
            GameObject indicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/PassiveHealing/WoodSpriteIndicator.prefab").WaitForCompletion().InstantiateClone("HANDMod_AllyIndicator", false);
            UnityEngine.Object.Destroy(indicator.GetComponentInChildren<RoR2.InputBindingDisplayController>());
            UnityEngine.Object.Destroy(indicator.GetComponentInChildren<TMPro.TextMeshPro>());

            Rewired.ComponentControls.Effects.RotateAroundAxis rot = indicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>();
            UnityEngine.Object.Destroy(rot);

            SpriteRenderer sr = indicator.GetComponentInChildren<SpriteRenderer>();
            sr.sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texIndicatorDroneHeal.png");
            sr.color = new Color(189f/255, 1f, 77f / 255f);
            sr.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            sr.transform.localScale = 0.5f * Vector3.one;

            return indicator;
        }
        private static GameObject CreateEnemyIndicator()
        {
            GameObject indicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiMissileTrackingIndicator.prefab").WaitForCompletion().InstantiateClone("HANDMod_EnemyIndicator", false);
            SpriteRenderer[] sr = indicator.GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer s in sr)
            {
                if (s.name == "Base Core")
                {
                    s.color = new Color(0.556862745f, 0.682352941f, 0.690196078f);
                    break;
                }
            }
            return indicator;
        }

        private static GameObject CreateDroneProjectile()
        {
            GameObject droneProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/EngiHarpoon").InstantiateClone("HANDMod_DroneProjectile", true);

            Shader hotpoo = LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/hgstandard");
            droneProjectileGhost = PrefabAPI.InstantiateClone(HANDMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("DronePrefab"), "HANDMod_DroneProjectileGhost", false);


            MeshRenderer[] mr = droneProjectileGhost.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer m in mr)
            {
                if (m.name != "DronePropeller")
                {
                    m.material.shader = hotpoo;
                }
            }

            SkinnedMeshRenderer[] smr = droneProjectileGhost.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer m in smr)
            {
                if (m.name != "DronePropeller")
                {
                    m.material.shader = hotpoo;
                }
            }

            droneProjectileGhost.AddComponent<ProjectileGhostController>();

            droneProjectileGhost.layer = LayerIndex.noCollision.intVal;

            droneProjectile.GetComponent<ProjectileController>().ghostPrefab = droneProjectileGhost;

            Material droneMat = Modules.Materials.CreateHopooMaterial("DroneBody");
            Modules.Materials.SetEmission(droneMat, 3f, Color.white);
            droneProjectileGhost.GetComponentInChildren<SkinnedMeshRenderer>().material = droneMat;

            Collider[] collidersG = droneProjectileGhost.GetComponentsInChildren<Collider>();
            foreach (Collider cG in collidersG)
            {
                UnityEngine.Object.Destroy(cG);
            }

            HANDMod.Modules.ContentPacks.projectilePrefabs.Add(droneProjectile);

            UnityEngine.Object.Destroy(droneProjectile.GetComponent<ApplyTorqueOnStart>());
            UnityEngine.Object.Destroy(droneProjectile.GetComponent<MissileController>());
            ProjectileSteerTowardTarget pst = droneProjectile.AddComponent<ProjectileSteerTowardTarget>();
            pst.yAxisOnly = false;
            pst.rotationSpeed = 360f;

            ProjectileSimple ps = droneProjectile.AddComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 40f;
            ps.lifetime = 30f;
            ps.updateAfterFiring = true;
            ps.enableVelocityOverLifetime = false;
            ps.oscillate = true;
            ps.oscillateMagnitude = 6f;
            ps.oscillateSpeed = 1.5f;

            ProjectileSphereTargetFinder pstf = droneProjectile.AddComponent<ProjectileSphereTargetFinder>();
            pstf.lookRange = 90f;
            pstf.targetSearchInterval = 0.3f;
            pstf.onlySearchIfNoTarget = true;
            pstf.allowTargetLoss = false;
            pstf.testLoS = false;
            pstf.ignoreAir = false;
            pstf.flierAltitudeTolerance = Mathf.Infinity;

            UnityEngine.Object.Destroy(droneProjectile.GetComponent<AkEvent>());
            UnityEngine.Object.Destroy(droneProjectile.GetComponent<AkGameObj>());
            UnityEngine.Object.Destroy(droneProjectile.GetComponent<ProjectileSingleTargetImpact>());

            ProjectileStickOnImpact stick = droneProjectile.AddComponent<ProjectileStickOnImpact>();
            stick.ignoreWorld = true;
            stick.ignoreCharacters = false;
            stick.alignNormals = false;


            Collider[] colliders = droneProjectile.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                UnityEngine.Object.Destroy(c);
            }
            SphereCollider sc = droneProjectile.AddComponent<SphereCollider>();
            sc.radius = 0.6f;
            sc.contactOffset = 0.01f;

            droneProjectile.AddComponent<Components.DroneProjectile.DroneDamageController>();
            droneProjectile.AddComponent<Components.DroneProjectile.DroneCollisionController>();

            ProjectileController pc = droneProjectile.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            //droneProjectile.layer = LayerIndex.collideWithCharacterHullOnly.intVal;

            return droneProjectile;
        }

        //Can't be bothered to refactor this right now. Will just copypaste the Drone setup.
        private static GameObject CreateDroneSpeedProjectile()
        {
            GameObject droneProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/EngiHarpoon").InstantiateClone("HANDMod_DroneSpeedProjectile", true);
            droneProjectile.GetComponent<ProjectileController>().ghostPrefab = droneProjectileGhost;

            HANDMod.Modules.ContentPacks.projectilePrefabs.Add(droneProjectile);

            UnityEngine.Object.Destroy(droneProjectile.GetComponent<ApplyTorqueOnStart>());
            UnityEngine.Object.Destroy(droneProjectile.GetComponent<MissileController>());
            ProjectileSteerTowardTarget pst = droneProjectile.AddComponent<ProjectileSteerTowardTarget>();
            pst.yAxisOnly = false;
            pst.rotationSpeed = 360f;

            ProjectileSimple ps = droneProjectile.AddComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 40f;
            ps.lifetime = 30f;
            ps.updateAfterFiring = true;
            ps.enableVelocityOverLifetime = false;
            ps.oscillate = true;
            ps.oscillateMagnitude = 6f;
            ps.oscillateSpeed = 1.5f;

            ProjectileSphereTargetFinder pstf = droneProjectile.AddComponent<ProjectileSphereTargetFinder>();
            pstf.lookRange = 90f;
            pstf.targetSearchInterval = 0.3f;
            pstf.onlySearchIfNoTarget = true;
            pstf.allowTargetLoss = false;
            pstf.testLoS = false;
            pstf.ignoreAir = false;
            pstf.flierAltitudeTolerance = Mathf.Infinity;

            UnityEngine.Object.Destroy(droneProjectile.GetComponent<AkEvent>());
            UnityEngine.Object.Destroy(droneProjectile.GetComponent<AkGameObj>());
            UnityEngine.Object.Destroy(droneProjectile.GetComponent<ProjectileSingleTargetImpact>());

            ProjectileStickOnImpact stick = droneProjectile.AddComponent<ProjectileStickOnImpact>();
            stick.ignoreWorld = true;
            stick.ignoreCharacters = false;
            stick.alignNormals = false;


            Collider[] colliders = droneProjectile.GetComponentsInChildren<Collider>();
            foreach (Collider c in colliders)
            {
                UnityEngine.Object.Destroy(c);
            }
            SphereCollider sc = droneProjectile.AddComponent<SphereCollider>();
            sc.radius = 0.6f;
            sc.contactOffset = 0.01f;

            //Attack speed buff is initialized before HAN-D stuff is initialized.
            //In retrospect reorganizing the project to try to associate Buffs/Projectiles with specific characters instead of being usable in the whole project was a bad idea.
            Components.DroneProjectile.DroneDamageController ddc = droneProjectile.AddComponent<Components.DroneProjectile.DroneDamageController>();
            ddc.damageHealFraction = 0f;
            ddc.buffOnHitDuration = 10f;
            ddc.buffOnHit = Shared.Buffs.AttackSpeed;   

            droneProjectile.AddComponent<Components.DroneProjectile.DroneCollisionController>();

            ProjectileController pc = droneProjectile.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            //droneProjectile.layer = LayerIndex.collideWithCharacterHullOnly.intVal;

            return droneProjectile;
        }

        private static GameObject CreateDroneFollower()
        {
            Shader hotpoo = LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/hgstandard");
            GameObject droneFollower = PrefabAPI.InstantiateClone(HANDMod.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("DroneFollowerPrefab"), "HANDMod_DroneFollower", false);

            MeshRenderer[] meshes = droneFollower.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer m in meshes)
            {
                if (m.name != "DronePropeller")
                {
                    m.material.shader = hotpoo;
                }
            }

            droneFollower.layer = LayerIndex.noCollision.intVal;

            Material droneMat = Modules.Materials.CreateHopooMaterial("DroneBody");
            Modules.Materials.SetEmission(droneMat, 3f, Color.white);
            droneFollower.GetComponentInChildren<SkinnedMeshRenderer>().material = droneMat;

            SkinnedMeshRenderer[] smr = droneFollower.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer m in smr)
            {
                if (m.name != "DronePropeller")
                {
                    m.material.shader = hotpoo;
                }
            }

            return droneFollower;
        }
    }
}
