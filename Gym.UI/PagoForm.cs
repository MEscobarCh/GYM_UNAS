using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Services;
using Gym.Core.Entities;
using System.Threading.Tasks;

namespace Gym.UI;

public class PagoForm : Form
{
    private readonly TarifaService _tarifaService;
    private readonly PagoService _pagoService;
    private readonly int _idPersona;
    private readonly int _idUsuario;
    private decimal _montoActual = 0;

    // Controles UI
    private Panel panelTop = new();
    private Label lblTitulo = new();
    private Button btnCerrar = new();

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
    }

    public PagoForm(TarifaService tarifaService, PagoService pagoService, int idPersona, int idUsuario)
    {
        _tarifaService = tarifaService;
        _pagoService = pagoService;
        _idPersona = idPersona;
        _idUsuario = idUsuario;
        
        ConfigurarUI();
        this.Load += PagoForm_Load;
    }

    private void ConfigurarUI()
    {
        this.Text = "Registrar Pago - Externo";
        this.Size = new Size(400, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.None;

        // Top Panel
        panelTop.BackColor = ColorTranslator.FromHtml("#005C3A");
        panelTop.Dock = DockStyle.Top;
        panelTop.Height = 40;

        lblTitulo.Text = "Registrar Pago - Externo";
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
        btnCerrar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCerrar.Cursor = Cursors.Hand;
        btnCerrar.Click += (s, e) => this.Close();

        panelTop.Controls.Add(lblTitulo);
        panelTop.Controls.Add(btnCerrar);

        // Controles Internos
        int startY = 60;
        int spacing = 60;
        int leftX = 30;

        lblTipoPase.Text = "Tipo de Pase:";
        lblTipoPase.Font = new Font("Segoe UI", 10);
        lblTipoPase.Location = new Point(leftX, startY);
        lblTipoPase.AutoSize = true;

        cbTipoPase.Location = new Point(leftX, startY + 20);
        cbTipoPase.Size = new Size(340, 30);
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

        cbMetodoPago.Location = new Point(leftX, startY + 20);
        cbMetodoPago.Size = new Size(340, 30);
        cbMetodoPago.Font = new Font("Segoe UI", 11);
        cbMetodoPago.DropDownStyle = ComboBoxStyle.DropDownList;
        cbMetodoPago.Items.Add("EFECTIVO");
        cbMetodoPago.Items.Add("YAPE");
        cbMetodoPago.SelectedIndexChanged += CbMetodoPago_SelectedIndexChanged;

        startY += spacing;

        lblNroOperacion.Text = "Nro. Operación:";
        lblNroOperacion.Font = new Font("Segoe UI", 10);
        lblNroOperacion.Location = new Point(leftX, startY);
        lblNroOperacion.AutoSize = true;

        txtNroOperacion.Location = new Point(leftX, startY + 20);
        txtNroOperacion.Size = new Size(340, 30);
        txtNroOperacion.Font = new Font("Segoe UI", 11);
        txtNroOperacion.Enabled = false; // Deshabilitado por defecto asumiendo EFECTIVO

        startY += spacing;

        lblMonto.Text = "Monto a Pagar: S/ 0.00";
        lblMonto.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblMonto.ForeColor = ColorTranslator.FromHtml("#005C3A");
        lblMonto.Location = new Point(leftX, startY);
        lblMonto.Size = new Size(340, 30);
        lblMonto.TextAlign = ContentAlignment.MiddleCenter;

        startY += 40;

        lblError.Text = "";
        lblError.Font = new Font("Segoe UI", 9);
        lblError.ForeColor = Color.Red;
        lblError.Location = new Point(leftX, startY);
        lblError.Size = new Size(340, 40);
        lblError.TextAlign = ContentAlignment.MiddleCenter;

        startY += 50;

        btnProcesar.Text = "Procesar Cobro";
        btnProcesar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnProcesar.ForeColor = Color.White;
        btnProcesar.FlatStyle = FlatStyle.Flat;
        btnProcesar.FlatAppearance.BorderSize = 0;
        btnProcesar.Size = new Size(160, 40);
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
        btnCancelar.Location = new Point(leftX + 180, startY);
        btnCancelar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCancelar.Cursor = Cursors.Hand;
        btnCancelar.Click += (s, e) => this.Close();

        this.Controls.Add(panelTop);
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

    private void PagoForm_Load(object? sender, EventArgs e)
    {
        cbMetodoPago.SelectedIndex = 0; // EFECTIVO
        cbTipoPase.SelectedIndex = 0; // Diario, disparar evento y cargar tarifa
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
