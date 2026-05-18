using System;

namespace VirusExe.SystemBreach.Rendering
{
    // 미니게임 모달 출력 보조
    // 미니게임 공통 화면 틀과 결과창 출력
    public partial class ConsoleRenderer
    {
        public void RenderMiniGameModal(string title, string[] lines, string footer)
        {
            int lineCount = lines == null ? 0 : lines.Length; // 본문 줄 수
            int bodyHint = Math.Max(18, lineCount + 3); // 대형 모달 높이 힌트

            BeginModal(title, bodyHint >= 16 ? ModalSize.Large : ModalSize.Medium); // 기존 모달 시작

            if (lines != null) // 본문 존재 체크
            {
                for (int i = 0; i < lines.Length; i++) // 본문 출력
                {
                    WriteModalTextLine(" " + lines[i], GetMiniGameLineColor(lines[i])); // 기존 모달 라인 색상 적용
                }
            }

            WriteMiniGameFooter(footer); // 기존 모달 Footer 출력
            EndModal(); // 기존 모달 종료
            HideCursor(); // 커서 유배
        }



        private ConsoleColor GetMiniGameLineColor(string line)
        {
            if (string.IsNullOrEmpty(line)) return ConsoleColor.DarkGray; // 빈 줄

            if (line.StartsWith("FOLDER")) return ConsoleColor.Magenta; // 폴더 경로
            if (line.StartsWith("TARGET")) return ConsoleColor.Yellow; // 목표 정보
            if (line.StartsWith("STATUS")) return ConsoleColor.Red; // 상태 정보
            if (line.StartsWith("SYSTEM LOG")) return ConsoleColor.Cyan; // 시스템 로그 제목
            if (line.StartsWith("SYSTEM LOG:")) return ConsoleColor.Yellow; // 로그 줄
            if (line.StartsWith("SYNC LOCK")) return ConsoleColor.Green; // 성공 카운트
            if (line.StartsWith("DRIFT")) return ConsoleColor.Red; // 실패 카운트
            if (line.StartsWith("TIME LEFT")) return ConsoleColor.Yellow; // 시간
            if (line.StartsWith("DELETED")) return ConsoleColor.Green; // 삭제 상태
            if (line.StartsWith("목표")) return ConsoleColor.Yellow; // 목표 안내

            if (line.IndexOf('═') >= 0) return ConsoleColor.Cyan; // 내부 구분선
            if (line.StartsWith("[") && line.EndsWith("]")) return ConsoleColor.DarkGray; // 플레이 영역 경계
            if (line.IndexOf("===") >= 0 || line.IndexOf("========") >= 0) return ConsoleColor.Green; // SAFE RANGE
            if (line.IndexOf("^^^") >= 0) return ConsoleColor.Yellow; // DELETE CANNON
            if (line.IndexOf('^') >= 0) return ConsoleColor.Yellow; // SIGNAL 포인터
            if (line.IndexOf('|') >= 0) return ConsoleColor.Cyan; // 삭제빔
            if (line.IndexOf("[DELETED]") >= 0) return ConsoleColor.Green; // 삭제 완료
            if (line.IndexOf('▓') >= 0 || line.IndexOf('█') >= 0 || line.IndexOf('#') >= 0) return ConsoleColor.Magenta; // 모자이크/파괴 문자
            if (line.IndexOf(".mp4") >= 0 || line.IndexOf(".zip") >= 0 || line.IndexOf(".dat") >= 0 || line.IndexOf(".exe") >= 0 || line.IndexOf(".bin") >= 0 || line.IndexOf(".jpg") >= 0) return ConsoleColor.White; // 파일명

            return ConsoleColor.Gray; // 기본 본문
        }

        private void WriteMiniGameFooter(string footer)
        {
            WriteModalFooter(
                new ColorSegment(" " + footer, ConsoleColor.DarkGray)); // 미니게임 Footer 1줄
        }
    }
}
