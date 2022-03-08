using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Timers;
using System.Windows.Interop;

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable IDE0090 // Use 'new(...)'
#pragma warning disable CS4014 // Call is not awaited
#pragma warning disable IDE0051 // Remove unused private members

namespace Funkin
{
    //44,3310394597799
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static double curVol = 1;
        static float volTime = 0;
        static Image volBG;
        static TextBlock volTXT;

        static float memUsage = 0f;
        static float cpuTime = 0f;
        static float peakMemUsage = 0f;
        static bool debugOpen = false;

        static TextBlock debug_cpuTime;
        static TextBlock debug_memUsage;
        static TextBlock debug_peakMemUsage;

        static TextBlock debug_version;

        static Image debug_bg1;
        static Image debug_bg2;
        static Image debug_logo1;
        static TextBlock debug_name;

        static TextBlock fps_count;

        // MENU
        static MediaPlayer player = new();
        static int BPM = 100;
        static int mili_BPM = 600;

        static bool pressedEnter;

        static string[] randTXT;
        static string wtrmrkTXT;
        static string verTXT;

        static Image fadeIN;
        static Image fadeOUT;
        static Image fade;
        static Image logo;
        static Image gf;
        static Image flash;
        static TextBlock enter;
        static TextBlock enter_flash;

        static Image BG1;
        static Image BG2;

        static Image FREEPLAY;
        static Image OPTIONS;

        static float CamX_MENU = 0f;
        static float CamY_MENU = 0f;
        static float camSpeed_MENU = 5f;

        static Key curKey;
        //MENU END

        //SOMETHING
        public static Window wdw;

        public MainWindow()
        {
            //LogMessage("HELLO WORLD!!");
            wtrmrkTXT = File.ReadAllText(Environment.CurrentDirectory + "/assets/watermark.txt");
            verTXT = File.ReadAllText(Environment.CurrentDirectory + "/assets/version.txt");
            InitializeComponent(); LogMessage("WINDOW INIT CALLED");
            wdw = this;
            CheckFocus();
            focused = true;
            ForceWindowSize();
            fps_count = FindName("FPS_COUNTER") as TextBlock;
            StartFPS();
            //CheckGC(); this shit is useless
            volBG = FindName("VOL_BG") as Image;
            volTXT = FindName("VOL_TEXT") as TextBlock;
            CheckInputVolume();
            TextBlock tr = FindName("WTRMRK") as TextBlock;
            tr.Text = wtrmrkTXT + verTXT;
            PlayMusic();
            Title = "Friday Night Funkin'";
            fadeIN = FindName("FADE_IN") as Image;
            fadeOUT = FindName("FADE_OUT") as Image;
            fade = FindName("FADE_MASK") as Image;
            
            //Setting Debug
            debug_memUsage = FindName("DEBUG_MEM_USAGE") as TextBlock;
            debug_cpuTime = FindName("DEBUG_CPU_TIME") as TextBlock;
            debug_peakMemUsage = FindName("DEBUG_PEAK_MEM") as TextBlock;

            debug_bg1 = FindName("DEBUG_BG1") as Image;
            debug_bg2 = FindName("DEBUG_BG2") as Image;

            debug_logo1 = FindName("DEBUG_LOGO1") as Image;
            debug_name = FindName("DEBUG_TEXT1") as TextBlock;

            debug_version = FindName("DEBUG_VERSION") as TextBlock;

            debug_version.Text = "ver" + verTXT;

            BitmapImage bitt1 = new BitmapImage();
            bitt1.BeginInit();
            bitt1.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/blk.png");
            bitt1.EndInit();
            debug_bg1.Source = bitt1;
            debug_bg2.Source = bitt1;

            BitmapImage bitt2 = new BitmapImage();
            bitt2.BeginInit();
            bitt2.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/c#logo.png");
            bitt2.EndInit();
            debug_logo1.Source = bitt2;

            getUsage();
            SetDebug();
            GetDebug();

            string[] elementNames = { "Creators1", "Creators2", "Creators3", "Creators4", "DDev", "Present" };
            TextBlock[] texts = { FindName(elementNames[0]) as TextBlock, FindName(elementNames[1]) as TextBlock, FindName(elementNames[2]) as TextBlock, FindName(elementNames[3]) as TextBlock, FindName(elementNames[4]) as TextBlock };
            TextBlock present = FindName(elementNames[5]) as TextBlock;
            TextBlock[] ass = { FindName("In_Association") as TextBlock, FindName("Nobody") as TextBlock };
            TextBlock[] rand = { FindName("Rand1") as TextBlock, FindName("Rand2") as TextBlock };
            TextBlock[] fnfText = { FindName("fnf1") as TextBlock, FindName("fnf2") as TextBlock, FindName("fnf3") as TextBlock };
            enter = FindName("PRESS_ENTER") as TextBlock;
            enter_flash = FindName("FLASH_ENTER") as TextBlock;
            TextBlock press2 = FindName("FLASH_ENTER") as TextBlock;
            Image img = FindName("Flash_ENTER") as Image;
            logo = FindName("LOGO_ENTER") as Image;
            logo.Visibility = Visibility.Hidden;
            gf = FindName("GF_ENTER") as Image;
            gf.Visibility = Visibility.Hidden;
            AnimateLogo(logo);
            AnimGF();

            FREEPLAY = FindName("FP") as Image;
            OPTIONS = FindName("OPT") as Image;

            BG1 = FindName("BG") as Image;
            BG2 = FindName("BGBlue") as Image;

            BG1.Visibility = Visibility.Hidden;
            BG2.Visibility = Visibility.Hidden;
            FREEPLAY.Visibility = Visibility.Hidden;
            OPTIONS.Visibility = Visibility.Hidden;

            BitmapImage bitBG1 = new BitmapImage();
            bitBG1.BeginInit();
            bitBG1.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/menuBG.png");
            bitBG1.EndInit();
            BG.Source = bitBG1;

            BitmapImage bitBG2 = new BitmapImage();
            bitBG2.BeginInit();
            bitBG2.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/menuBGMagenta.png");
            bitBG2.EndInit();
            BGBlue.Source = bitBG2;

            BitmapImage bitFP = new BitmapImage();
            bitFP.BeginInit();
            bitFP.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/freeplay basic0000.png");
            bitFP.EndInit();
            FP.Stretch = Stretch.Fill;
            FP.Source = bitFP;

            BitmapImage bitOPT = new BitmapImage();
            bitOPT.BeginInit();
            bitOPT.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/options basic0000.png");
            bitOPT.EndInit();
            OPT.Stretch = Stretch.Fill;
            OPT.Source = bitOPT;


            enter.Visibility = Visibility.Hidden;
            press2.Visibility = Visibility.Hidden;
            AnimateText(enter);

            foreach (TextBlock tb in texts)
            {
                tb.Visibility = Visibility.Hidden;
            }
            present.Visibility = Visibility.Hidden;
            foreach (TextBlock tbb in ass)
            {
                tbb.Visibility = Visibility.Hidden;
            }
            foreach (TextBlock tbbb in rand)
            {
                tbbb.Visibility = Visibility.Hidden;
            }
            foreach (TextBlock tbbbb in fnfText)
            {
                tbbbb.Visibility = Visibility.Hidden;
            }
            img.Visibility = Visibility.Hidden;

            ShowTexts(texts, present, ass, rand, fnfText, img, logo, enter); LogMessage("SHOW TEXT ANIMATION\nUSER PROBABLY SEES THE WINDOW!");
        }

        public static bool focused = true;

        static async Task CheckFocus()
        {
            while(true)
            {
                focused = IsApplicationActive();
                focusVol();
                await Task.Delay(1);
            }
        }

        static double lastVol;
        static bool lastFocus;
        static void focusVol()
        {
            if(lastFocus != focused)
            {
                //Value changed!
                if(lastFocus == false && focused == true)
                {
                    lastFocus = focused;
                    curVol = lastVol;
                }
                else if(lastFocus == true && focused == false)
                {
                    lastFocus = focused;
                    lastVol = curVol;
                    curVol = 0;
                }
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        private static bool IsActive(Window wnd)
        {
            // workaround for minimization bug
            // Managed .IsActive may return wrong value
            if (wnd == null) return false;
            return GetForegroundWindow() == new WindowInteropHelper(wnd).Handle;
        }

        public static bool IsApplicationActive()
        {
            foreach (var wnd in Application.Current.Windows.OfType<Window>())
                if (IsActive(wnd)) return true;
            return false;
        }

        static async Task ShowTexts(TextBlock[] txts, TextBlock prst, TextBlock[] ass, TextBlock[] rand, TextBlock[] fnf, Image img, Image img2, TextBlock press) //not THAT ass ;)
        {
            string r = File.ReadAllText(Environment.CurrentDirectory + "/assets/introText.txt"); LogMessage("LOADING introText.txt!!!");
            char[] splitter = { '\n' };
            randTXT = r.Split(splitter);
            txts[0].Visibility = Visibility.Visible;
            txts[1].Visibility = Visibility.Visible;
            txts[2].Visibility = Visibility.Visible;
            txts[3].Visibility = Visibility.Visible;
            txts[4].Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 2);
            prst.Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 1);
            foreach (TextBlock tb in txts)
            {
                tb.Visibility = Visibility.Hidden;
            }
            prst.Visibility = Visibility.Hidden;

            await Task.Delay(mili_BPM * 1);
            ass[0].Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 2);
            ass[1].Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 1);
            ass[0].Visibility = Visibility.Hidden;
            ass[1].Visibility = Visibility.Hidden;
            await Task.Delay(mili_BPM * 1);

            Random rnd = new();
            int randI = rnd.Next(0, randTXT.Length);
            string[] rsp = randTXT[randI].Split('-');
            string r1 = rsp[0];
            string r2 = rsp[2];

            //rand[0].Text = "THIS IS A GODDAMN PROTOTYPE";
            //rand[1].Text = "IM WORKIN ON IT OKAY";
            rand[0].Text = r1.ToUpper();
            rand[1].Text = r2.ToUpper();

            rand[0].Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 2);
            rand[1].Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 1);
            rand[0].Visibility = Visibility.Hidden;
            rand[1].Visibility = Visibility.Hidden;
            await Task.Delay(mili_BPM * 1);

            fnf[0].Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 1);
            fnf[1].Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 1);
            fnf[2].Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM * 1);
            fnf[0].Visibility = Visibility.Hidden;
            fnf[1].Visibility = Visibility.Hidden;
            fnf[2].Visibility = Visibility.Hidden;
            await Task.Delay(mili_BPM * 1);

            BitmapImage bit = new BitmapImage();
            bit.BeginInit();
            bit.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/empty.png");
            bit.EndInit();
            img.Stretch = Stretch.Fill;
            img.Source = bit;

            flash = img;
            ShowEnter(); LogMessage("SHOWING ENTER!!!");
        }

        static async Task WaitForEnter()
        {
            while(!pressedEnter)
            {
                EventManager.RegisterClassHandler(typeof(Window),
                Keyboard.KeyUpEvent, new KeyEventHandler(keyUp), true);

                async void keyUp(object sender, KeyEventArgs e)
                {
                    if (e.Key == Key.Enter && !pressedEnter)
                    {
                        pressedEnter = true; LogMessage("ENTER PRESSED!!!");
                        SoundPlayer sp = new();
                        FlashText();
                        await sp.PlaySound(Environment.CurrentDirectory + "/assets/music/confirm.wav", curVol);

                        logo.Visibility = Visibility.Hidden;
                        enter.Visibility = Visibility.Hidden;
                        ShowMenu();
                    }
                }
                await Task.Delay(1);
            }
        }

        static async Task ShowEnter()
        {
            flash.Opacity = 1;
            flash.Visibility = Visibility.Visible;
            logo.Visibility = Visibility.Visible;
            enter.Visibility = Visibility.Visible;
            gf.Visibility = Visibility.Visible;
            await Task.Delay(mili_BPM / 2);
            while (flash.Opacity > 0)
            {
                flash.Opacity -= 0.01;
                await Task.Delay(10);
            }
            WaitForEnter(); LogMessage("WAITING FOR ENTER...");
        }

        static async Task ShowMenu()
        {
            LogMessage("SHOWING MENU: LOADING BITMAPS");
            BitmapImage bitFP1 = new BitmapImage();
            bitFP1.BeginInit();
            bitFP1.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/freeplay basic0000.png");
            bitFP1.EndInit();
            BitmapImage bitFP2 = new BitmapImage();
            bitFP2.BeginInit();
            bitFP2.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/freeplay white0000.png");
            bitFP2.EndInit();

            BitmapImage bitOPT1 = new BitmapImage();
            bitOPT1.BeginInit();
            bitOPT1.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/options basic0000.png");
            bitOPT1.EndInit();
            BitmapImage bitOPT2 = new BitmapImage();
            bitOPT2.BeginInit();
            bitOPT2.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/options white0000.png");
            bitOPT2.EndInit();


            BG1.Visibility = Visibility.Visible;
            FREEPLAY.Visibility = Visibility.Visible;
            OPTIONS.Visibility = Visibility.Visible; LogMessage("SHOWING MENU: OBJECTS VISIBLE");
            AnimMMLogos();
            FPPOS();
            OPTPOS();
            //BGPOS();
            camSpeed_MENU = 5f;
            MENU_CameraGoToX(0);
            await MENU_CameraGoToY(250);
            FREEPLAY.Source = bitFP2;
            OPTIONS.Source = bitOPT1;
            camSpeed_MENU = 25f;
            while(pressedEnter)
            {
                if(Keyboard.IsKeyDown(Key.Up) && focused)
                {
                    SoundPlayer sp = new(); LogMessage("SHOWING MENU: PRESSED UP!");
                    FREEPLAY.Source = bitFP2;
                    OPTIONS.Source = bitOPT1;
                    sp.PlaySound(Environment.CurrentDirectory + "/assets/music/select.wav", curVol);
                    MENU_CameraGoToX(0);
                    await MENU_CameraGoToY(250);
                }
                else if(Keyboard.IsKeyDown(Key.Down) && focused)
                {
                    SoundPlayer sp = new(); LogMessage("SHOWING MENU: PRESSED DOWN!");
                    FREEPLAY.Source = bitFP1;
                    OPTIONS.Source = bitOPT2;
                    sp.PlaySound(Environment.CurrentDirectory + "/assets/music/select.wav", curVol);
                    MENU_CameraGoToX(0);
                    await MENU_CameraGoToY(-250);
                }
                else if(Keyboard.IsKeyDown(Key.Enter) && focused)
                {
                    if(CamY_MENU == 250)
                    {
                        SoundPlayer sp = new(); LogMessage("SHOWING MENU: ENTER PRESSED!");
                        AnimBG();
                        AnimFP(bitFP1, bitFP2);
                        await sp.PlaySound(Environment.CurrentDirectory + "/assets/music/confirm.wav", curVol);
                        LogMessage("SHOWING MENU: LOADING FREEPLAY!"); 
                        ShowFP(); 
                    }
                    else if (CamY_MENU == -250)
                    {
                        SoundPlayer sp = new(); LogMessage("SHOWING MENU: ENTER PRESSED!");
                        AnimBG();
                        AnimOPT(bitOPT1, bitOPT2);
                        await sp.PlaySound(Environment.CurrentDirectory + "/assets/music/confirm.wav", curVol);
                        LogMessage("SHOWING MENU: LOADING OPTIONS!"); 
                        ShowOPT(); 
                    }
                }
                else if(Keyboard.IsKeyDown(Key.Back) && focused)
                {
                    SoundPlayer sp = new(); LogMessage("SHOWING MENU: BACKSPACE PRESSED!");
                    await sp.PlaySound(Environment.CurrentDirectory + "/assets/music/cancel.wav", curVol);
                    BG1.Visibility = Visibility.Hidden;
                    FREEPLAY.Visibility = Visibility.Hidden;
                    OPTIONS.Visibility = Visibility.Hidden;
                    pressedEnter = false;
                    ShowEnter();
                }

                await Task.Delay(1);
            }
        }

        static async Task AnimFP(BitmapImage arg1, BitmapImage arg2)
        {
            /*
            int i = 0;
            while (i < 13)
            {
                FREEPLAY.Source = arg2;
                await Task.Delay(75);
                FREEPLAY.Source = arg1;
                await Task.Delay(75);
                i++;
            }
            */
            int i = 0;
            while (i < 13)
            {
                FREEPLAY.Visibility = Visibility.Hidden;
                await Task.Delay(75);
                FREEPLAY.Visibility = Visibility.Visible;
                await Task.Delay(75);
                i++;
            }
        }
        static async Task AnimOPT(BitmapImage arg1, BitmapImage arg2)
        {
            /*
            int i = 0;
            while (i < 13)
            {
                OPTIONS.Source = arg2;
                await Task.Delay(75);
                OPTIONS.Source = arg1;
                await Task.Delay(75);
                i++;
            }
            */
            int i = 0;
            while (i < 13)
            {
                OPTIONS.Visibility = Visibility.Hidden;
                await Task.Delay(75);
                OPTIONS.Visibility = Visibility.Visible;
                await Task.Delay(75);
                i++;
            }
        }

        static async Task AnimBG()
        {
            int i = 0;
            while(i < 13)
            {
                BG2.Visibility = Visibility.Hidden;
                BG1.Visibility = Visibility.Visible;
                await Task.Delay(75);
                BG1.Visibility = Visibility.Hidden;
                BG2.Visibility = Visibility.Visible;
                await Task.Delay(75);
                i++;
            }
            BG2.Visibility = Visibility.Hidden;
            BG1.Visibility = Visibility.Visible;
        }

        static async Task ShowFP()
        {
            MessageBox.Show("Feature not implemented!");
            LogWarning("SHOWFP: FEATURE NOT IMPLEMENTED!");
        }
        static async Task ShowOPT()
        {
            MessageBox.Show("Feature not implemented!");
            LogWarning("SHOWFP: FEATURE NOT IMPLEMENTED!");
        }

        static Thickness thMML = new();

        static async Task AnimMMLogos()
        {
            while (true)
            {
                int i = 0;
                float x = 0;
                float y = 0;
                while (i < 5)
                {
                    x += 1f;
                    y += 1f;
                    thMML.Bottom = y;
                    thMML.Left = x;
                    await Task.Delay(5);
                    i++;
                }
                await Task.Delay(10);
            }
        }

        static async Task FPPOS()
        {
            Thickness m = new();
            while(true)
            {
                m.Bottom = (250 + thMML.Bottom) - CamY_MENU;
                m.Left = (0 + thMML.Left) - CamX_MENU;

                FREEPLAY.Margin = m;
                await Task.Delay(1);
            }
        }
        static async Task OPTPOS()
        {
            Thickness m = new();
            while (true)
            {
                m.Bottom = (-250 + thMML.Bottom) - CamY_MENU;
                m.Left = (0 + thMML.Left) + CamX_MENU;

                OPTIONS.Margin = m;
                await Task.Delay(1);
            }
        }
        static async Task BGPOS()
        {
            Thickness m = new();
            //ScaleTransform sc = new();
            while (true)
            {
                //BG1.RenderTransform = sc;
                //BG2.RenderTransform = sc;

                m.Top = -384 + (CamY_MENU / 2);
                m.Left = -633 + (CamX_MENU / 2);

                BG1.Margin = m;
                BG2.Margin = m;
                await Task.Delay(1);
            }
        }

        static async Task getUsage()
        {
            while (true)
            {
                Process proc = Process.GetCurrentProcess();
                memUsage = (float)Math.Round((proc.PrivateMemorySize64 / 1024f) / 1024f);
                cpuTime = (float)proc.TotalProcessorTime.TotalSeconds;
                if(memUsage > peakMemUsage)
                {
                    peakMemUsage = memUsage;
                }

                await Task.Delay(10);
            }
        }

        static async Task GetDebug()
        {
            while(true)
            {
                if(debugOpen)
                {
                    if(Keyboard.IsKeyDown(Key.OemTilde) && focused)
                    {
                        debugOpen = false; LogMessage("ENGINE: DEBUGGER CLOSED");
                        while (Keyboard.IsKeyDown(Key.OemTilde))
                        {
                            await Task.Delay(1);
                        }
                    }
                }
                else
                {
                    if (Keyboard.IsKeyDown(Key.OemTilde) && focused)
                    {
                        debugOpen = true; LogMessage("ENGINE: DEBUGGER OPENED");
                        while (Keyboard.IsKeyDown(Key.OemTilde))
                        {
                            await Task.Delay(1);
                        }
                    }
                }
                await Task.Delay(1);
            }
        }

        static async Task SetDebug()
        {
            while(true)
            {
                if(debugOpen)
                {
                    debug_bg1.Visibility = Visibility.Visible;
                    debug_bg2.Visibility = Visibility.Visible;
                    debug_cpuTime.Visibility = Visibility.Visible;
                    debug_logo1.Visibility = Visibility.Visible;
                    debug_memUsage.Visibility = Visibility.Visible;
                    debug_name.Visibility = Visibility.Visible;
                    debug_peakMemUsage.Visibility = Visibility.Visible;
                    debug_version.Visibility = Visibility.Visible;

                    debug_cpuTime.Text = "time=" + cpuTime + "sec";
                    debug_memUsage.Text = "mem=" + memUsage;
                    debug_peakMemUsage.Text = "peak_mem=" + peakMemUsage;
                }
                else
                {
                    debug_bg1.Visibility = Visibility.Hidden;
                    debug_bg2.Visibility = Visibility.Hidden;
                    debug_cpuTime.Visibility = Visibility.Hidden;
                    debug_logo1.Visibility = Visibility.Hidden;
                    debug_memUsage.Visibility = Visibility.Hidden;
                    debug_name.Visibility = Visibility.Hidden;
                    debug_peakMemUsage.Visibility = Visibility.Hidden;
                    debug_version.Visibility = Visibility.Hidden;
                }
                await Task.Delay(1);
            }
        }

        public static void LogMessage(string message)
        {
            string source;
            string log;

            source = "Funkin.cs";
            log = "Application";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, log);

            EventLog.WriteEntry(source, message, EventLogEntryType.Information);
            Console.WriteLine("[" + source + "] [ INFO ] " + message);
            Debug.WriteLine("[" + source + "] [ INFO ] " + message);
        }

        public static void LogWarning(string message)
        {
            string source;
            string log;

            source = "Funkin.cs";
            log = "Application";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, log);

            EventLog.WriteEntry(source, message, EventLogEntryType.Warning);
            Console.WriteLine("[" + source + "] [ WARN ] " + message);
            Debug.WriteLine("[" + source + "] [ WARN ] " + message);
        }

        public static void LogError(string message)
        {
            string source;
            string log;

            source = "Funkin.cs";
            log = "Application";

            if (!EventLog.SourceExists(source))
                EventLog.CreateEventSource(source, log);

            EventLog.WriteEntry(source, message, EventLogEntryType.Error);
            Console.WriteLine("[" + source + "] [ ERR ] " + message);
            Debug.WriteLine("[" + source + "] [ ERR ] " + message);
        }

        /*
        static async Task GetInput()
        {
            while (true)
            {
                EventManager.RegisterClassHandler(typeof(Window),
                    Keyboard.KeyUpEvent, new KeyEventHandler(keyUp), true);

                async void keyUp(object sender, KeyEventArgs e)
                {
                    curKey = e.Key;
                    MessageBox.Show("key:" + e.Key.ToString());
                    await Keyboard.IsKeyDown;
                }
                await Task.Delay(1);
            }
        }
        */

        static async Task FadeIn()
        {
            fadeIN.Visibility = Visibility.Visible;
            int i = 0;
            Thickness m = new();
            while(i < 750)
            {
                m.Bottom = i;
                fadeIN.Margin = m;
                i++;
                await Task.Delay(1);
            }
            fadeIN.Visibility = Visibility.Hidden;
        }
        static async Task FadeOut()
        {
            fadeOUT.Visibility = Visibility.Visible;
            int i = 0;
            Thickness m = new();
            while (i < 1000)
            {
                m.Top = -250 + i;
                fadeOUT.Margin = m;
                i++;
                await Task.Delay(1);
            }
            fadeOUT.Visibility = Visibility.Hidden;
        }

        static async Task FadeI()
        {
            fade.Visibility = Visibility.Visible;
            float f = 1f;
            while(f > 0)
            {
                fade.Opacity = f;
                f -= 0.01f;
                await Task.Delay(1);
            }
            fade.Visibility = Visibility.Hidden;
        }

        static async Task FadeO()
        {
            fade.Visibility = Visibility.Visible;
            float f = 0f;
            while (f < 1)
            {
                fade.Opacity = f;
                f += 0.01f;
                await Task.Delay(1);
            }
        }

        static async Task MENU_CameraGoToX(float x)
        {
            //Going to x
            if (x > CamX_MENU)
            {
                while (x > CamY_MENU)
                {
                    CamX_MENU += camSpeed_MENU;
                    await Task.Delay(1);
                }
            }
            else if (x < CamX_MENU)
            {
                while(x < CamX_MENU)
                {
                    CamX_MENU -= camSpeed_MENU;
                    await Task.Delay(1);
                }
            }
        }

        static async Task MENU_CameraGoToY(float y)
        {
            //Going to x
            if (y > CamY_MENU)
            {
                while (y > CamY_MENU)
                {
                    CamY_MENU += camSpeed_MENU;
                    await Task.Delay(1);
                }
            }
            else if (y < CamY_MENU)
            {
                while(y < CamY_MENU)
                {
                    CamY_MENU -= camSpeed_MENU;
                    await Task.Delay(1);
                }
            }
        }

        static async Task ShowVol()
        {
            while(true)
            {
                volTXT.Text = "VOLUME:" + Math.Round(curVol, 1).ToString();
                if(volTime > 0)
                {
                    volBG.Visibility = Visibility.Visible;
                    volTXT.Visibility = Visibility.Visible;
                    if (volTime > 3f)
                    {
                        volTime = 3f;
                    }
                    volTime -= 0.01f;
                }
                else
                {
                    volBG.Visibility = Visibility.Hidden;
                    volTXT.Visibility = Visibility.Hidden;
                }
                await Task.Delay(1);
            }
        }

        static int frame = 0;
        static int FPS = 0;
        static double elapsedMS;
        static Stopwatch stopwatch;
        static async Task StartFPS()
        {
            stopwatch = new();
            stopwatch.Start();
            CountFrames();
            Frames();
            while(true) { fps_count.Text = "render: " + FPS.ToString() + "/" + elapsedMS.ToString(); await Task.Delay(1); }
        }
        static async Task Frames()
        {
            while (true) { FPS = frame; frame = 0; await Task.Delay(1000); }
        }
        static async Task CountFrames()
        {
            while(true) { elapsedMS = stopwatch.ElapsedTicks / 10000; stopwatch.Restart(); frame++; await Task.Delay(1); }
        }

        static async Task CheckGC()
        {
            float cool = 0;
            while (true)
            {
                if (cool > 0) { cool--; }
                if (memUsage > 500 && cool == 0)
                {
                    GC.Collect();
                    cool += 10;
                }
                await Task.Delay(1);
            }
        }

        static async Task CheckInputVolume()
        {
            ShowVol();
            while(true)
            {
                if(Keyboard.IsKeyDown(Key.OemPlus) && focused)
                {
                    if (curVol < 1)
                    {
                        curVol += 0.1;
                        volTime += 0.5f;
                        SoundPlayer pl = new();
                        await pl.PlaySound(Environment.CurrentDirectory + "/assets/music/vol_change.wav", curVol);
                    }
                    while (Keyboard.IsKeyDown(Key.OemPlus))
                    {
                        await Task.Delay(1);
                    }
                }
                else if(Keyboard.IsKeyDown(Key.OemMinus) && focused)
                {
                    if (curVol > 0)
                    {
                        curVol -= 0.1;
                        volTime += 0.5f;
                        SoundPlayer pl = new();
                        await pl.PlaySound(Environment.CurrentDirectory + "/assets/music/vol_change.wav", curVol);
                    }
                    while (Keyboard.IsKeyDown(Key.OemMinus))
                    {
                        await Task.Delay(1);
                    }
                }
                else if(Keyboard.IsKeyDown(Key.D0) && focused)
                {
                    if (curVol > 0)
                    {
                        curVol = 0;
                        volTime += 0.5f;
                        while (Keyboard.IsKeyDown(Key.D0))
                        {
                            await Task.Delay(1);
                        }
                    }
                    else
                    {
                        curVol = 1;
                        volTime += 0.5f;
                        while (Keyboard.IsKeyDown(Key.D0))
                        {
                            await Task.Delay(1);
                        }
                    }
                }
                else if(Keyboard.IsKeyDown(Key.NumPad0) && focused)
                {
                    if (curVol > 0)
                    {
                        curVol = 0;
                        volTime += 0.5f;
                        while (Keyboard.IsKeyDown(Key.D0))
                        {
                            await Task.Delay(1);
                        }
                    }
                    else
                    {
                        curVol = 1;
                        volTime += 0.5f;
                        while (Keyboard.IsKeyDown(Key.D0))
                        {
                            await Task.Delay(1);
                        }
                    }
                }
                await Task.Delay(1);
            }
        }

        void PlayMusic()
        {
            player.Open(new Uri(Environment.CurrentDirectory + "/assets/music/freakyMenu.wav"));
            player.MediaEnded += new EventHandler(Media_Ended);
            player.Volume = curVol;
            player.Play();
            SetPlayerVolume();
        }

        static async Task SetPlayerVolume()
        {
            while(true)
            {
                player.Volume = curVol;
                await Task.Delay(1);
            }
        }

        void Media_Ended(object sender, EventArgs e)
        {
            player.Position = TimeSpan.Zero;
            player.Play();
        }

        static async Task AnimateText(TextBlock txt)
        {
            float g = 55;
            while(true)
            {
                for (int i = 55; i < 255; i++)
                {
                    g = i;
                    txt.Foreground = new SolidColorBrush(Color.FromRgb(0, (byte)g, 255));
                    await Task.Delay(1);
                }

                for (int i = 255; i > 55; i--)
                {
                    g = i;
                    txt.Foreground = new SolidColorBrush(Color.FromRgb(0, (byte)g, 255));
                    await Task.Delay(1);
                }
            }
        }

        static async Task AnimGF()
        {
            BitmapImage[] bits = new BitmapImage[20];
            int e = 0;
            while (e < 20)
            {
                bits[e] = new();
                bits[e].BeginInit();
                bits[e].UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/gf/" + e + ".png");
                bits[e].EndInit();
                e++;
            }

            while (true)
            {
                int i = 0;
                while (i < 20)
                {
                    gf.Source = bits[i];
                    await Task.Delay(40);
                    i++;
                }
                await Task.Delay(15);
            }
        }

        static async Task FlashText()
        {
            enter.Visibility = Visibility.Hidden;
            enter_flash.Visibility = Visibility.Visible;
            int i = 0;
            while (i < 15)
            {
                enter_flash.Foreground = Brushes.White;
                await Task.Delay(75);
                enter_flash.Foreground = Brushes.Gray;
                await Task.Delay(75);
                i++;
            }
            enter.Visibility = Visibility.Visible;
            enter_flash.Visibility = Visibility.Hidden;
        }

        static async Task AnimateLogo(Image img)
        {
            BitmapImage bit = new BitmapImage();
            bit.BeginInit();
            bit.UriSource = new Uri(Environment.CurrentDirectory + "/assets/images/logo.png");
            bit.EndInit();
            img.Stretch = Stretch.Fill;
            img.Source = bit;

            /*
            double wid = 300.0;
            double hei = 191.0;
            double l = 20;
            double t = 20;
            Thickness margin = new();
            while (true)
            {
                img.Width = wid * 2.1;
                img.Height = hei * 2.1;

                margin.Left = l * 2.1;
                margin.Top = t * 2.1;
                img.Margin = margin;
                await Task.Delay(10);

                img.Width = wid * 2.2;
                img.Height = hei * 2.2;

                margin.Left = l * 2.2;
                margin.Top = t * 2.2;
                img.Margin = margin;
                await Task.Delay(10);

                img.Width = wid * 2.3;
                img.Height = hei * 2.3;

                margin.Left = l * 2.3;
                margin.Top = t * 2.3;
                img.Margin = margin;
                await Task.Delay(10);

                img.Width = wid * 2.4;
                img.Height = hei * 2.4;

                margin.Left = l * 2.4;
                margin.Top = t * 2.4;
                img.Margin = margin;
                await Task.Delay(20);

                img.Width = wid * 2.3;
                img.Height = hei * 2.3;

                margin.Left = l * 2.3;
                margin.Top = t * 2.3;
                img.Margin = margin;
                await Task.Delay(10);

                img.Width = wid * 2.2;
                img.Height = hei * 2.2;

                margin.Left = l * 2.2;
                margin.Top = t * 2.2;
                img.Margin = margin;
                await Task.Delay(10);

                img.Width = wid * 2.1;
                img.Height = hei * 2.1;

                margin.Left = l * 2.1;
                margin.Top = t * 2.1;
                img.Margin = margin;
                await Task.Delay(mili_BPM * 1);
            }
            */

            ScaleTransform sc = new();
            int wt = 10;
            int wt2 = 50;
            while(true)
            {
                sc.ScaleX = 2.1;
                sc.ScaleY = 2.1;
                img.RenderTransform = sc;
                await Task.Delay(wt);

                sc.ScaleX = 2.2;
                sc.ScaleY = 2.2;
                img.RenderTransform = sc;
                await Task.Delay(wt);

                sc.ScaleX = 2.3;
                sc.ScaleY = 2.3;
                img.RenderTransform = sc;
                await Task.Delay(wt);

                sc.ScaleX = 2.4;
                sc.ScaleY = 2.4;
                img.RenderTransform = sc;
                await Task.Delay(wt2);

                sc.ScaleX = 2.3;
                sc.ScaleY = 2.3;
                img.RenderTransform = sc;
                await Task.Delay(wt);

                sc.ScaleX = 2.2;
                sc.ScaleY = 2.2;
                img.RenderTransform = sc;
                await Task.Delay(wt);

                sc.ScaleX = 2.1;
                sc.ScaleY = 2.1;
                img.RenderTransform = sc;

                await Task.Delay(mili_BPM);
            }
        }

        static async Task ForceWindowSize()
        {
            while(true)
            {
                wdw.WindowState = WindowState.Normal;
                wdw.Height = 768;
                wdw.Width = 1366;

                await Task.Delay(1);
            }
        }

        private void DEBUG_LOGO1_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/DDev247");
            LogMessage("OPENING GITHUB!!!");
        }
    }

    public class SoundPlayer
    {
        async public Task PlaySound(string path, double vol)
        {
            MediaPlayer pl = new();
            pl.Open(new Uri(path));
            pl.Volume = vol;
            pl.Play();
            int time = (int)GetWavFileDuration(path).TotalMilliseconds;
            await Task.Delay(time);
        }

        public static TimeSpan GetWavFileDuration(string fileName)
        {
            NAudio.Wave.AudioFileReader wf = new NAudio.Wave.AudioFileReader(fileName);
            return wf.TotalTime;
        }
    }
}
