using System;
using System.Collections.Generic;
using System.Threading;
using VirusExe.SystemBreach.Core;
using VirusExe.SystemBreach.Rendering;

namespace VirusExe.SystemBreach.Systems
{
    // 미니게임 실행
    // SECURITY CODE, SIGNAL SYNC, FILE PURGE 로직 관리
    public class MiniGameManager
    {
        private readonly Random random = new Random(); // 정답/신호 생성용 Random
        private readonly ConsoleRenderer renderer; // 결과 모달 출력용 렌더러

        private class FallingFileTarget
        {
            public int X; // 파일 X 위치
            public int Y; // 파일 Y 위치
            public string Name; // 원본 파일명
            public string DisplayName; // 화면 출력용 모자이크 파일명
        }

        private class PurgeDeletionEffect
        {
            public int X; // 삭제 연출 X 위치
            public int Y; // 삭제 연출 Y 위치
            public int Frame; // 삭제 연출 프레임
            public string DisplayName; // 삭제 연출 기준 파일명
        }

        private class PurgeBullet
        {
            public int X; // 삭제빔 X 위치
            public int Y; // 삭제빔 Y 위치
        }

        public MiniGameManager(ConsoleRenderer renderer)
        {
            this.renderer = renderer;
        }

        public void ShowMiniGameTestMenu()
        {
            while (true) // 미니게임 테스트 메뉴 유지
            {
                int input = renderer.ShowSelectionModal("MINI GAME LAB", new string[] // 기존 모달 선택창 사용
                {
                    "랜덤 이벤트에서 등장하는 미니게임을 테스트합니다.",
                    "선택한 미니게임은 보상 없이 실행됩니다.",
                }, new string[]
                {
                    "SECURITY CODE BREACH    :: 1~5 보안코드 해독",
                    "SIGNAL SYNC             :: 불안정 신호 동기화",
                    "SUSPICIOUS CACHE PURGE  :: 수상한 파일 삭제",
                    "BACK                    :: 타이틀 메뉴로 복귀"
                }, ConsoleColor.Gray, 3); // Q는 BACK 처리

                if (input == 0) // 보안코드 선택 체크
                {
                    RunBackupRecoveryGame(); // 보안코드 미니게임 실행
                    continue; // 메뉴 복귀
                }

                if (input == 1) // SIGNAL SYNC 선택 체크
                {
                    RunSignalSyncGame(); // SIGNAL SYNC 실행
                    continue; // 메뉴 복귀
                }

                if (input == 2) // 파일 삭제 선택 체크
                {
                    RunFilePurgeGame(); // 파일 삭제 미니게임 실행
                    continue; // 메뉴 복귀
                }

                return; // 타이틀 메뉴 복귀
            }
        }

        public bool RunBackupRecoveryGame()
        {
            string answer = CreateAnswer(); // 1~5 보안코드 생성
            int maxTry = 5; // 최대 시도 횟수
            string lastResult = "SECURITY CODE MATRIX 대기 중"; // 최근 판정 로그

            for (int turn = 1; turn <= maxTry; turn++) // 제한 횟수 반복
            {
                string input = ReadSecurityCodeInput(turn, maxTry, lastResult); // 고정 프레임 입력

                if (input == "__CANCEL__") // 취소 입력 체크
                {
                    ShowMiniResult("DECRYPTION ABORTED", new string[] { "보안코드 해독을 중단했습니다." }, ConsoleColor.Red); // 취소 표시
                    return false;
                }

                if (!IsValidGuess(input)) // 입력 검증
                {
                    lastResult = "ERROR : 1~5 사이의 서로 다른 숫자 3개를 입력"; // 오류 로그
                    ShowMiniResult("INPUT ERROR", new string[] { "서로 다른 숫자 3개를 입력해야 합니다.", "사용 가능 숫자 : 1 2 3 4 5" }, ConsoleColor.Red); // 오류 표시
                    turn--; // 잘못된 입력은 횟수 제외
                    continue; // 재입력
                }

                int strike = 0; // 자리까지 일치
                int ball = 0; // 숫자만 일치

                for (int i = 0; i < 3; i++) // 3자리 비교
                {
                    if (input[i] == answer[i]) // 자리/숫자 일치
                    {
                        strike++; // 스트라이크 증가
                    }
                    else if (answer.Contains(input[i].ToString())) // 숫자 포함
                    {
                        ball++; // 볼 증가
                    }
                }

                if (strike == 3) // 보안코드 해독 성공
                {
                    ShowMiniResult("SECURITY CODE BREACH", new string[] { "ACCESS KEY 해독 완료", "BACKUP DATA 탈취 성공" }, ConsoleColor.Green); // 성공 표시
                    return true;
                }

                lastResult = "SCAN RESULT : " + strike + " STRIKE / " + ball + " BALL"; // 결과 로그
                ShowMiniResult("DECRYPTION TRACE", new string[] { input + "  =>  " + lastResult, "남은 시도 : " + (maxTry - turn) }, ConsoleColor.Yellow); // 힌트 표시
            }

            ShowMiniResult("SECURITY CODE LOCKED", new string[] { "보안코드 해독 실패", "BACKUP DATA 접근이 차단되었습니다." }, ConsoleColor.Red); // 실패 표시
            return false;
        }

        public bool RunSignalSyncGame()
        {
            int success = 0; // 동기화 성공 횟수
            int fail = 0; // 동기화 실패 횟수
            int marker = 0; // 신호 마커 위치
            int direction = 1; // 이동 방향
            int trackLength = 50; // 신호 트랙 길이
            int safeWidth = 8; // SAFE RANGE 폭
            int safeStart = random.Next(12, 31); // SAFE RANGE 시작 위치
            int timeLimit = 25; // 제한 시간 초
            DateTime startedAt = DateTime.Now; // 시작 시각
            string log = "불안정한 신호를 SAFE RANGE에 고정하십시오."; // 상태 로그

            Console.CursorVisible = false; // 커서 숨김

            while (success < 3 && fail < 3) // 성공/실패 조건 전까지 반복
            {
                int elapsed = (int)(DateTime.Now - startedAt).TotalSeconds; // 경과 시간
                int remain = timeLimit - elapsed; // 남은 시간

                if (remain <= 0) // 시간 초과
                {
                    ShowMiniResult("SIGNAL SYNC FAILED", new string[] { "동기화 시간이 초과되었습니다.", "SIGNAL LOCK 실패" }, ConsoleColor.Red); // 실패 표시
                    return false;
                }

                RenderSignalSyncFrame(marker, safeStart, safeWidth, trackLength, success, fail, remain, log); // 현재 프레임 출력

                if (Console.KeyAvailable) // 입력 체크
                {
                    ConsoleKey key = Console.ReadKey(true).Key; // 키 수신

                    if (key == ConsoleKey.Q) // 포기 입력
                    {
                        ShowMiniResult("SIGNAL SYNC ABORTED", new string[] { "동기화를 중단했습니다." }, ConsoleColor.Red); // 중단 표시
                        return false;
                    }

                    if (key == ConsoleKey.E || key == ConsoleKey.Enter) // 동기화 입력
                    {
                        if (marker >= safeStart && marker < safeStart + safeWidth) // SAFE RANGE 체크
                        {
                            success++; // 성공 증가
                            log = "SYNC LOCKED : " + success + " / 3"; // 성공 로그
                            safeStart = random.Next(10, 33); // 다음 SAFE RANGE
                            marker = random.Next(0, trackLength); // 마커 재배치
                            direction = random.Next(0, 2) == 0 ? -1 : 1; // 방향 재설정
                        }
                        else
                        {
                            fail++; // 실패 증가
                            log = "SIGNAL DRIFT : " + fail + " / 3"; // 실패 로그
                        }

                        RenderSignalSyncFrame(marker, safeStart, safeWidth, trackLength, success, fail, remain, log); // 판정 후 출력
                        Thread.Sleep(220); // 판정 피드백 유지
                    }
                }

                marker += direction; // 마커 이동

                if (marker <= 0) // 왼쪽 끝 체크
                {
                    marker = 0; // 위치 보정
                    direction = 1; // 오른쪽 이동
                }
                else if (marker >= trackLength - 1) // 오른쪽 끝 체크
                {
                    marker = trackLength - 1; // 위치 보정
                    direction = -1; // 왼쪽 이동
                }

                Thread.Sleep(45); // 이동 속도
            }

            if (success >= 3) // 성공 조건 체크
            {
                ShowMiniResult("SIGNAL SYNC COMPLETE", new string[] { "신호 동기화 완료", "TRACE 간섭 신호를 역추적했습니다." }, ConsoleColor.Green); // 성공 표시
                return true;
            }

            ShowMiniResult("SIGNAL SYNC FAILED", new string[] { "신호 동기화에 실패했습니다.", "불안정한 패킷이 확산되었습니다." }, ConsoleColor.Red); // 실패 표시
            return false;
        }

        private string CreateAnswer()
        {
            string answer = string.Empty; // 정답 문자열

            while (answer.Length < 3) // 3자리 생성
            {
                char digit = (char)('1' + random.Next(0, 5)); // 1~5 숫자 생성

                if (!answer.Contains(digit.ToString())) // 중복 체크
                {
                    answer += digit; // 정답 추가
                }
            }

            return answer; // 정답
        }

        private bool IsValidGuess(string input)
        {
            if (input == null || input.Length != 3) // 길이 체크
            {
                return false; // 유효하지 않음
            }

            for (int i = 0; i < input.Length; i++) // 각 자리 체크
            {
                if (input[i] < '1' || input[i] > '5') // 1~5 범위 체크
                {
                    return false; // 범위 오류
                }
            }

            return input[0] != input[1] && input[0] != input[2] && input[1] != input[2]; // 중복 없음 체크
        }

        private string ReadSecurityCodeInput(int turn, int maxTry, string log)
        {
            string input = string.Empty; // 현재 입력값

            while (true) // 입력 루프
            {
                RenderSecurityCodeFrame(turn, maxTry, input, log); // 입력 화면 출력
                Console.CursorVisible = true; // 입력 커서 표시
                ConsoleKeyInfo key = Console.ReadKey(true); // 키 입력
                Console.CursorVisible = false; // 입력 커서 숨김

                if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.E) // 입력 확정
                {
                    return input; // 입력
                }

                if (key.Key == ConsoleKey.Backspace) // 삭제 입력
                {
                    if (input.Length > 0) input = input.Substring(0, input.Length - 1); // 마지막 문자 제거
                    continue; // 화면 갱신
                }

                if (key.Key == ConsoleKey.Q) // 취소 입력
                {
                    return "__CANCEL__"; // 취소
                }

                if (input.Length >= 3) // 최대 3자리 체크
                {
                    continue; // 추가 입력 차단
                }

                char c = key.KeyChar; // 입력 문자
                if (c >= '1' && c <= '5') // 허용 숫자 체크
                {
                    input += c; // 숫자 추가
                }
            }
        }

        private void RenderSecurityCodeFrame(int turn, int maxTry, string input, string log)
        {
            string[] lines = new string[] // 보안코드 화면 내용
            {
                "          .--------------------------------------.",
                "          |  SECURITY CODE MATRIX : 1 2 3 4 5    |",
                "          |  ACCESS KEY LENGTH    : 3 DIGITS     |",
                "          '------------------+-------------------'",
                "                             |                    ",
                "                       .-----+-----.              ",
                "                       |  ### ###  |              ",
                "                       |  #0# #1#  |              ",
                "                       |  ### ###  |              ",
                "                       '-----------'              ",
                "",
                " 손상된 백업 데이터의 3자리 보안 코드를 해독하십시오.",
                " 사용 가능 숫자 : 1 2 3 4 5 / 같은 숫자 중복 불가",
                " 시도 횟수      : " + turn + " / " + maxTry,
                " 입력 코드      : [ " + TextUtil.Fit(input, 3) + " ]",
                " SYSTEM LOG     : " + log
            };

            renderer.RenderMiniGameModal("SECURITY CODE BREACH", lines, "1~5 입력   BACKSPACE 삭제   E/ENTER 실행   Q 취소"); // 기존 모달 출력
        }

        private void RenderSignalSyncFrame(int marker, int safeStart, int safeWidth, int trackLength, int success, int fail, int remain, string log)
        {
            string safeLine = BuildSignalSafeLine(safeStart, safeWidth, trackLength); // SAFE RANGE 줄
            string markerLine = BuildSignalMarkerLine(marker, trackLength); // 포인터 줄

            string[] lines = new string[] // SIGNAL SYNC 화면 내용
            {
                "TARGET : UNSTABLE ADMIN SIGNAL",
                "STATUS : SIGNAL DRIFT DETECTED",
                BuildSignalDivider(trackLength),
                "불안정한 신호 마커를 SAFE RANGE 안에서 고정하십시오.",
                "",
                "SIGNAL BUS / SAFE RANGE",
                "",
                "        " + safeLine,
                "        " + markerLine,
                "",
                "SYNC LOCK : " + success + " / 3      SIGNAL ERR : " + fail + " / 3      TIME : " + remain + "s",
                "",
                "SYSTEM LOG",
                "> " + log
            };

            renderer.RenderMiniGameModal("SIGNAL SYNC", lines, "E 동기화   Q 포기"); // 기존 모달 출력
        }

        private string BuildSignalDivider(int trackLength)
        {
            return new string('═', trackLength + 2); // SIGNAL SYNC 내부 구분선
        }

        private string BuildSignalSafeLine(int safeStart, int safeWidth, int trackLength)
        {
            char[] chars = new char[trackLength]; // 트랙 문자

            for (int i = 0; i < trackLength; i++) // 트랙 생성
            {
                chars[i] = '-'; // 기본 라인
            }

            for (int i = safeStart; i < safeStart + safeWidth && i < trackLength; i++) // SAFE RANGE 표시
            {
                chars[i] = '='; // 안전 구간
            }

            return "[" + new string(chars) + "]"; // 트랙
        }

        private string BuildSignalMarkerLine(int marker, int trackLength)
        {
            char[] chars = new char[trackLength]; // 마커 라인

            for (int i = 0; i < trackLength; i++) // 공백 생성
            {
                chars[i] = ' '; // 기본 공백
            }

            if (marker >= 0 && marker < trackLength) // 마커 범위 체크
            {
                chars[marker] = '^'; // 현재 신호 위치
            }

            return " " + new string(chars) + " "; // 마커
        }

        public bool RunFilePurgeGame()
        {
            const int trackWidth = 56; // 파일 낙하 영역 표시 폭
            const int trackHeight = 16; // 파일 낙하 영역 높이
            const int timeLimit = 25; // 제한 시간 초
            const int successGoal = 9; // 즉시 성공 삭제 수
            const int minimumGoal = 6; // 시간 종료 최소 성공 수
            const int missLimit = 5; // 실패 허용 수

            int playerX = trackWidth / 2; // DELETE CANNON 시작 위치
            int destroyed = 0; // 삭제한 파일 수
            int missed = 0; // 놓친 파일 수
            int frame = 0; // 애니메이션 프레임
            string log = "수상한 파일을 하단 DELETE CANNON으로 격추하십시오."; // 내부 로그
            DateTime startedAt = DateTime.Now; // 시작 시각
            List<FallingFileTarget> files = new List<FallingFileTarget>(); // 낙하 파일 목록
            List<PurgeDeletionEffect> effects = new List<PurgeDeletionEffect>(); // 삭제 연출 목록
            List<PurgeBullet> bullets = new List<PurgeBullet>(); // 삭제빔 목록

            Console.CursorVisible = false; // 커서 숨김

            while (true) // 미니게임 루프
            {
                int elapsed = (int)(DateTime.Now - startedAt).TotalSeconds; // 경과 시간
                int remain = timeLimit - elapsed; // 남은 시간

                if (destroyed >= successGoal) // 즉시 성공 체크
                {
                    ShowMiniResult("SUSPICIOUS CACHE PURGED", new string[] { "수상한 파일 캐시 삭제 완료", "DELETED FILES : " + destroyed, "남은 위협 파일을 격리했습니다." }, ConsoleColor.Green); // 성공 출력
                    return true;
                }

                if (missed >= missLimit) // 실패 한도 체크
                {
                    ShowMiniResult("SUSPICIOUS CACHE LEAKED", new string[] { "너무 많은 파일이 하단 버퍼를 통과했습니다.", "MISSED FILES : " + missed }, ConsoleColor.Red); // 실패 출력
                    return false;
                }

                if (remain <= 0) // 시간 종료 체크
                {
                    if (destroyed >= minimumGoal) // 최소 성공 체크
                    {
                        ShowMiniResult("SUSPICIOUS CACHE PURGED", new string[] { "제한 시간 내 최소 삭제 목표를 달성했습니다.", "DELETED FILES : " + destroyed + " / " + successGoal }, ConsoleColor.Green); // 성공 출력
                        return true;
                    }

                    ShowMiniResult("SUSPICIOUS CACHE LEAKED", new string[] { "삭제 수가 부족합니다.", "DELETED FILES : " + destroyed + " / " + minimumGoal }, ConsoleColor.Red); // 실패 출력
                    return false;
                }

                while (Console.KeyAvailable) // 누적 입력 처리
                {
                    ConsoleKey key = Console.ReadKey(true).Key; // 키 수신

                    if (key == ConsoleKey.Q) // 포기 입력 체크
                    {
                        ShowMiniResult("SUSPICIOUS CACHE ABORTED", new string[] { "수상한 캐시 삭제를 중단했습니다." }, ConsoleColor.Red); // 중단 출력
                        return false;
                    }

                    if (key == ConsoleKey.A || key == ConsoleKey.LeftArrow) // 왼쪽 이동
                    {
                        playerX -= 3; // 왼쪽 3칸 이동
                        if (playerX < 1) playerX = 1; // 캐논 3칸 폭 범위 보정
                    }
                    else if (key == ConsoleKey.D || key == ConsoleKey.RightArrow) // 오른쪽 이동
                    {
                        playerX += 3; // 오른쪽 3칸 이동
                        if (playerX > trackWidth - 2) playerX = trackWidth - 2; // 캐논 3칸 폭 범위 보정
                    }
                    else if (key == ConsoleKey.E || key == ConsoleKey.Spacebar) // 삭제빔 입력
                    {
                        bullets.Add(new PurgeBullet { X = playerX, Y = trackHeight - 2 }); // 캐논 위에서 빔 시작
                    }
                }

                if (frame % 9 == 0) // 파일 생성 간격 체크
                {
                    files.Add(SpawnFallingFile(trackWidth)); // 낙하 파일 추가
                }

                for (int i = bullets.Count - 1; i >= 0; i--) // 삭제빔 이동
                {
                    bullets[i].Y--; // 위로 이동
                    if (bullets[i].Y < 0) bullets.RemoveAt(i); // 화면 밖 제거
                }

                if (frame % 6 == 0) // 파일 낙하 속도 조절
                {
                    for (int i = files.Count - 1; i >= 0; i--) // 파일 이동
                    {
                        files[i].Y++; // 아래로 이동

                        if (files[i].Y >= trackHeight - 1) // 캐논 라인 통과 체크
                        {
                            log = "LEAKED : " + files[i].DisplayName; // 누락 로그
                            files.RemoveAt(i); // 파일 제거
                            missed++; // 실패 증가
                        }
                    }
                }

                for (int b = bullets.Count - 1; b >= 0; b--) // 충돌 검사
                {
                    bool bulletUsed = false; // 빔 사용 여부

                    for (int f = files.Count - 1; f >= 0; f--) // 파일 검사
                    {
                        FallingFileTarget target = files[f]; // 대상 파일
                        int fileWidth = GetPurgeDisplayWidth(target.DisplayName); // 모자이크 파일명 표시 폭

                        bool sameLine = Math.Abs(bullets[b].Y - target.Y) <= 1; // 빔 세로 범위
                        bool hitFile = bullets[b].X >= target.X && bullets[b].X < target.X + fileWidth; // 표시 폭 기준 충돌

                        if (sameLine && hitFile) // 충돌 체크
                        {
                            log = target.DisplayName + " -> " + BuildPurgeDestroyName(target.DisplayName, 2) + " -> [DELETED]"; // 삭제 로그
                            effects.Add(new PurgeDeletionEffect { X = target.X, Y = target.Y, DisplayName = target.DisplayName, Frame = 0 }); // 삭제 연출 추가
                            files.RemoveAt(f); // 파일 삭제
                            destroyed++; // 삭제 수 증가
                            bulletUsed = true; // 빔 소모
                            break; // 파일 루프 종료
                        }
                    }

                    if (bulletUsed) // 빔 충돌 체크
                    {
                        bullets.RemoveAt(b); // 빔 제거
                    }
                }

                UpdatePurgeDeletionEffects(effects); // 삭제 연출 갱신
                RenderFilePurgeFrame(files, effects, bullets, playerX, trackWidth, trackHeight, destroyed, missed, remain, log); // 화면 출력
                Thread.Sleep(55); // 프레임 속도
                frame++; // 프레임 증가
            }
        }

        private FallingFileTarget SpawnFallingFile(int trackWidth)
        {
            string name = ChooseCorruptedFileName(); // 원본 파일명 선택
            string displayName = BuildMaskedPurgeFileName(name); // 출력용 모자이크 생성
            int fileWidth = GetPurgeDisplayWidth(displayName); // 모자이크 후 표시 폭 계산
            int maxX = Math.Max(1, trackWidth - fileWidth); // 최대 X 위치

            return new FallingFileTarget
            {
                X = random.Next(0, maxX + 1), // 표시 폭 기준 랜덤 X
                Y = 0, // 상단 시작
                Name = name, // 원본 파일명 저장
                DisplayName = displayName // 출력용 파일명 저장
            };
        }

        private string ChooseCorruptedFileName()
        {
            string[] names = new string[] // 수상한 파일 후보
            {
                "19_backup.zip",
                "private_clip.mp4",
                "secret_folder.zip",
                "do_not_open.exe",
                "premium_sample.mp4",
                "not_homework.mp4",
                "직박구리_backup.zip",
                "비밀_영상.mp4",
                "마도요.zip",
                "개인소장.dat",
                "19사진.jpg",
                "과제입니다.mp4",
                "19금아님.dat",
                "혼자볼것.zip"
            };

            return names[random.Next(0, names.Length)]; // 랜덤 파일명
        }

        private string BuildMaskedPurgeFileName(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty; // 빈 이름 방지

            int dotIndex = name.LastIndexOf('.'); // 확장자 위치
            string body = dotIndex > 0 ? name.Substring(0, dotIndex) : name; // 파일명 본문
            string extension = dotIndex > 0 ? name.Substring(dotIndex) : string.Empty; // 확장자
            char[] chars = body.ToCharArray(); // 본문 문자
            int visibleIndex = 0; // 마스킹 기준 인덱스

            for (int i = 0; i < chars.Length; i++) // 본문 순회
            {
                if (!IsPurgeMaskTarget(chars[i])) // 마스킹 제외 문자 체크
                {
                    continue; // 구분자 유지
                }

                bool mask = visibleIndex % 5 == 2 || visibleIndex % 5 == 3; // 중간맛 모자이크 패턴

                if (mask) // 마스킹 대상 체크
                {
                    chars[i] = IsKoreanChar(chars[i]) ? '█' : '#'; // 한글은 블록, 영문/숫자는 # 처리
                }

                visibleIndex++; // 표시 문자 인덱스 증가
            }

            return new string(chars) + extension; // 모자이크 파일명
        }

        private bool IsPurgeMaskTarget(char c)
        {
            if (c == '_' || c == '-' || c == '.' || c == ' ') return false; // 구분자 제외
            return char.IsLetterOrDigit(c) || IsKoreanChar(c); // 문자/숫자/한글만 마스킹
        }

        private bool IsKoreanChar(char c)
        {
            int code = c; // 문자 코드
            return code >= 0xAC00 && code <= 0xD7AF; // 한글 완성형 체크
        }

        private string BuildPurgeDestroyName(string displayName, int step)
        {
            if (string.IsNullOrEmpty(displayName)) return "[DELETED]"; // 빈 이름 방지
            if (step >= 3) return "[DELETED]"; // 최종 삭제 표시

            char[] chars = displayName.ToCharArray(); // 출력 파일명 기준 파괴

            for (int i = 0; i < chars.Length; i++) // 전체 문자 순회
            {
                if (chars[i] == '.' || chars[i] == '_' || chars[i] == '-' || chars[i] == ' ') continue; // 구분자 유지

                if (step == 1) // 1차 파괴
                {
                    if (i % 3 == 0) chars[i] = '#'; // 일부 깨짐
                }
                else if (step == 2) // 2차 파괴
                {
                    chars[i] = i % 2 == 0 ? '▓' : '#'; // 강한 깨짐
                }
            }

            return new string(chars); // 파괴 파일명
        }

        private void UpdatePurgeDeletionEffects(List<PurgeDeletionEffect> effects)
        {
            if (effects == null) return; // null 방지

            for (int i = effects.Count - 1; i >= 0; i--) // 역순 갱신
            {
                effects[i].Frame++; // 연출 프레임 증가

                if (effects[i].Frame > 8) // 연출 종료 체크
                {
                    effects.RemoveAt(i); // 삭제 연출 제거
                }
            }
        }

        private void RenderFilePurgeFrame(List<FallingFileTarget> files, List<PurgeDeletionEffect> effects, List<PurgeBullet> bullets, int playerX, int trackWidth, int trackHeight, int destroyed, int missed, int remain, string log)
        {
            string[] grid = BuildFilePurgeGrid(files, effects, bullets, playerX, trackWidth, trackHeight); // 파일 파괴 영역 생성
            string[] lines = new string[grid.Length + 5]; // 출력 라인
            int index = 0; // 삽입 위치

            lines[index++] = "FOLDER : /Hidden/차민규의_직박구리/"; // 폴더 표시
            lines[index++] = BuildPurgeDivider(trackWidth); // 내부 구분선
            lines[index++] = "수상한 파일을 하단 DELETE CANNON으로 격추하십시오."; // 설명

            for (int i = 0; i < grid.Length; i++) // 그리드 복사
            {
                lines[index++] = grid[i]; // 그리드 줄 추가
            }

            lines[index++] = "DELETED : " + destroyed + " / 9    MISSED : " + missed + " / 5    TIME : " + remain + "s"; // 상태 표시
            lines[index++] = "목표 : 제한 시간 안에 파일 최소 6개 삭제"; // 목표 표시

            renderer.RenderMiniGameModal("SUSPICIOUS CACHE PURGE", lines, "A/D 3칸 이동   E 삭제빔   Q 포기"); // 기존 모달 출력
        }

        private string BuildPurgeDivider(int trackWidth)
        {
            return new string('═', trackWidth + 4); // 내부 구분선
        }

        private string[] BuildFilePurgeGrid(List<FallingFileTarget> files, List<PurgeDeletionEffect> effects, List<PurgeBullet> bullets, int playerX, int trackWidth, int trackHeight)
        {
            char[][] grid = new char[trackHeight][]; // 표시 셀 버퍼

            for (int y = 0; y < trackHeight; y++) // 줄 초기화
            {
                grid[y] = new string(' ', trackWidth).ToCharArray(); // 공백 셀
            }

            for (int i = 0; i < files.Count; i++) // 파일 출력
            {
                FallingFileTarget file = files[i]; // 파일 대상

                if (file.Y < 0 || file.Y >= trackHeight - 1) continue; // 파일 Y 범위 체크
                if (string.IsNullOrEmpty(file.DisplayName)) continue; // 출력 이름 체크

                OverlayPurgeText(grid[file.Y], file.X, file.DisplayName); // 모자이크 파일명 배치
            }

            if (effects != null) // 삭제 연출 체크
            {
                for (int i = 0; i < effects.Count; i++) // 삭제 연출 출력
                {
                    PurgeDeletionEffect effect = effects[i]; // 삭제 연출 대상

                    if (effect.Y < 0 || effect.Y >= trackHeight - 1) continue; // Y 범위 체크

                    string effectText = BuildPurgeDestroyName(effect.DisplayName, effect.Frame / 2); // 파괴 단계 텍스트
                    OverlayPurgeText(grid[effect.Y], effect.X, effectText); // 파괴 연출 출력
                }
            }

            for (int i = 0; i < bullets.Count; i++) // 삭제빔 출력
            {
                PurgeBullet bullet = bullets[i]; // 빔 대상

                if (bullet.Y >= 0 && bullet.Y < trackHeight - 1 && bullet.X >= 0 && bullet.X < trackWidth) // 범위 체크
                {
                    SetPurgeCell(grid[bullet.Y], bullet.X, '|'); // 삭제빔 표시
                }
            }

            if (playerX >= 1 && playerX < trackWidth - 1) // 캐논 3칸 표시 범위 체크
            {
                SetPurgeCell(grid[trackHeight - 1], playerX - 1, '^'); // 캐논 왼쪽 표시
                SetPurgeCell(grid[trackHeight - 1], playerX, '^'); // 캐논 중앙 표시
                SetPurgeCell(grid[trackHeight - 1], playerX + 1, '^'); // 캐논 오른쪽 표시
            }

            string[] lines = new string[trackHeight + 2]; // 테두리 포함 결과
            lines[0] = "[" + new string('-', trackWidth) + "]"; // 상단 플레이 영역

            for (int y = 0; y < trackHeight; y++) // 본문 변환
            {
                lines[y + 1] = BuildPurgeGridLine(grid[y], trackWidth); // 본문 줄
            }

            lines[lines.Length - 1] = "[" + new string('-', trackWidth) + "]"; // 하단 플레이 영역
            return lines;
        }

        private void OverlayPurgeText(char[] cells, int startCell, string text)
        {
            if (cells == null) return; // null 방지
            if (string.IsNullOrEmpty(text)) return; // 빈 문자열 방지

            int x = Math.Max(0, startCell); // 시작 셀

            for (int i = 0; i < text.Length && x < cells.Length; i++) // 문자 배치
            {
                int charWidth = GetPurgeCharWidth(text[i]); // 문자 표시 폭

                if (x + charWidth > cells.Length) break; // 폭 초과 방지

                SetPurgeCell(cells, x, text[i]); // 문자 배치

                if (charWidth == 2 && x + 1 < cells.Length) // 2칸 문자 체크
                {
                    cells[x + 1] = '\0'; // 두 번째 칸 예약
                }

                x += charWidth; // 표시 폭만큼 이동
            }
        }

        private void SetPurgeCell(char[] cells, int x, char value)
        {
            if (cells == null) return; // null 방지
            if (x < 0 || x >= cells.Length) return; // 범위 체크

            if (cells[x] == '\0' && x > 0) // 한글 두 번째 칸 위에 덮는 경우
            {
                cells[x - 1] = ' '; // 앞쪽 2칸 문자 제거
            }

            cells[x] = value; // 현재 칸 설정

            if (x + 1 < cells.Length && cells[x + 1] == '\0') // 기존 2칸 문자 잔여 체크
            {
                cells[x + 1] = ' '; // 예약 칸 제거
            }
        }

        private string BuildPurgeGridLine(char[] cells, int trackWidth)
        {
            if (cells == null) return TextUtil.Fit(string.Empty, trackWidth); // null 방지

            string line = string.Empty; // 출력 줄

            for (int i = 0; i < cells.Length; i++) // 셀 순회
            {
                if (cells[i] == '\0') continue; // 2칸 문자 예약 칸은 출력 제외
                line += cells[i]; // 문자 추가
            }

            return TextUtil.Fit(line, trackWidth); // 표시 폭 기준 보정
        }

        private int GetPurgeDisplayWidth(string text)
        {
            return TextUtil.GetDisplayWidth(text); // 한글/전각 포함 표시 폭
        }

        private int GetPurgeCharWidth(char c)
        {
            return TextUtil.GetCharWidth(c); // 한글/전각 문자 폭
        }

        private void ShowMiniResult(string title, string[] lines, ConsoleColor color)
        {
            renderer.RenderMiniGameModal(title, lines, "Q 창닫기"); // 결과창 닫기 안내

            while (true) // 닫기 대기
            {
                ConsoleKey key = Console.ReadKey(true).Key; // 키 입력

                if (key == ConsoleKey.Q) // Q 닫기 체크
                {
                    return; // 결과창 종료
                }
            }
        }
    }
}
