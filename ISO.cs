using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using XkeyBrew.Utils.IsoGameReader;

namespace xk3yDVDMenu
{
    [Serializable]
    public class ISO
    {
        public string Filename;
        public string GameBoxart;
        public string GameDesc;
        public string GameGenre;
        public string Gameimage;
        public string GameNameFromFilename;
        public string GameTitle;
        public FileInfo ISOFile;
        public string JumpToGameDetails;
        public string JumpToSelectThisGame;
        public string JumpToTrailler;
        public int Page;
        public string Path;
        public string RedirectURL;
        public string GameTrailer;
        public string WorkingDirectory;

        private string AddDiscDataToBanner(string bannerPath, int discNumber, int discCount)
        {
            int height;
            if (discCount <= 1)
            {
                return bannerPath;
            }
            const int scale = 30;
            byte[] bannerArray = File.ReadAllBytes(bannerPath);
            var bannerMs = new MemoryStream(bannerArray);
            Image banner = Image.FromStream(bannerMs);
            byte[] discIconArray = File.ReadAllBytes(string.Concat(WorkingDirectory, "media\\disk.png"));
            var discIconMs = new MemoryStream(discIconArray);
            Image image = Image.FromStream(discIconMs);
            if (banner.Height < scale)
            {
                height = banner.Height - 4;
            }
            else
            {
                height = scale;
            }
            Image discIcon = Resize(image, height);
            byte[] discCurrentIconArray =
                File.ReadAllBytes(string.Concat(WorkingDirectory, "media\\disc_current.png"));
            var discCurrentIconMs = new MemoryStream(discCurrentIconArray);
            Image discCurrentIcon = Resize(Image.FromStream(discCurrentIconMs), scale);
            const int offset = 10;
            var bitmapDisc = new Bitmap((discCount - 1)*10 + discIcon.Width, discIcon.Height);
            Graphics g = Graphics.FromImage(bitmapDisc);
            for (int i = 0; i < discCount; i++)
            {
                g.DrawImage(discNumber - 1 != i ? discIcon : discCurrentIcon, i*offset, 0, discIcon.Height,
                            discIcon.Width);
            }
            Graphics gr = Graphics.FromImage(banner);
            gr.DrawImage(bitmapDisc, banner.Width - bitmapDisc.Width - 2, 2, bitmapDisc.Width, bitmapDisc.Height);
            bannerPath = string.Concat(WorkingDirectory, "cache\\",
                                       Filename.Replace(ISOFile.Extension, "-banner.png"));
            banner.Save(bannerPath, ImageFormat.Png);
            return bannerPath;
        }

        public string GetGameBanner(bool chkArtwork)
        {
            string banner;
            string bannerPath = GetGameBannerBasic(chkArtwork);
            try
            {
                var isoFile = new IsoGameInfo(Path);
                banner = AddDiscDataToBanner(bannerPath, isoFile.XeXHeaderInfo.DiscNumber,
                                             isoFile.XeXHeaderInfo.DiscCount);
            }
            catch (Exception)
            {
                banner = bannerPath;
            }
            return banner;
        }

        public string GetGameBannerBasic(bool chkArtwork)
        {
            //Log.Text = string.Concat(Log.Text, "Loading banner:", Path, Environment.NewLine);
            if (!File.Exists(string.Concat(Path.Replace(ISOFile.Extension, ""), "-banner.png")))
            {
                var waffle = new WaffleXML(Path.Replace(ISOFile.Extension, ".xml"));
                if (waffle.Banner == null)
                {
                    if (chkArtwork)
                    {
                        try
                        {
                            var iso = new Iso(ISOFile.FullName, true);
                            if (iso.DefaultXeX != null)
                            {
                                //Log.Text = string.Concat(Log.Text, "Searching Xbox.com for banner:", Path, Environment.NewLine);
                                var wc = new WbClient();
                                byte[] data =
                                    wc.DownloadData(
                                        string.Concat(
                                            "http://download.xbox.com/content/images/66acd000-77fe-1000-9115-d802",
                                            iso.DefaultXeX.XeXHeader.TitleId.ToLower(), "/1033/banner.png"));
                                if (data == null)
                                {
                                    throw new Exception("Download Failed");
                                }
                                {
                                    waffle.Banner = data;
                                    //TextBox textBox1 = Log;
                                    //textBox1.Text = string.Concat(textBox1.Text, "Banner saved to XML File", Path, Environment.NewLine);
                                    return GetGameBanner(true);
                                }
                            }
                            {
                                throw new Exception("Not a Game");
                            }
                        }
                        catch (Exception)
                        {
                            //Log.Text = string.Concat(Log.Text, "Banner Download Failed:", Path, Environment.NewLine);
                            //Log.Text = string.Concat(Log.Text, "No banner found:", Path, Environment.NewLine);
                            return "media\\blank-banner.png";
                        }
                    }
                    //Log.Text = string.Concat(Log.Text, "No banner found:", Path, Environment.NewLine);
                    return "media\\blank-banner.png";
                }
                else
                {
                    var binWritter =
                        new StreamWriter(
                            string.Concat(WorkingDirectory, "cache\\",
                                          Filename.Replace(ISOFile.Extension, "-banner.png")), false);
                    binWritter.BaseStream.Write(waffle.Banner, 0, waffle.Banner.Length);
                    binWritter.Flush();
                    binWritter.Close();

                    //Log.Text = string.Concat(Log.Text, "Banner found in XML:", Path, Environment.NewLine);
                    return string.Concat(WorkingDirectory, "cache\\",
                                         Filename.Replace(ISOFile.Extension, "-banner.png"));
                }
            }
            else
            {
                //Log.Text = string.Concat(Log.Text, "Local Banner Found:", Path, Environment.NewLine);
                return string.Concat(Path.Replace(ISOFile.Extension, ""), "-banner.png");
            }
        }

        public string GetGameBoxart(bool chkArtwork)
        {
            //Log.Text = string.Concat(Log.Text, "Finding cover for:", Path, Environment.NewLine);
            if (
                !(File.Exists(string.Concat(Path.Replace(ISOFile.Extension, ""), "-cover.jpg")) |
                  File.Exists(string.Concat(Path.Replace(ISOFile.Extension, ""), "-cover.png"))))
            {
                var waffle = new WaffleXML(Path.Replace(ISOFile.Extension, ".xml"));
                if (waffle.BoxArt == null)
                {
                    if (chkArtwork)
                    {
                        try
                        {
                            var iso = new Iso(ISOFile.FullName, true);
                            if (iso.DefaultXeX != null)
                            {
                                //Log.Text = string.Concat(Log.Text, "Searching Xbox.com for cover", Path, Environment.NewLine);
                                var wc = new WbClient();
                                byte[] data =
                                    wc.DownloadData(string.Concat("http://tiles.xbox.com/consoleAssets/",
                                                                  iso.DefaultXeX.XeXHeader.TitleId.ToLower(),
                                                                  "/en-US/largeboxart.jpg"));
                                if (data != null)
                                {
                                    waffle.BoxArt = data;

                                    //Log.Text = string.Concat(Log.Text, "Cover saved to XML", Path, Environment.NewLine);
                                    return GetGameBoxart(true);
                                }
                                {
                                    throw new Exception("Download Failed");
                                }
                            }
                            {
                                throw new Exception("Not a Game");
                            }
                        }
                        catch (Exception)
                        {
                            //Log.Text = string.Concat(Log.Text, "Download Failed", Path, Environment.NewLine);
                            //Log.Text = string.Concat(Log.Text, "No Cover found", Path, Environment.NewLine);
                            return "media\\blank-cover.jpg";
                        }
                    }
                    //Log.Text = string.Concat(Log.Text, "No Cover found", Path, Environment.NewLine);
                    return "media\\blank-cover.jpg";
                }
                {
                    var binWritter =
                        new StreamWriter(
                            string.Concat(WorkingDirectory, "cache\\",
                                          Filename.Replace(ISOFile.Extension, "-cover.jpg")), false);
                    binWritter.BaseStream.Write(waffle.BoxArt, 0, waffle.BoxArt.Length);
                    binWritter.Flush();
                    binWritter.Close();
                    //Log.Text = string.Concat(Log.Text, "Found in XML", Path, Environment.NewLine);
                    return string.Concat(WorkingDirectory, "cache\\",
                                         Filename.Replace(ISOFile.Extension, "-cover.jpg"));
                }
            }
            {
                if (File.Exists(string.Concat(Path.Replace(ISOFile.Extension, ""), "-cover.png")))
                {
                    return string.Concat(Path.Replace(ISOFile.Extension, ""), "-cover.png");
                }
                //Log.Text = string.Concat(Log.Text, "Local cover found", Path, Environment.NewLine);
                return string.Concat(Path.Replace(ISOFile.Extension, ""), "-cover.jpg");
            }
        }

        public string GetGameDesc(bool chkArtwork)
        {
            //Log.Text = string.Concat(Log.Text, "Finding Desc", Path, Environment.NewLine);
            var waffleXMLFile = new WaffleXML(Path.Replace(ISOFile.Extension, ".xml"));
            if (waffleXMLFile.Summary == "")
            {
                if (chkArtwork)
                {
                    try
                    {
                        var iso = new Iso(ISOFile.FullName, true);
                        if (iso.DefaultXeX == null)
                        {
                        }
                        else
                        {
                            //Log.Text = string.Concat(Log.Text, "Xbox.com Desc", Path, Environment.NewLine);
                            var wc = new WbClient();
                            string page =
                                wc.DownloadString(wc.RedirectURL(this,
                                                                 "http://marketplace.xbox.com/en-US/games/media/66acd000-77fe-1000-9115-d802" +
                                                                 iso.DefaultXeX.XeXHeader.TitleId.ToLower() +
                                                                 "?nosplash=1"));
                            var replacer = new Regex("<meta name=\"description\" content=\".*\" />");
                            Match results = replacer.Match(page);
                            string desc = results.Value.Substring(34);
                            desc = desc.Substring(0, desc.Length - 5).Trim();
                            waffleXMLFile.Summary = desc;

                            //Log.Text = string.Concat(Log.Text, "Found", Path, Environment.NewLine);
                            return desc;
                        }
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                //Log.Text = string.Concat(Log.Text, "Not Found", Path, Environment.NewLine);
                return "";
            }
            {
                //Log.Text = string.Concat(Log.Text, "In XML", Path, Environment.NewLine);
                return waffleXMLFile.Summary;
            }
        }

        public string GetGameGenre(bool chkArtwork)
        {
            //Log.Text = string.Concat(Log.Text, "Finding Genre", Path, Environment.NewLine);
            var waffle = new WaffleXML(Path.Replace(ISOFile.Extension, ".xml"));
            if (waffle.InfoItem("Genre") == "")
            {
                if (chkArtwork)
                {
                    try
                    {
                        var iso = new Iso(ISOFile.FullName, true);
                        if (iso.DefaultXeX != null)
                        {
                            //Log.Text = string.Concat(Log.Text, "Xbox.com Search", Path, Environment.NewLine);
                            var wc = new WbClient();
                            string page =
                                wc.DownloadString(wc.RedirectURL(this,
                                                                 "http://marketplace.xbox.com/en-US/games/media/66acd000-77fe-1000-9115-d802" +
                                                                 iso.DefaultXeX.XeXHeader.TitleId.ToLower() +
                                                                 "?nosplash=1"));
                            if (page.IndexOf("Genre:", StringComparison.Ordinal) != 0)
                            {
                                int startIndex = page.IndexOf("Genre:", StringComparison.Ordinal) + 6;
                                string genre = page.Substring(startIndex);
                                genre = genre.Substring(0, genre.IndexOf("</li>", StringComparison.Ordinal));
                                string htmlDecode = HttpUtility.HtmlDecode(genre.Trim().Replace("\n", ""));
                                if (htmlDecode != null)
                                    genre = htmlDecode.Replace("<li>", "").Replace("</label>", "");
                                waffle.InfoItem("Genre", genre);

                                //Log.Text = string.Concat(Log.Text, "Found", Path, Environment.NewLine);
                                return genre;
                            }
                        }
                        else
                        {
                            throw new Exception("Not a Game");
                        }
                    }
                    catch (Exception)
                    {
                        return "";
                    }
                }
                //Log.Text = string.Concat(Log.Text, "No Genre Found", Path, Environment.NewLine);
                return "";
            }
            {
                //Log.Text = string.Concat(Log.Text, "Found in XML file", Path, Environment.NewLine);
                return waffle.InfoItem("Genre");
            }
        }

        public string GetGameTitle(bool chkArtwork)
        {
            //Log.Text = string.Concat(Log.Text, "Finding Title", Path, Environment.NewLine);
            var waffle = new WaffleXML(Path.Replace(ISOFile.Extension, ".xml"));
            if (waffle.Title == "")
            {
                if (chkArtwork)
                {
                    try
                    {
                        var iso = new Iso(ISOFile.FullName, true);
                        if (iso.DefaultXeX != null)
                        {
                            //Log.Text = string.Concat(Log.Text, "Xbox.com search", Path, Environment.NewLine);
                            var wc = new WbClient();
                            string page =
                                wc.DownloadString(wc.RedirectURL(this,
                                                                 "http://marketplace.xbox.com/en-US/games/media/66acd000-77fe-1000-9115-d802" +
                                                                 iso.DefaultXeX.XeXHeader.TitleId.ToLower() +
                                                                 "?nosplash=1"));
                            if (page.IndexOf("<title>", StringComparison.Ordinal) != 0)
                            {
                                int startIndex = page.IndexOf("<title>", StringComparison.Ordinal) + 7;
                                string title = page.Substring(startIndex);
                                title = title.Substring(0, title.IndexOf(" - Xbox.com", StringComparison.Ordinal));
                                title = title.Trim().Replace("\n", "");
                                title = HttpUtility.HtmlDecode(title);
                                if (title != null) title = title.Replace("&", "&amp;");
                                waffle.Title = title;

                                return title;
                            }
                        }
                        else
                        {
                            throw new Exception("Not a Game");
                        }
                    }
                    catch (Exception)
                    {
                        return GameNameFromFilename;
                    }
                }
                //Log.Text = string.Concat(Log.Text, "Using filename", Path, Environment.NewLine);
                return GameNameFromFilename;
            }
            else
            {
                //Log.Text = string.Concat(Log.Text, "Found in XML", Path, Environment.NewLine);
                return waffle.Title;
            }
        }

        public string GetGameTrailer(bool chkTrailers)
        {
            //Log.Text = string.Concat(Log.Text, "Finding Trailer", Path, Environment.NewLine);
            if (!File.Exists(Path.Replace(ISOFile.Extension, ".wmv")))
            {
                if (chkTrailers)
                {
                    try
                    {
                        var isoGame = new Iso(ISOFile.FullName, true);
                        if (isoGame.DefaultXeX != null)
                        {
                            //Log.Text = string.Concat(Log.Text, "Xbox.com", Path, Environment.NewLine);
                            var wc = new WbClient();
                            string page =
                                wc.DownloadString(wc.RedirectURL(this,
                                                                 ("http://marketplace.xbox.com/en-US/games/media/66acd000-77fe-1000-9115-d802" +
                                                                  isoGame.DefaultXeX.XeXHeader.TitleId.ToLower()) +
                                                                 "?nosplash=1"));
                            var regexReplacer = new Regex("addVideo\\('[^,]*,");
                            Match regexmatchResult = regexReplacer.Match(page);
                            if (!regexmatchResult.Success)
                            {
                            }
                            else
                            {
                                string strVideoAsx =
                                    wc.DownloadString(
                                        regexmatchResult.Value.Replace("\\x3a", ":").Replace("\\x2f", "/").Substring(10)
                                            .Replace(",", "").Trim());
                                regexReplacer = new Regex("href=\"[^\"]*\"");
                                regexmatchResult = regexReplacer.Match(strVideoAsx);
                                //Log.Text = string.Concat(Log.Text, "Downloading", Path, Environment.NewLine);
                                string strVideoUrl = regexmatchResult.Value.Replace("href=\"", "").Replace("\"", "");
                                wc.DownloadFile(strVideoUrl, Path.Replace(ISOFile.Extension, ".wmv"));
                                string str = Path.Replace(ISOFile.Extension, ".wmv");

                                return str;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //Log.Text = string.Concat(Log.Text, "Failed", Path, Environment.NewLine);
                    }
                }
                //Log.Text = string.Concat(Log.Text, "Not Found", Path, Environment.NewLine);
                return "media\\blank.mpg";
            }
            else
            {
                //Log.Text = string.Concat(Log.Text, "Local", Path, Environment.NewLine);
                return Path.Replace(ISOFile.Extension, ".wma");
            }
        }

        public Image Resize(Image srcImage, int newSize)
        {
            var newImage = new Bitmap(newSize, newSize);
            Graphics gr = Graphics.FromImage(newImage);
            using (gr)
            {
                gr.SmoothingMode = SmoothingMode.AntiAlias;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(srcImage, new Rectangle(0, 0, newSize, newSize));
            }
            return newImage;
        }
    }
}