// ******************************************************************
//       /\ /|       @file       ResourceManager.cs
//       \ V/        @brief      资源管理器 负责资源加载 缓存
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-08 15:55:49
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RB.Core
{
    public class ResourceManager : BaseSingleTon<ResourceManager>, IDisposable
    {
        private readonly Dictionary<string, AssetLoader> _cacheAssetLoaderDic = new Dictionary<string, AssetLoader>();

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="bundleName">bundle名称</param>
        /// <param name="assetName">资源名称</param>
        /// <param name="onLoadAssetComplete">资源加载完成回调</param>
        public void LoadAssetAsync(string bundleName, string assetName, Action<string, Object> onLoadAssetComplete)
        {
            BundleManager.Instance.LoadBundleAsync(bundleName, GetOnLoadBundleComplete(assetName, onLoadAssetComplete));
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetPath">资源完整路径</param>
        /// <param name="onLoadAssetComplete">资源加载完成回调</param>
        public void LoadAssetAsync(string assetPath, Action<string, Object> onLoadAssetComplete)
        {
            var bundleName = $"{ResourceDef.GetBundleName(assetPath)}.{ResourceDef.BundleSuffix}";
            Debug.Log($"bundle:{bundleName}");
            var assetName = ResourceDef.GetAssetName(assetPath);
            Debug.Log($"assetName:{assetName}");
            BundleManager.Instance.LoadBundleAsync(bundleName, GetOnLoadBundleComplete(assetName, onLoadAssetComplete));
        }

        /// <summary>
        /// 获取bundle加载完毕回调
        /// </summary>
        /// <param name="assetName"></param>
        /// <param name="onLoadAssetComplete"></param>
        /// <returns></returns>
        private Action<string, AssetBundle> GetOnLoadBundleComplete(string assetName,
            Action<string, Object> onLoadAssetComplete)
        {
            void OnLoadBundleComplete(string bundleName, AssetBundle assetBundle)
            {
                //有缓存的assetLoader
                if (_cacheAssetLoaderDic.ContainsKey(assetName))
                {
                    //资源已经加载完毕
                    if (_cacheAssetLoaderDic[assetName].isLoadComplete)
                    {
                        onLoadAssetComplete?.Invoke(assetName, _cacheAssetLoaderDic[assetName].GetAsset());
                        return;
                    }

                    //资源正在加载中
                    _cacheAssetLoaderDic[assetName].AddOnComplete(onLoadAssetComplete);
                    return;
                }

                //没有缓存 
                var assetLoader = new AssetLoader(assetName);
                _cacheAssetLoaderDic.Add(assetName, assetLoader);
                assetLoader.LoadAssetAsync(assetBundle, onLoadAssetComplete);
            }

            return OnLoadBundleComplete;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            foreach (var assetLoader in _cacheAssetLoaderDic.Values)
            {
                assetLoader.Dispose();
            }

            _cacheAssetLoaderDic.Clear();
            BundleManager.Instance.Dispose();
        }
    }
}