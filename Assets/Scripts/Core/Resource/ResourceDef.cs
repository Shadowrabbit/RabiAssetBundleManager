// ******************************************************************
//       /\ /|       @file       ResourceDef.cs
//       \ V/        @brief      资源相关定义类
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-08 11:46:59
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace RB.Core
{
    public static class ResourceDef
    {
        public const string OutBuildFolderName = "OutBundle"; //AB包存放目录

        public const string ManifestName = "AssetBundleManifest"; //manifest文件名称

        public const string BundleSuffix = "rb"; //AB包后缀名

        /// <summary>
        /// 获取资源相对目录
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetRelativeAssetFolder(string assetPath)
        {
            var assetFolderIndex = assetPath.IndexOf("Assets", StringComparison.Ordinal);
            Assert.AreNotEqual<int>(-1, assetFolderIndex);
            //相对Assets目录
            return assetPath.Substring(assetFolderIndex);
        }

        /// <summary>
        /// 获取AB包名称
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetBundleName(string assetPath)
        {
            var folderNameArray = GetRelativeAssetFolder(assetPath).Split('/');
            Assert.IsNotNull(folderNameArray);
            //场景资源长度会等于4
            var bundleFileName = folderNameArray.Length <= 4
                ? folderNameArray[2]
                : $"{folderNameArray[2]}/{folderNameArray[3]}";
            return $"{bundleFileName.ToLower()}";
        }

        /// <summary>
        /// 获取资源名称
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string GetAssetName(string assetPath)
        {
            var folderNameArray = GetRelativeAssetFolder(assetPath).Split('/');
            Assert.IsNotNull(folderNameArray);
            //最后一个是文件名
            return folderNameArray[folderNameArray.Length - 1];
        }

        /// <summary>
        /// 获取平台名称
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformName()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.Android:
                    return "Android";
                case RuntimePlatform.IPhonePlayer:
                    return "Iphone";
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取平台AB包构建目录
        /// </summary>
        /// <returns></returns>
        public static string GetPlatformFolder()
        {
            return $"{Application.dataPath}/StreamingAssets/{OutBuildFolderName}/{GetPlatformName()}";
        }
    }
}