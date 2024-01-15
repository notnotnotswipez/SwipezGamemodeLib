using System;
using System.Collections;
using BoneLib.Nullables;
using MelonLoader;
using SLZ.Bonelab;
using SLZ.Interaction;
using SLZ.Marrow;
using SLZ.Marrow.Data;
using SLZ.Marrow.Pool;
using SLZ.Marrow.Warehouse;
using SLZ.Player;
using SLZ.Rig;
using SLZ.SFX;
using SLZ.UI;
using SLZ.Utilities;
using SLZ.VRMK;
using UnhollowerBaseLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SwipezGamemodeLib.Spawning
{
    public class SpawnManager
    {
	    
	    public static T GetComponentOnObject<T>(GameObject go) where T : Component
	    {
		    if (go != null)
		    {
			    if (go.GetComponent<T>())
			    {
				    return go.GetComponent<T>();
			    }
			    if (go.GetComponentInParent<T>())
			    {
				    return go.GetComponentInParent<T>();
			    }
			    if (go.GetComponentInChildren<T>())
			    {
				    return go.GetComponentInChildren<T>();
			    }
		    }
		    return null;
	    }
	    
	    public static Barcode GetSpawnableBarcode(GameObject gameObject)
        {
            var assetPoolee = gameObject.GetComponent<AssetPoolee>();
            if (assetPoolee == null)
            {
                assetPoolee = gameObject.GetComponentInParent<AssetPoolee>();
                if (assetPoolee == null) assetPoolee = gameObject.GetComponentInChildren<AssetPoolee>();
            }

            if (assetPoolee == null)
            {
                var barcode = new Barcode();
                barcode._id = "empty";
                return barcode;
            }

            return assetPoolee.spawnableCrate._barcode;
        }

        public static bool IsCrate(string barcode)
        {
            GameObjectCrate gameObjectCrate =
                AssetWarehouse.Instance.GetCrate<GameObjectCrate>(barcode);
            if (gameObjectCrate == null)
            {
                return false;
            }

            return true;
        }

        public static void SpawnSpawnable(string barcode, Vector3 position, Quaternion rotation, Action<GameObject> onSpawn)
        {
	        SpawnableCrateReference spawnableCrate = new SpawnableCrateReference(barcode);
	        Spawnable spawnable = new Spawnable
	        {
		        crateRef = spawnableCrate,
		        policyData = null
	        };
	        AssetSpawner.Register(spawnable);

	        AssetSpawner.Spawn(spawnable, position, rotation, new BoxedNullable<Vector3>(null), false,
		        new BoxedNullable<int>(null), onSpawn);
        }

        public static void SpawnGameObject(string barcode, Vector3 position, Quaternion rotation,
            Action<GameObject> onSpawn)
        {
	        GameObjectCrate gameObjectCrate =
                AssetWarehouse.Instance.GetCrate<GameObjectCrate>(barcode);
	        

	        Action<GameObject> action = new Action<GameObject>(o =>
            {
                GameObject copy = GameObject.Instantiate(o);
                copy.transform.position = position;
                copy.transform.rotation = rotation;
                // Hopefully makes things removable with the spawn gun? Might cause unrelated issues.
                // AssetPoolee assetPoolee = copy.AddComponent<AssetPoolee>();
                onSpawn.Invoke(copy);
            });
            
	        gameObjectCrate.LoadAsset(action);
        }

        public static void SpawnRagdoll(string barcode, Vector3 position, Quaternion rotation, Action<RigManager> action)
        {
	        SpawnPlayerRep(position, rotation, (manager =>
	        {
		        manager.SwapAvatarCrate(barcode);
		        manager.physicsRig.RagdollRig();
		        manager.name = barcode;
		        foreach (var inventoryReceiver in manager.GetComponentsInChildren<InventorySlotReceiver>())
		        {
			        inventoryReceiver.enabled = false;
		        }
		        
		        action.Invoke(manager);
	        }));
        }

        private static void SpawnPlayerRep(Vector3 position, Quaternion rotation,
            Action<RigManager> onSpawn)
        {
            GameObjectCrate gameObjectCrate =
                AssetWarehouse.Instance.GetCrate<GameObjectCrate>("SLZ.BONELAB.Core.DefaultPlayerRig");
            Action<GameObject> action = new Action<GameObject>(obj =>
            {
                GameObject disableParent = new GameObject();
                disableParent.SetActive(false);
                
                RigManager childManager = obj.GetComponentInChildren<RigManager>();
                RigManager manager = GameObject.Instantiate<GameObject>(childManager.gameObject, disableParent.transform).GetComponent<RigManager>();
                GameObject rigManagerObject = manager.gameObject;
                
                rigManagerObject.SetActive(false);
                
                rigManagerObject.transform.position = position;
                rigManagerObject.transform.rotation = rotation;

                FixPlayerRep(manager);
                
                rigManagerObject.transform.parent = null;
                GameObject.Destroy(disableParent);
                
                rigManagerObject.SetActive(true);
                onSpawn.Invoke(manager);
            });
            gameObjectCrate.LoadAsset(action);
        }
        
        private static void FixPlayerRep(RigManager rigManager)
        {
            MelonLogger.Msg("Fixing player rep...");
			
            rigManager.physicsRig.headSfx.mouthSrc.spatialBlend = 1f;
            MelonCoroutines.Start(WaitForWind(rigManager.physicsRig.m_head.GetComponent<WindBuffetSFX>()));
			
			rigManager.bodyVitals.hasBodyLog = true;
			rigManager.bodyVitals.bodyLogEnabled = true;
			rigManager.loadAvatarFromSaveData = false;
			
			RemoveHaptics(rigManager.openControllerRig.leftController.GetComponent<Haptor>());
			RemoveHaptics(rigManager.openControllerRig.rightController.GetComponent<Haptor>());

			// Fixes hair and head meshes going invisible on the reps
			GameObject.DestroyImmediate(rigManager.GetComponent<PlayerAvatarArt>());
			
			GameObject.DestroyImmediate(rigManager.uiRig.transform.Find("DATAMANAGER").gameObject);
			rigManager.uiRig.gameObject.SetActive(false);
			rigManager.uiRig.Start();
			rigManager.uiRig.popUpMenu.radialPageView.Start();
			rigManager.uiRig.popUpMenu.Awake();
			
			// Throws an error
			try
			{
				rigManager.uiRig.popUpMenu.Start();
			}
			catch (Exception ex)
			{
				// Ignore
			}
			
			rigManager.tutorialRig.gameObject.SetActive(false);
			
			GameObject spawnGunUI = rigManager.GetComponentInChildren<SpawnGunUI>().gameObject;
			spawnGunUI.SetActive(false);
			
			rigManager.openControllerRig.isRightHanded = true;
			rigManager.openControllerRig.jumpEnabled = true;
			rigManager.openControllerRig.slowMoEnabled = false;
			rigManager.openControllerRig.primaryEnabled = true;
			rigManager.openControllerRig.quickmenuEnabled = false;
			rigManager.openControllerRig.autoLiftLegs = true;

			rigManager.openControllerRig.m_head.tag = null;

			rigManager.openControllerRig.OnLastCameraUpdate = new UnityEvent();
			rigManager.openControllerRig.cameras = new Il2CppReferenceArray<Camera>(0);

			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<AudioListener>());
			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<StreamingController>());
			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<CameraSettings>());
			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<VolumetricRendering>());
			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<VolumetricPlatformSwitch>());
			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<UniversalAdditionalCameraData>());
			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<XRLODBias>());
			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<Camera>());
			
			GameObject.DestroyImmediate(rigManager.openControllerRig.m_head.GetComponent<DebugDraw>());

			PhysHand lPhysHand = rigManager.physicsRig.leftHand.GetComponent<PhysHand>();
			PhysHand rPhysHand = rigManager.physicsRig.rightHand.GetComponent<PhysHand>();

			FixPhysHand(lPhysHand);
			FixPhysHand(rPhysHand);

			GameObject.DestroyImmediate(rigManager.openControllerRig.leftController.GetComponent<UIControllerInput>());
			GameObject.DestroyImmediate(rigManager.openControllerRig.rightController.GetComponent<UIControllerInput>());

			GameObject vignette = new GameObject("RepVignette");
			SkinnedMeshRenderer skinnedMeshRenderer = vignette.AddComponent<SkinnedMeshRenderer>();
			skinnedMeshRenderer.enabled = false;
			vignette.gameObject.SetActive(false);
			MelonCoroutines.Start(WaitAndFixPlayerHealth(rigManager.GetComponentInChildren<Player_Health>(),
				vignette));
			
			// Add all the ammo to the rep, makes us able to access the cartridge type and whatnot for every ammo.
			AmmoInventory ammoInventory = rigManager.AmmoInventory;
			ammoInventory.ammoReceiver.GetComponent<Collider>().enabled = false;
			ammoInventory.AddCartridge(ammoInventory.lightAmmoGroup, 99999);
			ammoInventory.AddCartridge(ammoInventory.heavyAmmoGroup, 99999);
			ammoInventory.AddCartridge(ammoInventory.mediumAmmoGroup, 99999);

			GameObject.DestroyImmediate(rigManager.GetComponent<ForceLevels>());
			GameObject.DestroyImmediate(rigManager.GetComponent<CheatTool>());
			GameObject.DestroyImmediate(rigManager.GetComponent<UtilitySpawnables>());
			GameObject.DestroyImmediate(rigManager.GetComponent<LineMesh>());
			GameObject.DestroyImmediate(rigManager.GetComponent<TempTextureRef>());
			GameObject.DestroyImmediate(rigManager.GetComponent<RigVolumeSettings>());
			GameObject.DestroyImmediate(rigManager.GetComponent<Volume>());
			
			RigScreenOptions rigScreenOptions = rigManager.GetComponent<RigScreenOptions>();
			GameObject.DestroyImmediate(rigScreenOptions.OverlayCam.gameObject);
			GameObject.DestroyImmediate(rigScreenOptions.cam.gameObject);
			GameObject.DestroyImmediate(rigScreenOptions);
			
        }

        private static void FixPhysHand(PhysHand hand)
        {
            hand.inventoryPlug.enabled = false;
            hand.inventoryPlug.gameObject.SetActive(false);
        }

        private static void RemoveHaptics(Haptor haptor)
        {
            haptor.hapticsAllowed = false;
            
            // I think this is all of them, jesus
            haptor.cur_sin_amp = 0;
            haptor.cur_sin_length = 0;
            haptor.hap_amplitude = 0;
            haptor.hap_calc_t = 0;
            haptor.hap_click_down_amplitude = 0;
            haptor.hap_delay = 0;
            haptor.low_thr_freq = 0f;
            haptor.hap_frequency = 0;
            haptor.hap_hit_frequency = 0;
            haptor.hap_click_down_t = 0;
            haptor.hap_click_down_frequency = 0;
            haptor.hap_click_up_t = 0;
            haptor.hap_click_up_amplitude = 0;
            haptor.hap_knock_frequency = 0;
            haptor.hap_click_up_frequency = 0;
            haptor.hap_duration = 0;
            haptor.sin_gateCount = 0;
            haptor.hap_knock_duration = 0;
            haptor.hap_tap_duration = 0;
            haptor.hap_hit_mod = 0;
            haptor.hap_tap_amplitude = 0;
            haptor.hap_tap_duration = 0;
            haptor.hap_tap_frequency = 0;
            haptor.hap_max_hardSin_amp = 0;
            haptor.hap_min_hardSin_amp = 0;
            haptor.hap_hardSin_freq = 0;
            haptor.hap_hardSin_length = 0;
            haptor.hap_subtle_amplitude = 0;
            haptor.hap_subtle_frequency = 0;
            haptor.hap_subtle_t = 0;
            haptor.hap_knock_amplitude = 0;
            haptor.hap_softSin_freq = 0;
            haptor.hap_max_softSin_amp = 0;
            haptor.hap_min_softSin_amp = 0;
            haptor.hap_softSin_length = 0;
            
            haptor.enabled = false;
        }

        private static IEnumerator WaitForWind(WindBuffetSFX sfx)
        {
	        yield return null;
	        if (sfx)
	        {
		        sfx._buffetSrc.spatialBlend = 1;
	        }
        }

        private static IEnumerator WaitAndFixPlayerHealth(Player_Health playerHealth, GameObject vignette)
        {
	        // Mods like ForeverMortal or other mortality control mods might affect the player reps health mode. So do this late.
	        yield return new WaitForSecondsRealtime(0.5f);
	        playerHealth.healthMode = Health.HealthMode.Invincible;
	        playerHealth.Vignetter = vignette;
        }
    }
}