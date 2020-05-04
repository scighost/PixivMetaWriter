﻿using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace PixivMetaWriter
{
    class FileOperation
    {
        /// <summary>
        /// 获取文件夹下匹配条件的文件
        /// </summary>
        /// <param name="dirPath">string Directory Path</param>
        /// <returns>FileInfo类的列表</returns>
        public static BindingList<AllInfo> GetMatchFiles(string dirPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);//获取文件夹信息
            List<FileInfo> files = new List<FileInfo>(dirInfo.GetFiles());//获取文件信息列表

            for (int i = 0; i < files.Count; i++)
            {
                FileInfo file = files[i];
                Match match = Regex.Match(file.Name, "^[0-9]+_p[0-9]+((.jpg)|(.png))$");//匹配如"44873217_p0.jpg"格式
                if (!match.Success)
                {
                    files.Remove(file);//不匹配则移除
                    i--;
                    continue;
                }
                if (!File.Exists(file.FullName))
                {
                    files.Remove(file);//移除不存在的文件
                    i--;
                    continue;
                }
            }
            BindingList<AllInfo> allInfos = new BindingList<AllInfo>();
            Parallel.ForEach(files, (file) =>
            {
                try
                {
                    AllInfo allInfo = new AllInfo(file);
                    var stream = new FileStream(file.FullName, FileMode.Open);
                    var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                    allInfo.metadata = decoder.Metadata;
                    stream.Close();
                    allInfos.Add(allInfo);
                }
                catch
                {
                    //todo 获取文件元数据
                }
            });
            return allInfos;
        }

        /// <summary>
        /// 文件操作(复制，写入元数据，重命名)
        /// </summary>
        /// <param name="file">FileInfo</param>
        /// <param name="pixivInfo">PixivInfo</param>
        /// <returns>int 0</returns>
        public static void ChangeInfo(AllInfo allInfo)
        {
            var file = allInfo.FileInfo;
            var pixivInfo = allInfo.PixivInfo;
            var metadata = allInfo.PixivInfo.metadata;
            MemoryStream ms;//创建记忆流
                            //todo 重构
            try
            {
                FileStream openStream = new FileStream(file.FullName, FileMode.Open);//创建文件流
                byte[] vs = new byte[openStream.Length];//创建临时数组
                openStream.Read(vs, 0, vs.Length);//写入数组
                openStream.Close();//关闭文件流
                ms = new MemoryStream(vs);//写入记忆流
            }
            catch (Exception ex)//读取异常
            {
                //todo 文件读取异常
                throw;
            }

            BitmapDecoder decoder = BitmapDecoder.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.Default);//解码图像
            BitmapFrame frame = decoder.Frames[0];//抽取图像帧
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();//设定编码
            encoder.QualityLevel = 100;//编码质量
            encoder.Frames.Add(BitmapFrame.Create(frame, frame.Thumbnail, metadata, frame.ColorContexts));//添加编码图像
            string newName = $"[{pixivInfo.User}] {pixivInfo.Title} [{pixivInfo.Pid_Page}].jpg";//新文件名
            string newFullName = file.DirectoryName + "\\" + newName;//新文件路径

            try
            {
                FileStream createStream = new FileStream(newFullName, FileMode.Create);//创建新文件流
                encoder.Save(createStream);//保存新文件
                createStream.Close();//关闭流
                if (file.Extension == ".png")
                {
                    Directory.CreateDirectory(file.DirectoryName + "\\png");//创建"\png"文件夹
                    file.CopyTo(file.DirectoryName + "\\png\\" + file.Name, true);//png文件复制到"\png"下
                }
            }
            catch (Exception ex)//写入异常
            {
                //todo 写入文件异常
                throw;
            }
            ms.Close();//关闭记忆流
        }
    }
}
