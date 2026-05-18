using System;
using System.Runtime.InteropServices;

namespace VirusExe.SystemBreach.Rendering
{
    // 공통 창 보조 구조
    // 윈도우/모달 테두리 계산에 쓰는 작은 데이터 구조
    public partial class ConsoleRenderer
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        private static void TryResizeConsoleWindowPixels(int width, int height)
        {
            try // Windows 콘솔 창 핸들 필요
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return; // Windows 환경 체크

                IntPtr handle = GetConsoleWindow(); // 콘솔 창 핸들
                if (handle == IntPtr.Zero) return; // 핸들 없음 체크

                MoveWindow(handle, 40, 40, width, height, true); // 픽셀 창 크기 적용
            }
            catch
            {
            }
        }

        private static void TryScrollConsoleToTop()
        {
            try // 스크롤 이동 실패 가능
            {
                Console.SetWindowPosition(0, 0); // 스크롤 맨 위 이동
            }
            catch
            {
            }
        }
    }
}
