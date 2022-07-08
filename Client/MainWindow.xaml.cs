﻿
using Client.Pages;
using Client.Windows;
using Common;
using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Data.Local;
using Client.CurrGlobal;
using static Common.UserGlobal;
using CoreDBModels;
using Common.Utils;
using System.Threading;
using Client.Events;
using Common.Events;
using Client.Timers;

namespace Client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : BaseMainWindow
    {
        /// <summary>
        /// 窗体是否加载完成
        /// </summary>
        bool windowLoaded = false;

        double windowWidth = 0;//窗体宽度 用于窗体大小发生改变
        double windowHeight = 0;//窗体高度  用于窗体大小发生改变

        #region UI Models

        class SecondMenuUIModel : BaseUIModel
        {
            public int Id { get; set; }
            public string Icon { get; set; }//图标
            public string Header { get; set; }//标题
            public string LinkUrl { get; set; }//链接
            public Thickness Margin { get; set; }
            public Thickness Padding { get; set; }
            public ModulePage Tag { get; set; }//数据
            private int fontSize = 16;
            public int FontSize //字体大小
            {
                get => fontSize;
                set
                {
                    fontSize = value;
                    NotifyPropertyChanged("FontSize");
                }
            }
        }

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Closing += WindowX_Closing;

            emails.OnClosing += OnEmailClosing;

            #region 初始化窗体宽高

            windowWidth = LocalSettings.settings.WindowWidth;
            windowHeight = LocalSettings.settings.WindowHeight;

            //设置窗体宽高
            Width = windowWidth;
            Height = windowHeight;

            #endregion 

            lblCurrUser.Text = UserGlobal.CurrUser.RealName;
        }

        #region override BaseMainWindow

        /// <summary>
        /// 在窗体底部显示文字
        /// </summary>
        /// <param name="_info"></param>
        /// <param name="_color"></param>
        public override void WriteInfoOnBottom(string _info, string _color = "#000000")
        {
            lblInfo.Content = _info;
            lblInfo.Foreground = ColorHelper.ConvertToSolidColorBrush(_color);
        }

        /// <summary>
        /// 加载主导航
        /// </summary>
        public override void ReLoadMenu()
        {
            tabMenu.Items.Clear();

            int currIndex = 0;
            List<Plugins> plugins = MainWindowGlobal.CurrPlugins.OrderBy(c => c.Order).ToList();//插件排序

            foreach (var pluginsInfo in plugins)
            {
                List<PluginsModule> pluginsModules = MainWindowGlobal.CurrPluginsModules.Where(c => c.PluginsId == pluginsInfo.Id).OrderBy(c => c.Order).ToList(); //模块排序
                foreach (var moduleInfo in pluginsModules)
                {
                    moduleInfo.DLLName = pluginsInfo.DLLName;
                    TabItem _tabItem = new TabItem();
                    _tabItem.Tag = moduleInfo;
                    _tabItem.Header = moduleInfo.ModuleName;
                    _tabItem.GotFocus += _tabItem_GotFocus;
                    TabControlHelper.SetItemIcon(_tabItem, FontAwesomeCommon.GetUnicode(moduleInfo.Icon));

                    tabMenu.Items.Add(_tabItem);

                    if (currIndex == 0)
                    {
                        currIndex = 1;
                        _tabItem_GotFocus(_tabItem, null);
                    }
                }
            }

            tabMenu.SelectedIndex = 0;
        }

        /// <summary>
        /// 主导航切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _tabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            PluginsModule moduleInfo = (sender as TabItem).Tag as PluginsModule;
            List<ModulePage> _pages = MainWindowGlobal.CurrModulePages.Where(c => c.ModuleId == moduleInfo.Id).OrderBy(c => c.Order).ToList();//页面排序

            MenuData.Clear();

            if (_pages == null || _pages.Count == 0)
            {
                MessageBoxX.Show("当前模块下没有任何页面", "空值提醒");
                return;
            }

            #region 填充二级导航

            foreach (var page in _pages)
            {
                page.FullPath = $"pack://application:,,,/{moduleInfo.DLLName};component/{page.PagePath}";

                SecondMenuUIModel model = new SecondMenuUIModel();
                model.Icon = FontAwesomeCommon.GetUnicode(page.Icon);
                model.Header = page.PageName;
                model.LinkUrl = page.FullPath;
                model.Tag = page;
                model.FontSize = 16;
                model.Id = page.Id;


                MenuData.Add(model);
            }

            #endregion

        }

        /// <summary>
        /// 设置页面
        /// </summary>
        /// <param name="_s"></param>
        public override void SetFrameSource(string _s)
        {
            mainFrame.Source = new Uri(_s, UriKind.RelativeOrAbsolute);
        }

        #endregion

        ObservableCollection<SecondMenuUIModel> MenuData = new ObservableCollection<SecondMenuUIModel>();//二级导航数据源

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateTitle();

            hideTopMenusAnimation.Completed += SecondMenuAnimationCompleted;//绑定导航动画完成事件
            showTopMenusAnimation.Completed += SecondMenuAnimationCompleted;

            EmailNotReadChangedEventObserver.Instance.AddEventListener(Codes.EmailNotReadChanged, OnEmailTimer);//监听邮件事件
            new EmailTimer().Start(5000);//开始定时读取邮件

            tvMenu.ItemsSource = MenuData;

            windowLoaded = true;
        }

        private void BaseMainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            //移除事件监听
            hideTopMenusAnimation.Completed -= SecondMenuAnimationCompleted;
            showTopMenusAnimation.Completed -= SecondMenuAnimationCompleted;
            Closing -= WindowX_Closing;
        }

        #region Timer

        private void OnEmailTimer(EmailChangedMessage p)
        {
            UIGlobal.RunUIAction(() =>
            {
                if (p.HasNotReadEmail)
                {
                    bdNotReadEmailCount.Visibility = Visibility.Visible;//是否显示原点
                    bdNotReadEmailCount.IsWaving = true;//原点是否闪烁
                }
                else
                {
                    bdNotReadEmailCount.Visibility = Visibility.Collapsed;//是否显示原点
                    bdNotReadEmailCount.IsWaving = false;//原点是否闪烁
                }
            });
        }

        #endregion

        #region Private 

        /// <summary>
        /// 更新Title
        /// </summary>
        public void UpdateTitle()
        {
            headerImge.ToolTip = lblTitle.Text = Title = LocalSettings.settings.MainWindowTitle;
            lblV.Content = $"{LocalSettings.settings.CompanyName}-{LocalSettings.settings.Versions}";
        }

        #endregion

        #region UI Method

        //选择插件
        private void btnSelectPlugins_Click(object sender, RoutedEventArgs e)
        {
            this.MaskVisible(true);
            ChangePlugins changePlugins = new ChangePlugins();
            changePlugins.ShowDialog();
            this.MaskVisible(false);
        }

        //更改密码
        private void btnChangePwd_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            UpdatePassword updatePassword = new UpdatePassword();
            updatePassword.ShowDialog();
            IsMaskVisible = false;
        }

        //皮肤
        private void btnSkin_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            SkinWindow skinWindow = new SkinWindow();
            skinWindow.ShowDialog();
            IsMaskVisible = false;
        }

        //设置
        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            IsMaskVisible = true;
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
            IsMaskVisible = false;
        }

        //切换二级导航
        private void TreeView_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (tvMenu.SelectedItem != null)
            {
                SecondMenuUIModel targetItem = tvMenu.SelectedItem as SecondMenuUIModel;
                ModulePage page = targetItem.Tag as ModulePage;
                if (page == null) return;
                mainFrame.Source = new Uri(page.FullPath, UriKind.RelativeOrAbsolute);
            }
        }

        #endregion

        #region 完全关闭窗体

        private void WindowX_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            var result = MessageBoxX.Show("是否退出？", "退出提醒", this, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                windowLoaded = false;//为防止触发ReSize方法

                Closing -= WindowX_Closing;
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            Storyboard sb = new Storyboard();
            DoubleAnimation dh = new DoubleAnimation();
            DoubleAnimation dw = new DoubleAnimation();
            dh.Duration = dw.Duration = sb.Duration = new Duration(new TimeSpan(0, 0, 1));
            dh.To = dw.To = 0;
            Storyboard.SetTarget(dh, this);
            Storyboard.SetTarget(dw, this);
            Storyboard.SetTargetProperty(dh, new PropertyPath("Height", new object[] { }));
            Storyboard.SetTargetProperty(dw, new PropertyPath("Width", new object[] { }));
            sb.Children.Add(dh);
            sb.Children.Add(dw);
            sb.Completed += AnimationCompleted;
            sb.Begin();
        }

        private void AnimationCompleted(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region 二级导航动画

        bool bigMenu = true;
        static Duration duration = new Duration(TimeSpan.FromSeconds(0.2));
        bool runningAnimation = false;
        DoubleAnimation hideTopMenusAnimation = new DoubleAnimation(190, 60, duration);
        DoubleAnimation showTopMenusAnimation = new DoubleAnimation(60, 190, duration);
        ThicknessAnimation hideSecondMenuAnimation = new ThicknessAnimation(new Thickness(60, 0, 0, 0), duration);
        ThicknessAnimation showSecondMenuAnimation = new ThicknessAnimation(new Thickness(190, 0, 0, 0), duration);
        DoubleAnimation hideFontSizeMenuAnimation = new DoubleAnimation(20, duration);
        DoubleAnimation showFontSizeMenuAnimation = new DoubleAnimation(16, duration);

        /// <summary>
        /// 显示/隐藏导航
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowSecondMenus_Click(object sender, RoutedEventArgs e)
        {
            if (runningAnimation) return;//动画进行中
            runningAnimation = true;
            if (bigMenu)
            {
                bigMenu = false;
                gHeader.BeginAnimation(WidthProperty, hideTopMenusAnimation);
                gPages.BeginAnimation(MarginProperty, hideSecondMenuAnimation);
                tvMenu.BeginAnimation(FontSizeProperty, hideFontSizeMenuAnimation);
            }
            else
            {
                bigMenu = true;
                for (int i = 0; i < MenuData.Count; i++)
                {
                    MenuData[i].FontSize = 15;
                }
                gHeader.BeginAnimation(WidthProperty, showTopMenusAnimation);
                gPages.BeginAnimation(MarginProperty, showSecondMenuAnimation);
                tvMenu.BeginAnimation(FontSizeProperty, showFontSizeMenuAnimation);
            }
        }

        //动画结束
        private void SecondMenuAnimationCompleted(object sender, EventArgs e)
        {
            runningAnimation = false;
            if (!bigMenu)
            {
                //小导航完成
                for (int i = 0; i < MenuData.Count; i++)
                {
                    MenuData[i].FontSize = 20;
                }
            }
            else
            {
                //大导航完成
            }
        }


        #endregion


        #region  邮件

        /// <summary>
        /// 通知和邮件关闭
        /// </summary>
        private void OnEmailClosing()
        {
            DoubleAnimation widthAni = new DoubleAnimation();
            widthAni.From = 300;
            widthAni.To = 0;
            widthAni.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            emails.BeginAnimation(WidthProperty, widthAni);
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            if (emails.Width > 100)
            {
                OnEmailClosing();
                return;
            }
            DoubleAnimation widthAni = new DoubleAnimation();
            widthAni.From = 0;
            widthAni.To = 300;
            widthAni.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            widthAni.EasingFunction = new BackEase() { EasingMode = EasingMode.EaseIn };

            emails.BeginAnimation(WidthProperty, widthAni);
            emails.UpdateEmail();
        }

        #endregion

        #region 窗体改变事件

        double newWindowWidth = 0;
        double newWindowHeight = 0;

        /// <summary>
        /// 窗体大小更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BaseMainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (windowLoaded)
            {
                if (reSizeGrid.Visibility == Visibility.Collapsed) reSizeGrid.Visibility = Visibility.Visible;//如果隐藏 显示出来
                lblNewSizeInfo.Content = $"当前界面宽高：{e.NewSize.Width} * {e.NewSize.Height}";

                newWindowWidth = e.NewSize.Width;
                newWindowHeight = e.NewSize.Height;
            }
        }

        private void btnSaveSize_Click(object sender, RoutedEventArgs e)
        {
            reSizeGrid.Visibility = Visibility.Collapsed;//隐藏
            LocalSettings.UpdateSize(newWindowWidth, newWindowHeight);//更新本地数据
            windowWidth = newWindowWidth;
            windowHeight = newWindowHeight;
        }

        private void btnOmit_Click(object sender, RoutedEventArgs e)
        {
            reSizeGrid.Visibility = Visibility.Collapsed;//隐藏
        }

        /// <summary>
        /// 恢复设置的大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReSize_Click(object sender, RoutedEventArgs e)
        {
            Width = windowWidth;
            Height = windowHeight;
        }

        #endregion

        #region 注销

        private void btnReLogin_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        #endregion

    }
}