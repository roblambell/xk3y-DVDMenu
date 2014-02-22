using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Ini;
using Microsoft.Win32;
using XkeyBrew.Utils.DvdReader;

namespace xk3yDVDMenu
{
    public partial class Form1 : Form
    {

        private const int TitlesetLimit = 30;

        public string[][] AlphaGroups = new[]
                                            {
                                                new[] {"A", "B", "C"},
                                                new[] {"D", "E", "F"},
                                                new[] {"G", "H", "I"},
                                                new[] {"J", "K", "L"},
                                                new[] {"M", "N", "O"},
                                                new[] {"P", "Q", "R", "S"},
                                                new[] {"T", "U", "V"},
                                                new[] {"W", "X", "Y", "Z"}
                                            };

        public ArrayList GameISOs = new ArrayList();
        public Dictionary<string, object> Values = new Dictionary<string, object>();

        public string PathToDVDStyler;
        public string WorkingDirectory;

        public Form1()
        {
            InitializeComponent();

            //test method

            /*ArrIsolist.Clear();
            Values["DRIVE"] = "K:\\";
            if (Directory.Exists(string.Concat(Values["DRIVE"], "\\games\\")))
            {
                DirSearch(string.Concat(Values["DRIVE"], "\\games\\"));
            }
            CreateSectorMap();*/
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool DefineDosDevice(int flags, string devname, string path);


        private void LoadDisks()
        {
            int selectedIndex = 0;
            int currentIndex = 0;

            DriveInfo[] driveList = DriveInfo.GetDrives();
            Log.Text += "Found " + driveList.Count() + (driveList.Count() == 1 ? " windows drive." : " windows drives.") + Environment.NewLine;

            comboBoxDriveList.Items.Clear();
            foreach (var drive in driveList)
            {
                if (drive.IsReady == true)
                {
                    string driveLabel;

                    if (drive.VolumeLabel.Length > 0)
                    {
                        driveLabel = drive.VolumeLabel;
                    }
                    else
                    {
                        switch (drive.DriveType)
                        {
                            case DriveType.CDRom:
                                driveLabel = "CD Drive";
                                break;
                            case DriveType.Fixed:
                                driveLabel = "Local Disk";
                                break;
                            case DriveType.Network:
                                driveLabel = "Network Drive";
                                break;
                            case DriveType.Removable:
                                driveLabel = "Removable Disk";
                                break;
                            default:
                                driveLabel = "Unknown";
                                break;
                        }
                    }

                    // Preselect drive
                    if (drive.DriveType == DriveType.Removable)
                    {
                        // Any removable drive is better than nothing..
                        if (selectedIndex == 0)
                        {
                            selectedIndex = currentIndex;
                        }
                        // BUT, one with our name in it, that must be it!
                        bool containsXbox = driveLabel.IndexOf("Xbox", StringComparison.OrdinalIgnoreCase) >= 0;
                        bool containsxk3y = driveLabel.IndexOf("xk3y", StringComparison.OrdinalIgnoreCase) >= 0;
                        if (containsXbox || containsxk3y)
                        {
                            selectedIndex = currentIndex;
                        }
                    }

                    string optionText = string.Format("({0}) {1} ({2})",
                        drive.Name.Substring(0, 2),
                        driveLabel,
                        GetBytesReadable(drive.TotalSize)
                        );
                    comboBoxDriveList.Items.Add(optionText);
                    currentIndex++;
                }
            }

            comboBoxDriveList.SelectedIndex = selectedIndex;

        }


        private string CreateDvdStylerTitleSets(IEnumerable<ISO> orderedIsos, int titlesetIsoLimit, string themepath)
        {
            Values.Add("TITLESETINDEX", "");
            Values.Add("TITLESETPGC", "");
            Values.Add("TITLESETTRAILERS", "");
            string strTitleset = "";

            var totalTitleSets = (int) Math.Ceiling((decimal) orderedIsos.Count()/titlesetIsoLimit);
            string titlesetHeader = (new StreamReader(string.Concat(themepath, "titlesetHeader.txt"))).ReadToEnd();
            string titlesetSelected = (new StreamReader(string.Concat(themepath, "titlesetSelected.txt"))).ReadToEnd();
            string titlesetDetails = (new StreamReader(string.Concat(themepath, "titlesetDetails.txt"))).ReadToEnd();


            int currentTitleSet = 0;
            while (currentTitleSet < totalTitleSets)
            {
                Values["TITLESETPGC"] = string.Empty;

                IEnumerable<ISO> titlesetIso =
                    (from ISO d in GameISOs orderby d.Gamename select d).Skip(currentTitleSet*titlesetIsoLimit).Take(
                        titlesetIsoLimit).ToArray();
                string titlesetindex = "if (g0 == 1) jump menu 2;";
                for (int i = 1; i < titlesetIso.Count()*2; i++)
                {
                    titlesetindex += "else if (g0 == " + (i + 1) + ") jump menu " + (i + 2) + ";";
                }
                Values["TITLESETINDEX"] = titlesetindex;
                int isoindex = 0;
                string titlesettrailers = "";

                foreach (ISO gameiso in titlesetIso)
                {
                    isoindex++;
                    gameiso.JumpToGameDetails = "g0 = " + (((isoindex - 1)*2) + 2) + "; jump titleset " +
                                                (currentTitleSet + 1) +
                                                " menu;";
                    gameiso.JumpToSelectThisGame = "g0 = " + (((isoindex - 1)*2) + 1) + "; jump titleset " +
                                                   (currentTitleSet + 1) +
                                                   " menu;";
                    gameiso.JumpToTrailler = "jump titleset " + +(currentTitleSet + 1) + " title " + isoindex + ";";


                    Values["JumpToGameDetails"] = gameiso.JumpToGameDetails;
                    Values["JumpToSelectThisGame"] = gameiso.JumpToSelectThisGame;
                    Values["JumpToTrailler"] = gameiso.JumpToTrailler;
                    Values["GAMETITLE"] = gameiso.Gametitle;
                    Values["GAMEGENRE"] = gameiso.Gamegenre;
                    Values["GAMEDESC"] = gameiso.Gamedesc;
                    Values["GAMEIMAGE"] = gameiso.Gameimage;
                    Values["GAMEBOX"] = gameiso.GAMEBOX;
                    Values["TRAILER"] = gameiso.TRAILER;
                    if (Values["TRAILER"].ToString() != "media\\blank.mpg")
                    {
                        Values["HIDETRAILER"] = "";
                    }
                    else
                    {
                        Values["HIDETRAILER"] = "//";
                    }

                    Values["TITLESETPGC"] += ThemeManager.ReplaceVals(titlesetSelected, Values) +
                                             ThemeManager.ReplaceVals(titlesetDetails, Values);
                    ;
                    titlesettrailers += "<pgc>\n<vob file=\"" + gameiso.TRAILER +
                                        "\">\n<video format=\"1\"/><audio format=\"3\"/></vob><post>call last menu;</post></pgc>";
                }


                Values["TITLESETTRAILERS"] = titlesettrailers;
                strTitleset += ThemeManager.ReplaceVals(titlesetHeader, Values);


                currentTitleSet++;
            }

            return strTitleset;
        }

        private void LoadGameDetails(IEnumerable<ISO> orderedIsos, int buttonCount)
        {
            int index = 0;
            foreach (ISO d in orderedIsos)
            {
                index++;
                d.Gametitle = HttpUtility.HtmlEncode(d.GameTitle(chkArtwork.Checked));
                d.Gamegenre = HttpUtility.HtmlEncode(d.GameGenre(chkArtwork.Checked));
                d.Gamedesc = HttpUtility.HtmlEncode(d.GameDesc(chkArtwork.Checked));
                d.Gameimage = HttpUtility.HtmlEncode(d.GameBanner(chkArtwork.Checked));
                d.GAMEBOX = HttpUtility.HtmlEncode(d.GameBox(chkArtwork.Checked));
                d.TRAILER = HttpUtility.HtmlEncode(d.GameTrailer(chkTraillers.Checked));
                d.Page = (int) Math.Floor((double) index)/buttonCount;
            }
        }

        private void FetchGameDataAndCreateProject()
        {
            TextBox log = Log;

            Values.Clear();
            var startupPath = new object[4];
            startupPath[0] = Application.StartupPath;
            startupPath[1] = "\\themes\\";
            startupPath[2] = comboBoxThemeList.SelectedItem;
            startupPath[3] = "\\";

            string pathToThemes = string.Concat(startupPath);

            Values.Add("DRIVE", comboBoxDriveList.SelectedItem.ToString().Substring(1, 2));

            GameISOs.Clear();
            if (Directory.Exists(string.Concat(Values["DRIVE"], "\\games\\")))
            {
                // Populates ArrIsolist
                RecursiveISOSearch(string.Concat(Values["DRIVE"], "\\games\\"));
            }

            Log.Text += "Found " + GameISOs.Count + (GameISOs.Count == 1 ? " ISO." : " ISOs.") + Environment.NewLine;

            if (GameISOs.Count > 0)
            {
                Log.Text += Environment.NewLine;

                TextBox log1 = Log;
                log1.Text = string.Concat(log1.Text, "             [Title][Genre][Desc][Banner][Cover][Trailer]", Environment.NewLine);
                Values.Add("APPPATH", string.Concat(Application.StartupPath, "\\"));
                Values.Add("PAGEINDEX", 0);
                Values.Add("PAGE", 1);
                Values.Add("ISOID", 0);
                Values.Add("CONTENT", "");
                Values.Add("GAMEIMAGE", "");
                Values.Add("GAMETITLE", "");
                Values.Add("GAMEGENRE", "");
                Values.Add("TRAILER", "");
                Values.Add("GAMEDESC", "");
                Values.Add("GAMEBOX", "");
                Values.Add("HIDETRAILER", "");

                Values.Add("JumpToGameDetails", "");
                Values.Add("JumpToSelectThisGame", "");
                Values.Add("JumpToTrailler", "");


                TextBox textBox1 = Log;
                textBox1.Text = string.Concat(textBox1.Text, "Loading theme", Environment.NewLine);

                string pgc = (new StreamReader(string.Concat(pathToThemes, "PGC.txt"))).ReadToEnd();
                int buttonCount =
                    (from d in new DirectoryInfo(pathToThemes).GetFiles("ButtonLocation*.txt") select d).Count();
                double totalPageCount = Math.Ceiling(GameISOs.Count/(double) buttonCount);
                Values.Add("TotalPageCount", totalPageCount);
                string buttonDef = (new StreamReader(string.Concat(pathToThemes, "ButtonStyle.txt"))).ReadToEnd();
                string objDef = (new StreamReader(string.Concat(pathToThemes, "GAMEOBJ.txt"))).ReadToEnd();
                string butActions = (new StreamReader(string.Concat(pathToThemes, "ButtonActions.txt"))).ReadToEnd();
                string objFiles = (new StreamReader(string.Concat(pathToThemes, "OBJFiles.txt"))).ReadToEnd();
                string prevDef = (new StreamReader(string.Concat(pathToThemes, "PrevButtonStyle.txt"))).ReadToEnd();
                string prevLoc = (new StreamReader(string.Concat(pathToThemes, "PrevButtonLocation.txt"))).ReadToEnd();
                string prevAct = (new StreamReader(string.Concat(pathToThemes, "PrevButtonAction.txt"))).ReadToEnd();
                string nextDef = (new StreamReader(string.Concat(pathToThemes, "NextButtonStyle.txt"))).ReadToEnd();
                string nextLoc = (new StreamReader(string.Concat(pathToThemes, "NextButtonLocation.txt"))).ReadToEnd();
                string nextAct = (new StreamReader(string.Concat(pathToThemes, "NextButtonAction.txt"))).ReadToEnd();
                string alphaDef = (new StreamReader(string.Concat(pathToThemes, "alphaButtonStyle.txt"))).ReadToEnd();
                string alphaLoc = (new StreamReader(string.Concat(pathToThemes, "alphaButtonLocation.txt"))).ReadToEnd();
                string alphaAct = (new StreamReader(string.Concat(pathToThemes, "alphaButtonAction.txt"))).ReadToEnd();

                string pgcs = "";


                ISO[] orderedISO = (from ISO d in GameISOs orderby d.Gamename select d).ToArray();
                LoadGameDetails(orderedISO, buttonCount);
                string titleSets = CreateDvdStylerTitleSets(orderedISO, TitlesetLimit, pathToThemes);

                for (int i = 0; (double) i < totalPageCount; i++)
                {
                    string defs = "<defs id=\"defs\">\n";
                    string locationsObJ = "<g id=\"objects\">\n";
                    string locationsBut = "<g id=\"buttons\">\n";
                    string actions = "";
                    string defObjs = "";
                    string objFilestxt = "";
                    Values["PAGE"] = i + 1;
                    Values["PAGEINDEX"] = 0;
                    IEnumerable<ISO> pageIso =
                        (from ISO d in GameISOs orderby d.Gamename select d).Skip(i*buttonCount).Take(buttonCount);
                    foreach (ISO d in pageIso)
                    {
                        Application.DoEvents();
                        Values["PAGEINDEX"] = string.Format("{0:00}", int.Parse(Values["PAGEINDEX"].ToString()) + 1);
                        Values["ISOID"] = (int) Values["ISOID"] + 1;
                        Values["GAMETITLE"] = d.Gametitle;
                        Values["GAMEGENRE"] = d.Gamegenre;
                        Values["GAMEDESC"] = d.Gamedesc;
                        Values["GAMEIMAGE"] = d.Gameimage;
                        Values["GAMEBOX"] = d.GAMEBOX;
                        Values["JumpToGameDetails"] = d.JumpToGameDetails;
                        Values["JumpToSelectThisGame"] = d.JumpToSelectThisGame;
                        Values["JumpToTrailler"] = d.JumpToTrailler;

                        var item = new object[4];
                        item[0] = pathToThemes;
                        item[1] = "ObjLocation";
                        item[2] = Values["PAGEINDEX"];
                        item[3] = ".txt";
                        string objLocation = (new StreamReader(string.Concat(item))).ReadToEnd();
                        var objArray = new object[4];
                        objArray[0] = pathToThemes;
                        objArray[1] = "ButtonLocation";
                        objArray[2] = Values["PAGEINDEX"];
                        objArray[3] = ".txt";
                        string butLocation = (new StreamReader(string.Concat(objArray))).ReadToEnd();
                        Values["TRAILER"] = d.TRAILER;
                        if (Values["TRAILER"].ToString() != "media\\blank.mpg")
                        {
                            Values["HIDETRAILER"] = "";
                        }
                        else
                        {
                            Values["HIDETRAILER"] = "//";
                        }

                        defs = string.Concat(defs, ThemeManager.ReplaceVals(buttonDef, Values));
                        defObjs = string.Concat(defObjs, ThemeManager.ReplaceVals(objDef, Values));
                        locationsObJ = string.Concat(locationsObJ, ThemeManager.ReplaceVals(objLocation, Values));
                        locationsBut = string.Concat(locationsBut, ThemeManager.ReplaceVals(butLocation, Values));
                        actions = string.Concat(actions, ThemeManager.ReplaceVals(butActions, Values));
                        objFilestxt = string.Concat(objFilestxt, ThemeManager.ReplaceVals(objFiles, Values));
                    }
                    if (File.Exists(string.Concat(pathToThemes, "alpha.txt")))
                    {
                        defs = string.Concat(defs, ThemeManager.ReplaceVals(alphaDef, Values));
                        locationsBut = string.Concat(locationsBut, ThemeManager.ReplaceVals(alphaLoc, Values));
                        actions = string.Concat(actions, ThemeManager.ReplaceVals(alphaAct, Values));
                    }
                    if (i > 0)
                    {
                        defs = string.Concat(defs, ThemeManager.ReplaceVals(prevDef, Values));
                        locationsBut = string.Concat(locationsBut, ThemeManager.ReplaceVals(prevLoc, Values));
                        actions = string.Concat(actions, ThemeManager.ReplaceVals(prevAct, Values));
                    }
                    if (totalPageCount > (i + 1))
                    {
                        defs = string.Concat(defs, ThemeManager.ReplaceVals(nextDef, Values));
                        locationsBut = string.Concat(locationsBut, ThemeManager.ReplaceVals(nextLoc, Values));
                        actions = string.Concat(actions, ThemeManager.ReplaceVals(nextAct, Values));
                    }
                    locationsObJ = string.Concat(locationsObJ, "</g>\n");
                    locationsBut = string.Concat(locationsBut, "</g>\n");
                    defs = string.Concat(defs, defObjs, "</defs>\n");
                    var strArrays = new string[6];
                    strArrays[0] = defs;
                    strArrays[1] = locationsObJ;
                    strArrays[2] = locationsBut;
                    strArrays[3] = "</svg>\n";
                    strArrays[4] = actions;
                    strArrays[5] = objFilestxt;
                    Values["CONTENT"] = string.Concat(strArrays);
                    pgcs = string.Concat(pgcs, ThemeManager.ReplaceVals(pgc, Values));
                }
                if (File.Exists(string.Concat(pathToThemes, "alpha.txt")))
                {
                    string allactions = "";
                    Values.Add("alphaletter", "A");
                    Values.Add("alphaaction", "");
                    Values.Add("alphaActions", "");
                    //const int alphahit = 0;
                    string alpha = (new StreamReader(string.Concat(pathToThemes, "alpha.txt"))).ReadToEnd();
                    string alphaActions = (new StreamReader(string.Concat(pathToThemes, "alpha-Actions.txt"))).ReadToEnd();
                    foreach (var letterGroup in AlphaGroups)
                    {
                        int PreviousFound = 0;
                        foreach (string letter in letterGroup)
                        {
                            int found = FirstLocationAlpha(letter, buttonCount);
                            if (found > -1)
                            {
                                PreviousFound = found;
                                break;
                            }
                        }


                        Values["alphaaction"] = string.Concat("jump vmgm menu ", PreviousFound + 1, ";");
                        Values["alphaletter"] = letterGroup[0];

                        allactions = string.Concat(allactions, ThemeManager.ReplaceVals(alphaActions, Values));
                    }
                    Values["alphaActions"] = allactions;
                    pgcs = string.Concat(pgcs, ThemeManager.ReplaceVals(alpha, Values));
                }
                Values.Add("PGCS", pgcs);
                Values.Add("TITLESETS", titleSets);
                string mainfile = (new StreamReader(string.Concat(pathToThemes, "Main.txt"))).ReadToEnd();
                mainfile = ThemeManager.ReplaceVals(mainfile, Values);
                var dvdFile = new StreamWriter(
                    string.Concat(Application.StartupPath, "\\dvdTemplate.dvds"), false);
                var chrArray = new char[1];
                chrArray[0] = '\n';
                string[] strArrays1 = mainfile.Split(chrArray);
                int num1 = 0;
                while (num1 < strArrays1.Length)
                {
                    string line = strArrays1[num1];
                    if (!line.Trim().StartsWith("//"))
                    {
                        dvdFile.Write(line);
                    }
                    num1++;
                }
                dvdFile.Close();

                buttonPrepareXML.Enabled = false;
                buttonGenerateDVDMenu.Enabled = true;
                buttonCopyToDrive.Enabled = false;

                chkArtwork.Enabled = false;
                chkTraillers.Enabled = false;

                buttonGenerateDVDMenu.Focus();

                Log.Text += Environment.NewLine;
                Log.Text += "Complete. Click `Generate DVDMenu`." + Environment.NewLine;

            }
            else
            {
                MessageBox.Show("No Games found");
                comboBoxDriveList.Enabled = true;
            }
        }

        private int FirstLocationAlpha(string letter, int buttonCount)
        {
            ISO[] orderedISO =
                (from ISO d in GameISOs orderby d.Gamename select d).ToArray();

            int num = 0;
            while (num < orderedISO.Length)
            {
                ISO d = orderedISO[num];

                if (!d.Gamename.ToUpper().StartsWith(letter))
                {
                    num++;
                }
                else
                {
                    return d.Page;
                }
            }


            return -1;
        }

        private void CreateSectorMap()
        {
            Log.Text = "Awaiting Code";
            Log.Text += "Creating sector map..." + Environment.NewLine;
            var testFile = new StreamWriter(Application.StartupPath + "\\Menu.xsk", false);
            try
            {
                List<byte[]> sectors =
                    new DvdMenuReadSectors(Application.StartupPath + "\\dvd.iso").FillListWithMenuSectors();

                SHA1 sha = new SHA1CryptoServiceProvider();
                int i = 0;

                ISO[] orderedISO =
                    (from ISO d in GameISOs orderby d.Gamename select d).ToArray();

                foreach (var sector in sectors)
                {
                    var encoding = new UTF8Encoding();
                    byte[] data =
                        encoding.GetBytes(
                            (orderedISO[i]).Path.Replace(Values["DRIVE"].ToString(), "").Replace("\\", "/").
                                Substring("/game/".Length));
                    testFile.BaseStream.Write(sector, 0, 4);
                    byte[] hash = sha.ComputeHash(data);
                    testFile.BaseStream.Write(hash, 0, hash.Length);
                    i += 1;
                }
                testFile.BaseStream.Flush();
                testFile.Flush();
                testFile.Close();
            }
            catch //(Exception ex)
            {
                throw;
            }
        }

        private void LoadThemes()
        {
            var themePaths = new DirectoryInfo(Application.StartupPath + "\\themes\\");
            var themeList = themePaths.GetDirectories();

            Log.Text += "Found " + themeList.Count() + (themeList.Count() == 1 ? " theme." : " themes.") + Environment.NewLine;

            comboBoxThemeList.Items.Clear();
            foreach (DirectoryInfo folder in themeList)
            {
                comboBoxThemeList.Items.Add(folder.Name);
            }
            comboBoxThemeList.SelectedIndex = 0;
        }

        private bool isDVDStylerInstalled()
        {
            PathToDVDStyler = ProgramFilesx86() + "\\DVDStyler\\bin\\DVDStyler.exe";

            if (File.Exists(PathToDVDStyler))
            {
                // DVDStyler doesn't store version info in its executable...
                //var versionInfo = FileVersionInfo.GetVersionInfo(pathToDVDStyler);
                //string version = versionInfo.ProductVersion;

                string version = (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\DVDStyler", "Version", "is installed");

                Log.Text += "DVDStyler " + version + Environment.NewLine;

                return true;
            }
            else
            {
                Log.Text += "DVDStyler not found. Please install to:" + Environment.NewLine;
                Log.Text += PathToDVDStyler + Environment.NewLine;

                return false;
            }

        }

        private void SetupWorkingDirectory()
        {
            // Current User Roaming App Data - equivalent to %AppData% if set
            string pathToRoamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the App Data\Roaming path with our desired directory name
            WorkingDirectory = Path.Combine(pathToRoamingAppData, "xk3yDVDMenu");

            if (!Directory.Exists(WorkingDirectory))
            {
                Directory.CreateDirectory(WorkingDirectory);
            }
             
        }

        private void LoadPlugins()
        {
            // TODO: Working on plugin support for scrappers
            FileInfo[] filelist = new DirectoryInfo(Application.StartupPath + "\\plugins\\").GetFiles("*.dll");
            foreach (FileInfo F in filelist)
            {
                Assembly asm = Assembly.LoadFrom(F.FullName);
                Type[] typeAsm = asm.GetTypes();
                Type ti = typeof (IMediaDownloader);

                foreach (Type type in typeAsm)
                {
                    IEnumerable<Type> results = from d in type.GetInterfaces()
                                                where d.Name == "IMediaDownloader"
                                                select d;
                    if (results.Any())
                    {
                        MessageBox.Show("Test");
                    }
                }

                //if type
                //    if  (typeAsm.GetInterface(GetType()))
                //    {

                //    }
                //Not
                //typeAsm.GetInterface(GetType(IInterface).FullName)
                //Is Nothing 
                //Then
                //' 
                //Type
                //typeAsm.Fullname in
                //asm.Fullname supports 
                //the interface
                //Dim
                //obj as Object,
                //iObj as IInterface
                //obj = asm.CreateInstance(typeAsm.FullName, True)
                //Return Ctype 
                //(obj, iObj)
                //End If 
                //Next
            }
        }

        // Returns the human-readable file size for an arbitrary, 64-bit file size 
        // The default format is "0.## XB", e.g. "4.2 KB" or "1.43 GB"
        public static string GetBytesReadable(long i)
        {
            string sign = (i < 0 ? "-" : "");
            double readable = (i < 0 ? -i : i);
            string suffix;
            if (i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (double)(i >> 50);
            }
            else if (i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (double)(i >> 40);
            }
            else if (i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (double)(i >> 30);
            }
            else if (i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (double)(i >> 20);
            }
            else if (i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (double)(i >> 10);
            }
            else if (i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = (double)i;
            }
            else
            {
                return i.ToString(sign + "0 B"); // Byte
            }
            readable /= 1024;

            return sign + readable.ToString("0.## ") + suffix;
        }

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private void Form1Load(object sender, EventArgs e)
        {
            Log.Text += "xk3y DVDMenu Tool" + Environment.NewLine;

            if (!isDVDStylerInstalled())
            {
                buttonPrepareXML.Enabled = false;
            }

            SetupWorkingDirectory();

            Log.Text += Environment.NewLine;

            // TODO: This one creates error, cannot find plugin - will add plugin support in next build - Diag
            //LoadPlugins();
            LoadDisks();
            LoadThemes();

            this.ActiveControl = buttonPrepareXML;
        }


        private void RecursiveISOSearch(string sDir)
        {
            foreach (FileInfo f in new DirectoryInfo(sDir).GetFiles("*.ISO"))
            {
                GameISOs.Add(new ISO
                                   {
                                       Filename = f.Name,
                                       Gamename = Regex.Replace(f.Name, ".iso", "", RegexOptions.IgnoreCase),
                                       Path = f.FullName,
                                       IsoFile = f,
                                       Log = Log,
                                       ProgressBar1 = progressBar1
                                   });
            }
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    RecursiveISOSearch(d);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void buttonPrepareXML_Click(object sender, EventArgs e)
        {
            comboBoxDriveList.Enabled = false;
            FetchGameDataAndCreateProject();
        }

        private void buttonGenerateDVDMenu_Click(object sender, EventArgs e)
        {
            if (File.Exists(Application.StartupPath + "\\dvd.iso"))
            {
                try
                {
                    File.Delete(Application.StartupPath + "\\dvd.iso");
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to delete existing ISO file");
                    return;
                }
            }

            string dvdStylePath = Application.StartupPath + "\\DVDstyler\\bin\\dvdstyler.exe";
            if (!File.Exists(dvdStylePath.ToLower().Replace("dvdstyler.exe", "") + "..\\DVDStyler.ini"))
            {
                File.Copy(Application.StartupPath + "\\DVDStyler.ini",
                          dvdStylePath.ToLower().Replace("dvdstyler.exe", "") + "..\\DVDStyler.ini");
            }
            Log.Text += "Setting DVD Styler Settings" + Environment.NewLine;
            var iniSettings =
                new IniFile(dvdStylePath.ToLower().Replace("dvdstyler.exe", "") + "..\\DVDStyler.ini");

            iniSettings.IniWriteValue("Burn", "Do", "0");
            iniSettings.IniWriteValue("Iso", "Do", "1");
            iniSettings.IniWriteValue("Generate", "MenuVideoBitrate", "8000");
            iniSettings.IniWriteValue("Generate", "MenuFrameCount", "50");
            iniSettings.IniWriteValue("Generate", "TempDir", Path.GetTempPath().Replace("\\", "\\\\"));
            Log.Text += "Finding first available drive" + Environment.NewLine;

            // Create symlinks drive - why?!
            string driveletter = ""; //GetAvailableDriveLetters().First();
            Log.Text += "Mapping " + driveletter + ": to startup folder" + Environment.NewLine;
            DefineDosDevice(0, driveletter.ToUpper() + ":",
                            dvdStylePath.ToLower().Replace("dvdstyler.exe", "") + "..\\");

            Log.Text += "Starting DVDStyler..." + Environment.NewLine;
            var start = new ProcessStartInfo
                            {
                                WorkingDirectory = driveletter + ":\\",
                                //Settings.Default.DVDStylePath.ToLower().Replace("dvdstyler.exe", ""),
                                //  FileName = Settings.Default.DVDStylePath ,
                                FileName = driveletter + ":\\bin\\dvdstyler.exe",
                                Arguments = " -s \"" + Application.StartupPath + "\\dvdTemplate.dvds\"",
                            };
            using (Process process = Process.Start(start))
            {
                process.WaitForExit();
                foreach (FileInfo f in new DirectoryInfo(driveletter + ":\\").GetFiles("*.vob"))
                {
                    f.Delete();
                }
                Log.Text += "Finding first available drive" + Environment.NewLine;
                DefineDosDevice(3, driveletter.ToUpper() + ":", null);
                if (File.Exists(Application.StartupPath + "\\dvd.iso"))
                {
                    CreateSectorMap();

                    buttonPrepareXML.Enabled = false;
                    buttonGenerateDVDMenu.Enabled = false;
                    buttonCopyToDrive.Enabled = true;

                    buttonCopyToDrive.Focus();

                    Log.Text += Environment.NewLine;
                    Log.Text += "Complete. Click `Copy DVDMenu to drive`." + Environment.NewLine;
                }
                else
                {
                    MessageBox.Show("ISO creation failed! Retry!");
                }
            }
        }
        

        private void buttonCopyToDrive_Click(object sender, EventArgs e)
        {
            try
            {
                File.Copy(Application.StartupPath + "\\dvd.iso", comboBoxDriveList.SelectedItem + "games\\Menu.xso", true);
                File.Copy(Application.StartupPath + "\\Menu.xsk", comboBoxDriveList.SelectedItem + "games\\Menu.xsk", true);
                MessageBox.Show("Complete");
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed. Retry!");
            }
        }
        
        public static string Wrap(string text, int maxLength)
        {
            text = text.Replace("\n", " ");
            text = text.Replace("\r", " ");
            text = text.Replace(".", ". ");
            text = text.Replace(">", "> ");
            text = text.Replace("\t", " ");
            text = text.Replace(",", ", ");
            text = text.Replace(";", "; ");
            text = text.Replace("", " ");
            text = text.Replace(" ", " ");
            string[] strWords = text.Split(' ');
            int currentLineLength = 0;
            string strLines = "";
            string currentLine = "";
            bool InTag = false;
            foreach (string currentWord in strWords)
            {
                if (currentWord.Length > 0)
                {
                    if (currentWord.Substring(0, 1) == "<")
                        InTag = true;
                    if (InTag)
                    {
                        if (currentLine.EndsWith("."))
                        {
                            currentLine += currentWord;
                        }
                        else
                            currentLine += " " + currentWord;
                        if (currentWord.IndexOf(">", StringComparison.Ordinal) > -1)
                            InTag = false;
                    }
                    else
                    {
                        if (currentLineLength + currentWord.Length + 1 < maxLength)
                        {
                            currentLine += " " + currentWord;
                            currentLineLength += (currentWord.Length + 1);
                        }
                        else
                        {
                            strLines += (currentLine) + "<tbreak/>";
                            currentLine = currentWord;
                            currentLineLength = currentWord.Length;
                        }
                    }
                }
            }
            if (currentLine != "")
                strLines += (currentLine) +
                            "<tbreak/>";
            return strLines;
        }

        private void Log_TextChanged(object sender, EventArgs e)
        {
            Log.SelectionStart = Log.Text.Length;
            Log.ScrollToCaret();
        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://k3yforums.com/");
        }

        private void comboBoxDriveList_DropDown(object sender, EventArgs e)
        {
            // Extend width of list beyond the ComboBox control as needed 
            ComboBox senderComboBox = (ComboBox)sender;
            int width = senderComboBox.DropDownWidth;
            Graphics g = senderComboBox.CreateGraphics();
            Font font = senderComboBox.Font;
            int vertScrollBarWidth =
                (senderComboBox.Items.Count > senderComboBox.MaxDropDownItems)
                ? SystemInformation.VerticalScrollBarWidth : 0;
            int newWidth;

            foreach (string s in ((ComboBox)sender).Items)
            {
                newWidth = (int)g.MeasureString(s, font).Width
                    + vertScrollBarWidth;

                if (width < newWidth)
                {
                    width = newWidth;
                }
            }

            senderComboBox.DropDownWidth = width;
        }

        private void pictureBoxLogo_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(this.pictureBoxLogo, "Visit k3y Forums");
        }

    }
}