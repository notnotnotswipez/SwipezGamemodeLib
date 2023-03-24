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