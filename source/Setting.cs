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
        [DataMember(Name = "filePath")]
        public string HTMLFilePath { get; set; } = "";
    }

    public class Json
    {
        /// <summary>
        /// Jsonの設定クラス
        /// </summary>
        public Setting SettingJson { get; set; }

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
                serializer.WriteObject(fileStream, SettingJson);
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
                    SettingJson = serializer.ReadObject(fileStream) as Setting;
                }

                // htmlファイルが存在しない場合は、空文字を設定し、デフォルトを表示させるようにする
                if (!File.Exists(SettingJson.HTMLFilePath))
                {
                    SettingJson.HTMLFilePath = "";
                }
            }
            else
            {
                // 存在しない場合は、値なしのJsonファイルを出力
                SettingJson = new Setting();
                WriteJson();
            }
        }
    }
}
