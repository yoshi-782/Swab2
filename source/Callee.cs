using System.Reflection;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace Swab2
{
    /// <summary>
    /// 呼び出し先クラス
    /// </summary>
    public class Callee
    {
        /// <summary>
        /// メインウィンドウクラス
        /// </summary>
        private readonly MainWindow MWindow;

        private bool isShow = false;

        /// <summary>
        /// Topmostの設定または取得
        /// </summary>
        public bool WinTopmost
        {
            get
            {
                return MWindow.GetTopmost();
            }
            set
            {
                MWindow.SetTopmost(value);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mw">MainWindowクラス</param>
        public Callee(MainWindow mw) => MWindow = mw;

        /// <summary>
        /// メッセージボックスの表示
        /// </summary>
        /// <param name="msg">メッセージ</param>
        /// <param name="title">タイトル</param>
        public void WinMessageBox(string msg, string title = "")
        {
            if (!isShow)
            {
                isShow = true;
                new Thread(new ThreadStart(delegate
                {
                    MessageBox.Show(msg, title.Length > 0 ? title : MWindow.HTMLTitle);
                    isShow = false;
                })).Start();
            }
        }

        /// <summary>
        /// 画面のサイズを設定する
        /// </summary>
        /// <param name="w">Width</param>
        /// <param name="h">Height</param>
        public void SetWinSize(int w, int h) => MWindow.SetWindowSize(w, h);

        /// <summary>
        /// 画面のサイズを固定にする
        /// </summary>
        /// <param name="isResize"></param>
        public void SetNotResize(bool isResize) => MWindow.SetWindowNotResize(isResize);

        /// <summary>
        /// 画面のタイトルを設定する
        /// </summary>
        /// <param name="title">タイトル</param>
        public void SetWinTitle(string title = "") => MWindow.SetWindowTitle(title);

        /// <summary>
        /// htmlファイルを読み込んで画面に表示させる
        /// </summary>
        public void LoadHTML(bool dialog = true) => MWindow.LoadHTMLFile(dialog);

        /// <summary>
        /// JsonのOpenTextEditorの値を取得
        /// </summary>
        /// <returns>OpenTextEditor</returns>
        public bool IsOpenTextEditor() => MWindow.JsonSetting.JsonProperties.IsDisplayTextEditor;

        /// <summary>
        /// JsonのTEPathの値をを取得
        /// </summary>
        /// <returns>TEPath</returns>
        public string GetTextEditorPath() => MWindow.JsonSetting.JsonProperties.EditorPath;

        /// <summary>
        /// DOSコマンド実行
        /// </summary>
        /// <param name="cmd">コマンド名</param>
        /// <param name="arg">引数</param>
        /// <returns>実行結果</returns>
        public string Command(string cmd, string arg = "")
        {
            // コマンド設定
            Process process = new Process();
            process.StartInfo.FileName = CmdPath();
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = false;
            process.StartInfo.Arguments = $"/c {cmd} {arg}";

            // コマンド実行
            process.Start();

            // 実行結果を取得
            string output = process.StandardOutput.ReadToEnd().Replace("\r\r\n", "\n");
            if (output.Where(c => c == '\n').Count() == 1)
            {
                var o = output.Split('\n');
                if (o[1].Length == 0)
                {
                    return o[0];
                }
            }
            return output;

            string CmdPath() => System.Environment.GetEnvironmentVariable("ComSpec");
        }

        /// <summary>
        /// 設定画面の時のみ実行
        /// オプションをJsonに設定
        /// </summary>
        /// <param name="isOpenTE">オプションのフラグ</param>
        /// <param name="TEPath">テキストエディタのパス</param>
        public void SetOption(bool isOpenTE, string TEPath = "")
        {
            if (MWindow.IsDuringSetting)
            {
                MWindow.JsonSetting.JsonProperties.IsDisplayTextEditor = TEPath.Length > 0 ? isOpenTE : false;
                MWindow.JsonSetting.JsonProperties.EditorPath = TEPath;
                SettingClose();
            }
        }

        /// <summary>
        /// 設定画面の時のみ実行
        /// 設定画面を閉じ、元の画面を表示
        /// </summary>
        public void SettingClose()
        {
            if (MWindow.IsDuringSetting)
            {
                MWindow.IsDuringSetting = false;
                LoadHTML(false);
            }
        }

        /// <summary>
        /// JavaScriptにBindされるすべての関数名を取得
        /// </summary>
        /// <returns>関数名配列</returns>
        public string GetAllMethodName()
        {
            MemberInfo[] members = typeof(Swab2.Callee).GetMembers(BindingFlags.Public);
            var method = "";
            MessageBox.Show(method.Length.ToString());
            foreach (var m in members)
            {
                method += m.Name.Substring(0, 1).ToLower() + m.Name.Substring(1) + m.MemberType == "Method" ? "()" : "" + "\n";
            }
            return method;
        }
    }
}
