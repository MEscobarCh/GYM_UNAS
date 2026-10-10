using System;
using System.Drawing;
using System.Windows.Forms;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using Gym.Core.Interfaces;
using System.Threading.Tasks;

namespace Gym.UI;

public class EnrolamientoForm : Form, DPFP.Capture.EventHandler
{
    private readonly IPersonaRepository _personaRepository;
    private int _idPersona = 0;

    private Label lblDNI = new();
    private TextBox txtDNI = new();
    private Button btnBuscar = new();
    private Label lblNombre = new();

    private PictureBox picHuella = new();
    private Panel[] pnlPasos = new Panel[4];
    private Label lblEstado = new();
    
    private Button btnGuardarHuella = new();
    private Button btnCancelar = new();

    private Capture? _capturer;
    private Enrollment? _enroller;
    private string? _templateCapturado;
    public string? TemplateCapturado => _templateCapturado;
    private bool _soloCaptura = false;

    // Parameterless constructor for VS Designer
    public EnrolamientoForm()
    {
        _personaRepository = null!;
    }

    public EnrolamientoForm(IPersonaRepository personaRepository)
    {
        _personaRepository = personaRepository;
        ConfigurarUI();
        this.FormClosing += EnrolamientoForm_FormClosing;
    }

    public void Inicializar(int idPersona, string dni, string nombres)
    {
        _idPersona = idPersona;
        txtDNI.Text = dni;
        lblNombre.Text = nombres;
        lblNombre.ForeColor = ColorTranslator.FromHtml("#005C3A");
        txtDNI.Enabled = false;
        btnBuscar.Enabled = false;
        IniciarLectorLocal();
    }

    public void InicializarParaCapturaLibre()
    {
        _soloCaptura = true;
        lblDNI.Visible = false;
        txtDNI.Visible = false;
        btnBuscar.Visible = false;
        lblNombre.Visible = false;

        this.Size = new Size(400, 380);
        picHuella.Location = new Point((this.Width - 150) / 2, 60);
        
        int startX = (this.Width - 150) / 2;
        for (int i = 0; i < 4; i++) pnlPasos[i].Location = new Point(startX + (i * 40), 250);
        
        lblEstado.Location = new Point(0, 270);
        btnGuardarHuella.Location = new Point(60, 320);
        btnCancelar.Location = new Point(210, 320);

        IniciarLectorLocal();
    }

    private void ConfigurarUI()
    {
        this.Text = "Enrolamiento de Huella";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Size = new Size(400, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;

        lblDNI.Text = "DNI / Código:";
        lblDNI.Location = new Point(30, 60);
        lblDNI.AutoSize = true;
        lblDNI.Font = new Font("Segoe UI", 10);

        txtDNI.Location = new Point(30, 85);
        txtDNI.Size = new Size(200, 30);
        txtDNI.Font = new Font("Segoe UI", 12);
        txtDNI.BorderStyle = BorderStyle.FixedSingle;
        txtDNI.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Enter) { e.Handled = true; btnBuscar.PerformClick(); } };

        btnBuscar.Text = "Buscar";
        btnBuscar.Location = new Point(240, 84);
        btnBuscar.Size = new Size(130, 32);
        btnBuscar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnBuscar.ForeColor = Color.White;
        btnBuscar.FlatStyle = FlatStyle.Flat;
        btnBuscar.FlatAppearance.BorderSize = 0;
        btnBuscar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnBuscar.Cursor = Cursors.Hand;
        btnBuscar.Click += BtnBuscar_Click;

        lblNombre.Text = "Ingrese DNI para buscar";
        lblNombre.Location = new Point(30, 125);
        lblNombre.Size = new Size(340, 20);
        lblNombre.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        lblNombre.ForeColor = Color.Gray;

        picHuella.Size = new Size(150, 180);
        picHuella.Location = new Point((this.Width - 150) / 2, 160);
        picHuella.BorderStyle = BorderStyle.FixedSingle;
        picHuella.BackColor = Color.WhiteSmoke;
        picHuella.SizeMode = PictureBoxSizeMode.StretchImage;

        int startX = (this.Width - 150) / 2;
        for (int i = 0; i < 4; i++)
        {
            pnlPasos[i] = new Panel();
            pnlPasos[i].Size = new Size(30, 8);
            pnlPasos[i].Location = new Point(startX + (i * 40), 350);
            pnlPasos[i].BackColor = Color.LightGray;
            this.Controls.Add(pnlPasos[i]);
        }

        lblEstado.Text = "Esperando búsqueda...";
        lblEstado.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblEstado.ForeColor = Color.Gray;
        lblEstado.Location = new Point(0, 370);
        lblEstado.Size = new Size(this.Width, 30);
        lblEstado.TextAlign = ContentAlignment.MiddleCenter;

        btnGuardarHuella.Text = "Guardar";
        btnGuardarHuella.Size = new Size(130, 40);
        btnGuardarHuella.Location = new Point(60, 420);
        btnGuardarHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnGuardarHuella.ForeColor = Color.White;
        btnGuardarHuella.FlatStyle = FlatStyle.Flat;
        btnGuardarHuella.FlatAppearance.BorderSize = 0;
        btnGuardarHuella.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnGuardarHuella.Cursor = Cursors.Hand;
        btnGuardarHuella.Enabled = false;
        btnGuardarHuella.Click += BtnGuardar_Click;

        btnCancelar.Text = "Cancelar";
        btnCancelar.Size = new Size(130, 40);
        btnCancelar.Location = new Point(210, 420);
        btnCancelar.BackColor = Color.LightGray;
        btnCancelar.ForeColor = Color.Black;
        btnCancelar.FlatStyle = FlatStyle.Flat;
        btnCancelar.FlatAppearance.BorderSize = 0;
        btnCancelar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCancelar.Cursor = Cursors.Hand;
        btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
        this.Controls.Add(lblDNI);
        this.Controls.Add(txtDNI);
        this.Controls.Add(btnBuscar);
        this.Controls.Add(lblNombre);
        this.Controls.Add(picHuella);
        this.Controls.Add(lblEstado);
        this.Controls.Add(btnGuardarHuella);
        this.Controls.Add(btnCancelar);
    }

    private async void BtnBuscar_Click(object? sender, EventArgs e)
    {
        string dni = txtDNI.Text.Trim();
        if (string.IsNullOrEmpty(dni)) return;

        btnBuscar.Enabled = false;
        try
        {
            var persona = await _personaRepository.ObtenerPorDniAsync(dni);
            if (persona != null)
            {
                _idPersona = persona.Id;
                lblNombre.Text = persona.Nombres;
                lblNombre.ForeColor = ColorTranslator.FromHtml("#005C3A");
                
                IniciarLectorLocal();
            }
            else
            {
                _idPersona = 0;
                lblNombre.Text = "Persona no encontrada.";
                lblNombre.ForeColor = Color.Red;
                DetenerLectorLocal();
                ActualizarPasos(4);
                ActualizarEstado("Esperando búsqueda...", Color.Gray);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al buscar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnBuscar.Enabled = true;
        }
    }

    private void IniciarLectorLocal()
    {
        try
        {
            DetenerLectorLocal();

            _capturer = new Capture();
            _enroller = new Enrollment();

            if (_capturer != null)
            {
                _capturer.EventHandler = this;
                _capturer.StartCapture();
                ActualizarPasos(4);
                ActualizarEstado("Lector listo. Toque el sensor 4 veces.", Color.DarkOrange);
            }
        }
        catch (Exception ex)
        {
            ActualizarEstado("Error al iniciar lector: " + ex.Message, Color.Red);
        }
    }

    private void DetenerLectorLocal()
    {
        if (_capturer != null)
        {
            try
            {
                _capturer.StopCapture();
                _capturer.Dispose();
            }
            catch { }
            _capturer = null;
        }
    }

    private void EnrolamientoForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        DetenerLectorLocal();
    }

    private void ActualizarEstado(string mensaje, Color color)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(() => ActualizarEstado(mensaje, color)));
            return;
        }
        lblEstado.Text = mensaje;
        lblEstado.ForeColor = color;
    }

    private void ActualizarPasos(int toquesRestantes)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(() => ActualizarPasos(toquesRestantes)));
            return;
        }

        int toquesCompletados = 4 - toquesRestantes;
        for (int i = 0; i < 4; i++)
        {
            if (i < toquesCompletados)
                pnlPasos[i].BackColor = ColorTranslator.FromHtml("#005C3A");
            else
                pnlPasos[i].BackColor = Color.LightGray;
        }
    }

    public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
    {
        ProcesarMuestra(Sample);
    }

    public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
    public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
    public void OnReaderConnect(object Capture, string ReaderSerialNumber)
    {
        ActualizarEstado("Lector conectado.", Color.Green);
    }
    public void OnReaderDisconnect(object Capture, string ReaderSerialNumber)
    {
        ActualizarEstado("Lector desconectado.", Color.Red);
    }
    public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback) { }

    private void ProcesarMuestra(Sample sample)
    {
        if (_idPersona <= 0 && !_soloCaptura) return; // Solo procesar si hay una persona seleccionada o es libre

        DibujarHuella(sample);

        try
        {
            FeatureExtraction extractor = new FeatureExtraction();
            CaptureFeedback feedback = CaptureFeedback.None;
            FeatureSet features = new FeatureSet();
            
            extractor.CreateFeatureSet(sample, DataPurpose.Enrollment, ref feedback, ref features);

            if (feedback == CaptureFeedback.Good && _enroller != null)
            {
                _enroller.AddFeatures(features);
                int restantes = (int)_enroller.FeaturesNeeded;
                ActualizarPasos(restantes);

                if (_enroller.TemplateStatus == Enrollment.Status.Ready)
                {
                    _templateCapturado = Convert.ToBase64String(_enroller.Template.Bytes);
                    
                    ActualizarEstado("Huella capturada exitosamente.", ColorTranslator.FromHtml("#005C3A"));
                    
                    this.Invoke(new Action(() => {
                        btnGuardarHuella.Enabled = true;
                        try { _capturer?.StopCapture(); } catch { }
                    }));
                }
                else if (_enroller.TemplateStatus == Enrollment.Status.Failed)
                {
                    _enroller.Clear();
                    ActualizarPasos(4);
                    ActualizarEstado("Enrolamiento fallido. Intente de nuevo.", Color.Red);
                    try { _capturer?.StartCapture(); } catch { }
                }
                else
                {
                    ActualizarEstado($"Faltan {restantes} toque(s) más...", Color.DarkOrange);
                }
            }
            else
            {
                ActualizarEstado("Mala calidad de huella. Intente de nuevo.", Color.Orange);
            }
        }
        catch (Exception ex)
        {
            ActualizarEstado("Error: " + ex.Message, Color.Red);
        }
    }

    private void DibujarHuella(Sample sample)
    {
        SampleConversion convertor = new SampleConversion();
        Bitmap? bitmap = null;
        convertor.ConvertToPicture(sample, ref bitmap);

        if (bitmap != null)
        {
            this.Invoke(new Action(() =>
            {
                if (picHuella.Image != null) picHuella.Image.Dispose();
                picHuella.Image = new Bitmap(bitmap, picHuella.Size);
            }));
        }
    }

    private async void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (_soloCaptura && !string.IsNullOrEmpty(_templateCapturado))
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
            return;
        }

        if (_idPersona > 0 && !string.IsNullOrEmpty(_templateCapturado))
        {
            btnGuardarHuella.Enabled = false;
            try
            {
                await _personaRepository.ActualizarHuellaAsync(_idPersona, _templateCapturado);
                MessageBox.Show("Huella registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar en BD: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnGuardarHuella.Enabled = true;
            }
        }
    }
}
