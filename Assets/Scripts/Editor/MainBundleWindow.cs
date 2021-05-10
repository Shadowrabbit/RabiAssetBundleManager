// ******************************************************************
//       /\ /|       @file       MainBundleWindow.cs
//       \ V/        @brief      AB包管理器主窗口
//       | "")       @author     Shadowrabbit, yingtu0401@gmail.com
//       /  |                    
//      /  \\        @Modified   2021-05-06 19:10:28
//    *(__\_\        @Copyright  Copyright (c) 2021, Shadowrabbit
// ******************************************************************

using System.IO;
using RB.Core;
using UnityEditor;
using UnityEngine;

namespace RB.Editor
{
    public class MainBundleWindow : EditorWindow
    {
        private MainBundleModel _mainBundleModel;

        private void Awake()
        {
            Init();
        }

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("资源目录", GUILayout.Width(MainBundleDef.ButtonWidth1),
                GUILayout.Height(MainBundleDef.ButtonHeight1)))
            {
                OnClickFolderSelect();
            }

            GUILayout.Label(_mainBundleModel.MainConfig.assetFolder, GUILayout.Height(MainBundleDef.ButtonHeight1));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("自动标记", GUILayout.Width(MainBundleDef.ButtonWidth1),
                GUILayout.Height(MainBundleDef.ButtonHeight1)))
            {
                OnClickAutoSetTag();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Build Win", GUILayout.Width(MainBundleDef.ButtonWidth2),
                GUILayout.Height(MainBundleDef.ButtonHeight1)))
            {
                OnClickBuildWinBundles();
            }

            if (GUILayout.Button("Build Android", GUILayout.Width(MainBundleDef.ButtonWidth2),
                GUILayout.Height(MainBundleDef.ButtonHeight1)))
            {
                OnClickBuildAndroidBundles();
            }

            if (GUILayout.Button("Build IOS", GUILayout.Width(MainBundleDef.ButtonWidth2),
                GUILayout.Height(MainBundleDef.ButtonHeight1)))
            {
                OnClickBuildIosBundles();
            }

            GUILayout.EndHorizontal();
        }

        [MenuItem("Rabi/AssetBundleManager/Open MainWindow &[")]
        private static void ShowWindow()
        {
            GetWindow<MainBundleWindow>();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Init()
        {
            _mainBundleModel = new MainBundleModel();
            CheckConfig();
        }

        /// <summary>
        /// 配置文件检测
        /// </summary>
        private void CheckConfig()
        {
            var mainConfig = AssetDatabase.LoadAssetAtPath<MainConfig>(MainBundleDef.ConfigPath);
            //文件未创建
            if (mainConfig == null)
            {
                Debug.LogWarning($"配置文件不存在:{MainBundleDef.ConfigPath}");
                //检查保存路径
                if (!Directory.Exists(MainBundleDef.ConfigFolder))
                {
                    Directory.CreateDirectory(MainBundleDef.ConfigFolder);
                }

                //创建资源实例
                mainConfig = CreateInstance<MainConfig>();
                //创建资源
                AssetDatabase.CreateAsset(mainConfig, MainBundleDef.ConfigPath);
                AssetDatabase.Refresh();
                Debug.Log($"配置文件已创建:{MainBundleDef.ConfigPath}");
            }

            //设置配置文件
            _mainBundleModel.MainConfig = mainConfig;
        }

        /// <summary>
        /// 自动设置标记
        /// </summary>
        /// <param name="rootFileSystemInfo">函数当前的根目录</param>
        private void AutoSetTag(FileSystemInfo rootFileSystemInfo)
        {
            if (!rootFileSystemInfo.Exists)
            {
                Debug.LogError($"文件不存在:{rootFileSystemInfo.FullName}");
                return;
            }

            //判断实例类型
            if (!(rootFileSystemInfo is DirectoryInfo rootDirectoryInfo))
            {
                Debug.LogError($"不是目录文件:{rootFileSystemInfo.FullName}");
                return;
            }

            //当前目录下的文件系统信息
            var fileSystemInfos = rootDirectoryInfo.GetFileSystemInfos();
            foreach (var fileSystemInfo in fileSystemInfos)
            {
                //当前文件系统信息类别为文件
                if (fileSystemInfo is FileInfo fileInfo)
                {
                    //不是meta文件
                    if (!fileInfo.Extension.Equals(".meta"))
                    {
                        //修改AB标签
                        SetTag(fileInfo);
                    }

                    continue;
                }

                //子目录递归处理
                AutoSetTag(fileSystemInfo);
            }
        }

        /// <summary>
        /// 设置资源文件的AB包标签
        /// </summary>
        /// <param name="fileInfo"></param>
        private void SetTag(FileSystemInfo fileInfo)
        {
            if (fileInfo == null)
            {
                Debug.LogError("文件不存在");
                return;
            }

            var filePath = fileInfo.FullName.Replace("\\", "/");
            //相对Assets路径
            var relativeAssetFolder = ResourceDef.GetRelativeAssetFolder(filePath);
            var bundleName = ResourceDef.GetBundleName(filePath);
            //Debug.Log($"文件:{fileInfo.FullName} 包名:{bundleName}");
            var assetImporter = AssetImporter.GetAtPath(relativeAssetFolder);
            assetImporter.assetBundleName = bundleName;
            assetImporter.assetBundleVariant = ResourceDef.BundleSuffix;
        }


        private void BuildBundles(BuildTarget buildTarget)
        {
            var outPath = _mainBundleModel.GetPlatformFolder(buildTarget);
            //目录存在 删除
            if (Directory.Exists(outPath))
            {
                Directory.Delete(outPath, true);
            }
            //目录不存在 创建
            else
            {
                Directory.CreateDirectory(outPath);
            }

            BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, buildTarget);
            AssetDatabase.Refresh();
            Debug.Log("打包完成");
        }

        /// <summary>   
        /// 目录选择点击回调
        /// </summary>
        private void OnClickFolderSelect()
        {
            _mainBundleModel.MainConfig.assetFolder =
                EditorUtility.SaveFolderPanel("目录选择", _mainBundleModel.MainConfig.assetFolder, "");
            EditorUtility.SetDirty(_mainBundleModel.MainConfig);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 设置标记按钮点击回调
        /// </summary>
        private void OnClickAutoSetTag()
        {
            //清除原标记
            AssetDatabase.RemoveUnusedAssetBundleNames();
            var rootDirectoryInfo = new DirectoryInfo(_mainBundleModel.MainConfig.assetFolder);
            Debug.Log($"待处理资源根目录:{rootDirectoryInfo}");
            AutoSetTag(rootDirectoryInfo);
            Debug.Log("bundle标签设置完成");
        }

        /// <summary>
        /// 构建AB包
        /// </summary>
        private void OnClickBuildWinBundles()
        {
            BuildBundles(BuildTarget.StandaloneWindows64);
        }

        /// <summary>
        /// 构建AB包
        /// </summary>
        private void OnClickBuildAndroidBundles()
        {
            BuildBundles(BuildTarget.Android);
        }

        /// <summary>
        /// 构建AB包
        /// </summary>
        private void OnClickBuildIosBundles()
        {
            BuildBundles(BuildTarget.iOS);
        }
    }
}