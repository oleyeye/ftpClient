using ftpClient.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ftpClient
{
    public partial class main : Form
    {
        public main()
        {
            InitializeComponent();
            btnConnect.Click += BtnConnect_Click;
            btnUpload.Click += BtnUpload_Click;
            btnDownload.Click += BtnDownload_Click;
            lblBack.Click += LblBack_Click;
            gvFiles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gvFiles.MultiSelect = false;
            gvFiles.SelectionChanged += GvFiles_SelectionChanged;
            gvFiles.CellFormatting += GvFiles_CellFormatting;
            gvFiles.CellDoubleClick += GvFiles_CellDoubleClick;
            this.FormClosing += Main_FormClosing;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ftp.Instance.Disconnct();
        }

        private void LblBack_Click(object sender, EventArgs e)
        {
            gvFiles.DataSource = Ftp.Instance.GoToParentFolder();
        }

        private void GvFiles_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var selectedRow = gvFiles.Rows[e.RowIndex];
            var item = selectedRow.DataBoundItem as FtpFileObject;
            if (item.IsFolder)
            {
                gvFiles.DataSource = Ftp.Instance.GetListing(item.Name);
            }
        }

        private void GvFiles_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (gvFiles.Columns[e.ColumnIndex].Name == "IsFolder")
            {
                var selectedRow = gvFiles.Rows[e.RowIndex];
                var item = selectedRow.DataBoundItem as FtpFileObject;

                var image = item.IsFolder ? UI.Properties.Resources.folder : UI.Properties.Resources.file;                
                
                e.Value = image;          
            }
        }

        private void GvFiles_SelectionChanged(object sender, EventArgs e)
        {
            btnDownload.Enabled = true;
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            var selectedRows = gvFiles.SelectedRows;
            if(selectedRows.Count < 1)
            {
                MessageBox.Show("请先选择一个文件");
                return;
            }

            var selectedRow = selectedRows[0];
            var file = selectedRow.DataBoundItem as FtpFileObject;
            if (file.IsFolder)
            {
                MessageBox.Show("请选择文件");
                return;
            }

            using(var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    var folderName = folderBrowserDialog.SelectedPath;
                    if (Ftp.Instance.DownloadFile(file.Name, Path.Combine(folderName, file.Name)))
                    {
                        MessageBox.Show("下载成功");
                    }
                }
            }                
        }        

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var selectedFiles = openFileDialog.FileNames;
                    if(Ftp.Instance.UploadFile(selectedFiles) > 0)
                    {
                        gvFiles.DataSource = Ftp.Instance.GetListing();   
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            string strUserName = tbUserName.Text.Trim();
            string strPassword = tbUserName.Text.Trim();
            string strHost = tbServer.Text.Trim();
            string strPort = tbPort.Text.Trim();

            int port = 21;

            if (string.IsNullOrWhiteSpace(strUserName))
            {
                MessageBox.Show("请输入用户名");
                return;
            }

            if (string.IsNullOrWhiteSpace(strPassword))
            {
                MessageBox.Show("请输入密码");
                return;
            }


            if (string.IsNullOrWhiteSpace(strHost))
            {
                MessageBox.Show("请输入远程ftp地址");
                return;
            }
            
            if(!int.TryParse(strPort, out port))
            {
                MessageBox.Show("请输入正确的端口号");
                return;
            }

            Ftp.Instance.Connect(strHost, port, strUserName, strPassword);
            var files = Ftp.Instance.GetListing();

            gvFiles.DataSource = files;            
        }
    }
}
