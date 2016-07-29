using Naanayam.Data;
using Naanayam.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Naanayam.Test
{
    public partial class MainWin : Form
    {
        protected IServer Server { get; set; }

        public MainWin()
        {
            InitializeComponent();

            Server = new Agent(new Database(Properties.Settings.Default.Database), new Context("system"));
        }

        private async void Uninstall_Click(object sender, EventArgs e)
        {
            await Server.UninstallAsync();

            MessageBox.Show("OK");
        }

        private async void Install_Click(object sender, EventArgs e)
        {
            await Server.InstallAsync();

            MessageBox.Show("OK");
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            Server.AddTransactionCategory("test", "1");
        }
    }
}
