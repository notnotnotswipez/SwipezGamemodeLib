using HarmonyLib;
using SLZ.Bonelab;

namespace SwipezGamemodeLib.Patches
{
    public class HeadTitlePatch
    {
        [HarmonyPatch(typeof(HeadTitles), nameof(HeadTitles.Start))]
        public class HeadTitlesStartPatch
        {
            public static void Postfix(HeadTitles __instance)
            {
                __instance.text_SubTitle.richText = true;
                __instance.text_Title.richText = true;
            }
        }
    }
}