using Il2Cpp;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.InteropTypes;
using MelonLoader;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LinearPixieRole;

[RegisterTypeInIl2Cpp]
public class Pixie : Role
{   
    public override ActedInfo GetInfo(Character charRef)
    {
        return new ActedInfo("", null);
    }
    public override ActedInfo GetBluffInfo(Character charRef)
    {
        return new ActedInfo("", null);
    }
    public override string Description
    {
        get
        {
            return "A good Minion";
        }
    }
    public override void Act(ETriggerPhase trigger, Character charRef)
    {
        if (trigger == ETriggerPhase.Start)
        {
            charRef.statuses.statuses.Add(ECharacterStatus.HealthyBluff);
            Gameplay gameplay = Gameplay.Instance;
            Characters instance = Characters.Instance;
            Il2CppSystem.Collections.Generic.List<CharacterData> lista = gameplay.GetAscensionAllStartingCharacters();
            Il2CppSystem.Collections.Generic.List<CharacterData> listb = instance.FilterNotInDeckCharactersUnique(lista);
            Il2CppSystem.Collections.Generic.List<CharacterData> listFin = instance.FilterRealCharacterType(listb, ECharacterType.Minion);
            if (listFin == null || listFin.Count == 0)
            {
                return;
            }

            CharacterData chosenMinion = listFin[UnityEngine.Random.RandomRangeInt(0, listFin.Count)];
            gameplay.AddScriptCharacter(ECharacterType.Minion, chosenMinion);

            chosenMinion.role.Act(trigger, charRef);
        }
    }
    public Pixie() : base(ClassInjector.DerivedConstructorPointer<Pixie>())
    {
        ClassInjector.DerivedConstructorBody((Il2CppObjectBase)this);
    }
    public Pixie(IntPtr ptr) : base(ptr)
    {
    }
}