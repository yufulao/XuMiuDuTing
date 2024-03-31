using Spine.Unity;
using UnityEngine;


namespace Yu
{
    public class EnemyEntityView : MonoBehaviour
    {
        [HideInInspector] public EnemyInfoItem infoItem;
        [HideInInspector] public EntityHUD entityHud;
        [HideInInspector] public GameObject objEntity;
        [HideInInspector] public GameObject objOutline;
        [HideInInspector] public MeshRenderer meshRenderer;

        public void Init(string enemyName, EnemyInfoItem infoItemT, EntityHUD entityHudT)
        {
            infoItem = infoItemT;
            entityHud = entityHudT;
            objEntity = gameObject;
            entityHud.uiHangAnimationComponent.RegisterAnimator(infoItem.animatorSelectedBg);
            objOutline = objEntity.transform.Find("ObjOutline").gameObject;
            meshRenderer = objEntity.GetComponent<MeshRenderer>();
        }
        
        
    }
}
