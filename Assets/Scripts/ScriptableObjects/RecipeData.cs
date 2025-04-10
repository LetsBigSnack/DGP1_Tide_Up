using System.Collections.Generic;
using Data;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "RecipeData", menuName = "Scriptable Objects/RecipeData")]
    public class RecipeData : ScriptableObject
    {
        public QuestItemData questItem;
        public List<TrashMaterialData> ingredients;

        public MaterialDetail GetMaterialDetail()
        {
            return new MaterialDetail(ingredients);
        }
        
    }
}
