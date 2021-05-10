// ******************************************************************
//       /\ /|       @file       MainBundleModel.cs
//       \ V/        @brief      AB包管理器主窗口数据模型
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-06 19:10:51
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using RB.Core;
using UnityEditor;
using UnityEngine;

namespace RB.Editor
{
    public class MainBundleModel
    {
        /// <summary>
        /// 主配置
        /// </summary>
        public MainConfig MainConfig { get; set; }

        /// <summary>
        /// 获取平台AB包构建目录 
        /// </summary>
        /// <returns></returns>
        public string GetPlatformFolder(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return $"{Application.dataPath}/StreamingAssets/{ResourceDef.OutBuildFolderName}/Windows";
                case BuildTarget.Android:
                    return $"{Application.dataPath}/StreamingAssets/{ResourceDef.OutBuildFolderName}/Android";
                case BuildTarget.iOS:
                    return $"{Application.dataPath}/StreamingAssets/{ResourceDef.OutBuildFolderName}/Iphone";
            }

            return string.Empty;
        }
    }
}