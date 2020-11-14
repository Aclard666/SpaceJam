using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class CVT_FO : MonoBehaviour, IConvertGameObjectToEntity
{
   

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var FO_Data = new CD_FO
        {
        };

        dstManager.AddComponentData(entity, FO_Data);
    }
}
