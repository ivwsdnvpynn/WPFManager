﻿using System.Collections.Generic;
using System.Linq;
using static Common.UserGlobal;

namespace Common
{
    public class MainWindowsGlobal
    {
        /// <summary>
        /// 所有主窗体
        /// </summary>
        public static Dictionary<string, BaseMainWindow> MainWindowsDic = new Dictionary<string, BaseMainWindow>();

        /// <summary>
        /// 将数据加入到窗体中
        /// </summary>
        /// <param name="_windowName">窗体名、标题</param>
        /// <param name="_models">插件数据</param>
        public static void Data2MainWindow(string _windowName, List<PluginsModel> _models)
        {
            BaseMainWindow mainWindow;
            if (MainWindowsDic.ContainsKey(_windowName))
            {
                mainWindow = MainWindowsDic[_windowName];
            }
            else
            {
                mainWindow = new MainWindow(_windowName);
                MainWindowsDic.Add(_windowName, mainWindow);//保存窗体
            }
            mainWindow.AddPluginModels(_models);
            mainWindow.Show();
        }

        /// <summary>
        /// 获取所有窗体名称
        /// </summary>
        /// <returns></returns>
        public static List<string> GetWindowNames()
        {
            return MainWindowsDic.Keys.ToList();
        }

        /// <summary>
        /// 设置MainWindow Enable
        /// </summary>
        /// <param name="_windowName"></param>
        /// <param name="_enable"></param>
        public static void EnableWindow(string _windowName, bool _enable)
        {
            if (MainWindowsDic.ContainsKey(_windowName))
            {
                MainWindowsDic[_windowName].IsEnabled = _enable;
            }
        }
    }
}
