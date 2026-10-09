using System;
using System.Drawing;
using System.Windows.Forms;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;

namespace Gym.UI;

public class CapturaHuellaForm : Form, DPFP.Capture.EventHandler
{
    public string? TemplateCapturado { get; private set; }

    private Panel panelTop = new();
    private Label lblTitulo = new();
    private Button btnCerrar = new();

    private PictureBox picHuella = new();
    private Label lblEstado = new();
    
    private Button btnGuardar = new();
    private Button btnCancelar = new();

    private Capture? _capturer;
    private Enrollment? _enroller;

    public CapturaHuellaForm()
    {
        ConfigurarUI();
        this.Load += CapturaHuellaForm_Load;
        this.FormClosing += CapturaHuellaForm_FormClosing;
    }

    private void ConfigurarUI()
    {
        this.Text = "Captura de Huella";
        this.Size = new Size(350, 400);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.None;

        panelTop.BackColor = ColorTranslator.FromHtml("#005C3A");
        panelTop.Dock = DockStyle.Top;
        panelTop.Height = 40;

        lblTitulo.Text = "Captura de Huella Biométrica";
        lblTitulo.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblTitulo.ForeColor = Color.White;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(10, 10);
        
        btnCerrar.Text = "X";
        btnCerrar.Size = new Size(40, 40);
        btnCerrar.Location = new Point(this.Width - 40, 0);
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.ForeColor = Color.White;
        btnCerrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCerrar.Cursor = Cursors.Hand;
        btnCerrar.Click += (s, e) => this.Close();

        panelTop.Controls.Add(lblTitulo);
        panelTop.Controls.Add(btnCerrar);

        picHuella.Size = new Size(150, 180);
        picHuella.Location = new Point((this.Width - 150) / 2, 60);
        picHuella.BorderStyle = BorderStyle.FixedSingle;
        picHuella.BackColor = Color.WhiteSmoke;
        picHuella.SizeMode = PictureBoxSizeMode.StretchImage;

        lblEstado.Text = "Iniciando lector...";
        lblEstado.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblEstado.ForeColor = Color.Orange;
        lblEstado.Location = new Point(0, 260);
        lblEstado.Size = new Size(this.Width, 30);
        lblEstado.TextAlign = ContentAlignment.MiddleCenter;

        btnGuardar.Text = "Guardar";
        btnGuardar.Size = new Size(120, 40);
        btnGuardar.Location = new Point(40, 320);
        btnGuardar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnGuardar.ForeColor = Color.White;
        btnGuardar.FlatStyle = FlatStyle.Flat;
        btnGuardar.FlatAppearance.BorderSize = 0;
        btnGuardar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnGuardar.Cursor = Cursors.Hand;
        btnGuardar.Enabled = false;
        btnGuardar.Click += BtnGuardar_Click;

        btnCancelar.Text = "Cancelar";
        btnCancelar.Size = new Size(120, 40);
        btnCancelar.Location = new Point(190, 320);
        btnCancelar.BackColor = Color.LightGray;
        btnCancelar.ForeColor = Color.Black;
        btnCancelar.FlatStyle = FlatStyle.Flat;
        btnCancelar.FlatAppearance.BorderSize = 0;
        btnCancelar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCancelar.Cursor = Cursors.Hand;
        btnCancelar.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

        this.Controls.Add(panelTop);
        this.Controls.Add(picHuella);
        this.Controls.Add(lblEstado);
        this.Controls.Add(btnGuardar);
        this.Controls.Add(btnCancelar);
    }

    private void CapturaHuellaForm_Load(object? sender, EventArgs e)
    {
        try
        {
            _capturer = new Capture();
            _enroller = new Enrollment();

            if (_capturer != null)
            {
                _capturer.EventHandler = this;
                _capturer.StartCapture();
                ActualizarEstado("Lector listo. Toque el sensor.", Color.Orange);
            }
        }
        catch (Exception ex)
        {
            ActualizarEstado("Error al iniciar lector: " + ex.Message, Color.Red);
        }
    }

    private void CapturaHuellaForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_capturer != null)
        {
            try
            {
                _capturer.StopCapture();
                _capturer.Dispose();
            }
            catch { }
        }
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

                if (_enroller.TemplateStatus == Enrollment.Status.Ready)
                {
                    TemplateCapturado = Convert.ToBase64String(_enroller.Template.Bytes);
                    
                    ActualizarEstado("Huella Capturada exitosamente", ColorTranslator.FromHtml("#005C3A"));
                    
                    this.Invoke(new Action(() => {
                        btnGuardar.Enabled = true;
                        try { _capturer?.StopCapture(); } catch { }
                    }));
                }
                else if (_enroller.TemplateStatus == Enrollment.Status.Failed)
                {
                    _enroller.Clear();
                    ActualizarEstado("Enrolamiento fallido. Intente de nuevo.", Color.Red);
                    try { _capturer?.StartCapture(); } catch { }
                }
                else
                {
                    int restantes = (int)_enroller.FeaturesNeeded;
                    ActualizarEstado($"Faltan {restantes} toques más...", Color.Orange);
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

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(TemplateCapturado))
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
