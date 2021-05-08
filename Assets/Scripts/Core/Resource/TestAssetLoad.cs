// ******************************************************************
//       /\ /|       @file       Test.cs
//       \ V/        @brief      
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-08 12:40:20
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using UnityEngine;

namespace RB.Core
{
    public class Test : MonoBehaviour
    {
        private BundleLoader _bundleLoader;
        private const string BundleName = "scene2/prefabs.rb";
        private const string AssetName = "Building_1.Prefab";

        private void Start()
        {
            _bundleLoader = new BundleLoader(BundleName, OnLoadComplete);
            StartCoroutine(_bundleLoader.LoadAssetBundle());
        }

        /// <summary>
        /// 加载完成回调
        /// </summary>
        private void OnLoadComplete(string bundleName)
        {
            var obj = _bundleLoader.LoadAsset<Object>(AssetName, true);
            Instantiate(obj);
            Debug.Log($"bundleName:{bundleName}");
        }
    }
}