using System;
using System.Drawing;
using System.Windows.Forms;
using Gym.Core.Services;
using System.Linq; // Necesario para ToList()
using Microsoft.Extensions.DependencyInjection;
using Gym.Core.Interfaces;

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
    private Button btnCerrar = new();
    private Button btnCaja = new();
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
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = Color.White;
        this.FormBorderStyle = FormBorderStyle.None;

        // Top Panel
        panelTop.BackColor = ColorTranslator.FromHtml("#005C3A"); // Verde institucional
        panelTop.Dock = DockStyle.Top;
        panelTop.Height = 50;

        lblTitulo.Text = "DASHBOARD";
        lblTitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblTitulo.ForeColor = Color.White;
        lblTitulo.AutoSize = true;
        lblTitulo.Location = new Point(20, 10);
        
        btnCerrar.Text = "X";
        btnCerrar.Size = new Size(40, 40);
        btnCerrar.Location = new Point(this.Width - 40, 5);
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.FlatAppearance.BorderSize = 0;
        btnCerrar.ForeColor = Color.White;
        btnCerrar.Font = new Font("Segoe UI", 12, FontStyle.Bold);
        btnCerrar.Cursor = Cursors.Hand;
        btnCerrar.Click += (s, e) => this.Close();

        btnCaja.Text = "Caja e Historial";
        btnCaja.Size = new Size(150, 30);
        btnCaja.Location = new Point(this.Width - 200, 10);
        btnCaja.FlatStyle = FlatStyle.Flat;
        btnCaja.FlatAppearance.BorderSize = 1;
        btnCaja.FlatAppearance.BorderColor = Color.White;
        btnCaja.ForeColor = Color.White;
        btnCaja.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnCaja.Cursor = Cursors.Hand;
        btnCaja.Click += BtnCaja_Click;

        btnModuloEnrolar.Text = "Enrolar / Actualizar Huella";
        btnModuloEnrolar.Size = new Size(220, 30);
        btnModuloEnrolar.Location = new Point(this.Width - 430, 10);
        btnModuloEnrolar.FlatStyle = FlatStyle.Flat;
        btnModuloEnrolar.FlatAppearance.BorderSize = 1;
        btnModuloEnrolar.FlatAppearance.BorderColor = Color.White;
        btnModuloEnrolar.ForeColor = Color.White;
        btnModuloEnrolar.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnModuloEnrolar.Cursor = Cursors.Hand;
        btnModuloEnrolar.Click += BtnModuloEnrolar_Click;

        panelTop.Controls.Add(lblTitulo);
        panelTop.Controls.Add(btnModuloEnrolar);
        panelTop.Controls.Add(btnCaja);
        panelTop.Controls.Add(btnCerrar);

        // Content - Sincronización
        lblSeleccion.Text = "Seleccione el Ciclo Académico:";
        lblSeleccion.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        lblSeleccion.Location = new Point(50, 80);
        lblSeleccion.AutoSize = true;

        cmbCiclos.Location = new Point(50, 110);
        cmbCiclos.Size = new Size(300, 30);
        cmbCiclos.Font = new Font("Segoe UI", 11);
        cmbCiclos.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCiclos.DisplayMember = "Nombre";
        cmbCiclos.ValueMember = "Id";
        cmbCiclos.FlatStyle = FlatStyle.Flat;
        cmbCiclos.BackColor = Color.WhiteSmoke;

        btnActualizarCache.Text = "Actualizar Lista OCDA";
        btnActualizarCache.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnActualizarCache.ForeColor = Color.White;
        btnActualizarCache.FlatStyle = FlatStyle.Flat;
        btnActualizarCache.FlatAppearance.BorderSize = 0;
        btnActualizarCache.Size = new Size(200, 40);
        btnActualizarCache.Location = new Point(370, 105);
        btnActualizarCache.Font = new Font("Segoe UI", 10, FontStyle.Bold);
        btnActualizarCache.Cursor = Cursors.Hand;
        btnActualizarCache.Click += BtnActualizarCache_Click;

        lblEstado.Text = "Estado: Esperando...";
        lblEstado.Font = new Font("Segoe UI", 9, FontStyle.Italic);
        lblEstado.ForeColor = Color.Gray;
        lblEstado.AutoSize = true;
        lblEstado.Location = new Point(50, 150);

        // Panel de Validación
        panelValidacion.Location = new Point(50, 200);
        panelValidacion.Size = new Size(700, 350);
        panelValidacion.BackColor = Color.WhiteSmoke;
        
        lblValidacionTitulo.Text = "Validar ingreso";
        lblValidacionTitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
        lblValidacionTitulo.Location = new Point(20, 20);
        lblValidacionTitulo.AutoSize = true;
        
        lblInstruccion.Text = "Escanee el código de barras o ingrese el DNI:";
        lblInstruccion.Font = new Font("Segoe UI", 11);
        lblInstruccion.Location = new Point(20, 70);
        lblInstruccion.AutoSize = true;

        txtEscaner.Location = new Point(20, 100);
        txtEscaner.Size = new Size(400, 40);
        txtEscaner.Font = new Font("Segoe UI", 16);
        txtEscaner.KeyDown += TxtEscaner_KeyDown;

        lblResultado.Location = new Point(20, 170);
        lblResultado.Size = new Size(660, 100);
        lblResultado.Font = new Font("Segoe UI", 18, FontStyle.Bold);
        lblResultado.TextAlign = ContentAlignment.MiddleCenter;
        lblResultado.ForeColor = Color.White;
        lblResultado.Visible = false; // Oculto al inicio

        btnEnrolarHuella.Text = "Registrar Huella";
        btnEnrolarHuella.Size = new Size(200, 40);
        btnEnrolarHuella.Location = new Point(460, 100);
        btnEnrolarHuella.BackColor = Color.SlateGray;
        btnEnrolarHuella.ForeColor = Color.White;
        btnEnrolarHuella.FlatStyle = FlatStyle.Flat;
        btnEnrolarHuella.FlatAppearance.BorderSize = 0;
        btnEnrolarHuella.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        btnEnrolarHuella.Cursor = Cursors.Hand;
        btnEnrolarHuella.Visible = false;
        btnEnrolarHuella.Click += BtnEnrolarHuella_Click;

        btnIngresoHuella.Text = "Ingreso por Huella (F2)";
        btnIngresoHuella.Size = new Size(220, 40);
        btnIngresoHuella.Location = new Point(440, 100);
        btnIngresoHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
        btnIngresoHuella.ForeColor = Color.White;
        btnIngresoHuella.FlatStyle = FlatStyle.Flat;
        btnIngresoHuella.FlatAppearance.BorderSize = 0;
        btnIngresoHuella.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        btnIngresoHuella.Cursor = Cursors.Hand;
        btnIngresoHuella.Click += BtnIngresoHuella_Click;

        panelValidacion.Controls.Add(btnIngresoHuella);

        panelValidacion.Controls.Add(btnEnrolarHuella);
        panelValidacion.Controls.Add(lblValidacionTitulo);
        panelValidacion.Controls.Add(lblInstruccion);
        panelValidacion.Controls.Add(txtEscaner);
        panelValidacion.Controls.Add(lblResultado);

        this.Controls.Add(panelTop);
        this.Controls.Add(lblSeleccion);
        this.Controls.Add(cmbCiclos);
        this.Controls.Add(btnActualizarCache);
        this.Controls.Add(lblEstado);
        this.Controls.Add(panelValidacion);
    }

    private async void DashboardForm_Load(object? sender, EventArgs e)
    {
        try
        {
            lblEstado.Text = "Sincronizando ciclos desde BD y OCDA...";
            var ciclos = await _cicloService.SincronizarCiclosAsync();
            
            cmbCiclos.DataSource = ciclos.ToList();
            lblEstado.Text = $"Ciclos cargados correctamente a las {DateTime.Now:HH:mm:ss}";
            txtEscaner.Focus(); // Mantener foco

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

    private async void BtnIngresoHuella_Click(object? sender, EventArgs e)
    {
        try
        {
            btnIngresoHuella.Enabled = false;
            btnIngresoHuella.Text = "Esperando lectura...";
            btnIngresoHuella.BackColor = Color.Orange;

            var personasConHuella = await _personaRepository.ObtenerTodasConHuellaAsync();
            var personaMatch = await _biometricoClient.IdentificarPersonaAsync(personasConHuella);

            if (personaMatch != null && !string.IsNullOrEmpty(personaMatch.Dni))
            {
                await ProcesarIngresoAsync(personaMatch.Dni);
            }
            else
            {
                lblResultado.Text = "Huella no reconocida";
                lblResultado.BackColor = Color.Red;
                lblResultado.Visible = true;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error en lector: " + ex.Message);
        }
        finally
        {
            btnIngresoHuella.Enabled = true;
            btnIngresoHuella.Text = "Ingreso por Huella (F2)";
            btnIngresoHuella.BackColor = ColorTranslator.FromHtml("#005C3A");
            txtEscaner.Focus();
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

    private void BtnEnrolarHuella_Click(object? sender, EventArgs e)
    {
        if (_ultimoIdPersonaEscaneado <= 0) return;
        var enrolamientoForm = _serviceProvider.GetRequiredService<EnrolamientoForm>();
        enrolamientoForm.Inicializar(_ultimoIdPersonaEscaneado);
        enrolamientoForm.ShowDialog();
    }

    private void BtnCaja_Click(object? sender, EventArgs e)
    {
        var pagoService = _serviceProvider.GetRequiredService<PagoService>();
        using var cajaForm = new CajaForm(pagoService);
        cajaForm.ShowDialog();
    }

    private async void BtnModuloEnrolar_Click(object? sender, EventArgs e)
    {
        string input = Prompt.ShowDialog("Ingrese DNI o Código de la persona:", "Enrolamiento de Huella");
        if (string.IsNullOrWhiteSpace(input)) return;

        var persona = await _personaRepository.ObtenerPorDniAsync(input);
        if (persona == null)
        {
            MessageBox.Show("No se encontró ninguna persona con ese DNI o Código en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        using var capturaForm = new CapturaHuellaForm();
        if (capturaForm.ShowDialog() == DialogResult.OK)
        {
            if (!string.IsNullOrEmpty(capturaForm.TemplateCapturado))
            {
                await _personaRepository.ActualizarHuellaAsync(persona.Id, capturaForm.TemplateCapturado);
                MessageBox.Show($"Huella actualizada correctamente para {persona.Nombres}.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
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

    private async void TxtEscaner_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;

            string texto = txtEscaner.Text.Trim();
            if (string.IsNullOrEmpty(texto))
                return;

            await ProcesarIngresoAsync(texto);
        }
    }

    private async Task ProcesarIngresoAsync(string texto)
    {
        txtEscaner.Text = texto;


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
                            
                            using var pagoForm = new PagoForm(tarifaService, pagoService, idPersona, 1); // 1 = idUsuario hardcodeado por ahora
                            
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
                finally
                {
                    // Limpiar y mantener foco
                    txtEscaner.Clear();
                    txtEscaner.Focus();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un ciclo primero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        } } 


















