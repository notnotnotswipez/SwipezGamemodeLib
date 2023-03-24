using HarmonyLib;
using LabFusion.Senders;
using SwipezGamemodeLib.Utilities;

namespace SwipezGamemodeLib.Patches
{
    public class SendPlayerDamagePatch
    {
        [HarmonyPatch(typeof(PlayerSender), "SendPlayerDamage")]
        public static class DamageCancelPatch
        {
            public static bool Prefix()
            {
                if (!FusionPlayerExtended.canSendDamage)
                {
                    return false;
                }

                return true;
            }
        }
    }
}