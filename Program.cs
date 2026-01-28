using EasyTabs;
using System;
using System.Windows.Forms;

namespace Bunifu_Browser
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppContainer container = new AppContainer();

            container.Tabs.Add(new TitleBarTab(container)
            {
                Content = new frmBrowser()
                {
                    Text = "Nova Aba"
                }
            });

            container.SelectedTabIndex = 0;

            TitleBarTabsApplicationContext context = new TitleBarTabsApplicationContext();
            context.Start(container);
            Application.Run(context);
        }
    }
}