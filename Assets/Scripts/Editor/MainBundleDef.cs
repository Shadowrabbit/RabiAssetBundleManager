// ******************************************************************
//       /\ /|       @file       MainBundleDef.cs
//       \ V/        @brief      AB包相关定义
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-06 19:12:32
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

namespace RB.Editor
{   
    public class MainBundleDef : BaseSingleTon<MainBundleDef>
    {
        public const string ConfigFolder = "Assets/Scripts/Editor/Assets/"; //设置文件储存目录
        public const string ConfigPath = "Assets/Scripts/Editor/Assets/MainConfig.asset"; //设置资源路径
        public const float ButtonWidth1 = 75f; //按钮宽度
        public const float ButtonWidth2 = 120f; //按钮宽度
        public const float ButtonHeight1 = 40f; //按钮高度
    }
}   