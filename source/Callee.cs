using System.Reflection;
using System.Windows;

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
            MessageBox.Show(msg, title.Length > 0 ? title : MWindow.HTMLTitle);
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
        public void LoadHTML() => MWindow.LoadHTMLFile();

        /// <summary>
        /// JavaScriptにBindされるすべての関数名を取得
        /// </summary>
        /// <returns>関数名配列</returns>
        //public string GetAllMethodName()
        //{
        //    MemberInfo[] members = typeof(Swab2.Callee).GetMembers(BindingFlags.Public);
        //    var method = "";
        //    MessageBox.Show(method.Length.ToString());
        //    foreach (var m in members)
        //    {
        //        method += m.Name.Substring(0, 1).ToLower() + m.Name.Substring(1) + m.MemberType == "Method" ? "()" : "" + "\n";
        //    }
        //    MessageBox.Show(method);
        //    return method;
        //}
    }
}
