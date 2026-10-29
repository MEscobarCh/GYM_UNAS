using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Exceptions;
using Gym.Core.Services;

namespace Gym.UI;

public class LoginForm : Form
{
    private readonly AuthService _authService;
    private readonly IServiceProvider _serviceProvider;

    // Controles
    private Panel panelIzquierdo = new();
    private Label lblSubtitulo = new();
    
    private TextBox txtUsuario = new();
    private TextBox txtPassword = new();
    private Label lblUsuario = new();
    private Label lblPassword = new();
    private Button btnLogin = new();
    private Label lblMensaje = new();

    public LoginForm(AuthService authService, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _serviceProvider = serviceProvider;
        ConfigurarUI();
    }

    private void ConfigurarUI()
    {
        // Propiedades del formulario base
        this.Text = "Gym UNAS - Acceso";
        this.BackColor = Color.White;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(720, 440);
        this.Size = new Size(720, 440);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // Panel decorativo (izquierdo)
        panelIzquierdo.BackColor = ColorTranslator.FromHtml("#005C3A"); // Verde institucional UNAS
        panelIzquierdo.Dock = DockStyle.Left;
        panelIzquierdo.Width = 240;

        // Botón Configuración (Engranaje) visible y anclado superior derecho del panel
        Button btnConfig = new();
        btnConfig.Text = "⚙";
        btnConfig.Size = new Size(36, 36);
        btnConfig.Location = new Point(panelIzquierdo.Width - 48, 12);
        btnConfig.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnConfig.FlatStyle = FlatStyle.Flat;
        btnConfig.FlatAppearance.BorderSize = 0;
        btnConfig.BackColor = Color.Transparent;
        btnConfig.ForeColor = Color.FromArgb(180, 255, 255, 255);
        btnConfig.Font = new Font("Segoe UI", 15);
        btnConfig.Cursor = Cursors.Hand;
        btnConfig.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#004A2E");
        btnConfig.Click += (s, e) => {
            var configForm = (ConfigForm)_serviceProvider.GetService(typeof(ConfigForm))!;
            configForm.ShowDialog();
        };
        panelIzquierdo.Controls.Add(btnConfig);

        // Título / Subtítulo
        lblSubtitulo.Text = "Sistema Gym UNAS";
        lblSubtitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblSubtitulo.ForeColor = ColorTranslator.FromHtml("#222222");
        lblSubtitulo.AutoSize = false;
        lblSubtitulo.Size = new Size(320, 28);
        lblSubtitulo.TextAlign = ContentAlignment.MiddleCenter;
        lblSubtitulo.Location = new Point(310, 48);

        // Inputs con separación vertical adecuada de +27px debajo de su Label
        lblUsuario.Text = "USUARIO:";
        lblUsuario.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        lblUsuario.ForeColor = Color.FromArgb(100, 100, 100);
        lblUsuario.AutoSize = false;
        lblUsuario.Size = new Size(320, 18);
        lblUsuario.Location = new Point(310, 95);

        txtUsuario.Location = new Point(310, 122);
        txtUsuario.Size = new Size(320, 28);
        txtUsuario.Font = new Font("Segoe UI", 10.5f);
        txtUsuario.BorderStyle = BorderStyle.FixedSingle;

        lblPassword.Text = "CONTRASEÑA:";
        lblPassword.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        lblPassword.ForeColor = Color.FromArgb(100, 100, 100);
        lblPassword.AutoSize = false;
        lblPassword.Size = new Size(320, 18);
        lblPassword.Location = new Point(310, 165);

        txtPassword.Location = new Point(310, 192);
        txtPassword.Size = new Size(320, 28);
        txtPassword.Font = new Font("Segoe UI", 10.5f);
        txtPassword.PasswordChar = '*';
        txtPassword.BorderStyle = BorderStyle.FixedSingle;

        // Botón Ingresar
        btnLogin.Text = "Ingresar al sistema";
        btnLogin.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnLogin.ForeColor = Color.White;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#004A2E");
        btnLogin.Size = new Size(320, 40);
        btnLogin.Location = new Point(310, 240);
        btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.Click += btnLogin_Click;
        this.AcceptButton = btnLogin;

        // Mensaje de estado (Badge)
        lblMensaje.AutoSize = false;
        lblMensaje.Size = new Size(320, 32);
        lblMensaje.Location = new Point(310, 295);
        lblMensaje.TextAlign = ContentAlignment.MiddleCenter;
        lblMensaje.Font = new Font("Segoe UI", 9);
        lblMensaje.Visible = false;

        // Agregar controles
        this.Controls.Add(panelIzquierdo);
        this.Controls.Add(lblSubtitulo);
        this.Controls.Add(lblUsuario);
        this.Controls.Add(txtUsuario);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(lblMensaje);
    }

    private void MostrarBadge(string mensaje, bool esError)
    {
        lblMensaje.Text = mensaje;
        lblMensaje.BackColor = esError ? Color.FromArgb(253, 237, 237) : Color.FromArgb(235, 243, 250);
        lblMensaje.ForeColor = esError ? Color.FromArgb(176, 0, 32) : Color.FromArgb(25, 103, 210);
        lblMensaje.Visible = true;
    }

    private async void btnLogin_Click(object? sender, EventArgs e)
    {
        // Pequeña validación de UI rápida antes de ir al backend
        if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            MostrarBadge("Complete todos los campos.", esError: true);
            return;
        }

        MostrarBadge("Verificando credenciales...", esError: false);
        btnLogin.Enabled = false;

        try
        {
            // El llamado asíncrono libera el hilo principal (no se congela la UI)
            var usuario = await _authService.LoginAsync(txtUsuario.Text, txtPassword.Text);
            
            // Login exitoso
            this.Hide();
            var dashboard = (DashboardForm)_serviceProvider.GetService(typeof(DashboardForm))!;
            dashboard.FormClosed += (s, args) => this.Close(); // Cierra toda la app si el dashboard se cierra
            dashboard.Show();
        }
        catch (CredencialesInvalidasException ex)
        {
            MostrarBadge(ex.Message, esError: true);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error de Login", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLogin.Enabled = true;
        }
    }
}
