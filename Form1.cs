using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Ini;
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

        public ArrayList ArrIsolist = new ArrayList();
        public Dictionary<string, object> Values = new Dictionary<string, object>();

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
            
            Log.Text += "Loading disk drives" + Environment.NewLine;
            IEnumerable<string> driveList = from d in DriveInfo.GetDrives() select d.Name;
            cmbDrive.Items.Clear();
            foreach (string item in driveList)
            {
                cmbDrive.Items.Add(item);
            }
            cmbDrive.SelectedIndex = 0;
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
                    (from ISO d in ArrIsolist orderby d.Gamename select d).Skip(currentTitleSet*titlesetIsoLimit).Take(
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

        private void CreateDVDStyleXML()
        {
            TextBox log = Log;
            log.Text = string.Concat(log.Text, "Creating dvds", Environment.NewLine);
            Values.Clear();
            var startupPath = new object[4];
            startupPath[0] = Application.StartupPath;
            startupPath[1] = "\\themes\\";
            startupPath[2] = cmbTheme.SelectedItem;
            startupPath[3] = "\\";
            string themepath = string.Concat(startupPath);
            log.Text = string.Concat(log.Text, "Finding ISO", Environment.NewLine);
            Values.Add("DRIVE", cmbDrive.SelectedItem);
            ArrIsolist.Clear();
            if (Directory.Exists(string.Concat(Values["DRIVE"], "\\games\\")))
            {
                DirSearch(string.Concat(Values["DRIVE"], "\\games\\"));
            }
            if (ArrIsolist.Count != 0)
            {
                TextBox log1 = Log;
                log1.Text = string.Concat(log1.Text, "Set init", Environment.NewLine);
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

                string pgc = (new StreamReader(string.Concat(themepath, "PGC.txt"))).ReadToEnd();
                int buttonCount =
                    (from d in new DirectoryInfo(themepath).GetFiles("ButtonLocation*.txt") select d).Count();
                double totalPageCount = Math.Ceiling(ArrIsolist.Count/(double) buttonCount);
                Values.Add("TotalPageCount", totalPageCount);
                string buttonDef = (new StreamReader(string.Concat(themepath, "ButtonStyle.txt"))).ReadToEnd();
                string objDef = (new StreamReader(string.Concat(themepath, "GAMEOBJ.txt"))).ReadToEnd();
                string butActions = (new StreamReader(string.Concat(themepath, "ButtonActions.txt"))).ReadToEnd();
                string objFiles = (new StreamReader(string.Concat(themepath, "OBJFiles.txt"))).ReadToEnd();
                string prevDef = (new StreamReader(string.Concat(themepath, "PrevButtonStyle.txt"))).ReadToEnd();
                string prevLoc = (new StreamReader(string.Concat(themepath, "PrevButtonLocation.txt"))).ReadToEnd();
                string prevAct = (new StreamReader(string.Concat(themepath, "PrevButtonAction.txt"))).ReadToEnd();
                string nextDef = (new StreamReader(string.Concat(themepath, "NextButtonStyle.txt"))).ReadToEnd();
                string nextLoc = (new StreamReader(string.Concat(themepath, "NextButtonLocation.txt"))).ReadToEnd();
                string nextAct = (new StreamReader(string.Concat(themepath, "NextButtonAction.txt"))).ReadToEnd();
                string alphaDef = (new StreamReader(string.Concat(themepath, "alphaButtonStyle.txt"))).ReadToEnd();
                string alphaLoc = (new StreamReader(string.Concat(themepath, "alphaButtonLocation.txt"))).ReadToEnd();
                string alphaAct = (new StreamReader(string.Concat(themepath, "alphaButtonAction.txt"))).ReadToEnd();

                string pgcs = "";


                ISO[] orderedISO =
                    (from ISO d in ArrIsolist orderby d.Gamename select d).ToArray();
                LoadGameDetails(orderedISO, buttonCount);
                string titleSets = CreateDvdStylerTitleSets(orderedISO, TitlesetLimit, themepath);

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
                        (from ISO d in ArrIsolist orderby d.Gamename select d).Skip(i*buttonCount).Take(buttonCount);
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
                        item[0] = themepath;
                        item[1] = "ObjLocation";
                        item[2] = Values["PAGEINDEX"];
                        item[3] = ".txt";
                        string objLocation = (new StreamReader(string.Concat(item))).ReadToEnd();
                        var objArray = new object[4];
                        objArray[0] = themepath;
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
                    if (File.Exists(string.Concat(themepath, "alpha.txt")))
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
                if (File.Exists(string.Concat(themepath, "alpha.txt")))
                {
                    string allactions = "";
                    Values.Add("alphaletter", "A");
                    Values.Add("alphaaction", "");
                    Values.Add("alphaActions", "");
                    //const int alphahit = 0;
                    string alpha = (new StreamReader(string.Concat(themepath, "alpha.txt"))).ReadToEnd();
                    string alphaActions = (new StreamReader(string.Concat(themepath, "alpha-Actions.txt"))).ReadToEnd();
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
                string mainfile = (new StreamReader(string.Concat(themepath, "Main.txt"))).ReadToEnd();
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
                MessageBox.Show("Done! Start step 2");
                btScan.Enabled = false;
                btDVDStyle.Enabled = true;
                btCopy.Enabled = false;
            }
            else
            {
                MessageBox.Show("No Games found");
                cmbDrive.Enabled = true;
            }
        }

        private int FirstLocationAlpha(string letter, int buttonCount)
        {
            ISO[] orderedISO =
                (from ISO d in ArrIsolist orderby d.Gamename select d).ToArray();

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
                    (from ISO d in ArrIsolist orderby d.Gamename select d).ToArray();

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
            Log.Text += "Loading themes from path:" + Application.StartupPath + "\\themes\\" + Environment.NewLine;
            cmbTheme.Items.Clear();
            var themepaths = new DirectoryInfo(Application.StartupPath + "\\themes\\");
            foreach (DirectoryInfo folder in themepaths.GetDirectories())
            {
                Log.Text += "Found theme:" + folder.Name + Environment.NewLine;
                cmbTheme.Items.Add(folder.Name);
            }
            cmbTheme.SelectedIndex = 0;
        }


        private void LoadPlugins()
        {
            //Todo: Working on plugin support for scrappers
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

        private void Form1Load(object sender, EventArgs e)
        {
            // TODO: This one creates error, cannot find plugin - will add plugin support in next build - Diag
            //LoadPlugins();


            LoadDisks();
            LoadThemes();
        }


        private void DirSearch(string sDir)
        {
            foreach (FileInfo f in new DirectoryInfo(sDir).GetFiles("*.ISO"))
            {
                ArrIsolist.Add(new ISO
                                   {
                                       Filename = f.Name,
                                       Gamename = f.Name.Replace(".iso", ""),
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
                    DirSearch(d);
                }
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void Button1Click(object sender, EventArgs e)
        {
            cmbDrive.Enabled = false;
            CreateDVDStyleXML();
        }

        private void Button2Click(object sender, EventArgs e)
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
            string driveletter = GetAvailableDriveLetters().First();
            Log.Text += "Mapping " + driveletter + ": to startup folder" + Environment.NewLine;
            DefineDosDevice(0, driveletter.ToUpper() + ":",
                            dvdStylePath.ToLower().Replace("dvdstyler.exe", "") + "..\\");

            //var exception = Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error());
            // MessageBox.Show(exception.Message);


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
                    MessageBox.Show("Done! Start step 3");
                    btScan.Enabled = false;
                    btDVDStyle.Enabled = false;
                    btCopy.Enabled = true;
                }
                else
                {
                    MessageBox.Show("ISO creation failed! Retry!");
                }
            }
        }

        private void Button4Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Button3Click(object sender, EventArgs e)
        {
            try
            {
                File.Copy(Application.StartupPath + "\\dvd.iso", cmbDrive.SelectedItem + "games\\Menu.xso", true);
                File.Copy(Application.StartupPath + "\\Menu.xsk", cmbDrive.SelectedItem + "games\\Menu.xsk", true);
                MessageBox.Show("Complete");
                Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Failed. Retry!");
            }
        }


        public List<string> GetAvailableDriveLetters()
        {
            var letters = new List<string>();
            //first let's get all avilable drive letters
            for (int i = Convert.ToInt16('a'); i < Convert.ToInt16('z'); i++)
                letters.Add(new string(new[] {(char) i}));
            //now loop through each and remove it's drive letter from our list
            foreach (DriveInfo drive in DriveInfo.GetDrives())
                letters.Remove(drive.Name.Substring(0, 1).ToLower());
            //return the letters left
            return letters;
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

        private void button5_Click_1(object sender, EventArgs e)
        {
            CreateSectorMap();
        }

        private void Log_TextChanged(object sender, EventArgs e)
        {
            Log.SelectionStart = Log.Text.Length;
            Log.ScrollToCaret();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            CreateSectorMap();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://k3yforums.com/");
        }
    }
}