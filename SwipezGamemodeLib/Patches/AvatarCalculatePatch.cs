using BoneLib;
using HarmonyLib;
using SLZ.VRMK;
using SwipezGamemodeLib.Data;

namespace SwipezGamemodeLib.Patches
{
    public class AvatarCalculatePatch
    {
        public static AvatarStatsCapture _avatarStatsCapture;
        
        [HarmonyPatch(typeof(Avatar), "ComputeBaseStats")]
        public class CalculateBaseStatsPatch
        {
            public static void Postfix(Avatar __instance)
            {
                // Sometimes this method gets called while Bonelib is still picking up the manager.
                // This check prevents a null ref, but it didnt really matter anyway.
                if (Player.rigManager != null)
                {
                    if (Player.rigManager.avatar == __instance)
                    {
                        if (_avatarStatsCapture != null)
                        {
                            _avatarStatsCapture.Apply(__instance);
                        }
                    }
                }
            }
        }
    }
}