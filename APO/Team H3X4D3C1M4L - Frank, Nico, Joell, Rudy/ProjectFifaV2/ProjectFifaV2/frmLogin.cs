﻿using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlServerCe;

namespace ProjectFifaV2
{
    public partial class frmLogin : Form
    {
        private DatabaseHandler dbh;
        private Form frmAdmin;
        private Form frmPlayer;
        private Form frmRanking;
        private Form frmWelcome;

        public frmLogin()
        {
            InitializeComponent();
            dbh = new DatabaseHandler();
            frmAdmin = new frmAdmin();
            frmRanking = new frmRanking();
            frmWelcome = new frmWelcome();
            frmWelcome.Show();
            frmWelcome.Focus();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "" || txtPassword.Text == "")
                MessageBox.Show("Both fields are required");
            else
            {
                dbh.TestConnection();
                dbh.OpenConnectionToDB();
                bool exist = false;

                using (SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(*) FROM [tblUsers] WHERE Username = @Username", dbh.GetCon()))
                {
                    cmd.Parameters.AddWithValue("Username", txtUsername.Text);
                    exist = (int)cmd.ExecuteScalar() > 0;
                }

                if (exist)
                    MessageHandler.ShowMessage("This user already exists.");
                else
                {
                    using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO [tblUsers] ([Username], [Password], [IsAdmin]) VALUES (@Username, @Password, @IsAdmin)"))
                    {
                        cmd.Parameters.AddWithValue("Username", txtUsername.Text);
                        cmd.Parameters.AddWithValue("Password", txtPassword.Text);
                        cmd.Parameters.AddWithValue("IsAdmin", 0);
                        cmd.Connection = dbh.GetCon();
                        cmd.ExecuteNonQuery();
                    }
                }

                dbh.CloseConnectionToDB();
            }
        }

        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !ExitPressed();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private bool ExitPressed()
        {
            bool closing = false;
            DialogResult result = MessageBox.Show("Are you sure you want to quit?", "Quit", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result.Equals(DialogResult.OK))
            {
                if (dbh.GetCon().State == ConnectionState.Open)
                    dbh.CloseConnectionToDB();
                closing = true;
            }
            return closing;
        }

        private void btnShowRanking_Click(object sender, EventArgs e)
        {
            frmRanking.Show(); 
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            dbh.TestConnection();
            dbh.OpenConnectionToDB();

            string username = txtUsername.Text;
            string password = txtPassword.Text;

            txtUsername.Text = "";
            txtPassword.Text = "";

            bool passwordTrue;
            
            using (SqlCeCommand cmd = new SqlCeCommand("SELECT Password FROM [tblUsers] WHERE Username = @Username", dbh.GetCon()))
            {
                cmd.Parameters.AddWithValue("Username", username);
                passwordTrue = ((string)cmd.ExecuteScalar() == password);
            }           

            if (passwordTrue)
            {
                bool admin;
                using (SqlCeCommand cmd = new SqlCeCommand("SELECT COUNT(*) from [tblUsers] WHERE Username = @Username AND IsAdmin = 1", dbh.GetCon()))
                {
                    cmd.Parameters.AddWithValue("Username", username);
                    admin = (int)cmd.ExecuteScalar() > 0;
                }
                dbh.CloseConnectionToDB();

                if (admin)
                    frmAdmin.Show();
                else
                {
                    frmPlayer = new frmPlayer(frmRanking, username);
                    frmPlayer.Show();
                }
                this.Hide();
            }
            else
            {
                dbh.CloseConnectionToDB();
                MessageHandler.ShowMessage("Wrong username and/or password.");
            }
        }
    }
}
