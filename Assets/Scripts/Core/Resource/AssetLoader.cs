// ******************************************************************
//       /\ /|       @file       AssetLoader.cs
//       \ V/        @brief      资源加载器 负责一个资源的加载与卸载
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-07 19:01:41
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System;
using UnityEngine;
using UnityEngine.Assertions;
using Object = UnityEngine.Object;

namespace RB.Core
{
    public class AssetLoader : IDisposable
    {
        public bool isLoadComplete; //加载完成
        private Object _asset; //加载的资源
        private readonly string _assetName; //待加载资源的名称
        private Action<string, Object> _onLoadAssetComplete; //资源加载完成回调

        public AssetLoader(string assetName)
        {
            _assetName = assetName;
            isLoadComplete = false;
        }

        /// <summary>
        /// 获取资源
        /// </summary>
        /// <returns></returns>
        public Object GetAsset()
        {
            return _asset;
        }

        /// <summary>
        /// 添加asset加载完成事件的监听者
        /// </summary>
        /// <param name="onComplete"></param>
        public void AddOnComplete(Action<string, Object> onComplete)
        {
            _onLoadAssetComplete += onComplete;
        }

        /// <summary>   
        /// 同步加载资源
        /// </summary>
        /// <param name="assetBundle">AB包</param>
        /// <typeparam name="T">资源Obj泛型</typeparam>    
        /// <returns></returns>
        public T LoadAssetSync<T>(AssetBundle assetBundle) where T : Object
        {
            Assert.IsFalse(isLoadComplete, "已加载");
            Assert.IsNotNull(assetBundle);
            var asset = assetBundle.LoadAsset<T>(_assetName);
            Assert.IsNotNull(asset, _assetName);
            _asset = asset;
            isLoadComplete = true;
            return asset;
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetBundle">AB包</param>
        /// <param name="onLoadComplete">资源加载完成回调 string 资源名称 T 资源泛型</param>
        /// <returns></returns>
        public void LoadAssetAsync(AssetBundle assetBundle, Action<string, Object> onLoadComplete)
        {
            Assert.IsFalse(isLoadComplete, "已加载");
            Assert.IsNotNull(assetBundle);
            _onLoadAssetComplete += onLoadComplete;
            var request = assetBundle.LoadAssetAsync(_assetName);
            request.completed += OnRequestComplete;
        }

        /// <summary>
        /// request异步加载完成回调
        /// </summary>
        /// <param name="obj"></param>
        private void OnRequestComplete(AsyncOperation obj)
        {
            var request = obj as AssetBundleRequest;
            Assert.IsNotNull(request, $"加载资源失败:{_assetName}");
            _asset = request.asset;
            Assert.IsNotNull(_asset);
            isLoadComplete = true;
            _onLoadAssetComplete?.Invoke(_assetName, _asset);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            Assert.IsNotNull(_asset);
            Resources.UnloadAsset(_asset);
            _asset = null;
            isLoadComplete = false;
        }
    }
}