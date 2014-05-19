using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LolTroll
{
    class TrollData
    {
        public TrollData(){
            trollList.CollectionChanged += (s, e) =>
            {
                if (e.NewStartingIndex > e.OldStartingIndex)
                {
                    //Inserting new
                    trollList[e.NewStartingIndex].Context = this;
                }
                

            };
        }

        public string DataDir{
            get { return System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
        }
        public string WorkingDir
        {
            get { return Directory.GetCurrentDirectory(); }
        }

        //User database troll list
        protected ObservableCollection<Troll> trollList = new ObservableCollection<Troll>();
        public ObservableCollection<Troll> TrollList
        {
            get { return trollList;  }
        }

        //Regions where are the summoner names
        protected static Rectangle[] sections = new Rectangle[] {
            new Rectangle(new Point(55, 138), new Size(150, 13)),
            new Rectangle(new Point(55, 238), new Size(150, 13)),
            new Rectangle(new Point(55, 338), new Size(150, 14)),
            new Rectangle(new Point(55, 437), new Size(150, 14)),
            new Rectangle(new Point(55, 537), new Size(150, 14))
        };
        public Rectangle[] Sections 
        {
            get { return sections; }
        }

        //Names of the actual queue
        protected ObservableCollection<string> trollNames = new ObservableCollection<string>(new string[sections.Length]);
        public ObservableCollection<string> TrollNames{
            get { return trollNames; }
        }

        //Fucktard level shown when troll is found
        protected ObservableCollection<string> fucktardInfo = new ObservableCollection<string>(new string[sections.Length]);
        public ObservableCollection<string> FucktardInfo
        {
            get { return fucktardInfo; }
        }

        //Background of the textedits
        protected ObservableCollection<SolidColorBrush> backgroundInfo = new ObservableCollection<SolidColorBrush>(new SolidColorBrush[sections.Length]);
        public ObservableCollection<SolidColorBrush> BackgroundInfo
        {
            get { return backgroundInfo; }
        }
    }
}
