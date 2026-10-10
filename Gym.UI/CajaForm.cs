using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Services;

namespace Gym.UI;

public class CajaForm : Form
{
    private readonly PagoService _pagoService;
    private readonly IServiceProvider _serviceProvider;

    // UI Controls
    private Panel panelTop = new();
    private Label lblTitulo = new();
    private Button btnCerrar = new();

    private Label lblFechaInicio = new();
    private DateTimePicker dtpFechaInicio = new();
    private Label lblFechaFin = new();
    private DateTimePicker dtpFechaFin = new();
    private Button btnConsultar = new();

    // Resumen de Caja
    private GroupBox gbResumen = new();
    private Label lblTotalEfectivo = new();
    private Label lblTotalYape = new();
    private Label lblTotalGeneral = new();
    private Label lblCantidadPagos = new();

    // Historial
    private DataGridView dgvHistorial = new();

    // Constructor para el Designer
    public CajaForm()
    {
        _pagoService = null!;
        _serviceProvider = null!;
    }

    public CajaForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _pagoService = (PagoService)serviceProvider.GetService(typeof(PagoService))!;
        ConfigurarUI();
    }

    private async void ConfigurarUI()
    {
        this.Text = "Reportes de Caja e Historial";
        this.Size = new Size(900, 650);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = ColorTranslator.FromHtml("#F0F2F5");
        this.FormBorderStyle = FormBorderStyle.None;

        // Top Panel
        panelTop.BackColor = ColorTranslator.FromHtml("#005C3A"); // Verde institucional
        panelTop.Dock = DockStyle.Top;
        panelTop.Height = 50;

        lblTitulo.Text = "CIERRE DE CAJA E HISTORIAL DE PAGOS";
        lblTitulo.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        lblTitulo.ForeColor = Color.White;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(20, 15);
        
        btnCerrar.Text = "✕";
        btnCerrar.Size = new Size(40, 40);
        btnCerrar.Location = new Point(this.Width - 40, 5);
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.ForeColor = Color.White;
        btnCerrar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        btnCerrar.Cursor = Cursors.Hand;
        btnCerrar.Click += (s, e) => this.Close();

        panelTop.Controls.Add(lblTitulo);
        panelTop.Controls.Add(btnCerrar);
        this.Controls.Add(panelTop);

        // Top Content: Resumen de Caja
        gbResumen.Text = "Resumen de Caja Hoy";
        gbResumen.Location = new Point(20, 60);
        gbResumen.Size = new Size(860, 80);
        gbResumen.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        gbResumen.BackColor = Color.White;

        lblTotalEfectivo.Text = "Total Efectivo: S/ 0.00";
        lblTotalEfectivo.Location = new Point(20, 35);
        lblTotalEfectivo.AutoSize = true;

        lblTotalYape.Text = "Total Yape: S/ 0.00";
        lblTotalYape.Location = new Point(220, 35);
        lblTotalYape.AutoSize = true;

        lblTotalGeneral.Text = "TOTAL: S/ 0.00";
        lblTotalGeneral.Location = new Point(420, 35);
        lblTotalGeneral.AutoSize = true;
        lblTotalGeneral.ForeColor = ColorTranslator.FromHtml("#005C3A");

        lblCantidadPagos.Text = "Cantidad Transacciones: 0";
        lblCantidadPagos.Location = new Point(620, 35);
        lblCantidadPagos.AutoSize = true;

        gbResumen.Controls.Add(lblTotalEfectivo);
        gbResumen.Controls.Add(lblTotalYape);
        gbResumen.Controls.Add(lblTotalGeneral);
        gbResumen.Controls.Add(lblCantidadPagos);
        this.Controls.Add(gbResumen);

        // Middle Content: Filtros
        Panel pnlFiltros = new Panel();
        pnlFiltros.Location = new Point(20, 150);
        pnlFiltros.Size = new Size(860, 50);
        pnlFiltros.BackColor = Color.White;

        lblFechaInicio.Text = "Desde:";
        lblFechaInicio.Location = new Point(20, 15);
        lblFechaInicio.AutoSize = true;
        
        dtpFechaInicio.Format = DateTimePickerFormat.Short;
        dtpFechaInicio.Location = new Point(70, 12);
        dtpFechaInicio.Size = new Size(120, 25);
        dtpFechaInicio.Value = DateTime.Today;

        lblFechaFin.Text = "Hasta:";
        lblFechaFin.Location = new Point(210, 15);
        lblFechaFin.AutoSize = true;

        dtpFechaFin.Format = DateTimePickerFormat.Short;
        dtpFechaFin.Location = new Point(260, 12);
        dtpFechaFin.Size = new Size(120, 25);
        dtpFechaFin.Value = DateTime.Today;

        btnConsultar.Text = "Consultar";
        btnConsultar.Location = new Point(400, 10);
        btnConsultar.Size = new Size(100, 30);
        btnConsultar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnConsultar.ForeColor = Color.White;
        btnConsultar.FlatStyle = FlatStyle.Flat;
        btnConsultar.FlatAppearance.BorderSize = 0;
        btnConsultar.Cursor = Cursors.Hand;
        btnConsultar.Click += BtnConsultar_Click;

        Button btnNuevoPago = new Button();
        btnNuevoPago.Text = "+ Nuevo Pago";
        btnNuevoPago.Location = new Point(720, 10);
        btnNuevoPago.Size = new Size(120, 30);
        btnNuevoPago.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnNuevoPago.ForeColor = Color.White;
        btnNuevoPago.FlatStyle = FlatStyle.Flat;
        btnNuevoPago.FlatAppearance.BorderSize = 0;
        btnNuevoPago.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        btnNuevoPago.Cursor = Cursors.Hand;
        btnNuevoPago.Click += BtnNuevoPago_Click;

        pnlFiltros.Controls.Add(lblFechaInicio);
        pnlFiltros.Controls.Add(dtpFechaInicio);
        pnlFiltros.Controls.Add(lblFechaFin);
        pnlFiltros.Controls.Add(dtpFechaFin);
        pnlFiltros.Controls.Add(btnConsultar);
        pnlFiltros.Controls.Add(btnNuevoPago);
        this.Controls.Add(pnlFiltros);

        // Bottom Content: DataGridView
        dgvHistorial.Location = new Point(20, 210);
        dgvHistorial.Size = new Size(860, 420);
        dgvHistorial.AllowUserToAddRows = false;
        dgvHistorial.AllowUserToDeleteRows = false;
        dgvHistorial.ReadOnly = true;
        dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHistorial.BackgroundColor = Color.White;
        dgvHistorial.BorderStyle = BorderStyle.None;
        dgvHistorial.RowTemplate.Height = 35;
        dgvHistorial.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8F9FA");
        dgvHistorial.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#343A40");
        dgvHistorial.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvHistorial.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        dgvHistorial.ColumnHeadersHeight = 40;
        dgvHistorial.EnableHeadersVisualStyles = false;
        
        this.Controls.Add(dgvHistorial);

        this.Load += CajaForm_Load;
    }

    private async void CajaForm_Load(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async void BtnConsultar_Click(object? sender, EventArgs e)
    {
        await CargarDatosAsync();
    }

    private async void BtnNuevoPago_Click(object? sender, EventArgs e)
    {
        var tarifaService = (TarifaService)_serviceProvider.GetService(typeof(TarifaService))!;
        var pagoService = (PagoService)_serviceProvider.GetService(typeof(PagoService))!;
        var personaRepository = (Gym.Core.Interfaces.IPersonaRepository)_serviceProvider.GetService(typeof(Gym.Core.Interfaces.IPersonaRepository))!;
        
        using var pagoForm = new PagoForm(tarifaService, pagoService, personaRepository, 0, 1); // idPersona = 0 (busca en form), idUsuario = 1
        if (pagoForm.ShowDialog() == DialogResult.OK)
        {
            await CargarDatosAsync();
        }
    }

    private async System.Threading.Tasks.Task CargarDatosAsync()
    {
        try
        {
            btnConsultar.Enabled = false;

            var start = dtpFechaInicio.Value.Date;
            var end = dtpFechaFin.Value.Date.AddDays(1).AddTicks(-1);

            var cierre = await _pagoService.ObtenerCierreCajaAsync(start, end);
            var historial = await _pagoService.ObtenerHistorialPagosAsync(start, end);

            lblTotalEfectivo.Text = $"Total Efectivo: S/ {cierre.TotalEfectivo:0.00}";
            lblTotalYape.Text = $"Total Yape: S/ {cierre.TotalYape:0.00}";
            lblTotalGeneral.Text = $"TOTAL: S/ {cierre.TotalGeneral:0.00}";
            lblCantidadPagos.Text = $"Cantidad Transacciones: {cierre.CantidadPagos}";

            dgvHistorial.DataSource = null;
            dgvHistorial.DataSource = historial;

            if (dgvHistorial.Columns["IdPago"] != null) dgvHistorial.Columns["IdPago"]!.Visible = false;
            if (dgvHistorial.Columns["IdPersona"] != null) dgvHistorial.Columns["IdPersona"]!.Visible = false;
            if (dgvHistorial.Columns["IdUsuario"] != null) dgvHistorial.Columns["IdUsuario"]!.Visible = false;

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al cargar datos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnConsultar.Enabled = true;
        }
    }
}
