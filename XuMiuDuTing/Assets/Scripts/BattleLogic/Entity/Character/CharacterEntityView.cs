using Spine.Unity;
using UnityEngine;


namespace Yu
{
    public class CharacterEntityView : MonoBehaviour
    {
        [HideInInspector] public CharacterInfoItem infoItem;
        [HideInInspector] public EntityHUD entityHud;
        [HideInInspector] public GameObject objEntity;
        [HideInInspector] public SkeletonRendererCustomMaterials outlineComponent;
        [HideInInspector] public MeshRenderer meshRenderer;

        public void Init(string characterName, CharacterInfoItem infoItemT, EntityHUD entityHudT)
        {
            infoItem = infoItemT;
            entityHud = entityHudT;
            objEntity = gameObject;
            outlineComponent = objEntity.GetComponent<SkeletonRendererCustomMaterials>();
            meshRenderer = objEntity.GetComponent<MeshRenderer>();
        }
    }
}