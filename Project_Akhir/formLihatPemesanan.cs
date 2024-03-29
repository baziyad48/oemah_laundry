﻿using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_Akhir
{
    public partial class FormLihatPemesanan : Form
    {
        ListViewItem item;
        string id, date;
        static string username;

        public FormLihatPemesanan(string param)
        {
            InitializeComponent();

            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            username = param;
            label2.Text = param;

            refresh();
        }

        private void formLihatPesanan_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            refresh();
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=oemah_laundry;SslMode=none";
            string query = "UPDATE pemesanan SET berat = '" + textBox1.Text + "', harga = '" + textBox2.Text + "', tanggal_keluar = '"+ dateTimePicker1.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + "', `status` = '" + comboBox1.Text + "' WHERE id_pemesanan = " + textBox3.Text;

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                int result = commandDatabase.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data berhasil diupdate");
                }
                else
                {
                    MessageBox.Show("Data gagal diupdate");

                }

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=oemah_laundry;SslMode=none";
            string query = "delete from pemesanan where id_pemesanan=" + id;

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                int result = commandDatabase.ExecuteNonQuery();

                if (result > 0)
                {
                    MessageBox.Show("Data berhasil dihapus");
                }
                else
                {
                    MessageBox.Show("Data gagal dihapus");

                }

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cobaHitung();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = listView1.SelectedItems[0].SubItems[5].Text;
            textBox2.Text = listView1.SelectedItems[0].SubItems[6].Text;
            textBox4.Text = listView1.SelectedItems[0].SubItems[4].Text;
            textBox5.Text = listView1.SelectedItems[0].SubItems[3].Text;
            textBox3.Text = listView1.SelectedItems[0].SubItems[0].Text;
            id = listView1.SelectedItems[0].SubItems[0].Text;
            if(listView1.SelectedItems[0].SubItems[8].Text != "")
            {
                dateTimePicker1.Value = Convert.ToDateTime(listView1.SelectedItems[0].SubItems[8].Text);
            }


        }

        private void refresh()
        {
            listView1.Items.Clear();

            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=oemah_laundry;SslMode=none";
            string query = "SELECT pm.id_pemesanan, pt.nama, pl.nama, pm.tipe_cucian, pm.barang_cucian, pm.berat, pm.harga, pm.tanggal_masuk, pm.tanggal_keluar, pm.`status` FROM pemesanan pm inner join petugas pt on pt.id_petugas = pm.id_petugas INNER JOIN pelanggan pl on pl.id_pelanggan = pm.id_pelanggan";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            listView1.Items.Clear();

            try
            {
                databaseConnection.Open();

                reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader["tanggal_masuk"].ToString(), reader["tanggal_keluar"].ToString(), reader["status"].ToString() };
                        var listViewItem = new ListViewItem(row);
                        listView1.Items.Add(listViewItem);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                databaseConnection.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void cobaHitung()
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=oemah_laundry;SslMode=none";
            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            string query = "SELECT harga FROM barang_cucian WHERE nama = @nama";
            MySqlCommand command = new MySqlCommand(query, databaseConnection);
            command.CommandTimeout = 60;
            MySqlDataReader reader;
            int barang_cucian = 0;
            int tipe_cucian = 0;
            int total = 0;

            try
            {
                
                databaseConnection.Open();
                command.Parameters.AddWithValue("@nama", textBox4.Text);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    barang_cucian = int.Parse(reader.GetString("harga"));

                }
                databaseConnection.Close();

                total = barang_cucian * int.Parse(textBox1.Text);

                if (textBox5.Text.Equals("Cuci Kering"))
                {
                    tipe_cucian = 3000;
                }
                else if (textBox5.Text.Equals("Cuci Setrika"))
                {
                    tipe_cucian = 4000;
                }

                total += tipe_cucian;
                textBox2.Text = "" + total;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
