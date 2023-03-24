using BoneLib;
using SwipezGamemodeLib.Data;
using SwipezGamemodeLib.Patches;

namespace SwipezGamemodeLib.Utilities
{
    public class FusionPlayerExtended
    {
        public static bool worldInteractable = true;
        public static bool canSendDamage = true;

        public static void SetCanDamageOthers(bool value)
        {
            canSendDamage = value;
        }

        public static void SetWorldInteractable(bool value)
        {
            worldInteractable = value;
        }

        private static void AssureStatsCapture()
        {
            if (AvatarCalculatePatch._avatarStatsCapture == null)
            {
                AvatarCalculatePatch._avatarStatsCapture = new AvatarStatsCapture();
            }
        }

        public static void SetSpeedMultiplier(float value)
        {
            AssureStatsCapture();
            AvatarCalculatePatch._avatarStatsCapture.speedMult = value;
        }
        
        public static void SetAgilityMultiplier(float value)
        {
            AssureStatsCapture();
            AvatarCalculatePatch._avatarStatsCapture.agilityMult = value;
        }
        
        public static void SetStrengthUpperMultiplier(float value)
        {
            AssureStatsCapture();
            AvatarCalculatePatch._avatarStatsCapture.strengthUpperMult = value;
        }
        
        public static void SetStrengthLowerMultiplier(float value)
        {
            AssureStatsCapture();
            AvatarCalculatePatch._avatarStatsCapture.strengthLowerMult = value;
        }

        public static void ClearModifiedStats()
        {
            AvatarCalculatePatch._avatarStatsCapture = null;
        }

        public static void RefreshAvatar()
        {
            Player.rigManager.SwapAvatarCrate(Player.rigManager.AvatarCrate._barcode);
        }
    }
}