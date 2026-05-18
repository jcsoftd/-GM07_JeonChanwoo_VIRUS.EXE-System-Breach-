using System;
using System.Text;
using VirusExe.SystemBreach.Game;
using VirusExe.SystemBreach.Rendering;

namespace VirusExe.SystemBreach
{
    // 프로그램 시작점
    // 콘솔 세팅 후 GameManager로 게임 흐름 넘김
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            ConsoleRenderer.TrySetupConsole(); // 콘솔 창 크기를 게임 화면에 맞게 조정
            GameManager gameManager = new GameManager();
            gameManager.Run(); // 게임 루프
            Console.CursorVisible = true; // 게임이 끝난 뒤 커서를 다시 보이게
            Console.ResetColor(); // 콘솔 색상 기본값
        }
    }
}
