using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Services;
using Gym.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gym.UI;

public class AsistenciasForm : Form
{
    private readonly IngresoService _ingresoService;
    
    // UI Controls
    
    private Panel panelFiltros = new();
    private Label lblFecha = new();
    private DateTimePicker dtpFecha = new();
    private Button btnBuscar = new();
    
    private DataGridView dgvAsistencias = new();
    private Label lblTotal = new();

    public AsistenciasForm()
    {
        _ingresoService = null!;
    }

    public AsistenciasForm(IngresoService ingresoService)
    {
        _ingresoService = ingresoService;
        ConfigurarUI();
        this.Load += async (s, e) => await AsistenciasForm_Load(s, e);
    }

    private void ConfigurarUI()
    {
        this.Text = "Módulo de Asistencias";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.ClientSize = new Size(900, 620);
        this.MinimumSize = new Size(920, 660);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = ColorTranslator.FromHtml("#F0F2F5");

        // Top Panel

        // Filtros Panel
        panelFiltros.Dock = DockStyle.Top;
        panelFiltros.Height = 70;
        panelFiltros.BackColor = Color.White;
        panelFiltros.Padding = new Padding(20);

        lblFecha.Text = "Fecha:";
        lblFecha.Location = new Point(20, 25);
        lblFecha.AutoSize = true;
        lblFecha.Font = new Font("Segoe UI", 10, FontStyle.Bold);

        dtpFecha.Location = new Point(80, 20);
        dtpFecha.Format = DateTimePickerFormat.Short;
        dtpFecha.Size = new Size(150, 25);
        dtpFecha.Font = new Font("Segoe UI", 10);

        btnBuscar.Text = "Refrescar / Buscar";
        btnBuscar.Location = new Point(250, 18);
        btnBuscar.Size = new Size(180, 30);
        btnBuscar.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnBuscar.ForeColor = Color.White;
        btnBuscar.FlatStyle = FlatStyle.Flat;
        btnBuscar.FlatAppearance.BorderSize = 0;
        btnBuscar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnBuscar.Cursor = Cursors.Hand;
        btnBuscar.Click += async (s, e) => await CargarAsistenciasAsync(dtpFecha.Value);

        panelFiltros.Controls.Add(lblFecha);
        panelFiltros.Controls.Add(dtpFecha);
        panelFiltros.Controls.Add(btnBuscar);

        // DataGridView
        dgvAsistencias.Dock = DockStyle.Fill;
        dgvAsistencias.BackgroundColor = Color.White;
        dgvAsistencias.BorderStyle = BorderStyle.None;
        dgvAsistencias.AllowUserToAddRows = false;
        dgvAsistencias.AllowUserToDeleteRows = false;
        dgvAsistencias.ReadOnly = true;
        dgvAsistencias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvAsistencias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvAsistencias.RowTemplate.Height = 35;
        dgvAsistencias.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8F9FA");
        dgvAsistencias.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#343A40");
        dgvAsistencias.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgvAsistencias.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        dgvAsistencias.ColumnHeadersHeight = 40;
        dgvAsistencias.EnableHeadersVisualStyles = false;

        // Bottom Panel para Totales
        Panel panelBottom = new Panel();
        panelBottom.Dock = DockStyle.Bottom;
        panelBottom.Height = 40;
        panelBottom.BackColor = Color.White;

        lblTotal.Text = "Total de ingresos: 0";
        lblTotal.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        lblTotal.Location = new Point(20, 10);
        lblTotal.AutoSize = true;
        
        panelBottom.Controls.Add(lblTotal);

        // Main Container para dar un padding alrededor del DGV
        Panel container = new Panel();
        container.Dock = DockStyle.Fill;
        container.Padding = new Padding(20);
        container.Controls.Add(dgvAsistencias);

        this.Controls.Add(container);
        this.Controls.Add(panelBottom);
        this.Controls.Add(panelFiltros);
    }

    private async Task AsistenciasForm_Load(object? sender, EventArgs e)
    {
        dtpFecha.Value = DateTime.Today;
        await CargarAsistenciasAsync(dtpFecha.Value);
    }

    private async Task CargarAsistenciasAsync(DateTime fecha)
    {
        try
        {
            btnBuscar.Enabled = false;
            btnBuscar.Text = "Cargando...";

            // Obtenemos los ingresos de la fecha exacta
            var registros = await _ingresoService.ObtenerHistorialAsync(fecha.Date, fecha.Date, string.Empty);

            dgvAsistencias.DataSource = null; 
            dgvAsistencias.DataSource = registros;

            ConfigurarColumnas();
            
            lblTotal.Text = $"Total de ingresos: {((List<IngresoDto>)registros).Count}";
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al cargar asistencias: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnBuscar.Enabled = true;
            btnBuscar.Text = "Refrescar / Buscar";
        }
    }

    private void ConfigurarColumnas()
    {
        if (dgvAsistencias.Columns.Count > 0)
        {
            var colId = dgvAsistencias.Columns["Id"];
            if (colId != null) colId.Visible = false;

            var colTipo = dgvAsistencias.Columns["Tipo"];
            if (colTipo != null) colTipo.Visible = false; // Se oculta a menos que se quiera ver si es Externo/Alumno

            var colValidacion = dgvAsistencias.Columns["Validacion"];
            if (colValidacion != null) colValidacion.Visible = false;
            
            var colFechaHora = dgvAsistencias.Columns["FechaHora"];
            if (colFechaHora != null)
            {
                colFechaHora.HeaderText = "Hora";
                colFechaHora.DefaultCellStyle.Format = "HH:mm:ss";
                colFechaHora.FillWeight = 15;
            }

            var colNombres = dgvAsistencias.Columns["Nombres"];
            if (colNombres != null)
            {
                colNombres.HeaderText = "Nombre del Alumno";
                colNombres.FillWeight = 45;
            }

            var colDni = dgvAsistencias.Columns["Dni"];
            if (colDni != null)
            {
                colDni.HeaderText = "DNI";
                colDni.FillWeight = 15;
            }

            // Agregamos la columna de Estado si no existe
            if (!dgvAsistencias.Columns.Contains("Estado"))
            {
                dgvAsistencias.Columns.Add("Estado", "Estado");
            }
            
            var colEstado = dgvAsistencias.Columns["Estado"];
            if (colEstado != null)
            {
                colEstado.FillWeight = 25;
                colEstado.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#005C3A");
                colEstado.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }

            // Dado que solo se inyectan en BD los accesos permitidos, los marcamos como tal:
            foreach (DataGridViewRow row in dgvAsistencias.Rows)
            {
                row.Cells["Estado"].Value = "Acceso Permitido";
            }
        }
    }
}
