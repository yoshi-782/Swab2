using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Swab2
{
    /// <summary>
    /// Webサーバーを起動させるクラス
    /// </summary>
    public class WebServer
    {
        /// <summary>
        /// htmlファイルがあるパス
        /// </summary>
        public string HTMLDirPath { get; set; }

        /// <summary>
        /// ブラウザで表示するURL
        /// </summary>
        public string Url { get; } = "http://localhost:8000/";

        /// <summary>
        /// Webサーバーが起動しているかどうか
        /// </summary>
        public bool IsRun { get; set; } = false;

        /// <summary>
        /// Pathが設定されているかどうか
        /// </summary>
        public bool IsPathSetting => this.HTMLDirPath.Length > 0 ? true : false;

        /// <summary>
        /// エラーイベント
        /// </summary>
        public event ErrorEventHandler ErrorHandler;

        /// <summary>
        /// イベントハンドラ用デリゲート
        /// </summary>
        /// <param name="e">例外クラス</param>
        public delegate void ErrorEventHandler(ErrorEventArgs e);

        /// <summary>
        /// HttpListenerクラス
        /// </summary>
        private readonly HttpListener listener = new HttpListener();

        /// <summary>
        /// Webサーバーの開始
        /// </summary>
        public void Start()
        {
            listener.Prefixes.Add(Url);
            listener.Start();
            this.IsRun = true;

            // タスクで管理
            var task = new Task(() =>
            {
                while (true)
                {
                    HttpListenerContext conText = listener.GetContext();
                    HttpListenerRequest req = conText.Request;
                    HttpListenerResponse res = conText.Response;
                    string path = HTMLDirPath + req.RawUrl.Replace("/", "\\");
                    
                    try
                    {
                        if (File.Exists(path))
                        {
                            res.StatusCode = 200;
                            byte[] content = File.ReadAllBytes(path);
                            res.OutputStream.Write(content, 0, content.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        // エラー
                        res.StatusCode = 500;
                        byte[] content = Encoding.UTF8.GetBytes(ex.Message);
                        res.OutputStream.Write(content, 0, content.Length);
                        ErrorHandler(new ErrorEventArgs(ex));
                        
                        // Pathが設定されていなければ、Startをキャンセル
                        if (!this.IsPathSetting)
                        {
                            break;
                        }
                    }
                    res.Close();
                }
            });
            task.Start();
        }

        /// <summary>
        /// Webサーバーの停止
        /// </summary>
        public void Stop()
        {
            listener.Stop();
            this.IsRun = false;
        }

        /// <summary>
        /// エラーイベントハンドラー用クラス
        /// </summary>
        public class ErrorEventArgs : EventArgs
        {
            /// <summary>
            /// 例外クラス
            /// </summary>
            private readonly Exception _ex;

            /// <summary>
            /// 例外クラス
            /// </summary>
            public Exception Ex { get { return _ex; } }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="ex">例外クラス</param>
            public ErrorEventArgs(Exception ex)
            {
                _ex = ex;
            }
        }
    }
}
