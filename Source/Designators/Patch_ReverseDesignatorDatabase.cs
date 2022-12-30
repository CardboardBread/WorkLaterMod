using HarmonyLib;
using Verse;

namespace CardboardBread.WorkLater.Designators
{
    // Ensure ReverseDesignatorDatabase can provide our custom designators.
    [HarmonyPatch(typeof(ReverseDesignatorDatabase), "InitDesignators")]
    public static class Patch_ReverseDesignatorDatabase
    {
        [HarmonyPostfix]
        public static void InitDesignators_Postfix(ReverseDesignatorDatabase __instance)
        {
            var des = __instance.AllDesignators; // This doesn't loop forever because desList is notnull at the beginning of InitDesignators()
            // for every mod loaded, get all their designators and add a new instance.
            des.Add(new Designator_SlaughterFullyGrown());
            des.Add(new Designator_SlaughterWhenGrown());
        }
    }
}
