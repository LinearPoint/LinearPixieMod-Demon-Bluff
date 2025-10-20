global using Il2Cpp;
using LinearPixieMod;
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

[assembly: MelonInfo(typeof(MainMod), "LinearPixieMod", "1.2", "LinearPoint")]
[assembly: MelonGame("UmiArt", "Demon Bluff")]

namespace LinearPixieMod;

public class MainMod : MelonMod
{
    public override void OnInitializeMelon()
    {
        ClassInjector.RegisterTypeInIl2Cpp<Pixie>();
    }
    public override void OnLateInitializeMelon()
    {
        CharacterData Pixie = new CharacterData();
        Pixie.role = new Pixie();
        Pixie.name = "Pixie";
        Pixie.description = "One random Minion is added to the Deck View.\nI have the abilities of an out of play Minion.";
        Pixie.flavorText = "\"She insists that all of her pranks are harmless. Yes, the poison was harmless!\"";
        Pixie.hints = "If bluffed a fake Minion is still added to the Deck View, but its abilities are not functioning.";
        Pixie.ifLies = "";
        Pixie.picking = false;
        Pixie.startingAlignment = EAlignment.Good;
        Pixie.type = ECharacterType.Outcast;
        Pixie.bluffable = true;
        Pixie.characterId = "Pixie_LP";
        Pixie.artBgColor = new Color(0.3679f, 0.2014f, 0.1541f);
        Pixie.cardBgColor = new Color(0.102f, 0.0667f, 0.0392f);
        Pixie.cardBorderColor = new Color(0.7843f, 0.6471f, 0f);
        Pixie.color = new Color(0.9659f, 1f, 0.4472f);
        Characters.Instance.startGameActOrder = InsertAfterAct("Pooka", Pixie);

        AscensionsData advancedAscension = ProjectContext.Instance.gameData.advancedAscension;
        foreach (CustomScriptData scriptData in advancedAscension.possibleScriptsData)
        {
            ScriptInfo script = scriptData.scriptInfo;
            AddRole(script.startingOutsiders, Pixie);
        }
    }
    public void AddRole(Il2CppSystem.Collections.Generic.List<CharacterData> list, CharacterData data)
    {
        if (list.Contains(data))
        {
            return;
        }
        list.Add(data);
    }
    public CharacterData[] allDatas = Array.Empty<CharacterData>();
    public override void OnUpdate()
    {
        if (allDatas.Length == 0)
        {
            var loadedCharList = Resources.FindObjectsOfTypeAll(Il2CppType.Of<CharacterData>());
            if (loadedCharList != null)
            {
                allDatas = new CharacterData[loadedCharList.Length];
                for (int i = 0; i < loadedCharList.Length; i++)
                {
                    allDatas[i] = loadedCharList[i]!.Cast<CharacterData>();
                }
            }
        }
    }
    public CharacterData[] InsertAfterAct(string previous, CharacterData data)
    {
        CharacterData[] actList = Characters.Instance.startGameActOrder;
        int actSize = actList.Length;
        CharacterData[] newActList = new CharacterData[actSize + 1];
        bool inserted = false;
        for (int i = 0; i < actSize; i++)
        {
            if (inserted)
            {
                newActList[i + 1] = actList[i];
            }
            else
            {
                newActList[i] = actList[i];
                if (actList[i].name == previous)
                {
                    newActList[i + 1] = data;
                    inserted = true;
                }
            }
        }
        if (!inserted)
        {
            LoggerInstance.Msg("");
        }
        return newActList;
    }
    // Codeblock provided by github.com/carlz54339
    public static class PixieRole
    {
        [HarmonyPatch(typeof(Minion), nameof(Minion.GetBluffIfAble))]
        [HarmonyPatch(typeof(Baron), nameof(Baron.SitNextToOutsider))]
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
        public static void Postfix(Baron __instance, Character charRef)
        {
            if(charRef.dataRef.name == "Pixie")
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