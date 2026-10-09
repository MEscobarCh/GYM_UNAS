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
    private Label lblTitulo = new();
    private Label lblSubtitulo = new();
    
    private TextBox txtUsuario = new();
    private TextBox txtPassword = new();
    private Label lblUsuario = new();
    private Label lblPassword = new();
    private Button btnLogin = new();
    private Label lblMensaje = new();
    private Button btnCerrar = new();

    public LoginForm(AuthService authService, IServiceProvider serviceProvider)
    {
        _authService = authService;
        _serviceProvider = serviceProvider;
        ConfigurarUI();
    }

    private void ConfigurarUI()
    {
        // Propiedades del formulario base
        this.FormBorderStyle = FormBorderStyle.None;
        this.BackColor = Color.White;
        this.Size = new Size(700, 400);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Panel decorativo (izquierdo)
        panelIzquierdo.BackColor = ColorTranslator.FromHtml("#005C3A"); // Verde institucional UNAS
        panelIzquierdo.Dock = DockStyle.Left;
        panelIzquierdo.Width = 250;

        // Botón de cerrar ventana (esquina superior derecha)
        btnCerrar.Text = "X";
        btnCerrar.Size = new Size(30, 30);
        btnCerrar.Location = new Point(this.Width - 30, 0);
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.ForeColor = Color.Gray;
        btnCerrar.Cursor = Cursors.Hand;
        btnCerrar.Click += (s, e) => Application.Exit();

        // Títulos
        lblTitulo.Text = "BIENVENIDO";
        lblTitulo.Font = new Font("Segoe UI", 20, FontStyle.Bold);
        lblTitulo.ForeColor = ColorTranslator.FromHtml("#005C3A");
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(380, 50);

        lblSubtitulo.Text = "Sistema Gym UNAS";
        lblSubtitulo.Font = new Font("Segoe UI", 12, FontStyle.Regular);
        lblSubtitulo.ForeColor = Color.Gray;
        lblSubtitulo.AutoSize = true;
        lblSubtitulo.Location = new Point(385, 85);

        // Inputs
        lblUsuario.Text = "USUARIO:";
        lblUsuario.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        lblUsuario.ForeColor = Color.Gray;
        lblUsuario.AutoSize = true;
        lblUsuario.Location = new Point(340, 150);

        txtUsuario.Location = new Point(340, 170);
        txtUsuario.Size = new Size(270, 30);
        txtUsuario.Font = new Font("Segoe UI", 11);
        txtUsuario.BorderStyle = BorderStyle.FixedSingle;

        lblPassword.Text = "CONTRASEÑA:";
        lblPassword.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        lblPassword.ForeColor = Color.Gray;
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(340, 220);

        txtPassword.Location = new Point(340, 240);
        txtPassword.Size = new Size(270, 30);
        txtPassword.Font = new Font("Segoe UI", 11);
        txtPassword.PasswordChar = '*';
        txtPassword.BorderStyle = BorderStyle.FixedSingle;

        // Botón Ingresar
        btnLogin.Text = "Ingresar al sistema";
        btnLogin.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnLogin.ForeColor = Color.White;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Size = new Size(270, 40);
        btnLogin.Location = new Point(340, 290);
        btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.Click += btnLogin_Click;

        // Mensaje de estado
        lblMensaje.AutoSize = false;
        lblMensaje.Size = new Size(270, 30);
        lblMensaje.Location = new Point(340, 340);
        lblMensaje.TextAlign = ContentAlignment.MiddleCenter;
        lblMensaje.Font = new Font("Segoe UI", 9);

        // Botón Configuración (Engranaje)
        Button btnConfig = new();
        btnConfig.Text = "⚙";
        btnConfig.Size = new Size(40, 40);
        btnConfig.Location = new Point(10, this.Height - 50);
        btnConfig.FlatStyle = FlatStyle.Flat;
        btnConfig.FlatAppearance.BorderSize = 0;
        btnConfig.ForeColor = Color.White;
        btnConfig.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        btnConfig.Cursor = Cursors.Hand;
        btnConfig.Click += (s, e) => {
            var configForm = (ConfigForm)_serviceProvider.GetService(typeof(ConfigForm))!;
            configForm.ShowDialog();
        };
        panelIzquierdo.Controls.Add(btnConfig);

        // Agregar controles
        this.Controls.Add(panelIzquierdo);
        this.Controls.Add(btnCerrar);
        this.Controls.Add(lblTitulo);
        this.Controls.Add(lblSubtitulo);
        this.Controls.Add(lblUsuario);
        this.Controls.Add(txtUsuario);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(lblMensaje);
    }

    private async void btnLogin_Click(object? sender, EventArgs e)
    {
        // Pequeña validación de UI rápida antes de ir al backend
        if (string.IsNullOrWhiteSpace(txtUsuario.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
        {
            lblMensaje.Text = "Complete todos los campos.";
            lblMensaje.ForeColor = Color.Red;
            return;
        }

        lblMensaje.Text = "Verificando credenciales...";
        lblMensaje.ForeColor = Color.Gray;
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
            lblMensaje.Text = ex.Message;
            lblMensaje.ForeColor = Color.Red;
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
