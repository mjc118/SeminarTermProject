using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace Dota2StatsApp
{
    partial class HomePage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HomePage));
            this.SuspendLayout();
            // 
            // HomePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(584, 262);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "HomePage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dota2Stats";
            this.ResumeLayout(false);

        }

        private void Dota2HeroListSelectedIndexChanged(object sender, System.EventArgs e)
        {
            MySqlConnection DotaDBconn = new MySqlConnection("server=localhost;uid=root;pwd=root;database=dota2;");
            DotaDBconn.Open();

            string SelectedHero = (string)Dota2HeroList.SelectedItem;
            string HeroPicLocation = @"..\..\Resources\Dota2HeroPics\";
            HeroPicLocation += SelectedHero + ".png";

            //load the Picture with selected Hero 
            Dota2HeroPicture.Load(HeroPicLocation);
            
            string DBselectString = "SELECT * FROM herodetails WHERE HeroName = @HeroName";
            MySqlCommand DBselection = new MySqlCommand(DBselectString, DotaDBconn);
            DBselection.Parameters.AddWithValue("@HeroName", SelectedHero);
            
            MySqlDataReader Reader = null;
            Reader = DBselection.ExecuteReader();
            Reader.Read();

            //Reader.GetInt64("matchSeqNum") + 1).ToString()

            if (Reader != null) { Reader.Close(); }

            if (DotaDBconn != null) { DotaDBconn.Close(); }
        }

        #endregion

        private System.Windows.Forms.ComboBox Dota2HeroList;
        private System.Windows.Forms.PictureBox Dota2HeroPicture;
    }
}

