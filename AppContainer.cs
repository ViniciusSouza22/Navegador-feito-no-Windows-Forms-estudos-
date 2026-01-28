using EasyTabs;
using System;
using System.Windows.Forms;

namespace Bunifu_Browser
{
    public partial class AppContainer : TitleBarTabs
    {
        public AppContainer()
        {
            InitializeComponent();

            // Configurações do container
            AeroPeekEnabled = true;
            TabRenderer = new ChromeTabRenderer(this);
            this.Text = "Bunifu Browser (WebView2)";
        }

        public override TitleBarTab CreateTab()
        {
            return new TitleBarTab(this)
            {
                Content = new frmBrowser()
                {
                    Text = "Nova Aba"
                }
            };
        }
    }
}