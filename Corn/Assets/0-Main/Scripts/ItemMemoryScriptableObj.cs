using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 1, fileName = "ItemMemoryList")]
public class ItemMemoryScriptableObj : ScriptableObject
{
   public List<ItemMemory>ItemMemories = new List<ItemMemory>();

   public ItemMemory GetMemoryWithName(string name)
   {
      return ItemMemories.Find(x => x.Name == name);
   }

}

[System.Serializable]
public class ItemMemory
{
   public string Name;
   public List<Sprite> MemoriesToDisplay = new List<Sprite>(); 
   
}
