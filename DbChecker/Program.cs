using System;
using MySqlConnector;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "Server=localhost;Database=gym_unas;Uid=root;Pwd=admin123;";
        using var connection = new MySqlConnection(connectionString);
        await connection.OpenAsync();

        try {
            using var cmd1 = new MySqlCommand("ALTER TABLE ingreso DROP FOREIGN KEY ingreso_fk_suscripcion;", connection);
            await cmd1.ExecuteNonQueryAsync();
            Console.WriteLine("FK dropped.");
        } catch (Exception ex) { Console.WriteLine(ex.Message); }

        try {
            using var cmd2 = new MySqlCommand("ALTER TABLE ingreso DROP COLUMN id_ciclo, DROP COLUMN validacion, DROP COLUMN id_suscripcion, DROP COLUMN id_usuario;", connection);
            await cmd2.ExecuteNonQueryAsync();
            Console.WriteLine("Columns dropped.");
        } catch (Exception ex) { Console.WriteLine(ex.Message); }

        try {
            using var cmd3 = new MySqlCommand("DROP TABLE suscripcion;", connection);
            await cmd3.ExecuteNonQueryAsync();
            Console.WriteLine("suscripcion table dropped.");
        } catch (Exception ex) { Console.WriteLine(ex.Message); }
    }
}
