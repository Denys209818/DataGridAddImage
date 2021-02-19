using BlogForm.Entities;
using BlogForm.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BlogForm
{
    public partial class EditPostForm : Form
    {
        private readonly int _id;
        private readonly EFContext _context;
        private string fileSelected = string.Empty;
        public EditPostForm(int id)
        {
            InitializeComponent();
            _id = id;
            _context = new EFContext();
            initDataEdit();
        }
        private void initDataEdit()
        {
            var post = _context.Posts
                .SingleOrDefault(p => p.Id == _id);
            foreach(var item in _context.Categories)
            {
                cbCategory.Items.Add(item);
                if (item.Id == post.CategoryId)
                    cbCategory.SelectedItem = item;
            }
            txtTitle.Text = post.Title;
            
            string imageDir = "images";
            string dirImagePath = Path.Combine(Directory.GetCurrentDirectory(), 
                imageDir);
            if (!Directory.Exists(dirImagePath))
            {
                Directory.CreateDirectory(dirImagePath);
            }

            if(!string.IsNullOrEmpty(post.Image))
            {
                var dir = Path.Combine(Directory.GetCurrentDirectory(),
                    "images", post.Image);
                pbImage.Image = Image.FromFile(dir);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var post = _context.Posts
                .SingleOrDefault(p => p.Id == _id);
            post.CategoryId = (cbCategory.SelectedItem as Category).Id;
            post.Title = txtTitle.Text;
            if (!string.IsNullOrEmpty(fileSelected))
            {
                string ext = Path.GetExtension(fileSelected);
                string fileName = Path.GetRandomFileName()+ext;
                string fileSavePath = Path.Combine(Directory.GetCurrentDirectory(),
                    "images", fileName);
                var bmp = ImageWorker.CreateImage(
                    new Bitmap(Image.FromFile(fileSelected)),75,75);

                bmp.Save(fileSavePath, ImageFormat.Jpeg);
                //File.Copy(fileSelected, fileSavePath);
                post.Image = fileName;
            }
            
            _context.SaveChanges();
            this.DialogResult = DialogResult.OK;
        }

        private void pbImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) " +
                "| *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                fileSelected = dlg.FileName;
                //txtSearchFile.Text = dlg.FileName;
                pbImage.Image = Image.FromFile(dlg.FileName);
                //MessageBox.Show(dlg.FileName);
            }
        }
    }
}
