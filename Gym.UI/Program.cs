using Gym.Core.Interfaces;
using Gym.Core.Services;
using Gym.Infrastructure.Repositories;
using Gym.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows.Forms;

namespace Gym.UI;

static class Program
{
    [STAThread]
    static void Main()
    {
        try
        {
            ApplicationConfiguration.Initialize();

            // Configuración de appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 1. Instanciar ServiceCollection
            var services = new ServiceCollection();
            
            services.AddSingleton<IConfiguration>(configuration);

            // 2. Obtener la cadena de conexión
            string connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no fue encontrada en appsettings.json.");

            // 3. Registrar dependencias (Inversión de Control)
            // La UI solo conoce interfaces (abstracciones), mientras que el contenedor inyecta las implementaciones reales.
            services.AddTransient<IUsuarioRepository>(provider => new UsuarioRepository(connectionString));
            services.AddTransient<IPasswordHasher, BcryptPasswordHasher>();
            services.AddTransient<AuthService>();

            // Registros para la HU-02 y HU-03
            services.AddTransient<ICicloRepository>(provider => new CicloRepository(connectionString));
            services.AddTransient<IOcdaClient, Gym.Infrastructure.Services.OcdaClientSimulado>();
            services.AddTransient<CicloService>();

            // Registros para la HU-04 (Configuración)
            services.AddTransient<IConfigRepository>(provider => new ConfigRepository(connectionString));
            services.AddTransient<ConfigService>();

            // Registros para la HU-05 (Validar Ingreso)
            services.AddTransient<IPersonaRepository>(provider => new PersonaRepository(connectionString));
            services.AddTransient<IMatriculaRepository>(provider => new MatriculaRepository(connectionString));
            services.AddTransient<IIngresoRepository>(provider => new IngresoRepository(connectionString));
            services.AddTransient<IAuditoriaRepository>(provider => new AuditoriaRepository(connectionString));
            services.AddTransient<ClasificacionService>();
            services.AddTransient<IngresoService>();

            // Registros para la HU-06 y HU-07 (Tarifas y Pagos)
            services.AddTransient<ITarifaRepository>(provider => new TarifaRepository(connectionString));
            services.AddTransient<IPagoRepository>(provider => new PagoRepository(connectionString));
            services.AddTransient<ISuscripcionRepository>(provider => new SuscripcionRepository(connectionString));
            services.AddTransient<SuscripcionService>();
            services.AddTransient<PagoService>();
            services.AddTransient<TarifaService>();

            // Registro Biometrico
            services.AddSingleton<IBiometricoClient, Gym.Infrastructure.Services.DigitalPersonaClient>();
            services.AddTransient<EnrolamientoForm>();

            // 4. Registrar formularios
            services.AddTransient<LoginForm>();
            services.AddTransient<DashboardForm>();
            services.AddTransient<ConfigForm>();
            services.AddTransient<PagoForm>();
            services.AddTransient<RegistroExternoForm>();

            // HTTP y Cliente de RENIEC (Sprint 4)
            services.AddHttpClient();
            services.AddTransient<IReniecClient, Gym.Infrastructure.Services.DecolectaReniecClient>();
            
            // Sincronización masiva (HU-09)
            services.AddTransient<ISincronizacionService, SincronizacionService>();

            // Construir el ServiceProvider
            using var serviceProvider = services.BuildServiceProvider();

            // 5. Iniciar la aplicación resolviendo el LoginForm desde el contenedor DI
            var loginForm = serviceProvider.GetRequiredService<LoginForm>();
            Application.Run(loginForm);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error fatal al iniciar la aplicación:\n\n{ex.Message}\n\n{ex.StackTrace}", "Error de Arranque", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }    
}

