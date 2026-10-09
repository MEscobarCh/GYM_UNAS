using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Gym.Core.Entities;
using Gym.Core.Interfaces;
using DPFP;
using DPFP.Capture;
using DPFP.Processing;
using DPFP.Verification;

namespace Gym.Infrastructure.Services;

public class DigitalPersonaClient : IBiometricoClient, DPFP.Capture.EventHandler
{
    private Capture? _capturer;
    private Verification? _verificator;
    private TaskCompletionSource<FeatureSet?>? _authTcs;

    public void InicializarLector()
    {
        try
        {
            _capturer = new Capture();
            _verificator = new Verification();
            _capturer.EventHandler = this;
            _capturer.StartCapture();
            Console.WriteLine("Lector DigitalPersona inicializado (Real).");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al inicializar lector en DigitalPersonaClient: " + ex.Message);
            throw;
        }
    }

    public void DetenerLector()
    {
        try
        {
            if (_capturer != null)
            {
                _capturer.StopCapture();
                _capturer.Dispose();
                _capturer = null;
            }
            Console.WriteLine("Lector DigitalPersona detenido.");
        }
        catch { }
    }

    public Task<string?> IniciarCapturaAsync()
    {
        // En esta arquitectura, la captura para enrolamiento se maneja exclusivamente 
        // a través de CapturaHuellaForm. Esta clase se encarga de la identificación.
        // Si se requiere un modo captura simple, puede implementarse devolviendo un base64 ficticio o nulo.
        return Task.FromResult<string?>(null);
    }

    public async Task<Persona?> IdentificarPersonaAsync(IEnumerable<Persona> personasConHuella)
    {
        try
        {
            InicializarLector();
            
            // Creamos la promesa para esperar la respuesta del evento OnComplete
            _authTcs = new TaskCompletionSource<FeatureSet?>();
            
            // Esperamos asíncronamente a que el usuario ponga el dedo en el lector
            FeatureSet? featureSet = await _authTcs.Task;
            DetenerLector();

            if (featureSet == null) return null;

            // Iteramos sobre las huellas guardadas en la base de datos
            foreach (var persona in personasConHuella)
            {
                if (string.IsNullOrEmpty(persona.Huella)) continue;

                try
                {
                    byte[] templateBytes = Convert.FromBase64String(persona.Huella);
                    using (MemoryStream ms = new MemoryStream(templateBytes))
                    {
                        Template templateBD = new Template(ms);

                        Verification.Result result = new Verification.Result();
                        _verificator?.Verify(featureSet, templateBD, ref result);

                        if (result.Verified)
                        {
                            return persona;
                        }
                    }
                }
                catch
                {
                    // Ignorar errores de un template particular corrupto y continuar con el siguiente
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error crítico al identificar persona: " + ex.Message);
        }
        finally
        {
            DetenerLector();
        }
        
        return null; // No match encontrado
    }

    public void OnComplete(object Capture, string ReaderSerialNumber, Sample Sample)
    {
        FeatureExtraction extractor = new FeatureExtraction();
        CaptureFeedback feedback = CaptureFeedback.None;
        FeatureSet features = new FeatureSet();
        
        // Extraer características con propósito explícito de Verificación (Comparación 1:N / 1:1)
        extractor.CreateFeatureSet(Sample, DataPurpose.Verification, ref feedback, ref features);

        if (feedback == CaptureFeedback.Good)
        {
            // Completar la tarea de identificación con éxito
            _authTcs?.TrySetResult(features);
        }
        else
        {
            // Completar con nulo si la muestra es de mala calidad
            _authTcs?.TrySetResult(null);
        }
    }

    public void OnFingerGone(object Capture, string ReaderSerialNumber) { }
    
    public void OnFingerTouch(object Capture, string ReaderSerialNumber) { }
    
    public void OnReaderConnect(object Capture, string ReaderSerialNumber) 
    {
        Console.WriteLine($"Lector conectado: {ReaderSerialNumber}");
    }
    
    public void OnReaderDisconnect(object Capture, string ReaderSerialNumber) 
    {
        Console.WriteLine($"Lector desconectado: {ReaderSerialNumber}");
    }
    
    public void OnSampleQuality(object Capture, string ReaderSerialNumber, CaptureFeedback CaptureFeedback) { }
}
