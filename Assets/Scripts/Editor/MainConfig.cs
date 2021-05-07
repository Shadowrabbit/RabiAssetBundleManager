// ******************************************************************
//       /\ /|       @file       MainConfig.cs
//       \ V/        @brief      主要配置
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-06 19:13:55
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using UnityEngine;

namespace RB
{
    public class MainConfig : ScriptableObject
    {
        public string targetFolderName; //存放打包资源的目录名
        public string assetFolder; //需要处理的资源目录
    }
}