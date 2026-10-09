using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Forms;
using Gym.Core.Services;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace Gym.UI;

public class ConfigForm : Form
{
    private readonly ConfigService _configService;
    private readonly IConfiguration _configuration;

    private Panel panelTop = new();
    private Label lblTitulo = new();
    private Button btnCerrar = new();
    
    private Label lblUrl = new();
    private TextBox txtUrl = new();
    
    private Label lblDbTitle = new();
    
    private Label lblDbServidor = new();
    private TextBox txtDbServidor = new();
    
    private Label lblDbUsuario = new();
    private TextBox txtDbUsuario = new();
    
    private Label lblDbPassword = new();
    private TextBox txtDbPassword = new();

    private Button btnGuardar = new();
    private Button btnTestear = new();
    private Label lblMensaje = new();

    // Necesario para el diseñador
    public ConfigForm()
    {
        _configService = null!;
        _configuration = null!;
    }

    public ConfigForm(ConfigService configService, IConfiguration configuration)
    {
        _configService = configService;
        _configuration = configuration;
        ConfigurarUI();
        this.Load += ConfigForm_Load;
    }

    private void ConfigurarUI()
    {
        this.Text = "Gym UNAS - Configuración";
        this.Size = new Size(500, 420);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.None;

        panelTop.BackColor = ColorTranslator.FromHtml("#005C3A");
        panelTop.Dock = DockStyle.Top;
        panelTop.Height = 40;

        lblTitulo.Text = "CONFIGURACIÓN";
        lblTitulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        lblTitulo.ForeColor = Color.White;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(10, 10);
        
        btnCerrar.Text = "X";
        btnCerrar.Size = new Size(30, 30);
        btnCerrar.Location = new Point(this.Width - 30, 5);
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.ForeColor = Color.White;
        btnCerrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCerrar.Cursor = Cursors.Hand;
        btnCerrar.Click += (s, e) => this.Close();

        panelTop.Controls.Add(lblTitulo);
        panelTop.Controls.Add(btnCerrar);

        // URL API
        lblUrl.Text = "URL de la API (OCDA):";
        lblUrl.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        lblUrl.ForeColor = Color.Gray;
        lblUrl.AutoSize = true;
        lblUrl.Location = new Point(40, 60);

        txtUrl.Location = new Point(40, 80);
        txtUrl.Size = new Size(420, 25);
        txtUrl.Font = new Font("Segoe UI", 10);
        txtUrl.BorderStyle = BorderStyle.FixedSingle;

        // Título BD
        lblDbTitle.Text = "Conexión a Base de Datos (MySQL):";
        lblDbTitle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        lblDbTitle.ForeColor = Color.Gray;
        lblDbTitle.AutoSize = true;
        lblDbTitle.Location = new Point(40, 120);

        // Servidor y BD
        lblDbServidor.Text = "Servidor y BD:";
        lblDbServidor.Font = new Font("Segoe UI", 9);
        lblDbServidor.AutoSize = true;
        lblDbServidor.Location = new Point(40, 150);

        txtDbServidor.Location = new Point(40, 170);
        txtDbServidor.Size = new Size(420, 25);
        txtDbServidor.Font = new Font("Segoe UI", 10);
        txtDbServidor.BorderStyle = BorderStyle.FixedSingle;

        // User
        lblDbUsuario.Text = "Usuario de BD:";
        lblDbUsuario.Font = new Font("Segoe UI", 9);
        lblDbUsuario.AutoSize = true;
        lblDbUsuario.Location = new Point(40, 200);

        txtDbUsuario.Location = new Point(40, 220);
        txtDbUsuario.Size = new Size(200, 25);
        txtDbUsuario.Font = new Font("Segoe UI", 10);
        txtDbUsuario.BorderStyle = BorderStyle.FixedSingle;

        // Password
        lblDbPassword.Text = "Contraseña de BD:";
        lblDbPassword.Font = new Font("Segoe UI", 9);
        lblDbPassword.AutoSize = true;
        lblDbPassword.Location = new Point(260, 200);

        txtDbPassword.Location = new Point(260, 220);
        txtDbPassword.Size = new Size(200, 25);
        txtDbPassword.Font = new Font("Segoe UI", 10);
        txtDbPassword.BorderStyle = BorderStyle.FixedSingle;
        txtDbPassword.UseSystemPasswordChar = true;

        // Botones
        btnGuardar.Text = "Guardar";
        btnGuardar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnGuardar.ForeColor = Color.White;
        btnGuardar.FlatStyle = FlatStyle.Flat;
        btnGuardar.FlatAppearance.BorderSize = 0;
        btnGuardar.Size = new Size(200, 35);
        btnGuardar.Location = new Point(40, 270);
        btnGuardar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        btnGuardar.Cursor = Cursors.Hand;
        btnGuardar.Click += BtnGuardar_Click;

        btnTestear.Text = "Testear conectividad";
        btnTestear.BackColor = Color.Orange;
        btnTestear.ForeColor = Color.White;
        btnTestear.FlatStyle = FlatStyle.Flat;
        btnTestear.FlatAppearance.BorderSize = 0;
        btnTestear.Size = new Size(200, 35);
        btnTestear.Location = new Point(260, 270);
        btnTestear.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        btnTestear.Cursor = Cursors.Hand;
        btnTestear.Click += BtnTestear_Click;

        lblMensaje.Text = "Estado: Listo";
        lblMensaje.Font = new Font("Segoe UI", 9, FontStyle.Italic);
        lblMensaje.ForeColor = Color.Gray;
        lblMensaje.AutoSize = false;
        lblMensaje.Size = new Size(420, 100);
        lblMensaje.Location = new Point(40, 315);

        this.Controls.Add(panelTop);
        this.Controls.Add(lblUrl);
        this.Controls.Add(txtUrl);
        this.Controls.Add(lblDbTitle);
        this.Controls.Add(lblDbServidor);
        this.Controls.Add(txtDbServidor);
        this.Controls.Add(lblDbUsuario);
        this.Controls.Add(txtDbUsuario);
        this.Controls.Add(lblDbPassword);
        this.Controls.Add(txtDbPassword);
        this.Controls.Add(btnGuardar);
        this.Controls.Add(btnTestear);
        this.Controls.Add(lblMensaje);
    }

    private async void ConfigForm_Load(object? sender, EventArgs e)
    {
        try
        {
            string currentConnectionString = _configuration.GetConnectionString("DefaultConnection") ?? "";
            
            if (!string.IsNullOrEmpty(currentConnectionString))
            {
                var builder = new MySqlConnectionStringBuilder(currentConnectionString);
                txtDbServidor.Text = $"Server={builder.Server};Database={builder.Database};";
                txtDbUsuario.Text = builder.UserID;
                txtDbPassword.Text = builder.Password;
            }

            txtUrl.Text = await _configService.ObtenerUrlApiAsync();
        }
        catch
        {
            lblMensaje.Text = "Aviso: No se pudo cargar toda la configuración (es posible que la DB no esté disponible aún).";
        }
    }

    private string ObtenerCadenaConexion()
    {
        var builder = new MySqlConnectionStringBuilder(txtDbServidor.Text.Trim())
        {
            UserID = txtDbUsuario.Text.Trim(),
            Password = txtDbPassword.Text
        };
        return builder.ConnectionString;
    }

    private async void BtnGuardar_Click(object? sender, EventArgs e)
    {
        try
        {
            string connectionString = ObtenerCadenaConexion();

            // Guardar en BD (la URL)
            await _configService.GuardarUrlApiAsync(txtUrl.Text);

            // Guardar en appsettings.json (la conexión)
            GuardarConnectionStringEnJson(connectionString);

            lblMensaje.Text = "Configuración guardada correctamente.\nNOTA: Los cambios en la base de datos requieren reiniciar la aplicación.";
            lblMensaje.ForeColor = Color.Green;
        }
        catch (Exception ex)
        {
            lblMensaje.Text = "Error al guardar: " + ex.Message;
            lblMensaje.ForeColor = Color.Red;
        }
    }

    private void GuardarConnectionStringEnJson(string connectionString)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var jsonNode = JsonNode.Parse(json);
            if (jsonNode != null)
            {
                if (jsonNode["ConnectionStrings"] == null)
                {
                    jsonNode["ConnectionStrings"] = new JsonObject();
                }
                jsonNode["ConnectionStrings"]!["DefaultConnection"] = connectionString;
                File.WriteAllText(path, jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            }
        }
        
        string projPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\appsettings.json"));
        if (File.Exists(projPath))
        {
            string json = File.ReadAllText(projPath);
            var jsonNode = JsonNode.Parse(json);
            if (jsonNode != null)
            {
                if (jsonNode["ConnectionStrings"] == null)
                {
                    jsonNode["ConnectionStrings"] = new JsonObject();
                }
                jsonNode["ConnectionStrings"]!["DefaultConnection"] = connectionString;
                File.WriteAllText(projPath, jsonNode.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            }
        }
    }

    private async void BtnTestear_Click(object? sender, EventArgs e)
    {
        btnTestear.Enabled = false;
        lblMensaje.Text = "Testeando conexiones...\n";
        lblMensaje.ForeColor = Color.Black;

        try
        {
            string testConnString = ObtenerCadenaConexion();
            bool dbOk = await _configService.TestearConexionBdAsync(testConnString);
            lblMensaje.Text += $"DB MySQL: {(dbOk ? "OK" : "FALLÓ")}\n";
            
            bool apiOk = false;
            if (dbOk)
            {
                apiOk = await _configService.TestearConexionApiAsync(txtUrl.Text);
            }
            lblMensaje.Text += $"API OCDA: {(apiOk ? "OK" : "FALLÓ (Requiere DB OK y URL válida)")}\n";
            
            if (dbOk && apiOk)
                lblMensaje.ForeColor = ColorTranslator.FromHtml("#005C3A");
            else
                lblMensaje.ForeColor = Color.Red;
        }
        catch (Exception ex)
        {
            lblMensaje.Text += "\nError: " + ex.Message;
            lblMensaje.ForeColor = Color.Red;
        }
        finally
        {
            btnTestear.Enabled = true;
        }
    }
}
