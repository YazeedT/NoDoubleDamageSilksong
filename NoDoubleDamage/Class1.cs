using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using GlobalEnums;
using HarmonyLib;
using UnityEngine;

namespace NoDoubleDamage
{
    [BepInPlugin("neko.NoDoubleDamage", "No Double Damage", "1.0.0")]
    public class NoDoubleDamage : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony("neko.NoDoubleDamage");

        void Awake()
        {
            harmony.PatchAll();
            Debug.Log("NoDoubleDamage plugin loaded");
        }

        [HarmonyPatch(typeof(HeroBox), nameof(HeroBox.TakeDamageFromDamager))]
        public static class TakeDamageFromDamager_Patch
        {
            [HarmonyPrefix]
            static void PrefixTakeDamageFromDamager(ref DamageHero damageHero, GameObject damagingObject)
            {
                // This is a Prefix patch, meaning it runs BEFORE the original method.
                // We can modify the arguments before the original method uses them.
                // Most importantly, we can modify the damageHero.damageDealt.

                if (damageHero == null || !damageHero.CanCauseDamage)
                {
                    return; // Let the original method handle the early return.
                }

                int originalDamage = damageHero.damageDealt;

                // Apply our cap: if the intended damage is greater than 1, change it to 1.
                if (originalDamage > 1)
                {
                    Debug.Log($"Capping damage from {originalDamage} to 1 from {damagingObject.name}");
                    damageHero.damageDealt = 1; // Modify the damager's value directly
                }

                // Note: The rest of the original method will now run using the potentially modified damageHero.
                // We don't need to replicate its logic.
            }
        }

    }
}
