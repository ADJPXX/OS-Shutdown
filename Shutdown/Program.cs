using System.Diagnostics;
using System.Threading;

namespace ShutdownCSharp;

class Program
{
    enum Estado
    {
        Menu,
        Desligamento,
        Sair
    }
    
    static Estado estado = Estado.Menu;

    private static void Main(string[] args)
    {
        while (estado != Estado.Sair)
            switch (estado)
            {
                case Estado.Menu: Menu(); break;
                case Estado.Desligamento: Desligamento(); break;
            }
    }

    
    private static void Menu()
    {
        Console.Clear();
        while (true)
        {
            Console.Write("[1]COLOCAR UM TIMER PARA DESLIGAMENTO DO COMPUTADOR\n[2]CANCELAR UM TIMER ATIVO\n[3]VERIFICAR SE EXISTE UM TIMER ATIVO\n[0]SAIR DO PROGRAMA\n");
            int opcao = LerInt("SUA ESCOLHA: ");
            
            if (opcao == 0)
            {
                Console.Clear();
                Console.WriteLine("OBRIGADO POR USAR O PROGRAMA!");
                estado = Estado.Sair;
                return;
            }

            if (opcao == 1)
            {
                estado = Estado.Desligamento;
                return;
            }
            
            else if (opcao == 2)
            {
                Console.Clear();
                string comando = "-a";
                
                int exitCode = Timer(comando);
                
                if (exitCode == 1116)
                    Console.WriteLine("NÃO EXISTE UM TIMER ATIVO.");
                else
                    Console.WriteLine("TIMER DESATIVADO.");
            }
            
            else if (opcao == 3)
            {
                Console.Clear();
                string comando = "-s -t 86400";
                int exitCode = Timer(comando);

                if (exitCode == 1190)
                    Console.WriteLine("UM TIMER ESTÁ ATIVO!");
                else
                {
                    comando = "-a";
                    Timer(comando);
                    Console.WriteLine(
                        "NÃO TINHA UM TIMER ATIVO, PARA VERIFICAÇÃO FOI ADICIONADO UM TIMER DE 1 DIA E LOGO APÓS CANCELADO");
                }
            }
        }
    }

    
    private static int Timer(string comando)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();

            Process process = new Process();

            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.FileName = "shutdown.exe";
            startInfo.Arguments = $"{comando}";

            process = Process.Start(startInfo);

            process.WaitForExit();

            int exitCode = process.ExitCode;

            return exitCode;
        }
        
        catch (Exception ex)
        {
            Console.WriteLine($"ERRO: {ex.Message}");
            return -1;
        }
    }

    
    private static void Desligamento()
    {
        int horas = -1;
        int minutos = -1;
        int segundos = -1;
        
        Console.Clear();
                
        while (horas < 0)
        { 
            horas = LerInt("DIGITE A QUANTIDADE DE HORAS PARA O COMPUTADOR DESLIGAR: ");
        }

        while (minutos < 0)
        {
            minutos = LerInt("DIGITE A QUANTIDADE DE MINUTOS PARA O COMPUTADOR DESLIGAR: ");
        }

        while (segundos < 0)
        {
            segundos = LerInt("DIGITE A QUANTIDADE DE SEGUNDOS PARA O COMPUTADOR DESLIGAR: ");
        }
        
        int total = (horas * 3600) + (minutos * 60) + segundos;
        
        if (segundos >= 60)
        {
            int inteiro = segundos / 60;
            int resto = segundos % 60;
            
            minutos += inteiro;
            segundos = resto;
        }
        
        if (minutos >= 60)
        {
            int inteiro = minutos / 60;
            int resto = minutos % 60;
            
            horas += inteiro;
            minutos = resto;
        }

        if (horas == 0 && minutos == 0 && segundos == 0)
        {
            Console.Clear();
            estado = Estado.Menu;
            return;
        }
        
        string comando = $"-s -t {total}";

        int exitCode = Timer(comando);

        if (exitCode == 1190)
        {
            Console.WriteLine("NÃO FOI POSSÍVEL COLOCAR O TIMER POIS JÁ EXISTE UM TIMER ATIVO");
            estado = Estado.Menu;
            return;
        }

        else if (exitCode != 1190)
        {
            Console.Clear();
            TelaDesligamento(horas, minutos, segundos);
        }
    }


    private static void TelaDesligamento(int horas, int minutos, int segundos)
    {
        while (true)
        {
            var total = (horas * 3600) + (minutos * 60) + segundos; 
            
            var agora = DateTime.Now;

            var desligamento = agora.AddSeconds(total);

            var horario = desligamento.ToString("HH:mm:ss");

            Console.WriteLine("APERTE C PARA SAIR DESSA TELA!");
            
            Console.WriteLine($"O SISTEMA IRÁ DESLIGAR AS: {horario}");
            
            Console.WriteLine($"DESLIGANDO EM: {horas:00}:{minutos:00}:{segundos:00}");

            if (Console.KeyAvailable)
            {
                if (Console.ReadKey().Key == ConsoleKey.C)
                {
                    estado = Estado.Menu;
                    return;
                }
            }

            if (segundos > 0)
                segundos -= 1;

            else if (minutos > 0)
            {
                minutos -= 1;
                segundos = 59;
            }
            
            else if (horas > 0)
            {
                horas -= 1;
                segundos = 59;
                minutos = 59;
            }

            Thread.Sleep(1000);
            Console.SetCursorPosition(0, 0);

            if (horas == 0 && minutos == 0 && segundos == 0)
            {
                estado = Estado.Menu;
                return;
            }
        }
    }
    
    
    private static int LerInt(string msg)
    {
        while (true)
        {
            Console.Write(msg);
            if (int.TryParse(Console.ReadLine().Trim(), out int inteiro))
            {
                return inteiro;
            }
        }
    }
}