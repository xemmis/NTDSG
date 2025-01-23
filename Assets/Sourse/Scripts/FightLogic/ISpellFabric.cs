using System;
using UnityEngine;

public interface ISpellFabric
{
    public ISpell SpelInit(string spellName, Transform fireFlyPos);
}

