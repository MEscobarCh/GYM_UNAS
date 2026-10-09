using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Interfaces;

namespace Gym.UI;

public class EnrolamientoForm : Form
{
    private readonly IBiometricoClient _biometricoClient;
    private readonly IPersonaRepository _personaRepository;
    private int _idPersona;

    // UI Controls
    private Panel panelTop = new();
    private Label lblTitulo = new();
    private Button btnCerrar = new();
    private Label lblEstado = new();
    private Button btnActivarSensor = new();
    private Button btnGuardarHuella = new();

    private string? _huellaBase64Temp;

    // Constructor para el Designer de VS
    public EnrolamientoForm()
    {
        _biometricoClient = null!;
        _personaRepository = null!;
    }

    public EnrolamientoForm(IBiometricoClient biometricoClient, IPersonaRepository personaRepository)
    {
        _biometricoClient = biometricoClient;
        _personaRepository = personaRepository;
        ConfigurarUI();
    }

    public void Inicializar(int idPersona)
    {
        _idPersona = idPersona;
        this.FormClosed += EnrolamientoForm_FormClosed;
    }

    private void ConfigurarUI()
    {
        this.Text = "Enrolamiento de Huella";
        this.Size = new Size(500, 300);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.None;

        // Top Panel
        panelTop.BackColor = ColorTranslator.FromHtml("#005C3A"); // Verde institucional
        panelTop.Dock = DockStyle.Top;
        panelTop.Height = 40;

        lblTitulo.Text = "Registrar Huella Dactilar";
        lblTitulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        lblTitulo.ForeColor = Color.White;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(10, 10);

        btnCerrar.Text = "X";
        btnCerrar.Size = new Size(40, 40);
        btnCerrar.Location = new Point(this.Width - 40, 0);
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.ForeColor = Color.White;
        btnCerrar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        btnCerrar.Cursor = Cursors.Hand;
        btnCerrar.Click += (s, e) => this.Close();

        panelTop.Controls.Add(lblTitulo);
        panelTop.Controls.Add(btnCerrar);
        this.Controls.Add(panelTop);

        // Estado
        lblEstado.Text = "Presione 'Activar Sensor' para iniciar.";
        lblEstado.Font = new Font("Segoe UI", 11);
        lblEstado.Location = new Point(20, 100);
        lblEstado.Size = new Size(460, 40);
        lblEstado.TextAlign = ContentAlignment.MiddleCenter;
        this.Controls.Add(lblEstado);

        // Botones
        btnActivarSensor.Text = "Activar Sensor";
        btnActivarSensor.Size = new Size(150, 40);
        btnActivarSensor.Location = new Point(80, 200);
        btnActivarSensor.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnActivarSensor.ForeColor = Color.White;
        btnActivarSensor.FlatStyle = FlatStyle.Flat;
        btnActivarSensor.Cursor = Cursors.Hand;
        btnActivarSensor.Click += BtnActivarSensor_Click;
        this.Controls.Add(btnActivarSensor);

        btnGuardarHuella.Text = "Guardar Huella";
        btnGuardarHuella.Size = new Size(150, 40);
        btnGuardarHuella.Location = new Point(270, 200);
        btnGuardarHuella.BackColor = Color.Gray;
        btnGuardarHuella.ForeColor = Color.White;
        btnGuardarHuella.FlatStyle = FlatStyle.Flat;
        btnGuardarHuella.Enabled = false; // Deshabilitado hasta que haya captura
        btnGuardarHuella.Cursor = Cursors.Hand;
        btnGuardarHuella.Click += BtnGuardarHuella_Click;
        this.Controls.Add(btnGuardarHuella);
    }

    private async void BtnActivarSensor_Click(object? sender, EventArgs e)
    {
        try
        {
            btnActivarSensor.Enabled = false;
            lblEstado.Text = "Esperando dedo en el sensor U.are.U...";
            lblEstado.ForeColor = Color.DarkOrange;

            _biometricoClient.InicializarLector();

            _huellaBase64Temp = await _biometricoClient.IniciarCapturaAsync();

            if (!string.IsNullOrEmpty(_huellaBase64Temp))
            {
                lblEstado.Text = "Huella capturada con xito";
                lblEstado.ForeColor = Color.Green;
                
                btnGuardarHuella.Enabled = true;
                btnGuardarHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
            }
            else
            {
                lblEstado.Text = "No se pudo capturar la huella.";
                lblEstado.ForeColor = Color.Red;
                btnActivarSensor.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            lblEstado.Text = $"Error: {ex.Message}";
            lblEstado.ForeColor = Color.Red;
            btnActivarSensor.Enabled = true;
        }
    }

    private async void BtnGuardarHuella_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_huellaBase64Temp)) return;

        try
        {
            btnGuardarHuella.Enabled = false;
            await _personaRepository.ActualizarHuellaAsync(_idPersona, _huellaBase64Temp);
            
            MessageBox.Show("Huella guardada correctamente en la base de datos.", "xito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            btnGuardarHuella.Enabled = true;
        }
    }

    private void EnrolamientoForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _biometricoClient.DetenerLector();
    }
}

