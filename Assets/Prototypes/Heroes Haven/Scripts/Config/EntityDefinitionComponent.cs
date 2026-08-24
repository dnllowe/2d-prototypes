using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityDefinitionComponent", menuName = "Scriptable Objects/EntityDefinitionComponent")]
public class EntityDefinitionComponent : SerializedScriptableObject
{
    public EntityDefinition EntityDefinition;    
}
