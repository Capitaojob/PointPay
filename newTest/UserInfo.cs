﻿using Npgsql;
using Workers;

namespace newTest
{
    public partial class UserInfo : UserControl
    {
        private Employee User;

        public UserInfo()
        {
            InitializeComponent();
        }

        public void UpdateUser(Employee User)
        {
            this.User = User;
        }

        private void UserInfo_Load(object sender, EventArgs e)
        {
            LblUserName.Text = User.Name;
            TxtEmail.Text = User.Email;
            TxtCpf.Text = User.CPF;
            SelectUserRole();
        }

        async private void SelectUserRole()
        {
            using NpgsqlConnection conn = new("Server=localhost; Port=5432; User Id=postgres; Password=JOpe2004!; Database=tzrh");
            conn.Open();

            NpgsqlCommand cmd = new($"SELECT nome_cargo FROM cargo JOIN funcionarios ON funcionarios.cargo = cargo.id_cargo WHERE email = '" + User.Email + "'", conn);
            NpgsqlDataReader dr = await cmd.ExecuteReaderAsync();

            if (await dr.ReadAsync())
            {
                TxtRole.Text = dr.GetString(0);
            }

            dr.Close();
        }

        private void BtnUpdatePw_Click(object sender, EventArgs e)
        {
            if (TxtPw.Text.Length < 6)
            {
                LblPwWarning.Text = "Senha muito curta!";
            }
            else if (TxtPw.Text.Length > 30)
            {
                LblPwWarning.Text = "Senha muito longa!";
            }
            else
            {
                User.Password = HashUtils.HashString(TxtPw.Text);
                using NpgsqlConnection conn = new("Server=localhost; Port=5432; User Id=postgres; Password=JOpe2004!; Database=tzrh");
                conn.Open();

                NpgsqlCommand cmd = new($"UPDATE funcionarios SET senha = '" + User.Password + "' WHERE email = '" + User.Email + "'", conn);
                cmd.ExecuteNonQuery();

                LblPwWarning.Text = "Senha alterada com sucesso!";
            }
        }

        private void ImgPwEye_Click(object sender, EventArgs e)
        {
            if (TxtPw.PasswordChar == '*')
            {
                TxtPw.PasswordChar = '\0';
                ImgPwEye.Image = Properties.Resources.hide;
                ImgPwEye.BackgroundImage = Properties.Resources.hide;
            }
            else
            {
                TxtPw.PasswordChar = '*';
                ImgPwEye.Image = Properties.Resources.view;
                ImgPwEye.BackgroundImage = Properties.Resources.view;
            }

            ImgPwEye.Refresh();
        }
    }
}
