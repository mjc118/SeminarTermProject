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
            public Label HeroNameLabel { get; set; }
            public PictureBox HeroPictureBox { get; set; }
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

        public HomePage()
        {
            Heroes = new HeroDetails[110];

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
                Heroes[CurrentRow].KDAratio = (Convert.ToDouble(Heroes[CurrentRow].Kills + Heroes[CurrentRow].Assists)
                    / Convert.ToDouble(Heroes[CurrentRow].Deaths));
                ++CurrentRow;
            }

            if (Reader != null) { Reader.Close(); }
            if (DotaDBconn != null) { DotaDBconn.Close(); }

            //alphabetically sort our array of heroes initially
            Array.Sort(Heroes, (x, y) => string.Compare(x.HeroName, y.HeroName));

            InitializeComponent();

            //Add our Colored Rectangles for Aesthetic Purposes
            PictureBox FillRectangle;
            for (int i = 0; i < Heroes.Length; ++i)
            {
                FillRectangle = new PictureBox();

                if (i % 2 == 0) { FillRectangle.BackColor = System.Drawing.SystemColors.InactiveCaptionText; }
                else { FillRectangle.BackColor = System.Drawing.SystemColors.WindowFrame; }
                
                FillRectangle.BackgroundImageLayout = ImageLayout.Zoom;
                FillRectangle.Location = new System.Drawing.Point(0, 21 + (i * 45) + (i * 10));
                FillRectangle.Size = new System.Drawing.Size(ClientSize.Width * 4, 55);
                FillRectangle.TabStop = false;
                this.Controls.Add(FillRectangle);
                this.Controls.SetChildIndex(FillRectangle, 2);
            }

            //Add our Labels to the Screen
            for (int i = 0; i < Heroes.Length; ++i)
            {
                Heroes[i].HeroNameLabel = new Label();
                Heroes[i].HeroNameLabel.Location = new System.Drawing.Point(125, 50 + (i * 45) + (i * 10));
                Heroes[i].HeroNameLabel.Name = "TextBox" + i;
                Heroes[i].HeroNameLabel.Size = new System.Drawing.Size(150, 21);
                Heroes[i].HeroNameLabel.TabIndex = 1;

                if (i % 2 == 0) { Heroes[i].HeroNameLabel.BackColor = SystemColors.InactiveCaptionText; }
                else { Heroes[i].HeroNameLabel.BackColor = SystemColors.WindowFrame; }

                Heroes[i].HeroNameLabel.ForeColor = Color.LawnGreen;
                Heroes[i].HeroNameLabel.Font = new Font(Heroes[i].HeroNameLabel.Font.FontFamily, 11, FontStyle.Bold);
                Heroes[i].HeroNameLabel.Text = Heroes[i].HeroName;
                this.Controls.Add(Heroes[i].HeroNameLabel);
                this.Controls.SetChildIndex(Heroes[i].HeroNameLabel, 0);
            }

            //Add our Pictures of Heroes to the Screen
            for (int i = 0; i < Heroes.Length; ++i)
            {
                string HeroPicLocation = @"..\..\Resources\Dota2HeroPics\" + Heroes[i].HeroName + ".png";

                Heroes[i].HeroPictureBox = new PictureBox();
                Heroes[i].HeroPictureBox.BackColor = System.Drawing.Color.Transparent;
                Heroes[i].HeroPictureBox.Load(HeroPicLocation);
                Heroes[i].HeroPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
                Heroes[i].HeroPictureBox.Location = new System.Drawing.Point(39, 26 + (i * 45) + (i * 10));
                Heroes[i].HeroPictureBox.Name = "Dota2Picture" + Heroes[i].HeroName;
                Heroes[i].HeroPictureBox.Size = new System.Drawing.Size(80, 45);
                Heroes[i].HeroPictureBox.TabIndex = 2;
                Heroes[i].HeroPictureBox.TabStop = false;
                this.Controls.Add(Heroes[i].HeroPictureBox);
                this.Controls.SetChildIndex(Heroes[i].HeroPictureBox, 0);
            }
        }
    }
}
