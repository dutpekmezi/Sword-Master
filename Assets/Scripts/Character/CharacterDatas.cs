using dutpekmezi;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatas", menuName = "Game/Scriptable Objects/Character/CharacterDatas")]
public class CharacterDatas : ScriptableObject
{
    public List<CharacterData> Characters;
}
