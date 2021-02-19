using BlogForm.Entities;
using BlogForm.Models;
using BlogForm.Service;
using Microsoft.EntityFrameworkCore;
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

namespace BlogForm
{
    public partial class MainForm : Form
    {
        private string _addedRow { get; set; } = null;
        private readonly EFContext _context;
        private List<PostDataModel> _posts { get; set; } = new List<PostDataModel>();
        public MainForm()
        {
            InitializeComponent();
            _context = new EFContext();
            Seeder.SeedDatabase(_context);
            loadFromData();
            this.dgvPosts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPosts_CellClick);
            this.dgvPosts.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPosts_CellValueChanged);
            this.dgvPosts.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgvPosts_RowsAdded);
        }
        private void loadFromData()
        {
            dgvPosts.Rows.Clear();
            var query = _context.Posts
               //.Include(x => x.Category)
               .AsQueryable();

            var list = query.Select(x => new {
                Id = x.Id,
                Title = x.Title,
                Image = x.Image,
                CategoryName = x.Category.Name
            })
                .AsQueryable().ToList();

            foreach (var item in list)
            {
                bool correctPath = false;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "images");
                if (!string.IsNullOrEmpty(item.Image)) 
                {
                    if (File.Exists(Path.Combine(path, item.Image))) 
                    {
                        correctPath = true;
                    }
                }
                object[] row =
                {
                    item.Id,
                    ///Тернарний оператор C#, якщо фото немає, то буде null
                    ///якщо фото є, то його вантажимо чере Image.FromFile
                    item.Image==null || !correctPath ? null:Image.FromFile(Path.Combine(path, item.Image)),
                    item.Title,
                    item.CategoryName
                };
                dgvPosts.Rows
                    .Add(row);

            }
        }
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (this._posts.Count() > 0)
            {
                if (MessageBox.Show("Зберегти усі зміни?", "DataGridView", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.SaveData();
                }
            }
            if (dgvPosts.CurrentRow != null)
            {
                int id = int.Parse(dgvPosts["ColId", dgvPosts.CurrentRow.Index].Value.ToString());
                EditPostForm dlg = new EditPostForm(id);
                if(dlg.ShowDialog()==DialogResult.OK)
                {
                    loadFromData();
                }
            }
        }
        private void btnAddPost_Click(object sender, EventArgs e)
        {
            if (this._posts.Count() > 0) 
            {
                if (MessageBox.Show("Зберегти усі зміни?", "DataGridView", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    this.SaveData();
                }
            }
            AddPostForm form = new AddPostForm();
            if (form.ShowDialog() == DialogResult.OK) 
            {
                this.loadFromData();
            }
        }
        private void dgvPosts_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            PostDataModel addPost = new PostDataModel();
            addPost.Id = e.RowIndex-1;
            
            this._posts.Add(addPost);
        }
        private void dgvPosts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var element = this._posts.SingleOrDefault(x => x.Id == e.RowIndex);
            if (this._posts.SingleOrDefault(x => x.Id == e.RowIndex) != null) 
            {
                if (e.ColumnIndex == 2)
                {
                    element.Title = dgvPosts["ColText", e.RowIndex].Value.ToString() == null ? string.Empty: 
                        dgvPosts["ColText", e.RowIndex].Value.ToString();
                }
            }
        }
        private void dgvPosts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var element = this._posts.FirstOrDefault(x => x.Id == e.RowIndex);
            if (element != null) 
            {
                if (e.ColumnIndex == 1)
                {
                    using (OpenFileDialog form = new OpenFileDialog())
                    {
                        form.Filter = "Image files (*.jpg; *.png) | *.jpg; *.png";
                        if (form.ShowDialog() == DialogResult.OK)
                        {
                            this._addedRow = form.FileName;

                            dgvPosts[e.ColumnIndex, e.RowIndex].Value = ImageCreater.CreateImage(
                                Image.FromFile(this._addedRow), 75, 75);
                        }
                    }
                } 
                else if (e.ColumnIndex == 3) 
                {
                    DataGridViewComboBoxCell cell = new DataGridViewComboBoxCell();
                    cell.DataSource = GetDataTable();
                    cell.DisplayMember = "Name";
                    cell.ValueMember = "Category";
                    
                    dgvPosts[e.ColumnIndex, e.RowIndex] = cell;
                }
            }
        }
        public void SaveData() 
        {
            foreach (var item in _posts) 
            {
                if (!string.IsNullOrEmpty(item.Title)) 
                {
                    Post post = new Post();
                    post.Title = item.Title;
                    string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
                    if (!Directory.Exists(dirPath)) 
                {
                    Directory.CreateDirectory(dirPath);
                }
                    if (!string.IsNullOrEmpty(this._addedRow)) 
                {
                    string fileName = Path.GetRandomFileName() + Path.GetExtension(this._addedRow);
                    Bitmap bmp = ImageCreater.CreateImage(Image.FromFile(this._addedRow), 75, 75);
                    post.Image = fileName;
                    bmp.Save(Path.Combine(dirPath, fileName));
                }
                    post.Text = "";
                    string cat = 
                        this.dgvPosts["ColCategory", item.Id].Value == null
                        ? null :
                        this.dgvPosts["ColCategory", item.Id].Value.ToString();
                    if (cat != null) 
                    {
                        var category = this._context.Categories.FirstOrDefault(x => x.Name.Contains(cat.ToString()));
                        post.Category = category == null ? null : category;
                    }
                    this._context.Posts.Add(post);
                }
            }

            this._context.SaveChanges();
            this._posts = new List<PostDataModel>();
            GC.Collect();

            loadFromData();
        }
        public DataTable GetDataTable() 
        {
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Category", typeof(Category));

            foreach (var item in this._context.Categories.ToList()) 
            {
                table.Rows.Add(item.Name, item);
            }

            return table;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.SaveData();
        }
    }
}
