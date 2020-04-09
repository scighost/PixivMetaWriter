using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;

class PixivInfo
{
    public string Pid_Page;//文件名
    public string User;//画师名
    public string Uid;//画师ID
    public string Title;//作品标题
    public string Pid;//作品ID
    public string Page;//图片序号，从0开始计数
    public string Date;//作品创建日期
    public string Type;//作品类型
    public string Intro;//作品介绍
    public int PageCount;//图片数量
    public int BookmarkCount;//收藏
    public int LikeCount;//点赞
    public int CommentCount;//评论
    public int ResponseCount;//回复
    public int ViewCount;//浏览量
    public List<string> Tags;//标签列表

    /// <summary>
    /// 构造函数，获取作品信息
    /// </summary>
    /// <param name="Pid_Page">文件名，如44873217_p0</param>
    public PixivInfo(string Pid_Page)
    {
        this.Pid_Page = Pid_Page;
        this.Pid = Pid_Page.Substring(0, Pid_Page.IndexOf('_'));//以"_"分割Pid和Page
        this.Page = Pid_Page.Substring(Pid_Page.IndexOf("_") + 1);
        string url = "https://www.pixiv.net/artworks/" + this.Pid;
        string html;

        try
        {
            WebRequest request = WebRequest.Create(url);            //实例化WebRequest对象  
            request.Headers.Add("Accept-Language", "zh-cn");        //获取中文页面
            request.Timeout = 5000;                                 //限制5s
            WebResponse response = request.GetResponse();           //创建WebResponse对象  
            Stream datastream = response.GetResponseStream();       //创建流对象
            StreamReader reader = new StreamReader(datastream, Encoding.UTF8);  //UTF-8编码
            html = reader.ReadToEnd();                       //读取网页内容 
            reader.Close();
            datastream.Close();
            response.Close();

        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("!!! HttpResquest {0}  {1}", ex.Message, Pid_Page);
            throw;
        }

        try
        {
            GetInfo(html);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine("!!! GetInfo {0}  {1}", ex.Message, Pid_Page);
            throw;
        }
    }

    void GetInfo(string source)
    {
        User = GetUser(source);
        Uid = GetUid(source);
        Title = GetTitle(source);
        Date = GetDate(source);
        Type = GetType(source);
        Intro = GetIntro(source);
        Tags = GetTags(source);
        source = source.Substring(source.IndexOf("\"likeData\""));//避免其他作品信息的影响
        PageCount = GetPageCount(source);
        BookmarkCount = GetBookmarkCount(source);
        LikeCount = GetLikeCount(source);
        CommentCount = GetCommentCount(source);
        ResponseCount = GetResponseCount(source);
        ViewCount = GetViewCount(source);
    }

    static List<string> GetTags(string source)
    {
        List<string> Tags = new List<string>();
        int start = source.IndexOf("\"tags\":");
        int end = source.IndexOf("}],", start);
        source = source.Substring(start, end - start);
        MatchCollection matches = Regex.Matches(source, "((\"tag\":\")|({\"en\":\"))(?<tags>[^\"]+)\"");
        foreach (Match match in matches)
        {
            GroupCollection groups = match.Groups;
            Tags.Add(WebUtility.HtmlDecode(groups["tags"].Value).Replace("\\u0027", "'"));// \u0027 变为 '
        }
        return Tags;
    }

    static string GetDate(string source)
    {
        int start = source.IndexOf("\"createDate\":\"") + 14;
        int end = source.IndexOf("\",", start);
        string Date = source.Substring(start, end - start);
        return Date;
    }

    static string GetType(string source)
    {
        int start = source.IndexOf("\"illustType\":\"") + 14;
        int end = source.IndexOf("\",", start);
        string Type = source.Substring(start, end - start);
        Type.Replace("0", "illustration");//插画
        Type.Replace("1", "manga");//漫画
        Type.Replace("2", "ugoira");//动图
        return Type;
    }

    static string GetPId(string source)//好像没啥用
    {
        int start = source.IndexOf("\"illustId\":\"") + 12;
        int end = source.IndexOf("\",", start);
        string Pid = source.Substring(start, end - start);
        return Pid;
    }
    static string GetUser(string source)
    {
        int start = source.IndexOf("\"userName\":\"") + 12;
        int end = source.IndexOf("\"}", start);
        string User = source.Substring(start, end - start);
        User = AvoidNameError(User);//避免Windows文件名错误
        User = Regex.Replace(User, "@.*", "");
        User = Regex.Replace(User, "＠.*", "");
        User = Regex.Replace(User, "個展.*", "");
        User = Regex.Replace(User, "しろすず.*", "しろすず");
        User = Regex.Replace(User, "碧風羽.*", "碧風羽");
        User = Regex.Replace(User, "Sila.*", "");
        User = Regex.Replace(User, "(仕事募集中)", "");
        User = Regex.Replace(User, "(お仕事募集中)", "");
        User = Regex.Replace(User, " ／ 仕事募集中", "");
        User = Regex.Replace(User, "お仕事募集中", "");
        return User;
    }

    static string GetTitle(string source)
    {
        int start = source.IndexOf("\"illustTitle\":\"") + 15;
        int end = source.IndexOf("\",", start);
        string Title = source.Substring(start, end - start);
        Title = AvoidNameError(Title);//避免Windows文件名错误
        return Title;
    }

    static string AvoidNameError(string name)//避免Windows文件名错误
    {
        name = WebUtility.HtmlDecode(name);
        name = name.Replace("\\u0027", "'");
        name = name.Replace("\\\"", "＂");
        name = name.Replace(":", "꞉");
        name = name.Replace("?", "？");
        name = name.Replace("*", "＊");
        name = name.Replace("|", "︱");
        name = name.Replace("<", "﹤");
        name = name.Replace(">", "﹥");
        name = name.Replace("/", "／");
        name = name.Replace("\\", "／");
        return name;
    }
    static string GetIntro(string source)
    {
        int start = source.IndexOf("\"illustComment\":\"") + 17;
        int end = source.IndexOf("\",\"", start);
        string Intro = source.Substring(start, end - start);
        Intro = HtmlToText(Intro);
        return Intro;
    }

    static string GetUid(string source)
    {
        int start = source.IndexOf("\"userId\":\"") + 10;
        int end = source.IndexOf("\",", start);
        string Uid = source.Substring(start, end - start);
        return Uid;
    }

    static int GetPageCount(string source)
    {
        int start = source.IndexOf("\"pageCount\":") + 12;
        int end = source.IndexOf(",", start);
        int PageCount = Convert.ToInt32(source.Substring(start, end - start));
        return PageCount;
    }

    static int GetBookmarkCount(string source)
    {
        int start = source.IndexOf("\"bookmarkCount\":") + 16;
        int end = source.IndexOf(",", start);
        int BookmarkCount = Convert.ToInt32(source.Substring(start, end - start));
        return BookmarkCount;
    }

    static int GetLikeCount(string source)
    {
        int start = source.IndexOf("\"likeCount\":") + 12;
        int end = source.IndexOf(",", start);
        int LikeCount = Convert.ToInt32(source.Substring(start, end - start));
        return LikeCount;
    }

    static int GetCommentCount(string source)
    {
        int start = source.IndexOf("\"commentCount\":") + 15;
        int end = source.IndexOf(",", start);
        int CommentCount = Convert.ToInt32(source.Substring(start, end - start));
        return CommentCount;
    }

    static int GetResponseCount(string source)
    {
        int start = source.IndexOf("\"responseCount\":") + 16;
        int end = source.IndexOf(",", start);
        int ResponseCount = Convert.ToInt32(source.Substring(start, end - start));
        return ResponseCount;
    }
    static int GetViewCount(string source)
    {
        int start = source.IndexOf("\"viewCount\":") + 12;
        int end = source.IndexOf(",", start);
        int ViewCount = Convert.ToInt32(source.Substring(start, end - start));
        return ViewCount;
    }
    static string HtmlToText(string source)  //! 仅对作品介绍进行适配
    {
        source = WebUtility.UrlDecode(source);
        source = WebUtility.HtmlDecode(source);
        source = Regex.Replace(source, @"<( )*br( )*>", "\n", RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"<( )*li( )*>", "\n", RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"<( )*br( )*/>", "\n", RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"<( )*li( )*/>", "\n", RegexOptions.IgnoreCase);
        source = Regex.Replace(source, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        return source;
    }

    public BitmapMetadata ToMetadata()
    {
        BitmapMetadata metadata = new BitmapMetadata("jpg");
        metadata.Author = new ReadOnlyCollection<string>(new List<string>() { User });
        metadata.Title = Title;
        metadata.Comment = Intro;
        metadata.DateTaken = Date;
        metadata.Keywords = Tags.AsReadOnly();
        metadata.Rating = SetRating();
        return metadata;
    }

    int SetRating()//TODO 根据浏览点赞收藏数设定分级
    {
        int rating;
        int index = ViewCount + LikeCount + BookmarkCount + CommentCount + ResponseCount;//综合指标
        if (index >= 0)//5级条件
            rating = 5;
        else if (index >= 0)//4级条件
            rating = 4;
        else if (index >= 0)//3级条件
            rating = 3;
        else if (index >= 0)//2级条件
            rating = 2;
        else if (index >= 0)//1级条件
            rating = 1;
        else
            rating = 0;//不分级
        return rating;
    }
}
