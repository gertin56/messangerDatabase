using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Transactions;

var tcpListener = new TcpListener(IPAddress.Any, 8888);
var database = new Database();

try
{
    tcpListener.Start();    // запускаем сервер
    Console.WriteLine("Сервер запущен. Ожидание подключений... ");

    while (true)
    {
        // получаем подключение в виде TcpClient
        using var tcpClient = await tcpListener.AcceptTcpClientAsync();
        // получаем объект NetworkStream для взаимодействия с клиентом
        var stream = tcpClient.GetStream();
        // буфер для входящих данных
        var response = new List<byte>();
        int bytesRead = 10;
        while (true)
        {
            // считываем данные до конечного символа
            while ((bytesRead = stream.ReadByte()) != '\n')
            {
                // добавляем в буфер
                response.Add((byte)bytesRead);
            }
            var request = Encoding.UTF8.GetString(response.ToArray());

            var words = request.Split(' ');

            if (words[0] == "1")
            {
                var code = database.addUser(words[1], words[2]);
                Console.WriteLine(code);
            }
            else if (words[0] == "2")
            {
                var code = database.autorize(words[1], words[2]);
                if(code == 1)
                {
                    string answer = "Вы успешно вошли" + "\n";
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(answer));
                }
                else
                {
                    string answer = "Ошибка" + "\n";
                    await stream.WriteAsync(Encoding.UTF8.GetBytes(answer));
                }
            }
            
            response.Clear();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    tcpListener.Stop(); // останавливаем сервер
}

class Database
{
    private readonly string connectionString = "server=localhost;user=root;database=messenger;password=lokobol2019;";

    public int addUser(string name, string password)
    {
        MySqlConnection connection = new MySqlConnection(connectionString);
        connection.Open();
        string query = $"INSERT INTO user (name, password) VALUES ('{name}', '{password}')";
        MySqlCommand command = new MySqlCommand(query, connection);
        var code = command.ExecuteNonQuery();
        connection.Close();
        Console.WriteLine("Пользователь зарегистрирован");
        return code;
    }

    public int autorize(string name, string password)
    {
        MySqlConnection conn = new MySqlConnection(connectionString);
        conn.Open();
        string sql = $"select * from user where name = '{name}' and password = '{password}'";
        MySqlCommand command = new MySqlCommand(sql, conn);
        if(command.ExecuteScalar() != null)
        {
            conn.Close();
            return 1;    
        }
        else
        {
            conn.Close();
            return -1;
        }
    }
}