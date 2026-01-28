using EasyTabs;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bunifu_Browser
{
    public partial class frmBrowser : Form
    {
        
        // Controles da interface - Você pode alterar os nomes se quiser
        private Panel pnlTop;
        private Button btnBack, btnForward, btnRefresh, btnHome, btnNewTab;
        private TextBox txtAddress;
        private WebView2 webView;
        private Panel pnlStatus;
        private Label lblStatus;
        private ProgressBar prgLoad;

        // Configurações - Edite esses valores para personalizar
        private readonly Color TOP_PANEL_COLOR = Color.FromArgb(51, 51, 51);
        private readonly Color STATUS_PANEL_COLOR = Color.FromArgb(30, 30, 30);
        private readonly Color BUTTON_COLOR = Color.FromArgb(66, 66, 66);
        private readonly Color TEXTBOX_COLOR = Color.FromArgb(30, 30, 30);
        private readonly Color TEXT_COLOR = Color.White;

        private readonly string HOME_PAGE = "https://www.google.com";
        private readonly string SEARCH_ENGINE = "https://www.google.com/search?q={0}";

        private readonly int TOP_PANEL_HEIGHT = 45;
        private readonly int STATUS_PANEL_HEIGHT = 30;
        private readonly int BUTTON_SIZE = 35;
        private readonly Font ADDRESS_FONT = new Font("Segoe UI", 10);
        private readonly Font STATUS_FONT = new Font("Segoe UI", 9);

       

        public frmBrowser()
        {
            InitializeComponent();
            InitializeAsync();
        }

       
        private async void InitializeAsync()
        {
            await InitializeWebView2();
            SetupBrowserControls();
            NavigateTo(HOME_PAGE);
        }

        private async System.Threading.Tasks.Task InitializeWebView2()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                CreationProperties = null,
                DefaultBackgroundColor = Color.White
            };

            //  Ambiente WebView2 - Pode mudar parâmetros
            var environment = await CoreWebView2Environment.CreateAsync();
            await webView.EnsureCoreWebView2Async(environment);

            //  Conectando eventos - Pode adicionar/remover eventos
            webView.NavigationStarting += WebView_NavigationStarting;
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.CoreWebView2.SourceChanged += CoreWebView2_SourceChanged;
            webView.CoreWebView2.HistoryChanged += CoreWebView2_HistoryChanged;

            //  Configurações do navegador - Edite conforme necessidade
            webView.CoreWebView2.Settings.AreDevToolsEnabled = true;          // F12 para DevTools
            webView.CoreWebView2.Settings.IsScriptEnabled = true;             // Habilita JavaScript
            webView.CoreWebView2.Settings.IsWebMessageEnabled = true;         // Mensagens Web
            webView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = true; // Diálogos JS
            webView.CoreWebView2.Settings.IsStatusBarEnabled = true;          // Barra de status

           
        }

        // ============================================
        //  CONFIGURAÇÃO DA INTERFACE - Aqui você edita o visual
        // ============================================

        private void SetupBrowserControls()
        {
            //  Tamanho da janela - Altere os valores
            this.ClientSize = new Size(1024, 768);

            // CRIAR PAINEL SUPERIOR
            pnlTop = new Panel
            {
                Height = TOP_PANEL_HEIGHT,
                Dock = DockStyle.Top,
                BackColor = TOP_PANEL_COLOR,
                Padding = new Padding(5)
            };

            //  BOTÕES DE NAVEGAÇÃO
            btnBack = CreateNavButton("←", 5, false);
            btnBack.Click += (s, e) => webView?.GoBack();

            btnForward = CreateNavButton("→", 45, false);
            btnForward.Click += (s, e) => webView?.GoForward();

            btnRefresh = CreateNavButton("↻", 85, true);
            btnRefresh.Click += (s, e) => webView?.Reload();

            btnHome = CreateNavButton("⌂", 125, true);
            btnHome.Click += (s, e) => NavigateTo(HOME_PAGE);

            btnNewTab = CreateNavButton("➕", 165, true);
            btnNewTab.Click += (s, e) => CreateNewTab();

            //  BARRA DE ENDEREÇO
            txtAddress = new TextBox
            {
                Location = new Point(205, 8),
                Size = new Size(650, 29),
                Text = HOME_PAGE,
                BackColor = TEXTBOX_COLOR,
                ForeColor = TEXT_COLOR,
                BorderStyle = BorderStyle.FixedSingle,
                Font = ADDRESS_FONT
            };
            txtAddress.KeyDown += TxtAddress_KeyDown;

           
            //  PAINEL DE STATUS
            pnlStatus = new Panel
            {
                Height = STATUS_PANEL_HEIGHT,
                Dock = DockStyle.Bottom,
                BackColor = STATUS_PANEL_COLOR
            };

            lblStatus = new Label
            {
                Text = "Pronto",
                Location = new Point(10, 8),
                ForeColor = TEXT_COLOR,
                AutoSize = true,
                Font = STATUS_FONT
            };

            prgLoad = new ProgressBar
            {
                Location = new Point(800, 5),
                Size = new Size(200, 20),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };

            //  MONTANDO A INTERFACE
            pnlTop.Controls.AddRange(new Control[]
            {
                btnBack, btnForward, btnRefresh, btnHome, btnNewTab, txtAddress
            });

            pnlStatus.Controls.AddRange(new Control[]
            {
                lblStatus, prgLoad
            });

            this.Controls.AddRange(new Control[]
            {
                webView, pnlStatus, pnlTop
            });
        }

        // ============================================
        //  MÉTODOS AUXILIARES - Edite para customizar
        // ============================================

        private Button CreateNavButton(string text, int x, bool enabled)
        {
            return new Button
            {
                Text = text,
                Size = new Size(BUTTON_SIZE, BUTTON_SIZE),
                Location = new Point(x, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = BUTTON_COLOR,
                ForeColor = TEXT_COLOR,
                Enabled = enabled
            };
        }

        // ============================================
        //  MÉTODOS DE NAVEGAÇÃO - Personalize a navegação
        // ============================================

        private void NavigateTo(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            //  Detecta se é busca ou URL
            if (!IsValidUrl(url))
            {
                // É uma busca
                url = string.Format(SEARCH_ENGINE, Uri.EscapeDataString(url));
            }
            else if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            //  Executa a navegação
            if (webView != null && webView.CoreWebView2 != null)
            {
                webView.CoreWebView2.Navigate(url);
                txtAddress.Text = url;
            }
        }

        private bool IsValidUrl(string input)
        {
            //  Lógica para detectar URLs - Pode melhorar
            return (input.Contains(".") && !input.Contains(" ")) ||
                   input.StartsWith("http://") ||
                   input.StartsWith("https://") ||
                   input.StartsWith("www.");
        }

        private void CreateNewTab()
        {
            if (ParentTabs != null)
            {
                ParentTabs.CreateTab();
            }
        }

      
        private void WebView_NavigationStarting(object sender, CoreWebView2NavigationStartingEventArgs e)
        {
            // ⏳ Mostra indicador de carregamento
            prgLoad.Style = ProgressBarStyle.Marquee;
            prgLoad.Visible = true;
            lblStatus.Text = "Carregando...";

            //  Bloqueia URLs indesejadas (adicione mais se quiser)
            string[] blockedUrls = { "edge://", "chrome://", "about:" };
            foreach (var blocked in blockedUrls)
            {
                if (e.Uri.StartsWith(blocked))
                {
                    e.Cancel = true;
                    lblStatus.Text = "Navegação bloqueada";
                    prgLoad.Visible = false;
                    break;
                }
            }
        }

        private void WebView_NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            //  Finaliza carregamento
            prgLoad.Visible = false;
            lblStatus.Text = "Pronto";

            //  Atualiza títulos
            if (webView != null && webView.CoreWebView2 != null)
            {
                this.Text = webView.CoreWebView2.DocumentTitle;

                if (ParentTabs != null && ParentTabs.SelectedTab != null)
                {
                    ParentTabs.SelectedTab.Content.Text = webView.CoreWebView2.DocumentTitle;
                }
            }
        }

        private void CoreWebView2_SourceChanged(object sender, CoreWebView2SourceChangedEventArgs e)
        {
            //  Atualiza barra de endereço
            if (webView != null && webView.CoreWebView2 != null && txtAddress != null)
            {
                txtAddress.Text = webView.CoreWebView2.Source;
            }
        }

        private void CoreWebView2_HistoryChanged(object sender, object e)
        {
            //  Atualiza estado dos botões de navegação
            if (webView != null && webView.CoreWebView2 != null)
            {
                btnBack.Enabled = webView.CoreWebView2.CanGoBack;
                btnForward.Enabled = webView.CoreWebView2.CanGoForward;
            }
        }

        private void TxtAddress_KeyDown(object sender, KeyEventArgs e)
        {
            //  Navega com Enter
            if (e.KeyCode == Keys.Enter)
            {
                NavigateTo(txtAddress.Text);
                e.Handled = e.SuppressKeyPress = true;
            }

          
        }

        // ============================================
        // MÉTODOS PÚBLICOS - Interface para controle externo
        // ============================================

        public void GoBack() => webView?.GoBack();
        public void GoForward() => webView?.GoForward();
        public void RefreshPage() => webView?.Reload();
        public void Navigate(string url) => NavigateTo(url);
        public string GetCurrentUrl() => webView?.CoreWebView2?.Source ?? "";

        // Propriedade para acessar o container de abas
        protected TitleBarTabs ParentTabs => ParentForm as TitleBarTabs;

        // ============================================
        // MÉTODOS ADICIONAIS - Adicione suas funções aqui
        // ============================================

        //  Exemplo: Método para adicionar favoritos
        public void AddToFavorites(string url)
        {
            // Implemente seu sistema de favoritos aqui
            MessageBox.Show($"Adicionado aos favoritos: {url}", "Favoritos",
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //  Exemplo: Método para salvar histórico
        public void SaveHistory()
        {
            // Implemente salvar histórico aqui
        }

        // 🎨 Exemplo: Método para mudar tema
        public void ChangeTheme(bool darkMode)
        {
            if (darkMode)
            {
                TOP_PANEL_COLOR = Color.FromArgb(30, 30, 30);
                STATUS_PANEL_COLOR = Color.FromArgb(20, 20, 20);
                TEXTBOX_COLOR = Color.FromArgb(40, 40, 40);
            }
            else
            {
                TOP_PANEL_COLOR = Color.FromArgb(240, 240, 240);
                STATUS_PANEL_COLOR = Color.FromArgb(220, 220, 220);
                TEXTBOX_COLOR = Color.White;
                TEXT_COLOR = Color.Black;
            }

            // Aplica as cores
            pnlTop.BackColor = TOP_PANEL_COLOR;
            pnlStatus.BackColor = STATUS_PANEL_COLOR;
            txtAddress.BackColor = TEXTBOX_COLOR;
            txtAddress.ForeColor = TEXT_COLOR;
            lblStatus.ForeColor = TEXT_COLOR;
        }

        // ⌨️ Exemplo: Atalhos de teclado globais
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.T:
                    CreateNewTab();
                    return true;

                case Keys.Control | Keys.W:
                    // Fecha aba atual
                    if (ParentTabs != null && ParentTabs.SelectedTabIndex >= 0)
                    {
                        ParentTabs.Tabs.RemoveAt(ParentTabs.SelectedTabIndex);
                        return true;
                    }
                    break;

                case Keys.F5:
                    RefreshPage();
                    return true;

                case Keys.Control | Keys.F5:
                    // Recarga completa
                    NavigateTo(GetCurrentUrl());
                    return true;

                case Keys.Control | Keys.L:
                    txtAddress.Focus();
                    txtAddress.SelectAll();
                    return true;

                case Keys.Control | Keys.D:
                    AddToFavorites(GetCurrentUrl());
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}