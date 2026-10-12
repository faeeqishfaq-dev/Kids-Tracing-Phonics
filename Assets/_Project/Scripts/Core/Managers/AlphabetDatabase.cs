using Game.Gameplay.ABCLearning;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LetterData
{
    public string letterName;

    public Sprite letterSprite;

    public Sprite maskSprite;

    public Sprite[] guideSprites;
}

[CreateAssetMenu(menuName = "Game/Alphabet Database")]
public class AlphabetDatabase : ScriptableObject
{
    public List<LetterData> letters = new();
}