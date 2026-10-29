using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using System.Threading.Tasks;

namespace Gym.UI;

public class RegistroExternoForm : Form
{
    private readonly IPersonaRepository _personaRepository;
    private readonly IReniecClient _reniecClient;
    private readonly IBiometricoClient _biometricoClient;
    private readonly string _dniEscaneado;
    
    public int NuevoIdPersona { get; private set; } = 0;

    // UI Controls

    private Label lblDni = new();
    private TextBox txtDni = new();

    private Label lblNombres = new();
    private TextBox txtNombres = new();

    private Label lblTelefono = new();
    private TextBox txtTelefono = new();

    private Label lblCorreo = new();
    private TextBox txtCorreo = new();

    private Label lblError = new();
    
    private Button btnCapturarHuella = new();
    private Label lblHuellaEstado = new();
    private string? _huellaSimulada = null;
    
    private Button btnGuardar = new();
    private Button btnCancelar = new();

    public RegistroExternoForm(IPersonaRepository personaRepository, IReniecClient reniecClient, IBiometricoClient biometricoClient, string dniEscaneado)
    {
        _personaRepository = personaRepository;
        _reniecClient = reniecClient;
        _biometricoClient = biometricoClient;
        _dniEscaneado = dniEscaneado;
        
        ConfigurarUI();
        this.Load += RegistroExternoForm_Load;
    }

    private void ConfigurarUI()
    {
        this.Text = "Registro de Nuevo Externo";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ClientSize = new Size(400, 440);
        this.MinimumSize = new Size(415, 480);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;

        // Top Panel

        int startY = 20;
        int spacing = 68;
        int leftX = 30;

        lblDni.Text = "DNI:";
        lblDni.Font = new Font("Segoe UI", 10);
        lblDni.Location = new Point(leftX, startY);
        lblDni.AutoSize = true;

        txtDni.Location = new Point(leftX, startY + 26);
        txtDni.Size = new Size(340, 30);
        txtDni.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtDni.Font = new Font("Segoe UI", 11);
        txtDni.Enabled = false; // Bloqueado
        txtDni.BackColor = Color.WhiteSmoke;

        startY += spacing;

        lblNombres.Text = "Nombres y Apellidos (*):";
        lblNombres.Font = new Font("Segoe UI", 10);
        lblNombres.Location = new Point(leftX, startY);
        lblNombres.AutoSize = true;

        txtNombres.Location = new Point(leftX, startY + 26);
        txtNombres.Size = new Size(340, 30);
        txtNombres.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtNombres.Font = new Font("Segoe UI", 11);

        startY += spacing;

        lblTelefono.Text = "Teléfono:";
        lblTelefono.Font = new Font("Segoe UI", 10);
        lblTelefono.Location = new Point(leftX, startY);
        lblTelefono.AutoSize = true;

        txtTelefono.Location = new Point(leftX, startY + 26);
        txtTelefono.Size = new Size(340, 30);
        txtTelefono.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtTelefono.Font = new Font("Segoe UI", 11);

        startY += spacing;

        lblCorreo.Text = "Correo Electrónico:";
        lblCorreo.Font = new Font("Segoe UI", 10);
        lblCorreo.Location = new Point(leftX, startY);
        lblCorreo.AutoSize = true;

        txtCorreo.Location = new Point(leftX, startY + 26);
        txtCorreo.Size = new Size(340, 30);
        txtCorreo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtCorreo.Font = new Font("Segoe UI", 11);

        startY += 55;

        btnCapturarHuella.Text = "Capturar Huella";
        btnCapturarHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnCapturarHuella.ForeColor = Color.White;
        btnCapturarHuella.FlatStyle = FlatStyle.Flat;
        btnCapturarHuella.FlatAppearance.BorderSize = 0;
        btnCapturarHuella.Size = new Size(130, 30);
        btnCapturarHuella.Location = new Point(leftX, startY);
        btnCapturarHuella.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        btnCapturarHuella.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        btnCapturarHuella.Cursor = Cursors.Hand;
        btnCapturarHuella.Click += BtnCapturarHuella_Click;

        lblHuellaEstado.Text = "Huella: No registrada";
        lblHuellaEstado.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        lblHuellaEstado.ForeColor = Color.Gray;
        lblHuellaEstado.Location = new Point(leftX + 140, startY + 5);
        lblHuellaEstado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblHuellaEstado.AutoSize = true;

        startY += 55;

        lblError.Text = "";
        lblError.Font = new Font("Segoe UI", 9);
        lblError.ForeColor = Color.Red;
        lblError.Location = new Point(leftX, startY);
        lblError.Size = new Size(340, 20);
        lblError.TextAlign = ContentAlignment.MiddleCenter;

        startY += 30;

        btnGuardar.Text = "Guardar";
        btnGuardar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnGuardar.ForeColor = Color.White;
        btnGuardar.FlatStyle = FlatStyle.Flat;
        btnGuardar.FlatAppearance.BorderSize = 0;
        btnGuardar.Size = new Size(160, 40);
        btnGuardar.Location = new Point(leftX, startY);
        btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnGuardar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnGuardar.Cursor = Cursors.Hand;
        btnGuardar.Click += BtnGuardar_Click;

        btnCancelar.Text = "Cancelar";
        btnCancelar.BackColor = Color.LightGray;
        btnCancelar.ForeColor = Color.Black;
        btnCancelar.FlatStyle = FlatStyle.Flat;
        btnCancelar.FlatAppearance.BorderSize = 0;
        btnCancelar.Size = new Size(160, 40);
        btnCancelar.Location = new Point(leftX + 180, startY);
        btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancelar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCancelar.Cursor = Cursors.Hand;
        btnCancelar.Click += (s, e) => this.Close();
        this.Controls.Add(lblDni);
        this.Controls.Add(txtDni);
        this.Controls.Add(lblNombres);
        this.Controls.Add(txtNombres);
        this.Controls.Add(lblTelefono);
        this.Controls.Add(txtTelefono);
        this.Controls.Add(lblCorreo);
        this.Controls.Add(txtCorreo);
        this.Controls.Add(btnCapturarHuella);
        this.Controls.Add(lblHuellaEstado);
        this.Controls.Add(lblError);
        this.Controls.Add(btnGuardar);
        this.Controls.Add(btnCancelar);
    }

    private async void RegistroExternoForm_Load(object? sender, EventArgs e)
    {
        txtDni.Text = _dniEscaneado;
        
        btnGuardar.Enabled = false;
        txtNombres.Text = "Consultando RENIEC...";
        txtNombres.ForeColor = Color.Gray;

        var persona = await _reniecClient.ConsultarDniAsync(_dniEscaneado);
        
        if (persona != null && !string.IsNullOrWhiteSpace(persona.Nombres))
        {
            txtNombres.Text = persona.Nombres;
            txtNombres.ForeColor = Color.Black;
        }
        else
        {
            txtNombres.Text = "";
            txtNombres.ForeColor = Color.Black;
        }

        btnGuardar.Enabled = true;
    }

    private void BtnCapturarHuella_Click(object? sender, EventArgs e)
    {
        using var enrolamientoForm = new EnrolamientoForm(_personaRepository);
        enrolamientoForm.InicializarParaCapturaLibre();
        if (enrolamientoForm.ShowDialog() == DialogResult.OK)
        {
            _huellaSimulada = enrolamientoForm.TemplateCapturado;
            lblHuellaEstado.Text = "Huella: Capturada";
            lblHuellaEstado.ForeColor = ColorTranslator.FromHtml("#005C3A"); // Verde
        }
        else
        {
            lblHuellaEstado.Text = "Captura cancelada";
            lblHuellaEstado.ForeColor = Color.Red;
        }
    }

    private async void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombres.Text))
        {
            lblError.Text = "El nombre es obligatorio.";
            return;
        }

        btnGuardar.Enabled = false;
        lblError.Text = "Guardando...";
        lblError.ForeColor = Color.Gray;

        try
        {
            var persona = new Persona
            {
                Dni = txtDni.Text.Trim(),
                Nombres = txtNombres.Text.Trim(),
                Telefono = txtTelefono.Text.Trim(),
                Correo = txtCorreo.Text.Trim(),
                FechaRegistro = DateTime.Now,
                Estado = 1, // Activo
                Huella = _huellaSimulada
            };

            NuevoIdPersona = await _personaRepository.CrearRetornandoIdAsync(persona);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Error: " + ex.Message;
            lblError.ForeColor = Color.Red;
            btnGuardar.Enabled = true;
        }
    }
}
