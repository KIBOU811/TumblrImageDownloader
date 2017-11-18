using System;

namespace TumblrImageDownloader
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("画像をダウンロードするTumblrアカウントのID");
            Console.Write("-> ");
            string id = Console.ReadLine();
            string url = $"http://{id}.tumblr.com";

            WebClass web = new WebClass(id);
            if (!web.IsWebPageExist(url))
            {
                Console.WriteLine("ページが存在しません");
                return;
            }
            Console.WriteLine("ページは存在します");
            Console.WriteLine("画像のダウンロードを開始します");

            int count = web.GetPostCount($"{url}/api/read/");
            Console.WriteLine($"投稿数 : {count}");
            int i = new int();
            while (i < count)
            {
                Console.WriteLine($"{i + 1}から{i + 20}件目の画像を取得中");
                string apiUrl = $"{url}/api/read?start={i}";
                web.SearchPicture(apiUrl);
                i += 20;
            }
        }
    }
}
