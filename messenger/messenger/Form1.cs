using System.Net.Sockets;
using System.Text;

namespace messenger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var name = userName.Text;
            var password = userPassword.Text;

            using TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 8888);

            var message = "1 " + name + " " + password+"\n";
            // считыванием строку в массив байт
            byte[] requestData = Encoding.UTF8.GetBytes(message);
            // получаем NetworkStream для взаимодействия с сервером
            var stream = tcpClient.GetStream();
            // отправляем данные
            await stream.WriteAsync(requestData);
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            using TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 8888);

            var name = Name2.Text;
            var password = Password2.Text;

            var stream = tcpClient.GetStream();

            // буфер для входящих данных
            var response = new List<byte>();
            int bytesRead = 10; // для считывания байтов из потока

            byte[] data = Encoding.UTF8.GetBytes("2 "+ name + " " + password + '\n');
            await stream.WriteAsync(data);

            while ((bytesRead = stream.ReadByte()) != '\n')
            {
                // добавляем в буфер
                response.Add((byte)bytesRead);
            }

            var translation = Encoding.UTF8.GetString(response.ToArray());
            MessageBox.Show(translation);
            response.Clear();
        }
    }
}