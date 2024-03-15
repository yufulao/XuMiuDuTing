using Spine.Unity;
using UnityEngine;


namespace Yu
{
    public class CharacterEntityView : MonoBehaviour
    {
        [HideInInspector] public CharacterInfoItem infoItem;
        [HideInInspector] public EntityHUD entityHud;
        [HideInInspector] public GameObject objEntity;
        [HideInInspector] public GameObject objOutline;
        [HideInInspector] public MeshRenderer meshRenderer;

        public void Init(string characterName, CharacterInfoItem infoItemT, EntityHUD entityHudT)
        {
            infoItem = infoItemT;
            entityHud = entityHudT;
            objEntity = gameObject;
            objOutline = objEntity.transform.Find("ObjOutline").gameObject;
            meshRenderer = objEntity.GetComponent<MeshRenderer>();
        }
    }
}