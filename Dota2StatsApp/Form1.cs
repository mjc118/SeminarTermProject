using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Dota2StatsApp
{
    public partial class HomePage : Form
    {
        public MySqlConnection DotaDBconn = new MySqlConnection("server=localhost;uid=root;pwd=root;database=dota2;");

        public class HeroDetails
        {
            public string HeroName { get; set; }
            public UInt64 Wins { get; set; }
            public UInt64 Losses { get; set; }
            public Double WinRate { get; set; }
            public UInt64 Kills { get; set; }
            public UInt64 Deaths { get; set; }
            public UInt64 Assists { get; set; }
            public Double KDAratio { get; set; }
            public UInt64 MatchesPlayed { get; set; }
        };

        public HeroDetails[] Heroes;
        public PictureBox[] HeroPictureBox;
        public Label[] HeroNameLabel;
        public Label[] TitleLabels;
        public Label[] MatchesPlayedLabels;
        public PictureBox[] MatchesPlayedRatioBars;
        public Label[] WinRateLabels;
        public PictureBox[] WinRateRatioBars;
        public Label[] KDAratioLabels;
        public PictureBox[] KDAratioRatioBars;
        public String[] TitleNames = { "Hero", "Matches Played", "Win Rate", "KDA Ratio" };
        //reference TitleNames array to see what column each indice of this array refers to
        public bool[] CurrentColumnSortedBy = { true, false, false, false};
        public int[] DistanceBetweenColumns = { 0, 175, 375, 500 };
        public UInt64 MaxMatchesPlayed = 0;
        public Double MaxWinRate = 0;
        public Double MaxKDAratio = 0;

        public HomePage()
        {
            Heroes = new HeroDetails[110];//holds all data require from DB for each hero
            HeroPictureBox = new PictureBox[110];//displays appropriate hero picture
            HeroNameLabel = new Label[110];//displays each heros name
            TitleLabels = new Label[4];//displays column titles
            MatchesPlayedLabels = new Label[110];//displays matches played for each hero
            MatchesPlayedRatioBars = new PictureBox[110];//keeps track of the Colored Rectangle Ratio Bars for Matches Played
            WinRateLabels = new Label[110];//displays winrate for each hero
            WinRateRatioBars = new PictureBox[110]; //keeps track of the Colored Rectangle Ratio Bars for WinRate
            KDAratioLabels = new Label[110];//displays KDA ratio for each hero
            KDAratioRatioBars = new PictureBox[110];//keeps track of the Colored Rectangle Ratio Bars for KDAratio
            DotaDBconn.Open();

            string DBselectString = "SELECT * FROM herodetails";
            MySqlCommand DBselection = new MySqlCommand(DBselectString, DotaDBconn);

            MySqlDataReader Reader = null;
            Reader = DBselection.ExecuteReader();

            int CurrentRow = 0;
            while (Reader.Read())
            {
                Heroes[CurrentRow] = new HeroDetails();
                Heroes[CurrentRow].HeroName = Reader.GetString("HeroName");
                Heroes[CurrentRow].Wins = Reader.GetUInt64("Wins");
                Heroes[CurrentRow].Losses = Reader.GetUInt64("Losses");
                Heroes[CurrentRow].Kills = Reader.GetUInt64("Kills");
                Heroes[CurrentRow].Deaths = Reader.GetUInt64("Deaths");
                Heroes[CurrentRow].Assists = Reader.GetUInt64("Assists");
                Heroes[CurrentRow].MatchesPlayed = Reader.GetUInt64("MatchesPlayed");
                Heroes[CurrentRow].WinRate = Math.Round(100 *(Convert.ToDouble(Heroes[CurrentRow].Wins) 
                    / Convert.ToDouble(Heroes[CurrentRow].MatchesPlayed)), 2);
                Heroes[CurrentRow].KDAratio = Math.Round((Convert.ToDouble(Heroes[CurrentRow].Kills + Heroes[CurrentRow].Assists)
                    / Convert.ToDouble(Heroes[CurrentRow].Deaths)), 2);

                MaxMatchesPlayed = Math.Max(MaxMatchesPlayed, Heroes[CurrentRow].MatchesPlayed);
                if (!Double.IsNaN(Heroes[CurrentRow].WinRate))
                {
                    MaxWinRate = Math.Max(MaxWinRate, Heroes[CurrentRow].WinRate);
                }
                if (!Double.IsNaN(Heroes[CurrentRow].KDAratio))
                {
                    MaxKDAratio = Math.Max(MaxKDAratio, Heroes[CurrentRow].KDAratio);
                }

                ++CurrentRow;
            }

            if (Reader != null) { Reader.Close(); }
            if (DotaDBconn != null) { DotaDBconn.Close(); }

            //alphabetically sort our array of heroes initially
            Array.Sort(Heroes, (x, y) => x.HeroName.CompareTo(y.HeroName));

            InitializeComponent();

            //Add our Colored Rectangles for Aesthetic Purposes
            PictureBox FillRectangle;
            for (int i = 0; i < Heroes.Length; ++i)
            {
                FillRectangle = new PictureBox();

                if (i % 2 == 0) { FillRectangle.BackColor = System.Drawing.SystemColors.InactiveCaptionText; }
                else { FillRectangle.BackColor = System.Drawing.SystemColors.WindowFrame; }
                
                FillRectangle.BackgroundImageLayout = ImageLayout.Zoom;
                FillRectangle.Location = new System.Drawing.Point(0, 65 + (i * 45) + (i * 10));
                FillRectangle.Size = new System.Drawing.Size(ClientSize.Width * 4, 55);
                FillRectangle.TabStop = false;
                this.Controls.Add(FillRectangle);
                this.Controls.SetChildIndex(FillRectangle, 2);
            }

            //Add Title Labels to the Screen
            for (int i = 0; i < TitleLabels.Length; ++i)
            {
                TitleLabels[i] = new Label();
                TitleLabels[i].Location = new System.Drawing.Point(125 + DistanceBetweenColumns[i], 40);
                TitleLabels[i].Name = TitleNames[i];
                TitleLabels[i].Size = new System.Drawing.Size(150, 21);
                TitleLabels[i].TabIndex = 1;

                TitleLabels[i].ForeColor = Color.DarkRed;
                TitleLabels[i].Font = new Font(TitleLabels[i].Font.FontFamily, 11, FontStyle.Bold);
                TitleLabels[i].Text = TitleNames[i];
                TitleLabels[i].MouseClick += new MouseEventHandler(SortByColumn);
                this.Controls.Add(TitleLabels[i]);
                this.Controls.SetChildIndex(TitleLabels[i], 0);
            }

            //Add our matches played labels to screen
            for (int i = 0; i < MatchesPlayedLabels.Length; ++i)
            {
                MatchesPlayedLabels[i] = new Label();
                MatchesPlayedLabels[i].Location = new System.Drawing.Point(300, 73 + (i * 45) + (i * 10));
                MatchesPlayedLabels[i].Name = "MatchesPlayedLabel" + i;
                MatchesPlayedLabels[i].Size = new System.Drawing.Size(150, 21);
                MatchesPlayedLabels[i].TabIndex = 1;

                if (i % 2 == 0) { MatchesPlayedLabels[i].BackColor = SystemColors.InactiveCaptionText; }
                else { MatchesPlayedLabels[i].BackColor = SystemColors.WindowFrame; }

                MatchesPlayedLabels[i].ForeColor = Color.AntiqueWhite;
                MatchesPlayedLabels[i].Font = new Font(MatchesPlayedLabels[i].Font.FontFamily, 11, FontStyle.Bold);
                MatchesPlayedLabels[i].Text = Heroes[i].MatchesPlayed.ToString("N0");
                this.Controls.Add(MatchesPlayedLabels[i]);
                this.Controls.SetChildIndex(MatchesPlayedLabels[i], 0);

                //Add our bar that is a ratio of MatchesPlayed for specific hero versus hero with highest Matches Played
                MatchesPlayedRatioBars[i] = new PictureBox();

                MatchesPlayedRatioBars[i].BackColor = Color.Red;

                Double FillRatio = (150 * (Convert.ToDouble(Heroes[i].MatchesPlayed)/MaxMatchesPlayed));

                MatchesPlayedRatioBars[i].BackgroundImageLayout = ImageLayout.Zoom;
                MatchesPlayedRatioBars[i].Location = new System.Drawing.Point(305, 100 + (i * 45) + (i * 10));
                MatchesPlayedRatioBars[i].Size = new System.Drawing.Size(Convert.ToInt16(FillRatio), 8);
                MatchesPlayedRatioBars[i].TabStop = false;
                this.Controls.Add(MatchesPlayedRatioBars[i]);
                this.Controls.SetChildIndex(MatchesPlayedRatioBars[i], 0);
            }

            //Add our Win Rate Labels to screen
            for (int i = 0; i < WinRateLabels.Length; ++i)
            {
                WinRateLabels[i] = new Label();
                WinRateLabels[i].Location = new System.Drawing.Point(500, 73 + (i * 45) + (i * 10));
                WinRateLabels[i].Name = "WinRateLabels" + i;
                WinRateLabels[i].Size = new System.Drawing.Size(125, 21);
                WinRateLabels[i].TabIndex = 1;

                if (i % 2 == 0) { WinRateLabels[i].BackColor = SystemColors.InactiveCaptionText; }
                else { WinRateLabels[i].BackColor = SystemColors.WindowFrame; }

                WinRateLabels[i].ForeColor = Color.AntiqueWhite;
                WinRateLabels[i].Font = new Font(WinRateLabels[i].Font.FontFamily, 11, FontStyle.Bold);

                if (Double.IsNaN(Heroes[i].WinRate)) { WinRateLabels[i].Text = "0%"; }
                else { WinRateLabels[i].Text = Heroes[i].WinRate.ToString() + "%"; }

                this.Controls.Add(WinRateLabels[i]);
                this.Controls.SetChildIndex(WinRateLabels[i], 0);

                //Add our bar that is a ratio of WinRate for specific hero versus hero with highest WinRate
                WinRateRatioBars[i] = new PictureBox();

                WinRateRatioBars[i].BackColor = Color.Red;
                if (!Double.IsNaN(Heroes[i].WinRate))
                {
                    Double FillRatio = (115 * (Heroes[i].WinRate / MaxWinRate));

                    WinRateRatioBars[i].BackgroundImageLayout = ImageLayout.Zoom;
                    WinRateRatioBars[i].Location = new System.Drawing.Point(500, 100 + (i * 45) + (i * 10));
                    WinRateRatioBars[i].Size = new System.Drawing.Size(Convert.ToInt16(FillRatio), 8);
                    WinRateRatioBars[i].TabStop = false;
                    this.Controls.Add(WinRateRatioBars[i]);
                    this.Controls.SetChildIndex(WinRateRatioBars[i], 0);
                }
            }

            //Add our KDA Ratio labels to screen
            for (int i = 0; i < KDAratioLabels.Length; ++i)
            {
                KDAratioLabels[i] = new Label();
                KDAratioLabels[i].Location = new System.Drawing.Point(625, 73 + (i * 45) + (i * 10));
                KDAratioLabels[i].Name = "WinRateLabels" + i;
                KDAratioLabels[i].Size = new System.Drawing.Size(125, 21);
                KDAratioLabels[i].TabIndex = 1;

                if (i % 2 == 0) { KDAratioLabels[i].BackColor = SystemColors.InactiveCaptionText; }
                else { KDAratioLabels[i].BackColor = SystemColors.WindowFrame; }

                KDAratioLabels[i].ForeColor = Color.AntiqueWhite;
                KDAratioLabels[i].Font = new Font(KDAratioLabels[i].Font.FontFamily, 11, FontStyle.Bold);

                if (Double.IsNaN(Heroes[i].KDAratio)) { KDAratioLabels[i].Text = "0"; }
                else { KDAratioLabels[i].Text = Heroes[i].KDAratio.ToString(); }

                this.Controls.Add(KDAratioLabels[i]);
                this.Controls.SetChildIndex(KDAratioLabels[i], 0);

                //Add our bar that is a ratio of KDAratio for specific hero versus hero with highest KDAratio
                KDAratioRatioBars[i] = new PictureBox();

                KDAratioRatioBars[i].BackColor = Color.Red;
                if (!Double.IsNaN(Heroes[i].KDAratio))
                {
                    Double FillRatio = (125 * (Heroes[i].KDAratio / MaxKDAratio));

                    KDAratioRatioBars[i].BackgroundImageLayout = ImageLayout.Zoom;
                    KDAratioRatioBars[i].Location = new System.Drawing.Point(625, 100 + (i * 45) + (i * 10));
                    KDAratioRatioBars[i].Size = new System.Drawing.Size(Convert.ToInt16(FillRatio), 8);
                    KDAratioRatioBars[i].TabStop = false;
                    this.Controls.Add(KDAratioRatioBars[i]);
                    this.Controls.SetChildIndex(KDAratioRatioBars[i], 0);
                }
            }

            //Add our Labels to the Screen
            for (int i = 0; i < Heroes.Length; ++i)
            {
                HeroNameLabel[i] = new Label();
                HeroNameLabel[i].Location = new System.Drawing.Point(125, 73 + (i * 45) + (i * 10));
                HeroNameLabel[i].Name = "TextBox" + i;
                HeroNameLabel[i].Size = new System.Drawing.Size(150, 21);
                HeroNameLabel[i].TabIndex = 1;
                
                if (i % 2 == 0) { HeroNameLabel[i].BackColor = SystemColors.InactiveCaptionText; }
                else { HeroNameLabel[i].BackColor = SystemColors.WindowFrame; }

                HeroNameLabel[i].ForeColor = Color.LawnGreen;
                HeroNameLabel[i].Font = new Font(HeroNameLabel[i].Font.FontFamily, 11, FontStyle.Bold);
                HeroNameLabel[i].Text = Heroes[i].HeroName;
                this.Controls.Add(HeroNameLabel[i]);
                this.Controls.SetChildIndex(HeroNameLabel[i], 0);
            }

            //Add our Pictures of Heroes to the Screen
            for (int i = 0; i < Heroes.Length; ++i)
            {
                string HeroPicLocation = @"..\..\Resources\Dota2HeroPics\" + Heroes[i].HeroName + ".png";

                HeroPictureBox[i] = new PictureBox();
                HeroPictureBox[i].BackColor = System.Drawing.Color.Transparent;
                HeroPictureBox[i].Load(HeroPicLocation);
                HeroPictureBox[i].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                HeroPictureBox[i].Location = new System.Drawing.Point(39, 70 + (i * 45) + (i * 10));
                HeroPictureBox[i].Name = "Dota2Picture" + Heroes[i].HeroName;
                HeroPictureBox[i].Size = new System.Drawing.Size(80, 45);
                HeroPictureBox[i].TabIndex = 2;
                HeroPictureBox[i].TabStop = false;
                this.Controls.Add(HeroPictureBox[i]);
                this.Controls.SetChildIndex(HeroPictureBox[i], 0);
            }
        }

        private void SortByColumn(object sender, MouseEventArgs e)
        {
            Label WhichLabel = sender as Label;
            Console.WriteLine(WhichLabel.Text);
            if (WhichLabel.Text == "Hero")
            {
                //if we are already sorted by Hero then we will sort it Z-A
                if (CurrentColumnSortedBy[0])
                {
                    Array.Sort(Heroes, (x, y) => y.HeroName.CompareTo(x.HeroName));
                }
                else //sort by hero A-Z and set the boolean array to say which column we are sorted by
                {
                    CurrentColumnSortedBy[0] = true;
                    for (int i = 1; i < 4; ++i) { CurrentColumnSortedBy[i] = false; }
                    Array.Sort(Heroes, (x, y) => x.HeroName.CompareTo(y.HeroName));
                }
            }
            else if (WhichLabel.Text == "Matches Played")
            {
                //if we are already sorted by Matches Played, sort it in ascending orer
                if (CurrentColumnSortedBy[1])
                {
                    Array.Sort(Heroes, (x, y) => x.MatchesPlayed.CompareTo(y.MatchesPlayed));
                }
                else//sort by Matches Played descending order, and set boolean array appropriately
                {
                    CurrentColumnSortedBy[1] = true;
                    CurrentColumnSortedBy[0] = false; CurrentColumnSortedBy[2] = false; CurrentColumnSortedBy[3] = false;   
                    Array.Sort(Heroes, (x, y) => y.MatchesPlayed.CompareTo(x.MatchesPlayed));
                }
            }
            else if (WhichLabel.Text == "Win Rate")
            {
                //if we are already sorted by WinRate, sort it in ascending orer
                if (CurrentColumnSortedBy[2])
                {
                    Array.Sort(Heroes, (x, y) => x.WinRate.CompareTo(y.WinRate));
                }
                else//sort by WinRate descending order, and set boolean array appropriately
                {
                    CurrentColumnSortedBy[2] = true;
                    CurrentColumnSortedBy[0] = false; CurrentColumnSortedBy[1] = false; CurrentColumnSortedBy[3] = false;
                    Array.Sort(Heroes, (x, y) => y.WinRate.CompareTo(x.WinRate));
                }
            }
            else if (WhichLabel.Text == "KDA Ratio")
            {
                //if we are already sorted by KDARatio, sort it in ascending orer
                if (CurrentColumnSortedBy[2])
                {
                    Array.Sort(Heroes, (x, y) => x.KDAratio.CompareTo(y.KDAratio));
                }
                else//sort by KDARatio descending order, and set boolean array appropriately
                {
                    CurrentColumnSortedBy[3] = true;
                    for (int i = 0; i < 3; ++i) { CurrentColumnSortedBy[i] = false; }
                    Array.Sort(Heroes, (x, y) => y.KDAratio.CompareTo(x.KDAratio));
                }
            }
            else
            {
                Console.WriteLine("Error with Label Mouse Click Event");
            }
            Console.WriteLine("Before {0}", this.Controls.Count);
            ReDrawScreen();
            Console.WriteLine("After {0}",this.Controls.Count);
            //throw new NotImplementedException();
        }

        public void ReDrawScreen()
        {
            //Add our matches played labels to screen
            for (int i = 0; i < MatchesPlayedLabels.Length; ++i)
            {
                //change our labels text to be the new text
                MatchesPlayedLabels[i].Text = Heroes[i].MatchesPlayed.ToString("N0");
               
                //Add our bar that is a ratio of MatchesPlayed for specific hero versus hero with highest Matches Played
                this.Controls.Remove(MatchesPlayedRatioBars[i]);
                MatchesPlayedRatioBars[i] = new PictureBox();

                MatchesPlayedRatioBars[i].BackColor = Color.Red;

                Double FillRatio = (150 * (Convert.ToDouble(Heroes[i].MatchesPlayed) / MaxMatchesPlayed));

                MatchesPlayedRatioBars[i].BackgroundImageLayout = ImageLayout.Zoom;
                MatchesPlayedRatioBars[i].Location = new System.Drawing.Point(305, 100 + (i * 45) + (i * 10));
                MatchesPlayedRatioBars[i].Size = new System.Drawing.Size(Convert.ToInt16(FillRatio), 8);
                MatchesPlayedRatioBars[i].TabStop = false;
                this.Controls.Add(MatchesPlayedRatioBars[i]);
                this.Controls.SetChildIndex(MatchesPlayedRatioBars[i], 0);
            }

            //Add our Win Rate Labels to screen
            for (int i = 0; i < WinRateLabels.Length; ++i)
            {
                if (Double.IsNaN(Heroes[i].WinRate)) { WinRateLabels[i].Text = "0%"; }
                else { WinRateLabels[i].Text = Heroes[i].WinRate.ToString() + "%"; }

                //Add our bar that is a ratio of WinRate for specific hero versus hero with highest WinRate
                this.Controls.Remove(WinRateRatioBars[i]);
                WinRateRatioBars[i] = new PictureBox();

                WinRateRatioBars[i].BackColor = Color.Red;
                if (!Double.IsNaN(Heroes[i].WinRate))
                {
                    //this.Controls.Remove(WinRateRatioBars[i]);

                    Double FillRatio = (115 * (Heroes[i].WinRate / MaxWinRate));

                    WinRateRatioBars[i].BackgroundImageLayout = ImageLayout.Zoom;
                    WinRateRatioBars[i].Location = new System.Drawing.Point(500, 100 + (i * 45) + (i * 10));
                    WinRateRatioBars[i].Size = new System.Drawing.Size(Convert.ToInt16(FillRatio), 8);
                    WinRateRatioBars[i].TabStop = false;
                    this.Controls.Add(WinRateRatioBars[i]);
                    this.Controls.SetChildIndex(WinRateRatioBars[i], 0);
                }
            }

            //Add our KDA Ratio labels to screen
            for (int i = 0; i < KDAratioLabels.Length; ++i)
            {
                if (Double.IsNaN(Heroes[i].KDAratio)) { KDAratioLabels[i].Text = "0"; }
                else { KDAratioLabels[i].Text = Heroes[i].KDAratio.ToString(); }

                //Add our bar that is a ratio of KDAratio for specific hero versus hero with highest KDAratio
                this.Controls.Remove(KDAratioRatioBars[i]);
                KDAratioRatioBars[i] = new PictureBox();

                KDAratioRatioBars[i].BackColor = Color.Red;
                if (!Double.IsNaN(Heroes[i].KDAratio))
                {
                    //this.Controls.Remove(KDAratioRatioBars[i]);

                    Double FillRatio = (125 * (Heroes[i].KDAratio / MaxKDAratio));

                    KDAratioRatioBars[i].BackgroundImageLayout = ImageLayout.Zoom;
                    KDAratioRatioBars[i].Location = new System.Drawing.Point(625, 100 + (i * 45) + (i * 10));
                    KDAratioRatioBars[i].Size = new System.Drawing.Size(Convert.ToInt16(FillRatio), 8);
                    KDAratioRatioBars[i].TabStop = false;
                    this.Controls.Add(KDAratioRatioBars[i]);
                    this.Controls.SetChildIndex(KDAratioRatioBars[i], 0);
                }
            }

            //Add our Labels to the Screen
            for (int i = 0; i < Heroes.Length; ++i)
            {
                HeroNameLabel[i].Text = Heroes[i].HeroName;
            }

            //Add our Pictures of Heroes to the Screen
            for (int i = 0; i < Heroes.Length; ++i)
            {
                string HeroPicLocation = @"..\..\Resources\Dota2HeroPics\" + Heroes[i].HeroName + ".png";
                HeroPictureBox[i].Load(HeroPicLocation);
            }
        }
    }
}
