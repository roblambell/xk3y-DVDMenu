using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
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

        public string WorkingDirectory;
        public string PathToDVDStyler;
        public string PathToVLC;

        public Form1()
        {
            InitializeComponent();
        }

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

        private void LoadGameDetails(IEnumerable<ISO> orderedISOs, int buttonCount)
        {
            int index = 0;
            foreach (ISO gameISO in orderedISOs)
            {
                index++;

                progressBar1.Value = index / orderedISOs.Count() * 100;

                Log.Text += gameISO.Filename + Environment.NewLine + "  ∟";

                gameISO.Gametitle = HttpUtility.HtmlEncode(gameISO.GameTitle(chkArtwork.Checked));
                gameISO.Gamegenre = HttpUtility.HtmlEncode(gameISO.GameGenre(chkArtwork.Checked));
                gameISO.Gamedesc = HttpUtility.HtmlEncode(gameISO.GameDesc(chkArtwork.Checked));
                gameISO.Gameimage = HttpUtility.HtmlEncode(gameISO.GameBanner(chkArtwork.Checked));
                gameISO.GAMEBOX = HttpUtility.HtmlEncode(gameISO.GameBox(chkArtwork.Checked));
                gameISO.TRAILER = HttpUtility.HtmlEncode(gameISO.GameTrailer(chkTraillers.Checked));

                Log.Text += Environment.NewLine;

                gameISO.Page = (int) Math.Floor((double) index) / buttonCount;
            }
        }

        private void FetchGameDataAndCreateProject()
        {

            Values.Clear();

            string pathToTheme = Application.StartupPath + "\\themes\\" + comboBoxThemeList.SelectedItem + "\\";

            // Selected item string expected as "(Z:)..."
            Values.Add("DRIVE", comboBoxDriveList.SelectedItem.ToString().Substring(1, 2) + "\\");

            // Copy media files to WorkingDirectory
            if (Directory.Exists(WorkingDirectory + "media"))
            {
                Directory.Delete(WorkingDirectory + "media", true);
            }
            new Microsoft.VisualBasic.Devices.Computer().
                FileSystem.CopyDirectory(Application.StartupPath + "\\media", WorkingDirectory + "media");

            // Create cache folder
            if (!Directory.Exists(WorkingDirectory + "cache"))
            {
                Directory.CreateDirectory(WorkingDirectory + "cache");
            }

            GameISOs.Clear();
            if (Directory.Exists(string.Concat(Values["DRIVE"], "games\\")))
            {
                // Populates ArrIsolist
                RecursiveISOSearch(string.Concat(Values["DRIVE"], "games\\"));
            }

            Log.Text += "Found " + GameISOs.Count + (GameISOs.Count == 1 ? " ISO." : " ISOs.") + Environment.NewLine;

            if (GameISOs.Count > 0)
            {
                Log.Text += Environment.NewLine;
                //Log.Text += "   [Title][Genre][Desc][Banner][Cover][Trailer]" + Environment.NewLine;

                Values.Add("APPPATH", WorkingDirectory);
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

                string pgc = (new StreamReader(pathToTheme + "PGC.txt")).ReadToEnd();
                int buttonCount =
                    (from d in new DirectoryInfo(pathToTheme).GetFiles("ButtonLocation*.txt") select d).Count();
                double totalPages = Math.Ceiling(GameISOs.Count/(double) buttonCount);
                Values.Add("TotalPageCount", totalPages);
                string buttonDef = (new StreamReader(pathToTheme + "ButtonStyle.txt")).ReadToEnd();
                string objDef = (new StreamReader(pathToTheme + "GAMEOBJ.txt")).ReadToEnd();
                string butActions = (new StreamReader(pathToTheme + "ButtonActions.txt")).ReadToEnd();
                string objFiles = (new StreamReader(pathToTheme + "OBJFiles.txt")).ReadToEnd();
                string prevDef = (new StreamReader(pathToTheme + "PrevButtonStyle.txt")).ReadToEnd();
                string prevLoc = (new StreamReader(pathToTheme + "PrevButtonLocation.txt")).ReadToEnd();
                string prevAct = (new StreamReader(pathToTheme + "PrevButtonAction.txt")).ReadToEnd();
                string nextDef = (new StreamReader(pathToTheme + "NextButtonStyle.txt")).ReadToEnd();
                string nextLoc = (new StreamReader(pathToTheme + "NextButtonLocation.txt")).ReadToEnd();
                string nextAct = (new StreamReader(pathToTheme + "NextButtonAction.txt")).ReadToEnd();
                string alphaDef = (new StreamReader(pathToTheme + "alphaButtonStyle.txt")).ReadToEnd();
                string alphaLoc = (new StreamReader(pathToTheme + "alphaButtonLocation.txt")).ReadToEnd();
                string alphaAct = (new StreamReader(pathToTheme + "alphaButtonAction.txt")).ReadToEnd();

                string pgcs = "";


                ISO[] orderedISOs = (from ISO d in GameISOs orderby d.Gamename select d).ToArray();
                LoadGameDetails(orderedISOs, buttonCount);
                string titleSets = CreateDvdStylerTitleSets(orderedISOs, TitlesetLimit, pathToTheme);

                for (int currentPage = 0; (double)currentPage < totalPages; currentPage++)
                {
                    string defs = "<defs id=\"defs\">\n";
                    string locationsObJ = "<g id=\"objects\">\n";
                    string locationsBut = "<g id=\"buttons\">\n";
                    string actions = "";
                    string defObjs = "";
                    string objFilestxt = "";
                    Values["PAGE"] = currentPage + 1;
                    Values["PAGEINDEX"] = 0;

                    IEnumerable<ISO> pageIso =
                        (from ISO d in GameISOs orderby d.Gamename select d).Skip(currentPage*buttonCount).Take(buttonCount);

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

                        string pathToobjectLocationFile = pathToTheme + "ObjLocation" + Values["PAGEINDEX"] + ".txt";
                        string objectLocation = (new StreamReader(pathToobjectLocationFile)).ReadToEnd();
                        
                        string pathToButtonLocationsFile = pathToTheme + "ButtonLocation" + Values["PAGEINDEX"] + ".txt";
                        string buttonLocations = (new StreamReader(pathToButtonLocationsFile)).ReadToEnd();

                        Values["TRAILER"] = d.TRAILER;
                        if (Values["TRAILER"].ToString() != "media\\blank.mpg")
                        {
                            Values["HIDETRAILER"] = "";
                        }
                        else
                        {
                            Values["HIDETRAILER"] = "//";
                        }

                        defs += ThemeManager.ReplaceVals(buttonDef, Values);
                        defObjs += ThemeManager.ReplaceVals(objDef, Values);
                        locationsObJ += ThemeManager.ReplaceVals(objectLocation, Values);
                        locationsBut += ThemeManager.ReplaceVals(buttonLocations, Values);
                        actions += ThemeManager.ReplaceVals(butActions, Values);
                        objFilestxt += ThemeManager.ReplaceVals(objFiles, Values);
                    }

                    if (File.Exists(pathToTheme + "alpha.txt"))
                    {
                        defs += ThemeManager.ReplaceVals(alphaDef, Values);
                        locationsBut += ThemeManager.ReplaceVals(alphaLoc, Values);
                        actions += ThemeManager.ReplaceVals(alphaAct, Values);
                    }

                    if (currentPage > 0)
                    {
                        defs += ThemeManager.ReplaceVals(prevDef, Values);
                        locationsBut += ThemeManager.ReplaceVals(prevLoc, Values);
                        actions += ThemeManager.ReplaceVals(prevAct, Values);
                    }

                    if (totalPages > (currentPage + 1))
                    {
                        defs += ThemeManager.ReplaceVals(nextDef, Values);
                        locationsBut += ThemeManager.ReplaceVals(nextLoc, Values);
                        actions += ThemeManager.ReplaceVals(nextAct, Values);
                    }

                    locationsObJ += "</g>\n";
                    locationsBut += "</g>\n";
                    defs += defObjs + "</defs>\n";

                    Values["CONTENT"] = defs + locationsObJ + locationsBut + "</svg>\n" + actions + objFilestxt;

                    pgcs += ThemeManager.ReplaceVals(pgc, Values);
                }
                if (File.Exists(pathToTheme + "alpha.txt"))
                {
                    string allactions = "";
                    Values.Add("alphaletter", "A");
                    Values.Add("alphaaction", "");
                    Values.Add("alphaActions", "");

                    string alpha = (new StreamReader(pathToTheme + "alpha.txt")).ReadToEnd();
                    string alphaActions = (new StreamReader(pathToTheme + "alpha-Actions.txt")).ReadToEnd();
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

                        allactions += ThemeManager.ReplaceVals(alphaActions, Values);
                    }
                    Values["alphaActions"] = allactions;
                    pgcs += ThemeManager.ReplaceVals(alpha, Values);
                }
                Values.Add("PGCS", pgcs);
                Values.Add("TITLESETS", titleSets);
                string mainfile = (new StreamReader(pathToTheme + "Main.txt")).ReadToEnd();
                mainfile = ThemeManager.ReplaceVals(mainfile, Values);

                // Write our project file (XML)
                var projectFile = new StreamWriter(WorkingDirectory + "\\project.xml", false);
                var chrArray = new char[1];
                chrArray[0] = '\n';
                string[] strArrays1 = mainfile.Split(chrArray);
                int num1 = 0;
                while (num1 < strArrays1.Length)
                {
                    string line = strArrays1[num1];
                    if (!line.Trim().StartsWith("//"))
                    {
                        projectFile.Write(line);
                    }
                    num1++;
                }
                projectFile.Close();

                // Update UI
                buttonBuildProject.Enabled = false;
                buttonTranscodeMenu.Enabled = true;
                buttonCopyToDrive.Enabled = false;

                chkArtwork.Enabled = false;
                chkTraillers.Enabled = false;

                buttonTranscodeMenu.Focus();

                Log.Text += Environment.NewLine;
                Log.Text += "Step 1 of 3 Complete." + Environment.NewLine + Environment.NewLine;

            }
            else
            {
                MessageBox.Show("No Games found.");
                comboBoxDriveList.Enabled = true;
                comboBoxThemeList.Enabled = true;
            }
        }

        private int FirstLocationAlpha(string letter, int buttonCount)
        {
            ISO[] orderedISO = (from ISO gameISO in GameISOs orderby gameISO.Gamename select gameISO).ToArray();

            int num = 0;
            while (num < orderedISO.Length)
            {
                ISO gameISO = orderedISO[num];

                if (!gameISO.Gamename.ToUpper().StartsWith(letter))
                {
                    num++;
                }
                else
                {
                    return gameISO.Page;
                }
            }

            return -1;
        }

        private void CreateSectorMap()
        {
            //Log.Text += "Awaiting Code";
            Log.Text += "Creating Sector Map..." + Environment.NewLine;

            var sectorMapFile = new StreamWriter(WorkingDirectory + "dvd.xsk", false);
            try
            {
                List<byte[]> sectors = new DvdMenuReadSectors(WorkingDirectory + "dvd.iso").FillListWithMenuSectors();

                SHA1 sha = new SHA1CryptoServiceProvider();
                int i = 0;

                ISO[] orderedISO = (from ISO gameISO in GameISOs orderby gameISO.Gamename select gameISO).ToArray();

                foreach (var sector in sectors)
                {
                    var encoding = new UTF8Encoding();
                    byte[] data =
                        encoding.GetBytes(
                            (orderedISO[i]).Path.Replace(Values["DRIVE"].ToString(), "").Replace("\\", "/").
                                Substring("/game/".Length));
                    sectorMapFile.BaseStream.Write(sector, 0, 4);
                    byte[] hash = sha.ComputeHash(data);
                    sectorMapFile.BaseStream.Write(hash, 0, hash.Length);
                    i += 1;
                }
                sectorMapFile.BaseStream.Flush();
                sectorMapFile.Flush();
                sectorMapFile.Close();
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
            foreach (DirectoryInfo theme in themeList)
            {
                comboBoxThemeList.Items.Add(theme.Name);
            }
            comboBoxThemeList.SelectedIndex = 0;
        }

        private bool isDVDStylerInstalled()
        {
            PathToDVDStyler = ProgramFilesx86() + "\\DVDStyler\\bin\\DVDStyler.exe";

            if (File.Exists(PathToDVDStyler))
            {
                // DVDStyler doesn't store version info in its executable...
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

        private bool isVLCInstalled()
        {
            PathToVLC = ProgramFilesx86() + "\\VideoLAN\\VLC\\vlc.exe";
            return File.Exists(PathToVLC);
        }
        
        private void SetupWorkingDirectory()
        {
            // Current User Roaming App Data - equivalent to %AppData% if set
            string pathToRoamingAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Combine the App Data\Roaming path with our desired directory name
            WorkingDirectory = Path.Combine(pathToRoamingAppData, "xk3y-DVDMenu\\");

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
                buttonBuildProject.Enabled = false;
            }

            SetupWorkingDirectory();

            Log.Text += Environment.NewLine;

            // TODO: This one creates error, cannot find plugin - will add plugin support in next build - Diag
            //LoadPlugins();
            LoadDisks();
            LoadThemes();

            this.ActiveControl = buttonBuildProject;
        }

        private void RecursiveISOSearch(string targetDirectory)
        {
            foreach (FileInfo fileInfo in new DirectoryInfo(targetDirectory).GetFiles("*.ISO"))
            {
                GameISOs.Add(new ISO
                                   {
                                       Filename = fileInfo.Name,
                                       Gamename = Regex.Replace(fileInfo.Name, ".iso", "", RegexOptions.IgnoreCase),
                                       Path = fileInfo.FullName,
                                       IsoFile = fileInfo,
                                       Log = Log,
                                       WorkingDirectory = WorkingDirectory,
                                       ProgressBar1 = progressBar1
                                   });
            }
            try
            {
                foreach (string directory in Directory.GetDirectories(targetDirectory))
                {
                    RecursiveISOSearch(directory);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void buttonBuildProject_Click(object sender, EventArgs e)
        {
            comboBoxDriveList.Enabled = false;
            comboBoxThemeList.Enabled = false;
            FetchGameDataAndCreateProject();
        }

        private void buttonTranscodeMenu_Click(object sender, EventArgs e)
        {

            if (File.Exists(WorkingDirectory + "dvd.iso"))
            {
                try
                {
                    File.Delete(WorkingDirectory + "dvd.iso");
                }
                catch (Exception)
                {
                    MessageBox.Show("Unable to delete existing ISO file");
                    return;
                }
            }

            Log.Text += "Launching DVDStyler..." + Environment.NewLine;

            var start = new ProcessStartInfo
                            {
                                FileName = PathToDVDStyler,
                                Arguments = " --stderr --start \"" + WorkingDirectory + "project.xml\"",
                            };

            using (Process DVDStylerProcess = Process.Start(start))
            {
                DVDStylerProcess.WaitForExit();
                foreach (FileInfo fileInfo in new DirectoryInfo(WorkingDirectory).GetFiles("*.vob"))
                {
                    fileInfo.Delete();
                }

                if (File.Exists(WorkingDirectory + "dvd.iso"))
                {
                    CreateSectorMap();

                    buttonBuildProject.Enabled = false;
                    buttonTranscodeMenu.Enabled = false;
                    buttonCopyToDrive.Enabled = true;

                    buttonCopyToDrive.Focus();

                    Log.Text += Environment.NewLine;
                    Log.Text += "Step 2 of 3 Complete." + Environment.NewLine + Environment.NewLine;

                    // Preview before copying to drive
                    if (isVLCInstalled())
                    {
                        DialogResult dialogResult = MessageBox.Show("Preview DVDMenu in VLC?", "Step 2 of 3 Complete", MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            var previewInVLC = new ProcessStartInfo
                            {
                                FileName = PathToVLC,
                                Arguments = " \"" + WorkingDirectory + "dvd.iso\"",
                            };
                            Process.Start(previewInVLC);
                        }
                    }

                }
                else
                {
                    MessageBox.Show("ISO creation failed!");
                }
            }
        }
        
        private void buttonCopyToDrive_Click(object sender, EventArgs e)
        {
            try
            {
                File.Copy(WorkingDirectory + "dvd.iso", Values["DRIVE"] + "games\\menu.xso", true);
                File.Copy(WorkingDirectory + "dvd.xsk", Values["DRIVE"] + "games\\menu.xsk", true);
                Log.Text += "Step 3 of 3 Complete." + Environment.NewLine;

                buttonCopyToDrive.Enabled = false;

                MessageBox.Show("DVDMenu has been copied to your drive.\n" +
                                "Make sure your xkey.cfg has MENUDVD=Y and enjoy :)",
                                "Step 3 of 3 Complete");
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Could not copy to drive.");
            }
        }
        
        private void Log_TextChanged(object sender, EventArgs e)
        {
            Log.SelectionStart = Log.Text.Length;
            Log.ScrollToCaret();
        }

        private void Log_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+A support
            if (e.Control & e.KeyCode == Keys.A)
            {
                Log.SelectAll();
            }
        }

        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            Process.Start("http://k3yforums.com/");
        }

        private void pictureBoxLogo_MouseHover(object sender, EventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.SetToolTip(this.pictureBoxLogo, "Visit k3y Forums");
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

        private void comboBoxThemeList_DropDown(object sender, EventArgs e)
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

    }
}