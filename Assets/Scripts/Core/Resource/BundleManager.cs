// ******************************************************************
//       /\ /|       @file       BundleManager.cs
//       \ V/        @brief      AB包管理器 负责bundle多重加载与缓存
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-08 18:39:40
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System;
using System.Collections.Generic;
using UnityEngine;

namespace RB.Core
{
    public class BundleManager : BaseSingleTon<BundleManager>, IDisposable
    {
        private readonly Dictionary<string, int>
            _subBundleToBeLoadCountDic = new Dictionary<string, int>(); //各个bundle待加载的依赖包数量

        private readonly Dictionary<string, BundleLoader>
            _cacheBundleLoaderDic = new Dictionary<string, BundleLoader>(); //缓存的bundleLoader

        private AssetBundleManifest _assetBundleManifest; //清单实例

        /// <summary>
        /// 异步加载一个可能包含依赖的AB包
        /// </summary>
        /// <param name="bundleName">AB包名称</param>
        /// <param name="onBundleLoadComplete">加载完成回调</param>
        /// <returns></returns>
        public void LoadBundleAsync(string bundleName, Action<string, AssetBundle> onBundleLoadComplete)
        {
            if (_assetBundleManifest == null)
            {
                LoadManifest();
            }

            //从缓存中查找对应的bundleLoader bundleLoader存在
            if (_cacheBundleLoaderDic.ContainsKey(bundleName))
            {
                //bundle已加载完成 回调onBundleLoadComplete
                if (_cacheBundleLoaderDic[bundleName].isLoadComplete)
                {
                    onBundleLoadComplete?.Invoke(bundleName, _cacheBundleLoaderDic[bundleName].GetBundle());
                    return;
                }

                //bundle加载中 为bundleLoader加载完成注册回调
                _cacheBundleLoaderDic[bundleName].AddOnComplete(onBundleLoadComplete);
                return;
            }

            //bundleLoader不存在 创建loader
            var bundleLoader = new BundleLoader(bundleName);
            _cacheBundleLoaderDic.Add(bundleName, bundleLoader);

            //设置bundle加载的依赖包数量
            var subBundleNameArray = _assetBundleManifest.GetAllDependencies(bundleName);
            _subBundleToBeLoadCountDic.Add(bundleName, subBundleNameArray.Length);
            //没有依赖包 直接加载
            if (subBundleNameArray.Length == 0)
            {
                bundleLoader.LoadBundleAsync(onBundleLoadComplete);
                return;
            }

            //递归加载当前bundle的依赖包 并为依赖包加载完成注册回调
            foreach (var subBundleName in subBundleNameArray)
            {
                LoadBundleAsync(subBundleName,
                    GetOnDependencyBundleLoadComplete(bundleName, onBundleLoadComplete));
            }
        }

        /// <summary>
        /// 同步加载manifest清单
        /// </summary>  
        private void LoadManifest()
        {
            var manifestPath = $"{ResourceDef.GetPlatformFolder()}/{ResourceDef.GetPlatformName()}";
            var request = AssetBundle.LoadFromFileAsync(manifestPath);
            var maniBundle = request.assetBundle;
            _assetBundleManifest = maniBundle.LoadAsset<AssetBundleManifest>(ResourceDef.ManifestName);
        }

        /// <summary>
        /// 依赖包加载完成回调
        /// </summary>
        /// <param name="onMasterBundleComplete">被引用包加载完成回调</param>
        /// <param name="masterBundleName">被引用包名称</param>
        private Action<string, AssetBundle> GetOnDependencyBundleLoadComplete(
            string masterBundleName, Action<string, AssetBundle> onMasterBundleComplete)
        {
            void OnDependencyBundleLoadComplete(string bundleName, AssetBundle assetBundle)
            {
                //检测依赖包是否全部加载完成
                _subBundleToBeLoadCountDic[masterBundleName]--;
                if (_subBundleToBeLoadCountDic[masterBundleName] > 0)
                {
                    return;
                }

                _cacheBundleLoaderDic[masterBundleName].LoadBundleAsync(onMasterBundleComplete);
            }

            return OnDependencyBundleLoadComplete;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            foreach (var bundleLoader in _cacheBundleLoaderDic.Values)
            {
                bundleLoader.Dispose();
            }

            _subBundleToBeLoadCountDic.Clear();
            _cacheBundleLoaderDic.Clear();
        }
    }
}