﻿using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
namespace Invoice
{
    public partial class StoreList : Form
    {
        DbConnectorClass db;
        SqlDataAdapter adapter;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        public StoreList()
        {
            InitializeComponent();
            GetStoreList();
            AddEditBtn();
        }
        public void GetStoreList()
        {
            this.ShowMarketCheckBox.Checked = true;
            this.ShowCustomerCheckBox.Checked = true;
            GetStoreList(true, true);
        }
        public void GetStoreList(bool isMarket, bool isCustomer)
        {
            try
            {
                String whereStr = "";
                if (isMarket && isCustomer)
                {
                    whereStr = "";
                }
                else if (isMarket)
                {
                    whereStr = "where isMarket = 1";
                }
                else if (isCustomer)
                {
                    whereStr = "where isMarket = 0";
                }
                else
                {
                    whereStr = "where isMarket = 2";
                }
                db = new DbConnectorClass();
                adapter = new SqlDataAdapter("SELECT store_id AS 'Store Id', store_name AS 'Store Name'," +
                    "store_address AS Address, store_phone AS Phone, store_fax AS Fax, " +
                    "contact_name AS 'Contact Name', contact_phone AS 'Contact #', store_detail AS 'Store Detail', " +
                    "CASE WHEN  isMarket = 1 THEN 'True' ELSE 'False' END AS 'Is Market'" +
                    "FROM invoice.dbo.store " + whereStr + " order by store_name ", db.GetConnection());
                // Create one DataTable with one column.
                DataSet DS = new DataSet();
                adapter.Fill(DS);
                this.StoreDataView.DataSource = DS.Tables[0];
                this.StoreDataView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AddEditBtn()
        {
            DataGridViewButtonColumn EditBtn = new DataGridViewButtonColumn
            {
                HeaderText = "",
                Name = "Edit",
                Text = "Edit",
                UseColumnTextForButtonValue = true
            };
            this.StoreDataView.Columns.Add(EditBtn);
            this.StoreDataView.CellClick += new DataGridViewCellEventHandler(DataGridView_CellClick);
            //

        }
        // Calls the Employee.RequestStatus method.
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex !=
                this.StoreDataView.Columns["Edit"].Index) return;

            // Retrieve the task ID.
            String storeId = (String)this.StoreDataView[1, e.RowIndex].Value.ToString();
            CreateStore cs = new CreateStore(storeId, this);
            cs.Show();
        }

        private void ShowOptionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool isMarket = this.ShowMarketCheckBox.Checked;
            bool isCustomer = this.ShowCustomerCheckBox.Checked;
            GetStoreList(isMarket, isCustomer);
        }

        private void CloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void DragTitlePanel(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
