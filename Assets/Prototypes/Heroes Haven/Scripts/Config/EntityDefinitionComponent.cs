using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityDefinitionComponent", menuName = "Scriptable Objects/EntityDefinitionComponent")]
public class EntityDefinitionComponent : SerializedScriptableObject
{
    [OdinSerialize]
    public EntityDefinition EntityDefinition;    
}
