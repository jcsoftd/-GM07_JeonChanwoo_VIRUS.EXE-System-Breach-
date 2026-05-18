using System;
using System.Threading;
using VirusExe.SystemBreach.Rendering;
using VirusExe.SystemBreach.Characters; // 임시 테스트 삭제해야함

namespace VirusExe.SystemBreach.Systems
{
    // 타이틀 메뉴
    public class TitleMenuManager
    {
        private readonly ConsoleRenderer renderer; // 타이틀 화면 출력
        private int selectedIndex; // 현재 선택 메뉴 인덱스

        public TitleMenuManager(ConsoleRenderer renderer)
        {
            this.renderer = renderer;
            selectedIndex = 0; // 첫 메뉴 선택
        }

        public bool Open()
        {
            int frame = 0; // 타이틀 애니메이션 프레임

            while (true) // 타이틀 메뉴 유지
            {
                renderer.RenderTitleMenu(selectedIndex, frame); // 현재 선택 상태 출력

                if (Console.KeyAvailable) // 입력 존재 체크
                {
                    ConsoleKey key = Console.ReadKey(true).Key; // 메뉴 입력 수신

                    if (key == ConsoleKey.F9) // 숨김 변이 테스트 체크
                    {
                        RunMutationAnimationTestEasterEgg(); // 변이 연출 테스트
                        frame = 0; // 복귀 프레임 초기화
                        continue;
                    }

                    if (key == ConsoleKey.W || key == ConsoleKey.UpArrow) // 위 메뉴 이동 체크
                    {
                        selectedIndex--; // 선택 인덱스 감소
                        if (selectedIndex < 0) selectedIndex = 4; // 첫 항목 위는 마지막 항목
                    }
                    else if (key == ConsoleKey.S || key == ConsoleKey.DownArrow) // 아래 메뉴 이동 체크
                    {
                        selectedIndex++; // 선택 인덱스 증가
                        if (selectedIndex > 4) selectedIndex = 0; // 마지막 항목 아래는 첫 항목
                    }
                    else if (key == ConsoleKey.E || key == ConsoleKey.Enter) // 선택 실행 체크
                    {
                        if (selectedIndex == 0) // START BREACH 선택 체크
                        {
                            renderer.PlayTitleStartSequence(); // 침투 시작 연출
                            return true; // 게임 시작
                        }

                        if (selectedIndex == 1) // LOAD GAME 잠금 선택 체크
                        {
                            renderer.ShowTitleLoadGameTemporary(); // 잠금 안내 모달
                            frame = 0; // 타이틀 복귀 프레임 초기화
                        }
                        else if (selectedIndex == 2) // MINI GAME 선택 체크
                        {
                            MiniGameManager miniGameManager = new MiniGameManager(renderer); // 테스트용 미니게임 매니저
                            miniGameManager.ShowMiniGameTestMenu(); // 미니게임 선택 모달
                            frame = 0; // 타이틀 복귀 프레임 초기화
                        }
                        else if (selectedIndex == 3) // SYSTEM INFO 선택 체크
                        {
                            renderer.ShowTitleSystemInfo(); // 정보 화면 출력
                            frame = 0; // 타이틀 복귀 프레임 초기화
                        }
                        else if (selectedIndex == 4) // EXIT 선택 체크
                        {
                            renderer.PlayTitleTerminateSequence(); // 감염 종료 연출
                            return false; // 게임 종료
                        }
                    }
                }

                frame++; // 애니메이션 진행
                Thread.Sleep(90); // 타이틀 프레임 속도
            }
        }


        private void RunMutationAnimationTestEasterEgg() // 테스트용 삭제해야함
        {
            Player testPlayer = new Player(); // 연출 테스트용 더미 플레이어

            renderer.PlayPayloadMutationDetectedSequence(); // 변이 전 연출

            VirusMutation mutation = renderer.ShowPayloadMutationSelection(testPlayer); // 변이 선택

            renderer.PlayPayloadMutationCompleteSequence(mutation); // 변이 후 연출

            Console.Clear(); // 타이틀 복귀 전 정리
            selectedIndex = 0; // 시작 메뉴로 복귀
        }
    }
}
