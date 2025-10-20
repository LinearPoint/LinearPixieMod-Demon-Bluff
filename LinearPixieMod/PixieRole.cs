// class created by github.com/carlz54339

using HarmonyLib;
using Il2CppDissolveExample;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
using MelonLoader;
using System;
using UnityEngine;
using static Il2Cpp.Interop;
using static Il2CppSystem.Array;

public static class PixieRole
{
    [HarmonyPatch(typeof(Minion), nameof(Minion.GetBluffIfAble))]
    public static class pvc
    {
        public static void Postfix(Minion __instance, ref CharacterData __result, Character charRef)
        {
            if (__result.name == "Pixie")
            {
                __result.role.BluffAct(ETriggerPhase.Start, charRef);
            }
        }
    }

    [HarmonyPatch(typeof(Baron), nameof(Baron.SitNextToOutsider))]
    public static class snto
    {
        public static void Postfix(Baron __instance, Character charRef)
        {
            if (charRef.dataRef.name == "Pixie")
            {
                Il2CppSystem.Collections.Generic.List<Character> outsiders = new Il2CppSystem.Collections.Generic.List<Character>(Gameplay.CurrentCharacters.Pointer);
                outsiders.Remove(charRef);
                outsiders = Characters.Instance.FilterCharacterType(outsiders, ECharacterType.Outcast);

                Character pickedOutsider = outsiders[UnityEngine.Random.Range(0, outsiders.Count)];
                pickedOutsider.statuses.AddStatus(ECharacterStatus.MessedUpByEvil, charRef);

                Il2CppSystem.Collections.Generic.List<Character> adjacentCharacters = Characters.Instance.GetAdjacentAliveCharacters(pickedOutsider);
                Character pickedSwapCharacter = adjacentCharacters[UnityEngine.Random.Range(0, adjacentCharacters.Count)];
                CharacterData pickedData = pickedSwapCharacter.dataRef;
                pickedSwapCharacter.Init(charRef.dataRef);
                charRef.Init(pickedData);
            }
        }
    }
}