using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Services;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using System.Threading.Tasks;

namespace Gym.UI;

public class PagoForm : Form
{
    private readonly TarifaService _tarifaService;
    private readonly PagoService _pagoService;
    private readonly IPersonaRepository _personaRepository;
    private int _idPersona;
    private readonly int _idUsuario;
    private decimal _montoActual = 0;

    // Controles UI

    private Label lblDni = new();
    private TextBox txtDni = new();
    private Button btnBuscar = new();
    private Label lblNombres = new();

    private Label lblTipoPase = new();
    private ComboBox cbTipoPase = new();
    
    private Label lblMetodo = new();
    private ComboBox cbMetodoPago = new();

    private Label lblNroOperacion = new();
    private TextBox txtNroOperacion = new();

    private Label lblMonto = new();
    private Label lblError = new();

    private Button btnProcesar = new();
    private Button btnCancelar = new();

    // Constructor sin parmetros para el designer
    public PagoForm()
    {
        _tarifaService = null!;
        _pagoService = null!;
        _personaRepository = null!;
    }

    public PagoForm(TarifaService tarifaService, PagoService pagoService, IPersonaRepository personaRepository, int idPersona, int idUsuario)
    {
        _tarifaService = tarifaService;
        _pagoService = pagoService;
        _personaRepository = personaRepository;
        _idPersona = idPersona;
        _idUsuario = idUsuario;
        
        ConfigurarUI();
        this.Load += PagoForm_Load;
    }

    private void ConfigurarUI()
    {
        this.Text = "Registrar Pago - Externo";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ClientSize = new Size(400, 470);
        this.MinimumSize = new Size(420, 510);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;

        // Top Panel

        // Controles Internos
        int startY = 25;
        int spacing = 65;
        int leftX = 30;

        lblDni.Text = "DNI del Alumno / Externo:";
        lblDni.Font = new Font("Segoe UI", 10);
        lblDni.Location = new Point(leftX, startY);
        lblDni.AutoSize = true;
        this.Controls.Add(lblDni);

        txtDni.Location = new Point(leftX, startY + 27);
        txtDni.Size = new Size(200, 30);
        txtDni.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtDni.Font = new Font("Segoe UI", 11);
        this.Controls.Add(txtDni);

        btnBuscar.Text = "Buscar";
        btnBuscar.Location = new Point(leftX + 210, startY + 26);
        btnBuscar.Size = new Size(130, 30);
        btnBuscar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnBuscar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnBuscar.ForeColor = Color.White;
        btnBuscar.FlatStyle = FlatStyle.Flat;
        btnBuscar.FlatAppearance.BorderSize = 0;
        btnBuscar.Cursor = Cursors.Hand;
        btnBuscar.Click += BtnBuscar_Click;
        this.Controls.Add(btnBuscar);

        lblNombres.Text = "---";
        lblNombres.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        lblNombres.ForeColor = ColorTranslator.FromHtml("#005C3A");
        lblNombres.Location = new Point(leftX, startY + 62);
        lblNombres.Size = new Size(340, 20);
        this.Controls.Add(lblNombres);

        startY += 95;

        lblTipoPase.Text = "Tipo de Pase:";
        lblTipoPase.Font = new Font("Segoe UI", 10);
        lblTipoPase.Location = new Point(leftX, startY);
        lblTipoPase.AutoSize = true;

        cbTipoPase.Location = new Point(leftX, startY + 27);
        cbTipoPase.Size = new Size(340, 30);
        cbTipoPase.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cbTipoPase.Font = new Font("Segoe UI", 11);
        cbTipoPase.DropDownStyle = ComboBoxStyle.DropDownList;
        cbTipoPase.Items.Add("Diario");
        cbTipoPase.Items.Add("Mensual");
        cbTipoPase.SelectedIndexChanged += CbTipoPase_SelectedIndexChanged;

        startY += spacing;

        lblMetodo.Text = "Método de Pago:";
        lblMetodo.Font = new Font("Segoe UI", 10);
        lblMetodo.Location = new Point(leftX, startY);
        lblMetodo.AutoSize = true;

        cbMetodoPago.Location = new Point(leftX, startY + 27);
        cbMetodoPago.Size = new Size(340, 30);
        cbMetodoPago.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cbMetodoPago.Font = new Font("Segoe UI", 11);
        cbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;
        cbMetodoPago.Items.Add("EFECTIVO");
        cbMetodoPago.Items.Add("YAPE");
        cbMetodoPago.SelectedIndexChanged += CbMetodoPago_SelectedIndexChanged;

        startY += spacing;

        lblNroOperacion.Text = "Nro. Operación (Solo YAPE):";
        lblNroOperacion.Font = new Font("Segoe UI", 10);
        lblNroOperacion.Location = new Point(leftX, startY);
        lblNroOperacion.AutoSize = true;

        txtNroOperacion.Location = new Point(leftX, startY + 27);
        txtNroOperacion.Size = new Size(340, 30);
        txtNroOperacion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtNroOperacion.Font = new Font("Segoe UI", 11);
        txtNroOperacion.Enabled = false; 

        startY += spacing;

        lblMonto.Text = "Monto a Pagar: S/ 0.00";
        lblMonto.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblMonto.ForeColor = ColorTranslator.FromHtml("#005C3A");
        lblMonto.Location = new Point(leftX, startY);
        lblMonto.Size = new Size(340, 30);
        lblMonto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblMonto.TextAlign = ContentAlignment.MiddleCenter;

        startY += 40;

        lblError.Text = "";
        lblError.Font = new Font("Segoe UI", 9);
        lblError.ForeColor = Color.Red;
        lblError.Location = new Point(leftX, startY);
        lblError.Size = new Size(340, 40);
        lblError.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblError.TextAlign = ContentAlignment.MiddleCenter;

        startY += 50;

        btnProcesar.Text = "Procesar Cobro";
        btnProcesar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnProcesar.ForeColor = Color.White;
        btnProcesar.FlatStyle = FlatStyle.Flat;
        btnProcesar.FlatAppearance.BorderSize = 0;
        btnProcesar.Size = new Size(160, 40);
        btnProcesar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        btnProcesar.Location = new Point(leftX, startY);
        btnProcesar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnProcesar.Cursor = Cursors.Hand;
        btnProcesar.Click += BtnProcesar_Click;

        btnCancelar.Text = "Cancelar";
        btnCancelar.BackColor = Color.LightGray;
        btnCancelar.ForeColor = Color.Black;
        btnCancelar.FlatStyle = FlatStyle.Flat;
        btnCancelar.FlatAppearance.BorderSize = 0;
        btnCancelar.Size = new Size(160, 40);
        btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancelar.Location = new Point(leftX + 180, startY);
        btnCancelar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCancelar.Cursor = Cursors.Hand;
        btnCancelar.Click += (s, e) => this.Close();
        this.Controls.Add(lblTipoPase);
        this.Controls.Add(cbTipoPase);
        this.Controls.Add(lblMetodo);
        this.Controls.Add(cbMetodoPago);
        this.Controls.Add(lblNroOperacion);
        this.Controls.Add(txtNroOperacion);
        this.Controls.Add(lblMonto);
        this.Controls.Add(lblError);
        this.Controls.Add(btnProcesar);
        this.Controls.Add(btnCancelar);
    }

    private async void PagoForm_Load(object? sender, EventArgs e)
    {
        cbMetodoPago.SelectedIndex = 0; // EFECTIVO
        cbTipoPase.SelectedIndex = 0; // Diario, disparar evento y cargar tarifa

        if (_idPersona > 0)
        {
            txtDni.Enabled = false;
            btnBuscar.Enabled = false;
            try
            {
                var persona = await _personaRepository.ObtenerPorIdAsync(_idPersona);
                if (persona != null)
                {
                    txtDni.Text = persona.Dni;
                    lblNombres.Text = persona.Nombres;
                }
            }
            catch { }
        }
    }

    private async void BtnBuscar_Click(object? sender, EventArgs e)
    {
        string dni = txtDni.Text.Trim();
        if (string.IsNullOrEmpty(dni)) return;

        btnBuscar.Enabled = false;
        try
        {
            var persona = await _personaRepository.ObtenerPorDniAsync(dni);
            if (persona != null)
            {
                _idPersona = persona.Id;
                lblNombres.Text = persona.Nombres;
                lblError.Text = "";
            }
            else
            {
                _idPersona = 0;
                lblNombres.Text = "No encontrado.";
                lblError.Text = "Persona no registrada. Registre al alumno primero.";
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error al buscar: " + ex.Message;
        }
        finally
        {
            btnBuscar.Enabled = true;
        }
    }

    private void CbMetodoPago_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (cbMetodoPago.SelectedItem?.ToString() == "YAPE")
        {
            txtNroOperacion.Enabled = true;
        }
        else
        {
            txtNroOperacion.Enabled = false;
            txtNroOperacion.Clear();
        }
    }

    private async void CbTipoPase_SelectedIndexChanged(object? sender, EventArgs e)
    {
        string tipoSeleccionado = cbTipoPase.SelectedItem?.ToString() ?? "Diario";
        
        try
        {
            var tarifa = await _tarifaService.ObtenerTarifaVigenteAsync(tipoSeleccionado);
            if (tarifa != null)
            {
                _montoActual = tarifa.Monto;
                lblMonto.Text = $"Monto a Pagar: S/ {_montoActual:0.00}";
                lblError.Text = "";
            }
            else
            {
                _montoActual = 0;
                lblMonto.Text = "Monto a Pagar: S/ 0.00";
                lblError.Text = "No se encontró tarifa vigente.";
            }
        }
        catch (Exception ex)
        {
            lblError.Text = "Error cargando tarifa: " + ex.Message;
        }
    }

    private async void BtnProcesar_Click(object? sender, EventArgs e)
    {
        if (_idPersona <= 0)
        {
            lblError.Text = "Busque un alumno o externo válido.";
            return;
        }

        btnProcesar.Enabled = false;
        lblError.Text = "Procesando...";
        lblError.ForeColor = Color.Gray;

        try
        {
            string metodo = cbMetodoPago.SelectedItem?.ToString() ?? "EFECTIVO";
            string tipoPase = cbTipoPase.SelectedItem?.ToString() ?? "Diario";
            string nroOperacion = txtNroOperacion.Text.Trim();

            await _pagoService.ProcesarPagoYSuscripcionAsync(
                idPersona: _idPersona,
                monto: _montoActual,
                metodo: metodo,
                nroOperacion: string.IsNullOrEmpty(nroOperacion) ? null : nroOperacion,
                idUsuario: _idUsuario,
                tipoSuscripcion: tipoPase
            );

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            lblError.Text = "Error: " + ex.Message;
            lblError.ForeColor = Color.Red;
            btnProcesar.Enabled = true;
        }
    }
}
