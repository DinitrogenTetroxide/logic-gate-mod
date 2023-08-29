using HarmonyLib;
using SFS.Parts;
using Cysharp.Threading.Tasks;
using LogicGateInjector;
using SFS;

namespace LogicGateModules
{
    public class Patches
    {
        [HarmonyPatch(typeof(CustomAssetsLoader), "LoadAllCustomAssets")]
        public static class CALPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref UniTask __result) 
            {
                __result.GetAwaiter().OnCompleted(() => 
                {
                    ExternalModule exMod;

                    foreach (Part part in Base.partsLoader.parts.Values)
                    {
                        exMod = part.GetComponentInChildren<ExternalModule>();
                        if (exMod)
                        {
                            switch (exMod.moduleType)
                            {
                                case 0:
                                    break;
                                case 1:
                                    var am = exMod.GetOrAddComponent<AltimeterModule>();
                                    am.vars = exMod.vars;
                                    break;
                                case 2:
                                    var em = exMod.GetOrAddComponent<LogicEngineModule>();
                                    em.vars = exMod.vars;
                                    break;
                            }
                        }
                    }
                });
            }
        }
    }
}
