using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Swab2
{
    [DataContract]
    public class Setting
    {
        /// <summary>
        /// htmlファイルのパス
        /// </summary>
        [DataMember(Name = "dirPath")]
        public string HTMLDirPath { get; set; } = "";

        /// <summary>
        /// htmlファイル名
        /// </summary>
        [DataMember(Name = "fileName")]
        public string HTMLFileName { get; set; } = "";

        [DataMember(Name = "openTextEditor")]
        public bool OpenTextEditor { get; set; } = false;

        [DataMember(Name = "TextEditor_Path")]
        public string TEPath { get; set; } = "";
    }

    public class Json
    {
        /// <summary>
        /// Jsonの設定クラス
        /// </summary>
        public Setting JsonProperties { get; set; }

        /// <summary>
        /// Jsonのファイル名
        /// </summary>
        private const string fileName = "setting.json";

        /// <summary>
        /// Jsonのファイルパス
        /// </summary>
        private readonly string filePath = Directory.GetParent(Assembly.GetExecutingAssembly().Location) + $"/{fileName}";

        /// <summary>
        /// コンストラクタ
        /// Jsonを読み込む
        /// </summary>
        public Json()
        {
            ReadJson();
        }

        /// <summary>
        /// Jsonファイルを出力する
        /// </summary>
        public void WriteJson()
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var serializer = new DataContractJsonSerializer(typeof(Setting));
                serializer.WriteObject(fileStream, JsonProperties);
            }
        }

        /// <summary>
        /// Jsonファイルを読み込む
        /// </summary>
        public void ReadJson()
        {
            if (File.Exists(filePath) == true)
            {
                using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    var serializer = new DataContractJsonSerializer(typeof(Setting));
                    JsonProperties = serializer.ReadObject(fileStream) as Setting;
                }

                // htmlファイルが存在しない場合は、空文字を設定し、デフォルトを表示させるようにする
                if (!Directory.Exists(JsonProperties.HTMLDirPath))
                {
                    JsonProperties.HTMLDirPath = "";
                }

                // テキストエディタのパスが設定されていない場合は、テキストエディタを開かない
                if (JsonProperties.TEPath.Length == 0)
                {
                    JsonProperties.OpenTextEditor = false;
                }
            }
            else
            {
                // 存在しない場合は、値なしのJsonファイルを出力
                JsonProperties = new Setting();
                WriteJson();
            }
        }

        /// <summary>
        /// htmlファイルの場所を一括設定
        /// </summary>
        /// <param name="filePath">ディレクトリパス</param>
        public void SetHTMLFilePath(string filePath)
        {
            this.JsonProperties.HTMLDirPath = Path.GetDirectoryName(filePath);
            this.JsonProperties.HTMLFileName = Path.GetFileName(filePath);
        }
    }
}
