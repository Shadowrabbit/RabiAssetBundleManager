// ******************************************************************
//       /\ /|       @file       BundleLoader.cs
//       \ V/        @brief      AB包加载器 负责一个AB包的加载与卸载
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-07 19:30:20
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace RB.Core
{
    public class BundleLoader : IDisposable
    {
        public bool isLoadComplete; //加载完成
        private AssetBundle _assetBundle; //加载的AB包
        private readonly string _bundleName; //AB包名称
        private Action<string, AssetBundle> _onLoadBundleComplete; //AB包加载完成回调

        public BundleLoader(string bundleName)
        {
            isLoadComplete = false;
            _bundleName = bundleName;
        }

        /// <summary>
        /// 获取bundle
        /// </summary>
        /// <returns></returns>
        public AssetBundle GetBundle()
        {
            return _assetBundle;
        }

        /// <summary>
        /// 添加bundle加载完成事件的监听者
        /// </summary>
        /// <param name="onComplete"></param>
        public void AddOnComplete(Action<string, AssetBundle> onComplete)
        {
            _onLoadBundleComplete += onComplete;
        }

        /// <summary>
        /// 异步加载AB包
        /// </summary>
        /// <param name="onLoadComplete">bundle加载完成回调</param>
        public void LoadBundleAsync(Action<string, AssetBundle> onLoadComplete)
        {
            Assert.IsFalse(isLoadComplete, "已加载");
            //如果某个bundle的依赖包未加载完 这个bundle未开始加载 但已经注册了回调
            _onLoadBundleComplete += onLoadComplete;
            var request = AssetBundle.LoadFromFileAsync($"{ResourceDef.GetPlatformFolder()}/{_bundleName}");
            request.completed += OnRequestComplete;
        }

        /// <summary>
        /// 释放镜像
        /// </summary>
        public void Dispose()
        {
            Assert.IsNotNull(_assetBundle);
            _assetBundle.Unload(false);
            _assetBundle = null;
            isLoadComplete = false;
        }

        /// <summary>
        /// 释放镜像和实例
        /// </summary>
        public void DisposeAll()
        {
            _assetBundle.Unload(true);
        }

        /// <summary>
        /// request异步加载完成回调
        /// </summary>
        /// <param name="obj"></param>
        private void OnRequestComplete(AsyncOperation obj)
        {
            var request = obj as AssetBundleCreateRequest;
            Assert.IsNotNull(request, $"加载AB包失败:{_bundleName}");
            _assetBundle = request.assetBundle;
            Assert.IsNotNull(_assetBundle, $"加载AB包失败:{_bundleName}");
            //加载完成回调
            _onLoadBundleComplete?.Invoke(_bundleName, _assetBundle);
            isLoadComplete = true;
            Debug.Log($"bundle:{_bundleName}加载完成");
        }
    }
}