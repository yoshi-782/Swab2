using CefSharp;
using CefSharp.Wpf;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace Swab2
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitMenuEvent();
            InitChromiumWebBrowser();
        }

        /// <summary>
        /// アプリ名
        /// </summary>
        public string AppName { get; } = "Swab2";

        /// <summary>
        /// Webアプリのタイトル
        /// </summary>
        public string HTMLTitle { get; set; } = "";

        /// <summary>
        /// Jsonファイルの設定を読み書きするクラス
        /// </summary>
        private readonly Json json = new Json();

        /// <summary>
        /// ブラウザクラス
        /// </summary>
        public ChromiumWebBrowser browser;

        /// <summary>
        /// ウィンドウが表示された時に一度実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            LoadHTML();
        }

        /// <summary>
        /// ブラウザの初期化
        /// </summary>
        private void InitChromiumWebBrowser()
        {
            var settings = new CefSettings
            {
                AcceptLanguageList = "ja-JP",
                Locale = "ja"
            };
            Cef.Initialize(settings);

            browser = new ChromiumWebBrowser();
            CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            browser.JavascriptObjectRepository.Register("csCallee", new Callee(this), true);

            dockPanel.Children.Add(browser);
        }

        /// <summary>
        /// デフォルトの画面(タイトル、画面サイズ)を設定
        /// </summary>
        private void SetDefaultWindow()
        {
            Dispatcher.Invoke(() =>
            {
                this.HTMLTitle = "Swab2 (Untitled)";
                this.SetWindowSize(800, 450);
            });
        }

        /// <summary>
        /// 各メニューアイテムのイベントを設定
        /// </summary>
        private void InitMenuEvent()
        {
            // 新規作成
            menu_New.Click += (s, e) =>
            {
                MessageBox.Show("htmlのテンプレートファイルを出力します。", this.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
                var sfd = new SaveFileDialog()
                {
                    FileName = "index.html",
                    Filter = "htmlファイル (*.html;*.htm)|*.html;*.htm|テキストファイル (*.txt)|*.txt|すべてのファイル(*.*)|*.*",
                    Title = "テンプレートファイルの保存先を選択してください。",
                    RestoreDirectory = true
                };

                if (sfd.ShowDialog() == true)
                {
                    File.WriteAllText(sfd.FileName, Properties.Resources.Template);
                }
            };

            // 開く
            menu_Open.Click += (s, e) =>
            {
                LoadHTMLFile();
            };

            // 開き直す
            menu_Reopen.Click += (s, e) =>
            {
                LoadHTML();
            };

            // 終了
            manu_Exit.Click += (s, e) => this.Close();

            // デベロッパーツールを開く
            tool_dvTool.Click += (s, e) => browser.ShowDevTools();
        }

        /// <summary>
        /// Topmostの取得
        /// </summary>
        /// <returns>Topmost</returns>
        public bool GetTopmost() => this.Topmost;

        /// <summary>
        /// 最前面に表示させるかの設定
        /// </summary>
        /// <param name="topmost"></param>
        public void SetTopmost(bool topmost) => this.Topmost = topmost;

        /// <summary>
        /// ウィンドウのサイズを設定
        /// </summary>
        /// <param name="w">幅</param>
        /// <param name="h">高さ</param>
        public void SetWindowSize(int w, int h)
        {
            this.Dispatcher.Invoke(() =>
            {
               this.Width = w;
               this.Height = h;
            });
        }

        /// <summary>
        /// リサイズモードの設定
        /// </summary>
        /// <param name="isReSize"></param>
        public void SetWindowNotResize(bool isResize)
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.ResizeMode = isResize ? ResizeMode.CanMinimize : ResizeMode.CanResize;
            }));
        }

        /// <summary>
        /// ウィンドウのタイトルを設定
        /// </summary>
        /// <param name="title">タイトル</param>
        public void SetWindowTitle(string title = "")
        {
            this.Dispatcher.Invoke((Action)(() =>
            {
                this.Title = title.Length > 0 ? title : this.HTMLTitle;
            }));
        }

        /// <summary>
        /// htmlファイルを読み込む
        /// </summary>
        public void LoadHTMLFile()
        {
            var ofd = new OpenFileDialog()
            {
                FileName = "index.html",
                Filter = "HTMLファイル(*.html;*.htm)|*.html;*.htm|すべてのファイル(*.*)|*.*",
                Title = "開く",
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() == true)
            {
                json.SettingJson.HTMLFilePath = ofd.FileName;
                LoadHTML();
            }
        }

        /// <summary>
        /// htmlを読み込んで表示する
        /// </summary>
        /// <param name="path">htmlファイルのパス</param>
        /// <param name="init">初期設定の有無(デフォルトはなし)</param>
        private void LoadHTML()
        {
            if (json.SettingJson.HTMLFilePath.Length > 0)
            {
                browser.Load(this.json.SettingJson.HTMLFilePath);
                SetWindowNotResize(false);
            }
            else
            {
                browser.LoadHtml(Properties.Resources.index);
                SetWindowNotResize(true);
            }

            // htmlの読み込みが完了したら、JavaScriptのinit()を実行
            EventHandler<FrameLoadEndEventArgs> eventHandler = null;
            eventHandler += (object s, FrameLoadEndEventArgs e) =>
            {
                if (e.Frame.IsMain)
                {
                    Dispatcher.Invoke(() =>
                    {
                        // JavaScriptのinit()実行
                        e.Frame.EvaluateScriptAsync("init();").ContinueWith(x =>
                        {
                            if (x.Result.Message.IndexOf("init is not defined") > -1)
                            {
                                SetDefaultWindow();
                            }
                        });

                        // htmlのタイトル取得とウィンドウタイトルに反映
                        this.HTMLTitle = browser.Title;
                        SetWindowTitle();
                    });
                    
                    browser.FrameLoadEnd -= eventHandler;
                }
            };
            browser.FrameLoadEnd += eventHandler;
        }

        /// <summary>
        /// ウィンドウを閉じる時に一度実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            json.WriteJson();
        }
    }
}
