using System;

namespace VirusExe.SystemBreach.Core
{
    // 콘솔 문자열 폭 계산 유틸
    // 한글/박스문자/ASCII가 섞여도 UI 정렬
    public static class TextUtil
    {
        public static string Fit(string text, int width)
        {
            if (text == null) 
                text = string.Empty;

            int displayWidth = GetDisplayWidth(text); // 문자열의 콘솔 표시 폭을 계산
            if (displayWidth == width) // 표시 폭이 정확히 맞는지 체크
                return text; // 그대로

            if (displayWidth < width) // 표시 폭이 부족한지 체크
                return text + new string(' ', width - displayWidth); // 부족한 만큼 공백을 붙

            string result = string.Empty; // 잘라낸 결과를 저장할 문자열
            int currentWidth = 0; // 현재까지 누적된 표시 폭

            for (int i = 0; i < text.Length; i++) // 문자열의 각 글자를 순회
            {
                int charWidth = GetCharWidth(text[i]); // 현재 글자의 표시 폭을 계산
                if (currentWidth + charWidth > width) // 글자 추가 폭 넘는지 체크
                    break;

                result += text[i]; // 결과 문자열에 현재 글자를 추가
                currentWidth += charWidth; // 표시 폭을 누적
            }
            return result + new string(' ', Math.Max(0, width - currentWidth)); // 남은 폭을 공백으로 채워
        }

        public static int GetDisplayWidth(string text)
        {
            if (string.IsNullOrEmpty(text)) // 문자열이 비어있는지 체크
                return 0; 

            int width = 0; // 누적 표시 폭 변수
            for (int i = 0; i < text.Length; i++) // 문자열을 한 글자씩 순회
                width += GetCharWidth(text[i]); // 글자 폭을 누적

            return width;
        }

        public static int GetCharWidth(char c)
        {
            int code = c; // 문자 코드를 정수로 변환
            if (code >= 0x1100 && code <= 0x11FF) return 2; // 한글 자모는 2칸으로 처리
            if (code >= 0x2E80 && code <= 0x2EFF) return 2; // CJK 부수 영역은 2칸으로 처리
            if (code >= 0x3000 && code <= 0x303F) return 2; // CJK 구두점 영역은 2칸으로 처리
            if (code >= 0x3040 && code <= 0x30FF) return 2; // 히라가나/가타카나는 2칸으로 처리
            if (code >= 0x3130 && code <= 0x318F) return 2; // 한글 호환 자모는 2칸으로 처리
            if (code >= 0x4E00 && code <= 0x9FFF) return 2; // 한자 영역은 2칸으로 처리
            if (code >= 0xAC00 && code <= 0xD7AF) return 2; // 한글 완성형은 2칸으로 처리
            if (code >= 0xFF00 && code <= 0xFFEF) return 2; // 전각 문자는 2칸으로 처리
            return 1; // 그 외 문자는 1칸으로 처리
        }
    }
}
