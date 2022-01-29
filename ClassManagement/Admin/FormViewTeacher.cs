using System;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClassManagement.Admin {
	public partial class FormViewTeacher : Form {
		StepSchedulerEntities bd = null; // создаем объект entity framework
		Users user = null;

		public FormViewTeacher(Users users) {
			InitializeComponent();
			try {
				bd = new StepSchedulerEntities();
				tsb_search.Image = Image.FromFile(@"images/search.png");
				tsb_show_blocked.Image = Image.FromFile(@"images/uncheck.png");
				this.user = users;
				InitUser();
				dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
				dataGridView.RowHeadersVisible = false;
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}		
		}

		private void InitUser() {
			try {
				bd.Users.Load(); //подключаемся к базе к таб.User
				dataGridView.DataSource = null;
				dataGridView.DataSource = bd.Users.Local.ToBindingList();//выводим данные в dataGrid
				dataGridView.Columns["Requests"].Visible = false;//убираем колонки таблицы Requests
				dataGridView.Columns["UserId"].Visible = false;
				dataGridView.Columns["Photo"].Visible = false;
				dataGridView.Columns["Login"].Visible = false;
				dataGridView.Columns["Password"].Visible = false;
				dataGridView.Columns["IsAdmin"].Visible = false;
				dataGridView.Columns["IsBlocked"].Visible = false;
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void tsb_add_Click(object sender, EventArgs e) {
			try {
				user = new Users();
				TeacherInfoForm form = new TeacherInfoForm(user);
				form.Show();
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void tsb_edit_Click(object sender, EventArgs e) {
			try {
				int index = dataGridView.SelectedRows[0].Index;
				int Id = 0;
				bool converted = Int32.TryParse(dataGridView[0, index].Value.ToString(), out Id);
				if (converted == false) return;
				user = bd.Users.Find(Id);//находим по индексу значение
				TeacherInfoForm f = new TeacherInfoForm(user);//открываем доп. форму
				if (f.ShowDialog() == DialogResult.OK) {
					bd.SaveChanges();//сохраняем изменения
					dataGridView.Update();//обновляем отредактированые данные
					dataGridView.Refresh();
					MessageBox.Show("Данные отредактированные!");
				}
				else {
					MessageBox.Show("Не удалось отредактировать данные!");
				}
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void tsb_block_Click(object sender, EventArgs e) {
			try {
				if (dataGridView.SelectedRows.Count == 1) {
					int index = dataGridView.SelectedRows[0].Index;
					int Id;
					bool converted = Int32.TryParse(dataGridView[0, index].Value.ToString(), out Id);
					if (converted == false) return;
					user = bd.Users.Find(Id);
					user.IsBlocked = true;
					bd.SaveChanges();
					//выделяем пользователя серым цветом
					dataGridView.SelectedRows[0].DefaultCellStyle.BackColor = Color.Gray;
					dataGridView.Update();
					dataGridView.Refresh();
					MessageBox.Show("Пользователь заблокирован!");
				}
				else {
					MessageBox.Show("Не удалось заблокировать данные!");
				}
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void tsb_unblock_Click(object sender, EventArgs e) {

		}

		private void tsb_clear_search_Click(object sender, EventArgs e) {
			FullInformation();
		}

		private void tsb_search_Click(object sender, EventArgs e) {
			try {
				if (ts_txb_search.Text != "") {
					using (bd = new StepSchedulerEntities()) {
						bd.Users.Where(x => x.Name.Contains(ts_txb_search.Text) || x.Surname.Contains(ts_txb_search.Text) || x.Login.Contains(ts_txb_search.Text)
						|| x.E_Mail.Contains(ts_txb_search.Text)).Load();
						dataGridView.DataSource = bd.Users.Local.ToBindingList();
						dataGridView.Columns["Requests"].Visible = false;
					}
				}
				else if (ts_txb_search.Text == "") {
					FullInformation();
				}
				else {
					MessageBox.Show("Введите текст!");
				}
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void FullInformation() {
			try {
				bd = new StepSchedulerEntities();
				bd.Users.Load(); //подключаемся к базе к таб.User
				dataGridView.DataSource = null;
				dataGridView.DataSource = bd.Users.Local.ToBindingList();//выводим данные в dataGrid
				dataGridView.Columns["Requests"].Visible = false;//убираем колонки таблицы Requests           
				dataGridView.Columns["Photo"].Visible = false;
				if (tsb_show_blocked.Checked) {
					tsb_show_blocked.Checked = false;
				}
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}

		private void tsb_show_blocked_CheckedChanged(object sender, EventArgs e) {
			try {
				if (tsb_show_blocked.Checked) {
					tsb_show_blocked.Image = Image.FromFile(@"images/check.png");
					bd = new StepSchedulerEntities();
					bd.Users.Load();
					var query = bd.Users.Local.Where(x => x.IsBlocked == true).ToList();
					dataGridView.DataSource = null;
					dataGridView.DataSource = query;
					dataGridView.Columns["Requests"].Visible = false;//убираем колонки таблицы Requests
					dataGridView.Columns["UserId"].Visible = false;
					dataGridView.Columns["Photo"].Visible = false;
					dataGridView.Columns["BirthDate"].Visible = false;
				}
				else {
					InitUser();
					tsb_show_blocked.Image = Image.FromFile(@"images/uncheck.png");
				}
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
		}
	}
}
