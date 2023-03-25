using System;
using System.Collections.Generic;
using LabFusion.MarrowIntegration;
using LabFusion.Network;
using LabFusion.SDK.Gamemodes;
using MelonLoader;
using UnhollowerBaseLib.Attributes;
using SLZ.Marrow.Pool;
using SLZ.Marrow.Warehouse;

namespace SwipezGamemodeLib.SDK
{
    [RegisterTypeInIl2Cpp]
    public sealed class GamemodeResetSpawnable : FusionMarrowBehaviour
    {
        private string barcode;
        
        public GamemodeResetSpawnable(IntPtr intPtr) : base(intPtr)
        {
        }

        private void Awake()
        {
            barcode = gameObject.GetComponent<SpawnableCratePlacer>().spawnableCrateReference._barcode;
            GamemodeManager.OnGamemodeChanged += OnGamemodeChanged;
        }

        private void OnDestroy()
        {
            GamemodeManager.OnGamemodeChanged -= OnGamemodeChanged;
        }

        [HideFromIl2Cpp]
        private void OnGamemodeChanged(Gamemode gamemode) {
            if (gamemode == null) {
                if (!NetworkInfo.HasServer || NetworkInfo.IsServer) {
                    var barcodeToPool = AssetSpawner._instance._barcodeToPool;
                    foreach (var pair in barcodeToPool) {
                        if (pair.key.ToString() == barcode) {
                            var spawnedObjects = pair.value.spawned.ToArray();
                            if (spawnedObjects.Count > 0)
                            {
                                foreach (var spawned in spawnedObjects) {
                                    spawned.Despawn();
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else {
                if (NetworkInfo.IsServer)
                {
                    gameObject.GetComponent<SpawnableCratePlacer>().RePlaceSpawnable();
                }
            }
        }
    }
}