using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Core;

namespace VirusExe.SystemBreach.Rendering
{
    // 공통 모달 UI
    // 상점/이벤트/결과창/확인창에서 재사용하는 고정 창 구조
    public partial class ConsoleRenderer
    {
        private const int ModalSmallWidth = 58; // 소형 모달 폭
        private const int ModalSmallBodyLines = 5; // 소형 모달 내용 줄
        private const int ModalMediumWidth = 76; // 중형 모달 폭
        private const int ModalMediumBodyLines = 11; // 중형 모달 내용 줄
        private const int ModalLargeWidth = 96; // 대형 모달 폭
        private const int ModalLargeBodyLines = 25; // 대형 모달 내용 줄

        private int modalLeft; // 모달 시작 X
        private int modalTop; // 모달 시작 Y
        private int modalWidth; // 모달 전체 폭
        private int modalInnerWidth; // 모달 내부 폭
        private int modalCursorTop; // 모달 현재 출력 Y
        private int modalBodyLines; // 모달 고정 내용 줄
        private int modalTotalHeight; // 모달 전체 높이
        private bool modalRedirectActive; // 기존 Write 계열 모달 리다이렉트
        private bool modalFooterWritten; // Footer 출력 여부
        private const string ModalWindowButtons = "-  []  x"; // ASCII 전용 창 버튼 장식
        private const int ModalWindowButtonRightPadding = 3; // 우측 버튼 여백

        private void BeginModal(string title, int width, int bodyLines)
        {
            ApplyFixedModalLayout(width, bodyLines); // 기존 숫자 호출 호환
            BeginModalCore(title); // 공통 시작 처리
        }

        private void BeginModal(string title, ModalSize size)
        {
            ApplyFixedModalLayout(size); // 명시 모달 크기 적용
            BeginModalCore(title); // 공통 시작 처리
        }

        private void BeginModalCore(string title)
        {
            modalInnerWidth = modalWidth - 2; // 좌우 프레임 제외
            modalTotalHeight = modalBodyLines + 4; // 기존 고정 전체 높이 유지
            modalLeft = Math.Max(0, (GameConfig.ConsoleWidth - modalWidth) / 2); // 중앙 X
            modalTop = Math.Max(0, (GameConfig.ConsoleHeight - modalTotalHeight) / 2); // 중앙 Y
            modalCursorTop = modalTop; // 출력 시작 위치
            modalFooterWritten = false; // Footer 상태 초기화

            Console.CursorVisible = false; // 커서 숨김
            DrawModalShadow(modalTotalHeight); // 그림자 출력
            WriteModalBorderLine("╔", "═", "╗", ConsoleColor.Cyan); // 상단 프레임
            WriteModalTitleLine(title); // 제목 + 창 버튼 장식
            WriteModalBorderLine("╠", "═", "╣", ConsoleColor.Cyan); // 헤더 구분선
        }

        private void ApplyFixedModalLayout(ModalSize size)
        {
            if (size == ModalSize.Large) // 대형 모달
            {
                modalWidth = Math.Min(ModalLargeWidth, GameConfig.ConsoleWidth - 4); // 대형 폭 고정
                modalBodyLines = ModalLargeBodyLines; // 대형 높이 고정
                return;
            }

            if (size == ModalSize.Medium) // 중형 모달
            {
                modalWidth = Math.Min(ModalMediumWidth, GameConfig.ConsoleWidth - 4); // 중형 폭 고정
                modalBodyLines = ModalMediumBodyLines; // 중형 높이 고정
                return;
            }

            modalWidth = Math.Min(ModalSmallWidth, GameConfig.ConsoleWidth - 4); // 소형 폭 고정
            modalBodyLines = ModalSmallBodyLines; // 소형 높이 고정
        }

        private void ApplyFixedModalLayout(int requestedWidth, int requestedBodyLines)
        {
            bool large = requestedWidth >= 90 || requestedBodyLines >= 16; // 대형 모달 조건
            bool medium = requestedWidth >= 70 || requestedBodyLines >= 8; // 중형 모달 조건

            if (large) // 대형 모달 체크
            {
                modalWidth = Math.Min(ModalLargeWidth, GameConfig.ConsoleWidth - 4); // 대형 폭 고정
                modalBodyLines = ModalLargeBodyLines; // 대형 높이 고정
                return;
            }

            if (medium) // 중형 모달 체크
            {
                modalWidth = Math.Min(ModalMediumWidth, GameConfig.ConsoleWidth - 4); // 중형 폭 고정
                modalBodyLines = ModalMediumBodyLines; // 중형 높이 고정
                return;
            }

            modalWidth = Math.Min(ModalSmallWidth, GameConfig.ConsoleWidth - 4); // 소형 폭 고정
            modalBodyLines = ModalSmallBodyLines; // 소형 높이 고정
        }

        private void EndModal()
        {
            int bottomLine = modalTop + modalTotalHeight - 1; // 하단 프레임 Y

            while (modalCursorTop < bottomLine) // Footer 없는 연출 잔여 영역 체크
            {
                WriteModalEmptyLine(); // 고정 높이 공백 채움
            }

            modalCursorTop = bottomLine; // 하단 프레임 위치 고정
            WriteModalBorderLine("╚", "═", "╝", ConsoleColor.Cyan); // 하단 프레임
            HideCursor(); // 커서 유배
        }

        private void DrawModalShadow(int totalHeight)
        {
            int shadowLeft = Math.Min(Console.BufferWidth - 1, modalLeft + 2); // 그림자 X
            int shadowTop = Math.Min(Console.BufferHeight - 1, modalTop + 1); // 그림자 Y
            int shadowWidth = Math.Min(modalWidth, Math.Max(0, Console.BufferWidth - shadowLeft)); // 그림자 폭

            Console.ForegroundColor = ConsoleColor.DarkGray;

            for (int y = 0; y < totalHeight; y++) // 우측 그림자
            {
                int targetY = shadowTop + y;
                if (targetY < 0 || targetY >= Console.BufferHeight) continue; // 범위 체크

                Console.SetCursorPosition(shadowLeft, targetY);
                Console.Write(new string(' ', shadowWidth)); // 배경 덮기
            }

            Console.ResetColor();
        }

        private void WriteModalBorderLine(string left, string middle, string right, ConsoleColor color)
        {
            if (!TrySetModalCursor()) return; // 위치 설정 체크

            SetColor(color);
            Console.Write(ApplyBattleGlitch(left + new string(middle[0], modalInnerWidth) + right)); // 프레임 출력
            Console.ResetColor();
            ClearModalLineRemainder(); // 오른쪽 잔상 제거
            modalCursorTop++; // 다음 줄
        }

        private void WriteModalEmptyLine()
        {
            WriteModalTextLine(string.Empty, ConsoleColor.DarkGray); // 빈 줄
        }

        private void WriteModalTitleLine(string title)
        {
            string buttons = ModalWindowButtons; // 우측 창 버튼 장식
            int buttonWidth = TextUtil.GetDisplayWidth(buttons); // 버튼 폭
            int gapWidth = 1; // 제목과 버튼 간격
            int rightPadding = ModalWindowButtonRightPadding; // 우측 여백
            int titleWidth = Math.Max(1, modalInnerWidth - buttonWidth - gapWidth - rightPadding); // 제목 영역 폭
            string fittedTitle = TextUtil.Fit(" " + title, titleWidth); // 제목 폭 보정

            WriteModalSegmentsLine(
                new ColorSegment(fittedTitle, ConsoleColor.Cyan),
                new ColorSegment(new string(' ', gapWidth), ConsoleColor.DarkGray),
                new ColorSegment(buttons, ConsoleColor.DarkGray),
                new ColorSegment(new string(' ', rightPadding), ConsoleColor.DarkGray)); // 버튼 장식 출력
        }


        private void WriteModalSeparator()
        {
            WriteModalBorderLine("╠", "═", "╣", ConsoleColor.Cyan); // 내부 구분선
        }

        private void WriteModalTextLine(string text, ConsoleColor color)
        {
            WriteModalSegmentsLine(new ColorSegment(TextUtil.Fit(text, modalInnerWidth), color)); // 단일 색상 줄
        }

        private void WriteModalCentered(string text, ConsoleColor color)
        {
            int width = TextUtil.GetDisplayWidth(text); // 텍스트 폭
            int left = Math.Max(0, (modalInnerWidth - width) / 2); // 중앙 여백
            WriteModalTextLine(new string(' ', left) + text, color); // 중앙 출력
        }

        private void WriteModalFooterText(string text, ConsoleColor color)
        {
            WriteModalFooter(new ColorSegment(" " + text, color)); // 단일 Footer 출력
        }

        private void WriteModalFooter(params ColorSegment[] segments)
        {
            List<ColorSegment> list = new List<ColorSegment>(); // Footer 세그먼트

            for (int i = 0; i < segments.Length; i++) // 세그먼트 복사
            {
                list.Add(segments[i]); // 세그먼트 추가
            }

            WriteModalFooter(list); // Footer 출력
        }

        private void WriteModalFooter(List<ColorSegment> segments)
        {
            int bottomLine = modalTop + modalTotalHeight - 1; // 하단 프레임 Y
            int footerSeparatorLine = Math.Max(modalTop + 3, bottomLine - 2); // Footer 구분선 Y
            int footerLine = Math.Max(modalTop + 4, bottomLine - 1); // Footer Y

            while (modalCursorTop < footerSeparatorLine) // Body 잔여 공간 체크
            {
                WriteModalEmptyLine(); // Body 내부 공백 채움
            }

            modalCursorTop = footerSeparatorLine; // Footer 구분선 위치
            WriteModalSeparator(); // Footer 구분선 출력
            modalCursorTop = footerLine; // Footer 줄 위치
            WriteModalSegmentsLine(segments); // Footer 1줄 출력
            modalFooterWritten = true; // Footer 출력 완료
        }

        private void WriteModalControlFooter(string executeText, string logMessage, ConsoleColor logColor)
        {
            List<ColorSegment> segments = new List<ColorSegment>(); // Footer 세그먼트
            segments.Add(new ColorSegment(" W/S", ConsoleColor.Cyan));
            segments.Add(new ColorSegment(" 이동   ", ConsoleColor.DarkGray));
            segments.Add(new ColorSegment("E", ConsoleColor.Green));
            segments.Add(new ColorSegment(" " + executeText + "   ", ConsoleColor.DarkGray));
            segments.Add(new ColorSegment("Q", ConsoleColor.Red));
            segments.Add(new ColorSegment(" 창닫기", ConsoleColor.DarkGray));

            if (!string.IsNullOrEmpty(logMessage)) // 로그 메시지 체크
            {
                segments.Add(new ColorSegment("   LOG: ", ConsoleColor.DarkGray));
                segments.Add(new ColorSegment(logMessage, logColor));
            }

            WriteModalFooter(segments); // Footer 1줄 출력
        }

        private void WriteModalSegmentsLine(params ColorSegment[] segments)
        {
            List<ColorSegment> list = new List<ColorSegment>(); // 세그먼트 목록

            for (int i = 0; i < segments.Length; i++) // 배열 복사
            {
                list.Add(segments[i]); // 세그먼트 추가
            }

            WriteModalSegmentsLine(list); // 목록 출력
        }

        private void WriteModalSegmentsLine(List<ColorSegment> segments)
        {
            int bottomLine = modalTop + modalTotalHeight - 1; // 하단 프레임 Y
            if (modalCursorTop >= bottomLine) // 내용 영역 초과 체크
            {
                modalCursorTop++; // 초과 줄 소모
                return;
            }

            if (!TrySetModalCursor()) return; // 위치 설정 체크

            SetColor(ConsoleColor.Cyan);
            Console.Write(ApplyBattleGlitch("║"));
            Console.ResetColor();

            int width = WriteSegments(segments, modalInnerWidth); // 내부 출력
            int remain = modalInnerWidth - width; // 남은 폭
            if (remain > 0) Console.Write(new string(' ', remain)); // 여백 채움

            SetColor(ConsoleColor.Cyan);
            Console.Write(ApplyBattleGlitch("║"));
            Console.ResetColor();
            ClearModalLineRemainder(); // 오른쪽 잔상 제거
            modalCursorTop++; // 다음 줄
        }

        private bool TrySetModalCursor()
        {
            try // 콘솔 위치 설정 실패 가능
            {
                int x = Math.Max(0, Math.Min(modalLeft, Console.BufferWidth - 1)); // X 보정
                int y = Math.Max(0, Math.Min(modalCursorTop, Console.BufferHeight - 1)); // Y 보정
                Console.SetCursorPosition(x, y); // 위치 이동
                return true;
            }
            catch
            {
                return false; // 위치 실패
            }
        }

        private void ClearModalLineRemainder()
        {
            int clearRight = Math.Min(Console.BufferWidth, modalLeft + modalWidth + 2); // 그림자 영역 끝
            int remain = clearRight - Console.CursorLeft; // 남은 폭
            if (remain > 0) Console.Write(new string(' ', remain)); // 오른쪽 그림자 제거
        }

        private void BeginModalRedirect(string title, int width, int bodyLines)
        {
            BeginModal(title, width, bodyLines); // 기존 숫자 호출 모달 시작
            modalRedirectActive = true; // 기존 Write 계열 모달 출력
        }

        private void BeginModalRedirect(string title, ModalSize size)
        {
            BeginModal(title, size); // 명시 크기 모달 시작
            modalRedirectActive = true; // 기존 Write 계열 모달 출력
        }

        private void EndModalRedirect()
        {
            modalRedirectActive = false; // 기존 Write 계열 원복
            EndModal(); // 모달 종료
        }

        public string ReadModalInputBox(string title, string[] lines, ConsoleColor color, string prompt)
        {
            int bodyLines = Math.Max(5, (lines == null ? 0 : lines.Length) + 4); // 입력 포함 높이
            BeginModal(title, 76, bodyLines); // 입력 모달 시작

            if (lines != null) // 안내 줄 체크
            {
                for (int i = 0; i < lines.Length; i++) // 안내 출력
                {
                    WriteModalTextLine(" " + lines[i], color); // 안내 줄
                }
            }

            WriteModalEmptyLine();
            string input = ReadModalInputLine(" " + prompt); // 입력 줄
            WriteModalFooterText("ENTER 입력", ConsoleColor.Green); // 입력 Footer
            EndModal(); // 입력 모달 종료
            return input == null ? string.Empty : input.Trim(); // 입력
        }

        private string ReadModalInputLine(string prompt)
        {
            if (!TrySetModalCursor()) return string.Empty; // 위치 설정 체크

            SetColor(ConsoleColor.Cyan);
            Console.Write(ApplyBattleGlitch("║"));
            Console.ResetColor();

            string safePrompt = TextUtil.Fit(prompt, Math.Min(modalInnerWidth - 1, TextUtil.GetDisplayWidth(prompt))); // 프롬프트 보정
            SetColor(ConsoleColor.Green);
            Console.Write(safePrompt); // 프롬프트 출력
            Console.ResetColor();

            int inputLeft = Console.CursorLeft; // 입력 시작 X
            int inputTop = Console.CursorTop; // 입력 시작 Y
            int remain = Math.Max(1, modalInnerWidth - TextUtil.GetDisplayWidth(safePrompt)); // 입력 가능 폭
            Console.Write(new string(' ', remain)); // 입력 영역 확보

            SetColor(ConsoleColor.Cyan);
            Console.Write(ApplyBattleGlitch("║"));
            Console.ResetColor();
            ClearModalLineRemainder(); // 오른쪽 잔상 제거

            Console.SetCursorPosition(inputLeft, inputTop); // 입력 위치 이동
            Console.CursorVisible = true; // 입력 커서 표시
            string input = Console.ReadLine(); // 입력 수신
            Console.CursorVisible = false; // 입력 커서 숨김
            modalCursorTop++; // 다음 줄
            return input; // 입력
        }

        private void WaitModalKey()
        {
            WaitModalInput(ModalInputMode.CloseOnly); // 기본 모달 닫기 대기
        }

        private ConsoleKey WaitModalInput(ModalInputMode mode)
        {
            HideCursor(); // 커서 유배

            while (true) // 모달 입력 대기
            {
                ConsoleKey key = Console.ReadKey(true).Key; // 키 입력

                if (mode == ModalInputMode.NextOnly && (key == ConsoleKey.E || key == ConsoleKey.Enter)) return key; // 다음
                if (mode == ModalInputMode.CloseOnly && key == ConsoleKey.Q) return key; // 창닫기
                if (mode == ModalInputMode.NextOrClose && (key == ConsoleKey.E || key == ConsoleKey.Enter || key == ConsoleKey.Q)) return key; // 다음/닫기
            }
        }
    }
}
