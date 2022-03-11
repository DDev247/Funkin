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

namespace Funkin
{
    public partial class Window1 : Window
    {
        public static ListBox lb;
        public MainWindow parent;

        public Window1(MainWindow main, bool wasClosed)
        {
            InitializeComponent();
            lb = LB;
            parent = main;
            //Scroll();
            Closed += parent.Debug_Closed;
            after(wasClosed);
        }

        static async Task after(bool wasClosed)
        {
            
            if (wasClosed)
            {
                foreach (string s in MainWindow.debugList)
                {
                    lb.Items.Add(s);
                    MainWindow.LogMessage("LOADING: " + s);
                }
            }
            //MainWindow.LogMessage("wasclosed: " + wasClosed);
        }

        static async Task Scroll()
        {
            while (true)
            {
                lb.BringIntoView();
                await Task.Delay(1);
            }
        }

        public async Task AddLog(Log log)
        {
            lb.Items.Add(log.S);
        }
    }

    public class Log
    {
        public string Source { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }

        public string S;

        public Log(string source, string weight, string desc)
        {
            Source = source;
            Weight = weight;
            Description = desc;
        }

        public Log(string s)
        {
            S = s;
        }
    }
}
