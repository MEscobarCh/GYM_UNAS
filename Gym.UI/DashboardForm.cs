using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Services;
using System.Linq; // Necesario para ToList()
using Microsoft.Extensions.DependencyInjection;
using Gym.Core.Interfaces;
using System.Threading.Channels;
using System.Text;

namespace Gym.UI;

public class DashboardForm : Form
{
    private readonly CicloService _cicloService;
    private readonly ClasificacionService _clasificacionService;
    private readonly IngresoService _ingresoService;
    private readonly IPersonaRepository _personaRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISincronizacionService _sincronizacionService;
    private readonly IBiometricoClient _biometricoClient;
    // UI Controls
    private Panel panelTop = new();
    private Label lblTitulo = new();
    private Button btnCaja = new();
    private Button btnAsistencias = new();
    private Button btnModuloEnrolar = new();
    
    // Panel de Sincronización
    private Label lblSeleccion = new();
    private ComboBox cmbCiclos = new();
    private Button btnActualizarCache = new();
    private Label lblEstado = new();

    // Panel de Validación
    private Panel panelValidacion = new();
    private Label lblValidacionTitulo = new();
    private TextBox txtEscaner = new();
    private Label lblInstruccion = new();
    private Label lblResultado = new();
    private Button btnEnrolarHuella = new();
    private Button btnIngresoHuella = new();
    private int _ultimoIdPersonaEscaneado = 0;

    // FASE 2: Motor de Lectura Continua
    private readonly Channel<string> _lecturasChannel = Channel.CreateUnbounded<string>();
    private readonly StringBuilder _barcodeBuffer = new StringBuilder();

    // Constructor sin parámetros por si el Designer de VS lo necesita
    public DashboardForm()
    {
        _cicloService = null!;
        _clasificacionService = null!;
        _ingresoService = null!;
        _personaRepository = null!;
        _serviceProvider = null!;
        _sincronizacionService = null!;
        _biometricoClient = null!;
    }

    public DashboardForm(CicloService cicloService, ClasificacionService clasificacionService, IngresoService ingresoService, IPersonaRepository personaRepository, IServiceProvider serviceProvider, ISincronizacionService sincronizacionService, IBiometricoClient biometricoClient)
    {
        _cicloService = cicloService;
        _clasificacionService = clasificacionService;
        _ingresoService = ingresoService;
        _personaRepository = personaRepository;
        _serviceProvider = serviceProvider;
        _sincronizacionService = sincronizacionService;
        _biometricoClient = biometricoClient;
        ConfigurarUI();
        this.Load += DashboardForm_Load;
        this.FormClosed += DashboardForm_FormClosed;
    }

    private void ConfigurarUI()
    {
        this.Text = "Gym UNAS - Dashboard";
        this.MinimumSize = new Size(1024, 768);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = ColorTranslator.FromHtml("#F0F2F5");
        this.WindowState = FormWindowState.Maximized;

        // Top Panel
        panelTop.BackColor = ColorTranslator.FromHtml("#005C3A");
        panelTop.Dock = DockStyle.Top;
        panelTop.Height = 60;
        panelTop.Width = this.Width; 

        lblTitulo.Text = "GYM UNAS - DASHBOARD";
        lblTitulo.Font = new Font("Segoe UI", 16, FontStyle.Bold);
        lblTitulo.ForeColor = Color.White;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(20, 15);

        btnCaja.Text = "Caja";
        btnCaja.Size = new Size(100, 36);
        btnCaja.Location = new Point(this.Width - 160, 12);
        btnCaja.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnCaja.FlatStyle = FlatStyle.Flat;
        btnCaja.FlatAppearance.BorderSize = 1;
        btnCaja.FlatAppearance.BorderColor = Color.White;
        btnCaja.ForeColor = Color.White;
        btnCaja.BackColor = ColorTranslator.FromHtml("#004A2E");
        btnCaja.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCaja.Cursor = Cursors.Hand;
        btnCaja.Click += BtnCaja_Click;

        btnAsistencias.Text = "Asistencias";
        btnAsistencias.Size = new Size(120, 36);
        btnAsistencias.Location = new Point(this.Width - 290, 12);
        btnAsistencias.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnAsistencias.FlatStyle = FlatStyle.Flat;
        btnAsistencias.FlatAppearance.BorderSize = 1;
        btnAsistencias.FlatAppearance.BorderColor = Color.White;
        btnAsistencias.ForeColor = Color.White;
        btnAsistencias.BackColor = ColorTranslator.FromHtml("#004A2E");
        btnAsistencias.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnAsistencias.Cursor = Cursors.Hand;
        btnAsistencias.Click += BtnAsistencias_Click;

        btnModuloEnrolar.Text = "Enrolar / Actualizar Huella";
        btnModuloEnrolar.Size = new Size(220, 36);
        btnModuloEnrolar.Location = new Point(this.Width - 520, 12);
        btnModuloEnrolar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnModuloEnrolar.FlatStyle = FlatStyle.Flat;
        btnModuloEnrolar.FlatAppearance.BorderSize = 1;
        btnModuloEnrolar.FlatAppearance.BorderColor = Color.White;
        btnModuloEnrolar.ForeColor = Color.White;
        btnModuloEnrolar.BackColor = ColorTranslator.FromHtml("#004A2E");
        btnModuloEnrolar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnModuloEnrolar.Cursor = Cursors.Hand;
        btnModuloEnrolar.Click += BtnModuloEnrolar_Click;

        panelTop.Controls.Add(lblTitulo);
        panelTop.Controls.Add(btnModuloEnrolar);
        panelTop.Controls.Add(btnAsistencias);
        panelTop.Controls.Add(btnCaja);

        // Usamos un TableLayoutPanel central para centrar el contenido en pantalla completa
        TableLayoutPanel layoutCenter = new TableLayoutPanel();
        layoutCenter.Dock = DockStyle.Fill;
        layoutCenter.ColumnCount = 3;
        layoutCenter.RowCount = 3;
        layoutCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layoutCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 850F)); // Ancho fijo del contenedor
        layoutCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        layoutCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        layoutCenter.RowStyles.Add(new RowStyle(SizeType.Absolute, 620F)); // Alto fijo del contenedor
        layoutCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        Panel mainContainer = new Panel();
        mainContainer.Dock = DockStyle.Fill;
        mainContainer.BackColor = Color.Transparent;

        // Content - Sincronización (Tarjeta 1)
        Panel cardSincronizacion = new Panel();
        cardSincronizacion.Size = new Size(850, 140);
        cardSincronizacion.Location = new Point(0, 0);
        cardSincronizacion.BackColor = Color.White;
        cardSincronizacion.BorderStyle = BorderStyle.FixedSingle; // Simular borde de tarjeta
        
        lblSeleccion.Text = "Seleccione el Ciclo Académico:";
        lblSeleccion.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        lblSeleccion.Location = new Point(25, 25);
        lblSeleccion.AutoSize = true;

        cmbCiclos.Location = new Point(25, 55);
        cmbCiclos.Size = new Size(400, 30);
        cmbCiclos.Font = new Font("Segoe UI", 12);
        cmbCiclos.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCiclos.DisplayMember = "Nombre";
        cmbCiclos.ValueMember = "Id";
        cmbCiclos.FlatStyle = FlatStyle.Flat;
        cmbCiclos.BackColor = ColorTranslator.FromHtml("#F8F9FA");

        btnActualizarCache.Text = "Actualizar Lista OCDA";
        btnActualizarCache.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnActualizarCache.ForeColor = Color.White;
        btnActualizarCache.FlatStyle = FlatStyle.Flat;
        btnActualizarCache.FlatAppearance.BorderSize = 0;
        btnActualizarCache.Size = new Size(220, 36);
        btnActualizarCache.Location = new Point(445, 53);
        btnActualizarCache.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnActualizarCache.Cursor = Cursors.Hand;
        btnActualizarCache.Click += BtnActualizarCache_Click;

        lblEstado.Text = "Estado: Esperando...";
        lblEstado.Font = new Font("Segoe UI", 10, FontStyle.Italic);
        lblEstado.ForeColor = Color.Gray;
        lblEstado.AutoSize = true;
        lblEstado.Location = new Point(25, 100);

        cardSincronizacion.Controls.Add(lblSeleccion);
        cardSincronizacion.Controls.Add(cmbCiclos);
        cardSincronizacion.Controls.Add(btnActualizarCache);
        cardSincronizacion.Controls.Add(lblEstado);

        // Panel de Validación (Tarjeta 2)
        panelValidacion.Location = new Point(0, 160);
        panelValidacion.Size = new Size(850, 450);
        panelValidacion.BackColor = Color.White;
        panelValidacion.BorderStyle = BorderStyle.FixedSingle;
        
        lblValidacionTitulo.Text = "Validación de Ingreso";
        lblValidacionTitulo.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        lblValidacionTitulo.ForeColor = ColorTranslator.FromHtml("#333333");
        lblValidacionTitulo.Location = new Point(25, 25);
        lblValidacionTitulo.AutoSize = true;
        
        lblInstruccion.Text = "Escanee el código de barras o ingrese el DNI:";
        lblInstruccion.Font = new Font("Segoe UI", 12);
        lblInstruccion.ForeColor = ColorTranslator.FromHtml("#555555");
        lblInstruccion.Location = new Point(25, 75);
        lblInstruccion.AutoSize = true;

        txtEscaner.Location = new Point(25, 110);
        txtEscaner.Size = new Size(500, 40);
        txtEscaner.Font = new Font("Segoe UI", 18);
        txtEscaner.BackColor = Color.White;
        txtEscaner.BorderStyle = BorderStyle.FixedSingle;
        txtEscaner.ReadOnly = true; // El motor continuo escribe aquí

        // FASE 2: Interceptar todo el teclado a nivel Form
        this.KeyPreview = true;
        this.KeyPress += DashboardForm_KeyPress;

        btnIngresoHuella.Text = "Activar Lector de Puerta (F2)";
        btnIngresoHuella.Size = new Size(270, 40);
        btnIngresoHuella.Location = new Point(545, 110);
        btnIngresoHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnIngresoHuella.ForeColor = Color.White;
        btnIngresoHuella.FlatStyle = FlatStyle.Flat;
        btnIngresoHuella.FlatAppearance.BorderSize = 0;
        btnIngresoHuella.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        btnIngresoHuella.Cursor = Cursors.Hand;
        btnIngresoHuella.Click += BtnIngresoHuella_Click;

        btnEnrolarHuella.Text = "Registrar Huella (Escaneado)";
        btnEnrolarHuella.Size = new Size(270, 40);
        btnEnrolarHuella.Location = new Point(545, 165);
        btnEnrolarHuella.BackColor = ColorTranslator.FromHtml("#6C757D"); 
        btnEnrolarHuella.ForeColor = Color.White;
        btnEnrolarHuella.FlatStyle = FlatStyle.Flat;
        btnEnrolarHuella.FlatAppearance.BorderSize = 0;
        btnEnrolarHuella.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        btnEnrolarHuella.Cursor = Cursors.Hand;
        btnEnrolarHuella.Visible = false;
        btnEnrolarHuella.Click += BtnEnrolarHuella_Click;

        lblResultado.Location = new Point(25, 230);
        lblResultado.Size = new Size(790, 180);
        lblResultado.Font = new Font("Segoe UI", 24, FontStyle.Bold);
        lblResultado.TextAlign = ContentAlignment.MiddleCenter;
        lblResultado.ForeColor = Color.White;
        lblResultado.BackColor = Color.LightGray; 
        lblResultado.Visible = false; 

        panelValidacion.Controls.Add(lblValidacionTitulo);
        panelValidacion.Controls.Add(lblInstruccion);
        panelValidacion.Controls.Add(txtEscaner);
        panelValidacion.Controls.Add(btnIngresoHuella);
        panelValidacion.Controls.Add(btnEnrolarHuella);
        panelValidacion.Controls.Add(lblResultado);

        mainContainer.Controls.Add(cardSincronizacion);
        mainContainer.Controls.Add(panelValidacion);

        layoutCenter.Controls.Add(mainContainer, 1, 1); // Agregar a la celda central

        this.Controls.Add(layoutCenter);
        this.Controls.Add(panelTop);
    }

    private async void DashboardForm_Load(object? sender, EventArgs e)
    {
        try
        {
            lblEstado.Text = "Sincronizando ciclos desde BD y OCDA...";
            var ciclos = await _cicloService.SincronizarCiclosAsync();
            
            cmbCiclos.DataSource = ciclos.ToList();
            lblEstado.Text = $"Ciclos cargados correctamente a las {DateTime.Now:HH:mm:ss}";
            
            // FASE 2: Iniciar motor continuo
            _ = MotorValidacionAsync();
        }
        catch (Exception ex)
        {
            lblEstado.Text = "Error al sincronizar ciclos: " + ex.Message;
            lblEstado.ForeColor = Color.Red;
        }
    }

    private void DashboardForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        _biometricoClient.DetenerLector();
    }

    private bool _lectorHuellaActivo = false;

    private void BtnIngresoHuella_Click(object? sender, EventArgs e)
    {
        if (!_lectorHuellaActivo)
        {
            try
            {
                _biometricoClient.OnHuellaDetectada -= BiometricoClient_OnHuellaDetectada;
                _biometricoClient.OnHuellaDetectada += BiometricoClient_OnHuellaDetectada;
                _biometricoClient.InicializarLector();
                _lectorHuellaActivo = true;

                btnIngresoHuella.Text = "Desactivar Lector";
                btnIngresoHuella.BackColor = Color.Red;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al activar lector: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            try
            {
                _biometricoClient.DetenerLector();
                _biometricoClient.OnHuellaDetectada -= BiometricoClient_OnHuellaDetectada;
            }
            catch { }

            _lectorHuellaActivo = false;
            btnIngresoHuella.Text = "Activar Lector de Puerta (F2)";
            btnIngresoHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
        }
    }

    private void BiometricoClient_OnHuellaDetectada(object? sender, object featureSet)
    {
        // Se ejecuta en el background thread del SDK de DigitalPersona
        // Usamos BeginInvoke (fire-and-forget) para no bloquear la captura de nuevas huellas
        if (this.InvokeRequired)
        {
            this.BeginInvoke(new Action(async () => await ProcesarMatchHuellaAsync(featureSet)));
        }
        else
        {
            _ = ProcesarMatchHuellaAsync(featureSet);
        }
    }

    private async Task ProcesarMatchHuellaAsync(object featureSet)
    {
        try
        {
            var personasConHuella = await _personaRepository.ObtenerTodasConHuellaAsync();
            var personaMatch = _biometricoClient.IdentificarDesdeFeatureSet(featureSet, personasConHuella);

            if (personaMatch != null && !string.IsNullOrEmpty(personaMatch.Dni))
            {
                if (cmbCiclos.SelectedValue is int idCiclo)
                {
                    _ultimoIdPersonaEscaneado = personaMatch.Id;
                    btnEnrolarHuella.Visible = true;

                    // Validar si es ALUMNO o EXTERNO
                    string tipoPersona = await _clasificacionService.DeterminarTipoPersonaAsync(personaMatch.Dni, idCiclo);
                    bool accesoPermitido = false;
                    string mensajeAcceso = "";

                    if (tipoPersona == "ALUMNO")
                    {
                        accesoPermitido = true;
                        mensajeAcceso = $"{personaMatch.Nombres} - Acceso Permitido (ALUMNO)";
                    }
                    else
                    {
                        var suscripcionService = _serviceProvider.GetRequiredService<SuscripcionService>();
                        var suscripcionVigente = await suscripcionService.ObtenerSuscripcionVigenteAsync(personaMatch.Id);
                        if (suscripcionVigente != null)
                        {
                            accesoPermitido = true;
                            mensajeAcceso = $"{personaMatch.Nombres} - Acceso Permitido (EXTERNO Activo)";
                        }
                    }

                    if (accesoPermitido)
                    {
                        await _ingresoService.RegistrarIngresoAlumnoAsync(personaMatch.Dni, idCiclo, "HUELLA", 1);
                        lblResultado.Text = mensajeAcceso;
                        lblResultado.BackColor = ColorTranslator.FromHtml("#005C3A");
                    }
                    else
                    {
                        lblResultado.Text = "Membresía Vencida. Pase a Caja.";
                        lblResultado.BackColor = Color.Red;
                    }
                    lblResultado.Visible = true;
                }
                else
                {
                    lblResultado.Text = "Seleccione un ciclo primero para registrar.";
                    lblResultado.BackColor = Color.Orange;
                    lblResultado.Visible = true;
                }
            }
            else
            {
                lblResultado.Text = "Huella no registrada o intente nuevamente";
                lblResultado.BackColor = Color.Red;
                lblResultado.Visible = true;
            }
        }
        catch (Exception ex)
        {
            lblResultado.Text = "Error interno: " + ex.Message;
            lblResultado.BackColor = Color.Red;
            lblResultado.Visible = true;
        }
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.F2)
        {
            if (btnIngresoHuella.Enabled) btnIngresoHuella.PerformClick();
            return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    private async void BtnEnrolarHuella_Click(object? sender, EventArgs e)
    {
        if (_ultimoIdPersonaEscaneado <= 0) return;
        
        var persona = await _personaRepository.ObtenerPorIdAsync(_ultimoIdPersonaEscaneado);
        if (persona == null) return;

        bool estabaActivo = _lectorHuellaActivo;
        if (estabaActivo)
        {
            _biometricoClient.DetenerLector();
            _biometricoClient.OnHuellaDetectada -= BiometricoClient_OnHuellaDetectada;
            _lectorHuellaActivo = false;
            btnIngresoHuella.Text = "Activar Lector de Puerta (F2)";
            btnIngresoHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
        }

        var enrolamientoForm = _serviceProvider.GetRequiredService<EnrolamientoForm>();
        enrolamientoForm.Inicializar(_ultimoIdPersonaEscaneado, persona.Dni ?? string.Empty, persona.Nombres ?? string.Empty);
        enrolamientoForm.ShowDialog();

        if (estabaActivo)
        {
            _biometricoClient.OnHuellaDetectada += BiometricoClient_OnHuellaDetectada;
            _biometricoClient.InicializarLector();
            _lectorHuellaActivo = true;
            btnIngresoHuella.Text = "Desactivar Lector";
            btnIngresoHuella.BackColor = Color.Red;
        }
    }

    private void BtnCaja_Click(object? sender, EventArgs e)
    {
        using var cajaForm = new CajaForm(_serviceProvider);
        cajaForm.ShowDialog();
    }

    private void BtnAsistencias_Click(object? sender, EventArgs e)
    {
        var ingresoService = _serviceProvider.GetRequiredService<IngresoService>();
        using var asistenciasForm = new AsistenciasForm(ingresoService);
        asistenciasForm.ShowDialog();
    }

    private void BtnModuloEnrolar_Click(object? sender, EventArgs e)
    {
        bool estabaActivo = _lectorHuellaActivo;
        if (estabaActivo)
        {
            _biometricoClient.DetenerLector();
            _biometricoClient.OnHuellaDetectada -= BiometricoClient_OnHuellaDetectada;
            _lectorHuellaActivo = false;
            btnIngresoHuella.Text = "Activar Lector de Puerta (F2)";
            btnIngresoHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
        }

        var enrolamientoForm = _serviceProvider.GetRequiredService<EnrolamientoForm>();
        enrolamientoForm.ShowDialog();

        if (estabaActivo)
        {
            _biometricoClient.OnHuellaDetectada += BiometricoClient_OnHuellaDetectada;
            _biometricoClient.InicializarLector();
            _lectorHuellaActivo = true;
            btnIngresoHuella.Text = "Desactivar Lector";
            btnIngresoHuella.BackColor = Color.Red;
        }
    }

    private async void BtnActualizarCache_Click(object? sender, EventArgs e)
    {
        if (cmbCiclos.SelectedValue is int idCiclo)
        {
            try
            {
                btnActualizarCache.Enabled = false;
                lblEstado.Text = $"Descargando alumnos del ciclo {cmbCiclos.Text}...";
                lblEstado.ForeColor = Color.Gray;

                await _cicloService.ActualizarCacheAlumnosAsync(idCiclo);

                lblEstado.Text = $"Reclasificando alumnos en BD...";
                await _sincronizacionService.EjecutarReclasificacionMasivaAsync(idCiclo, 1); // 1 = idUsuarioAdmin

                lblEstado.Text = $"Sincronizado y Reclasificado: {DateTime.Now:HH:mm:ss}";
                lblEstado.ForeColor = ColorTranslator.FromHtml("#005C3A");
                txtEscaner.Focus(); // Mantener foco
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Error al actualizar caché: " + ex.Message;
                lblEstado.ForeColor = Color.Red;
            }
            finally
            {
                btnActualizarCache.Enabled = true;
            }
        }
        else
        {
            MessageBox.Show("Por favor seleccione un ciclo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async Task MotorValidacionAsync()
    {
        var reader = _lecturasChannel.Reader;
        // Bucle infinito, esperando lecturas sin bloquear la UI
        while (await reader.WaitToReadAsync())
        {
            if (reader.TryRead(out string? texto) && !string.IsNullOrEmpty(texto))
            {
                // Mostramos estado en UI
                lblEstado.Text = $"Validando: {texto}...";
                lblEstado.ForeColor = Color.Orange;
                
                await ProcesarIngresoAsync(texto);
                
                lblEstado.Text = "Esperando lectura...";
                lblEstado.ForeColor = Color.Gray;
            }
        }
    }

    private void DashboardForm_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Enter)
        {
            string texto = _barcodeBuffer.ToString().Trim();
            _barcodeBuffer.Clear();
            txtEscaner.Text = ""; // Limpieza visual inmediata
            
            if (!string.IsNullOrEmpty(texto))
            {
                // Enviar a la cola para validación asíncrona
                _lecturasChannel.Writer.TryWrite(texto);
            }
            e.Handled = true;
        }
        else if (e.KeyChar == (char)Keys.Back)
        {
            if (_barcodeBuffer.Length > 0)
            {
                _barcodeBuffer.Length--;
                txtEscaner.Text = _barcodeBuffer.ToString();
            }
            e.Handled = true;
        }
        else if (!char.IsControl(e.KeyChar))
        {
            _barcodeBuffer.Append(e.KeyChar);
            txtEscaner.Text = _barcodeBuffer.ToString();
            e.Handled = true;
        }
    }

    private async Task ProcesarIngresoAsync(string texto)
    {
        if (cmbCiclos.SelectedValue is int idCicloSeleccionado)
        {
            try
            {
                    // Determinar tipo de persona
                    string resultado = await _clasificacionService.DeterminarTipoPersonaAsync(texto, idCicloSeleccionado);

                    if (resultado == "ALUMNO")
                    {
                        // 1 es el idUsuario hardcodeado por ahora (podríamos sacarlo del AuthService o sesión)
                        // "DNI" o "CARNET" dependiendo del escaneo, asumimos "DNI" por ahora
                        await _ingresoService.RegistrarIngresoAlumnoAsync(texto, idCicloSeleccionado, "DNI", 1);
                        
                        lblResultado.Text = "Acceso Permitido - ALUMNO";
                        lblResultado.BackColor = ColorTranslator.FromHtml("#005C3A"); // Verde
                        lblResultado.Visible = true;

                        var personaAlumno = await _personaRepository.ObtenerPorDniAsync(texto);
                        _ultimoIdPersonaEscaneado = personaAlumno?.Id ?? 0;
                        btnEnrolarHuella.Visible = (_ultimoIdPersonaEscaneado > 0);
                    }
                    else // "EXTERNO"
                    {
                        var persona = await _personaRepository.ObtenerPorDniAsync(texto);
                        int idPersona = persona?.Id ?? 0;
                        
                        using var scope = _serviceProvider.CreateScope();

                        if (idPersona == 0)
                        {
                            // 1. Mostrar modal de registro
                            var reniecClient = scope.ServiceProvider.GetRequiredService<IReniecClient>();
                            var biometricoClient = scope.ServiceProvider.GetRequiredService<IBiometricoClient>();
                            using var registroForm = new RegistroExternoForm(_personaRepository, reniecClient, biometricoClient, texto);
                            if (registroForm.ShowDialog() == DialogResult.OK)
                            {
                                idPersona = registroForm.NuevoIdPersona;
                            }
                            else
                            {
                                // Operador canceló el registro
                                lblResultado.Text = "EXTERNO - Registro Cancelado";
                                lblResultado.BackColor = Color.Orange;
                                lblResultado.Visible = true;
                                return; // abortar
                            }
                        }

                        // A este punto ya tenemos IdPersona sí o sí
                        var suscripcionService = scope.ServiceProvider.GetRequiredService<SuscripcionService>();
                        var suscripcionVigente = await suscripcionService.ObtenerSuscripcionVigenteAsync(idPersona);

                        if (suscripcionVigente != null)
                        {
                            // 2. Tiene suscripción activa
                            await _ingresoService.RegistrarIngresoAlumnoAsync(texto, idCicloSeleccionado, "DNI", 1);
                            lblResultado.Text = "EXTERNO - Mensualidad Activa. Acceso Permitido";
                            lblResultado.BackColor = ColorTranslator.FromHtml("#005C3A"); // Verde
                            lblResultado.Visible = true;

                            _ultimoIdPersonaEscaneado = idPersona;
                            btnEnrolarHuella.Visible = true;
                        }
                        else
                        {
                            // 3. No tiene suscripción activa, toca cobrar
                            var tarifaService = scope.ServiceProvider.GetRequiredService<TarifaService>();
                            var pagoService = scope.ServiceProvider.GetRequiredService<PagoService>();
                            var personaRepository = scope.ServiceProvider.GetRequiredService<IPersonaRepository>();
                            
                            using var pagoForm = new PagoForm(tarifaService, pagoService, personaRepository, idPersona, 1); // 1 = idUsuario hardcodeado por ahora
                            
                            if (pagoForm.ShowDialog() == DialogResult.OK)
                            {
                                await _ingresoService.RegistrarIngresoAlumnoAsync(texto, idCicloSeleccionado, "DNI", 1);
                                lblResultado.Text = "EXTERNO - Pago Confirmado. Acceso Permitido";
                                lblResultado.BackColor = ColorTranslator.FromHtml("#005C3A"); // Verde
                                _ultimoIdPersonaEscaneado = idPersona;
                                btnEnrolarHuella.Visible = true;
                            }
                            else
                            {
                                lblResultado.Text = "EXTERNO - Pago Cancelado/Pendiente";
                                lblResultado.BackColor = Color.Orange;
                            }
                            lblResultado.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblResultado.Text = "Error: " + ex.Message;
                    lblResultado.BackColor = Color.Red;
                    lblResultado.Visible = true;
                }
            }
            else
            {
                MessageBox.Show("Seleccione un ciclo primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}

















