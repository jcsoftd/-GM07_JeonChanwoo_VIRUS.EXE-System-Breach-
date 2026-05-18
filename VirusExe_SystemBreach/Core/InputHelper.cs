using System;

namespace VirusExe.SystemBreach.Core
{
    public static class InputHelper // 키 입력 관련
    {
        // 공통 키 입력 헬퍼 선택형 UI에서 입력을 한 번씩 깔끔하게 받기 위한 용도
        public static ConsoleKey ReadKey()
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true); // 키 입력을 화면에 표시하지 않고 읽음
            return keyInfo.Key; // 읽은 키
        }

    }
}
