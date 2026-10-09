using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Services;

namespace Gym.UI;

public class CajaForm : Form
{
    private readonly PagoService _pagoService;

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
    }

    public CajaForm(PagoService pagoService)
    {
        _pagoService = pagoService;
        ConfigurarUI();
    }

    private void ConfigurarUI()
    {
        this.Text = "Reportes de Caja e Historial";
        this.Size = new Size(850, 650);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;
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
        
        btnCerrar.Text = "X";
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

        // Filtros
        lblFechaInicio.Text = "Desde:";
        lblFechaInicio.Location = new Point(20, 70);
        lblFechaInicio.AutoSize = true;
        
        dtpFechaInicio.Format = DateTimePickerFormat.Short;
        dtpFechaInicio.Location = new Point(70, 65);
        dtpFechaInicio.Size = new Size(120, 25);
        dtpFechaInicio.Value = DateTime.Today;

        lblFechaFin.Text = "Hasta:";
        lblFechaFin.Location = new Point(210, 70);
        lblFechaFin.AutoSize = true;

        dtpFechaFin.Format = DateTimePickerFormat.Short;
        dtpFechaFin.Location = new Point(260, 65);
        dtpFechaFin.Size = new Size(120, 25);
        dtpFechaFin.Value = DateTime.Today;

        btnConsultar.Text = "Consultar";
        btnConsultar.Location = new Point(400, 62);
        btnConsultar.Size = new Size(100, 30);
        btnConsultar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnConsultar.ForeColor = Color.White;
        btnConsultar.FlatStyle = FlatStyle.Flat;
        btnConsultar.Click += BtnConsultar_Click;

        this.Controls.Add(lblFechaInicio);
        this.Controls.Add(dtpFechaInicio);
        this.Controls.Add(lblFechaFin);
        this.Controls.Add(dtpFechaFin);
        this.Controls.Add(btnConsultar);

        // Resumen
        gbResumen.Text = "Resumen de Caja";
        gbResumen.Location = new Point(20, 110);
        gbResumen.Size = new Size(810, 80);
        gbResumen.Font = new Font("Segoe UI", 10, FontStyle.Bold);

        lblTotalEfectivo.Text = "Total Efectivo: S/ 0.00";
        lblTotalEfectivo.Location = new Point(20, 30);
        lblTotalEfectivo.AutoSize = true;

        lblTotalYape.Text = "Total Yape: S/ 0.00";
        lblTotalYape.Location = new Point(220, 30);
        lblTotalYape.AutoSize = true;

        lblTotalGeneral.Text = "TOTAL: S/ 0.00";
        lblTotalGeneral.Location = new Point(420, 30);
        lblTotalGeneral.AutoSize = true;
        lblTotalGeneral.ForeColor = Color.DarkGreen;

        lblCantidadPagos.Text = "N de Pagos: 0";
        lblCantidadPagos.Location = new Point(620, 30);
        lblCantidadPagos.AutoSize = true;

        gbResumen.Controls.Add(lblTotalEfectivo);
        gbResumen.Controls.Add(lblTotalYape);
        gbResumen.Controls.Add(lblTotalGeneral);
        gbResumen.Controls.Add(lblCantidadPagos);
        this.Controls.Add(gbResumen);

        // Grid
        dgvHistorial.Location = new Point(20, 210);
        dgvHistorial.Size = new Size(810, 410);
        dgvHistorial.AllowUserToAddRows = false;
        dgvHistorial.AllowUserToDeleteRows = false;
        dgvHistorial.ReadOnly = true;
        dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHistorial.BackgroundColor = Color.WhiteSmoke;
        
        this.Controls.Add(dgvHistorial);
    }

    private async void BtnConsultar_Click(object? sender, EventArgs e)
    {
        try
        {
            btnConsultar.Enabled = false;

            var start = dtpFechaInicio.Value.Date;
            var end = dtpFechaFin.Value.Date.AddDays(1).AddTicks(-1); // Fin del da

            var cierre = await _pagoService.ObtenerCierreCajaAsync(start, end);
            var historial = await _pagoService.ObtenerHistorialPagosAsync(start, end);

            // Actualizar Resumen
            lblTotalEfectivo.Text = $"Total Efectivo: S/ {cierre.TotalEfectivo:0.00}";
            lblTotalYape.Text = $"Total Yape: S/ {cierre.TotalYape:0.00}";
            lblTotalGeneral.Text = $"TOTAL: S/ {cierre.TotalGeneral:0.00}";
            lblCantidadPagos.Text = $"N de Pagos: {cierre.CantidadPagos}";

            // Actualizar Grid
            dgvHistorial.DataSource = historial;

            // Formato de columnas si es necesario
            if (dgvHistorial.Columns["IdPago"] != null) dgvHistorial.Columns["IdPago"]!.Visible = false;
            if (dgvHistorial.Columns["IdPersona"] != null) dgvHistorial.Columns["IdPersona"]!.Visible = false;
            if (dgvHistorial.Columns["IdUsuario"] != null) dgvHistorial.Columns["IdUsuario"]!.Visible = false;

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al consultar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnConsultar.Enabled = true;
        }
    }
}
