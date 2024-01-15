using UnityEngine;

namespace SwipezGamemodeLib.Extensions
{
    public static class AssetBundleExtensioner
    {
        public static T LoadPersistentAsset<T>(this AssetBundle bundle, string name) where T : UnityEngine.Object
        {
            var asset = bundle.LoadAsset(name);

            if (asset != null)
            {
                asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
                return asset.TryCast<T>();
            }

            return null;
        }
    }
}