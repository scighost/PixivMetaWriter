using System.Threading;
using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.IO;
using PixivMetaWriter;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintIntro();//软件介绍
            string  dirPath = ScanDirpath();//获取文件夹路径
            string retry = "y";//重复失败项
            for (int repeatTime = 0; retry == "y" || retry == "Y"; repeatTime++)
            {
                Console.CursorVisible = false;//隐藏光标，好看
                List<FileInfo> files;
                if (repeatTime == 0)
                {
                    files = FileOperation.GetMatchFiles(dirPath);//获取匹配条件的文件列表
                    Console.WriteLine("匹配到符合条件的文件{0}个，开始写入信息……", files.Count);
                }
                else
                {
                    files = FileOperation.GetMatchFiles(dirPath+"failure");//获取匹配条件的文件列表
                    Console.WriteLine("第{0}次重复……", repeatTime);
                }
                int progressNum = 0;//任务进度
                int successNum = 0;//成功数
                Parallel.ForEach(files, (file) =>
                {
                    Match match = Regex.Match(file.Name, "[0-9]+_p[0-9]+");//匹配 pid_page
                    string pid_page = match.Value;
                    try
                    {
                        PixivInfo pixivInfo = new PixivInfo(pid_page);//获取作品信息
                        FileOperation.ChangeInfo(file, pixivInfo,dirPath);//文件操作
                        successNum++;//成功
                    }
                    catch (Exception ex)
                    {

                        Directory.CreateDirectory(dirPath+"failure");//创建"\failure"文件夹
                        if (ex.HResult == -2146233079)
                        {
                            file.CopyTo(dirPath + $"[!404] [{pid_page}]" + file.Extension, true);//404
                        }
                        if (repeatTime==0)
                        {
                            file.CopyTo(dirPath + "failure\\" + file.Name, true);//写入失败的文件复制到"\failure"下
                            file.Delete();
                        }
                        
                    }
                    progressNum++;
                    PrintProgress(progressNum, files.Count);
                });

                Console.SetCursorPosition(0, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.CursorVisible = true;
                if ((files.Count - successNum) == 0)
                {
                    retry = "n";
                    Console.WriteLine("任务完成，任意键退出");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("失败文件{0}个已复制到\"failure\"文件夹下，是否重试(y/n)", files.Count - successNum);
                    Console.ForegroundColor = ConsoleColor.Black;
                    retry = Console.ReadLine();
                    if (retry != "y" && retry != "Y" && retry != "n" && retry != "N")
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine("除 y/n 之外的值也是退出哦");
                        Console.ForegroundColor = ConsoleColor.Black;
                        retry = Console.ReadLine();
                    }
                }
            }
        }

        private static void PrintProgress(int progressNum, int count)
        {
            Console.SetCursorPosition(0, Console.CursorTop);//光标位置
            Console.ForegroundColor = ConsoleColor.DarkBlue;//文字颜色
            Console.Write(new string(' ', $" {progressNum} / {count}".Length * 3));//空白字符
            Console.SetCursorPosition(0, Console.CursorTop);//返回行首
            Console.Write(" {0} / {1}", progressNum, count);//显示进度
        }

        //程序介绍
        private static void PrintIntro()
        {
            string intro = "PixivMetaWriter\nVersion 0.1.2    Author:Scighost\n\n";
            //intro += "本程序功能简陋，限制颇高，单线操作，等待难熬，源码难读，修改不好，如何评价？粗制滥造！\n\n";
            intro += "文件名必须以“pid_page”命名，如“44873217_p0.jpg”；只支持jpg和png格式；";
            //intro += "程序将写入以下元数据：{标题(作品名)，作者(画师名)，日期时间(上传时间)，关键字(作品标签)，备注(作品介绍)}，作品标签包含日文原文和中文翻译；";
            intro += "原文件将会删除，新文件以JPEG编码保存在相同目录下，命名格式：[user] title [pid_page].jpg，如[Anmi] 鵜飼い [44873217_p0].jpg。\n";
            intro += "png格式的文件会复制到\\png下。\n\n";
            intro += "如果您觉得可以接受的话，那么请在下面输入文件夹的路径；不能接受，回车退出。\n";
            intro += "PS. 系统代理，梯子自备。\n";
            Console.WriteLine(intro);
        }

        //获取文件夹路径
        static string ScanDirpath()
        {
            string dirPath = Console.ReadLine();

            for (int i = 1; !Directory.Exists(dirPath); i++)//文件夹不存在则进行循环
            {
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                if (dirPath == "")
                {
                    i = 5;//输入回车退出
                }
                switch (i)
                {
                    case 1:
                        Console.WriteLine("手抖了一下，输错了？");
                        break;
                    case 2:
                        Console.WriteLine("要不仔细看看？");
                        break;
                    case 3:
                        Console.WriteLine("眼睛花了吗，再认真看看？");
                        break;
                    case 4:
                        Console.WriteLine(@"你到底行不行啊！(╯‵□′)╯︵┻━┻");
                        break;
                    case 5:
                        Console.WriteLine("_(:_ ∠)_ emmmmm");
                        Thread.Sleep(2000);
                        Console.WriteLine("告辞！");
                        Thread.Sleep(1000);
                        Environment.Exit(0);
                        break;
                }
                Console.ForegroundColor = ConsoleColor.Black;
                dirPath = Console.ReadLine();
            }
            dirPath = dirPath.Substring(dirPath.Length-1,1) == "\\" ? dirPath : dirPath + "\\";
            return dirPath;
        }
    }
}