using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shop
{
    [CreateAssetMenu(fileName = "ItemVariantsDatabase", menuName = "ScriptableObjects/ItemVariantsDatabase")]
    public class ItemVariationsDatabase : ScriptableObject
    {
        public List<ItemVariant> ItemVariants = new List<ItemVariant>();
    }
    
    [Serializable]
    public class ItemVariant
    {
        public string ItemID;
        public Sprite ItemSprite;
        public GameObject Prefab;
        public Color Color;
    }
}