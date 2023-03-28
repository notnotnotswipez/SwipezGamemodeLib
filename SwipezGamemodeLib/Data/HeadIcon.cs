using LabFusion.Extensions;
using LabFusion.Representation;
using UnityEngine;
using UnityEngine.UI;

namespace SwipezGamemodeLib.Data
{
    // This is from Fusion. 
    internal class HeadIcon
    {
        protected const float LogoDivider = 270f;

        public GameObject go;
        public Canvas canvas;
        public RawImage image;
            
        public PlayerRep rep;
            
        public HeadIcon(PlayerId id) 
        {
            go = new GameObject($"{id.SmallId} Head Icon");

            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100000;
            go.transform.localScale = Vector3.one / LogoDivider;

            image = go.AddComponent<RawImage>();

            GameObject.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.DontUnloadUnusedAsset;
                
            PlayerRepManager.TryGetPlayerRep(id, out rep);
        }
            
        public void Cleanup() 
        {
            if (go)
            {
                GameObject.Destroy(go);
            }
        }

        public void SetIcon(Texture2D texture2D)
        {
            image.texture = texture2D;
        }
        
        public void Update() 
        {
            if (rep != null) {
                var rm = rep.RigReferences.RigManager;

                if (rm) {
                    var head = rm.physicsRig.m_head;

                    go.transform.position = head.position + Vector3.up * rep.GetNametagOffset();
                    go.transform.LookAtPlayer();
                }
            }
        }
    }
}