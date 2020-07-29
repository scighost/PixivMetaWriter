# PixivMetaWriter

一个把pixiv作品信息作为元数据（标签）写入图片的小工具。

**项目已并入[MyToolkit](https://github.com/scighost/MyToolkit)，本项目停止更新。**

A tool for writing metadata from pixiv information especially tag.

**The project has been merged into [MyToolkit](https://github.com/scighost/MyToolkit), and this project is no longer updated.**

简体中文 | [English](README.en.md)

### 有哪些功能

- 重命名，格式如 [Anmi] 鵜飼い [44873217_p0].jpg
- 写入元数据
  - 标题 
  - 作者（画师名）
  - 日期时间（上传时间）
  - 关键字（作品标签，包含日文原文和中文翻译）
  - 备注（作品介绍）
- 以JPEG编码方式保存

### 怎么用

双击“PixivMetaWriter.exe”，输入文件夹。

### 要注意什么

- 原文件会删除
- 文件必须以“44873217_p0.jpg”的格式命名
- 仅支持jpg和png文件
- png文件会以JPEG编码方式保存，但会复制原文件到“\png”文件夹下
- 转换失败的文件会复制到“\failure”文件夹下

### 更新记录

- **v0.1.2**  任务完成后可重复失败项
- **v0.1.1**	添加并行操作
- **v0.1.0**	首次提交

### 瞎扯淡

P 站收藏一时爽，欲寻图片忙上头。

闲来无事想整理，木有头绪难下手。

无奈成为仓鼠君，抱着图库直发愁。

P站标签非常全面，IPTC标记十分方便，Adobe Bridge特别好用，三者合一岂不美哉！

功能简陋，限制颇高；
~~单线操作，等待难熬；~~
源码难读，修改不好；
如何评价？粗制滥造！

<img src="assets/又不是不能用.jpg" alt="又不是不能用" align="left" />