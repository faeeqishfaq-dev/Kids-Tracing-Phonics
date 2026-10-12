using UnityEngine;
using System;
using System.Collections.Generic;

namespace Game.Gameplay.ABCLearning
{
    [Serializable]
    public struct LetterData
    {
        public char Character;
        public string ExampleWord;
        public Sprite UppercaseSprite;
        public Sprite LowercaseSprite;
        public GameObject AnimatedObjectPrefab;
        public AudioClip PronunciationAudio;
        public AudioClip ExampleWordAudio;
    }

    [CreateAssetMenu(fileName = "AlphabetDatabase", menuName = "Game Data/Alphabet Database")]
    public class AlphabetDatabase : ScriptableObject
    {
        public List<LetterData> Letters = new List<LetterData>();

        public LetterData GetLetterData(int index)
        {
            if (Letters != null && index >= 0 && index < Letters.Count)
            {
                return Letters[index];
            }
            return default(LetterData);
        }

        public int GetTotalLetters()
        {
            return Letters != null ? Letters.Count : 0;
        }
    }
}
