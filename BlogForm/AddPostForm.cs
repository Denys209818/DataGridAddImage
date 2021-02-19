using BlogForm.Entities;
using BlogForm.Service;
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
    public partial class AddPostForm : Form
    {
        private EFContext _context { get; set; }
        private string ImagePath { get; set; } = null;
        public AddPostForm()
        {
            InitializeComponent();
        }

        private void AddPostForm_Load(object sender, EventArgs e)
        {
            _context = new EFContext();
            foreach (var item in _context.Categories.ToList()) 
            {
                this.cbCategories.Items.Add(item);
            }
            this.cbCategories.Text = "Категорію не обрано!";
        }

        private void pbImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog form = new OpenFileDialog()) 
            {
                form.Filter = "Image files (*.jpg; *.png) | *.jpg; *.png";
                if (form.ShowDialog() == DialogResult.OK) 
                {
                    this.ImagePath = form.FileName;
                    this.pbImage.Image = Image.FromFile(this.ImagePath);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txtTitle.Text) && !string.IsNullOrEmpty(this.txtText.Text))
            {
                string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                Post newPost = new Post();
                if (!string.IsNullOrEmpty(this.ImagePath))
                {
                    string oldPath = this.ImagePath;
                    Bitmap bmp = BitmapCreater.ResizeImage(Image.FromFile(this.ImagePath), 75, 75);
                    string ext = Path.GetExtension(this.ImagePath);
                    string fileName = Path.GetRandomFileName() + ext;
                    string newPath = Path.Combine(dirPath, fileName);

                    bmp.Save(newPath);

                    newPost.Image = fileName;
                }

                newPost.Title = this.txtTitle.Text;
                newPost.Text = this.txtText.Text;

                if (this.cbCategories.SelectedItem != null)
                {
                    newPost.Category = this.cbCategories.SelectedItem as Category;
                }

                this._context.Posts.Add(newPost);
                this._context.SaveChanges();
                this.DialogResult = DialogResult.OK;
            }
            else 
            {
                MessageBox.Show("Заповніть усі поля!");
            }
        }
    }
}
