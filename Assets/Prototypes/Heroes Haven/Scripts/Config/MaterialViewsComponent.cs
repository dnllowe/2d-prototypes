using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialViewsComponent", menuName = "Scriptable Objects/MaterialViewsComponent")]
public class MaterialViewsComponent : SerializedScriptableObject
{
    public Dictionary<ItemMaterials, MaterialViewComponent> MaterialViews = new Dictionary<ItemMaterials, MaterialViewComponent>(); 
}
