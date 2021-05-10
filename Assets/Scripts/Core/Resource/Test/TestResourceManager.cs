// ******************************************************************
//       /\ /|       @file       TestResourceManager.cs
//       \ V/        @brief      资源管理器 测试用例
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-08 12:40:20
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using UnityEngine;

namespace RB.Core
{
    public class TestResourceManager : MonoBehaviour
    {
        private BundleLoader _bundleLoader;
        private const string BundleName = "scene2/prefabs.rb";
        private const string AssetName = "Building_1.Prefab";
        private const string AssetName1 = "Building_2.Prefab";
        private const string AssetName2 = "Building_3.Prefab";
        private const string AssetPath = "Assets/BundleAssets/Scene2/Prefabs/Floor/Floor.Prefab";

        /// <summary>
        /// 资源加载完成回调
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="asset"></param>
        private void OnLoadAssetComplete(string assetName, Object asset)
        {
            Instantiate(asset);
            Debug.Log($"资源加载完成:{assetName}");
        }

        private void Start()
        {
            ResourceManager.Instance.LoadAssetAsync(BundleName, AssetName, OnLoadAssetComplete);
            ResourceManager.Instance.LoadAssetAsync(BundleName, AssetName1, OnLoadAssetComplete);
            ResourceManager.Instance.LoadAssetAsync(BundleName, AssetName2, OnLoadAssetComplete);
            ResourceManager.Instance.LoadAssetAsync(AssetPath, OnLoadAssetComplete);
        }
    }
}