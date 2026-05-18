using System;
using System.Collections.Generic;
using VirusExe.SystemBreach.Characters;
using VirusExe.SystemBreach.Core;

namespace VirusExe.SystemBreach.Rendering
{
    // 몬스터 ASCII 아트 라우터
    // Enemy 상태에 맞는 일반/엘리트/보스 아트를 골라줌
    
    public partial class ConsoleRenderer
    {
        private const int BattleEnemyViewportLines = 15; // 전투 몬스터 뷰포트 고정 높이

        private void RenderEnemyArt(Enemy enemy, int phase, int frame)
        {
            string[] art; // 출력할 몬스터 아트
            ConsoleColor color; // 몬스터 색상

            if (renderEnemyHit) // 피격 아트 체크
            {
                art = BuildHitEnemyArt(enemy, phase, frame); // 피격 아트 생성

                if (renderEnemyDamagePopup) // 데미지 팝업 체크
                {
                    color = ConsoleColor.DarkRed; // 피격 기본 색상
                    WriteArtViewportWithDamagePopup(art, color, renderEnemyAttackOffset, GetFixedEnemyName(enemy), renderEnemyDamagePopupText, renderEnemyCriticalPopup, renderEnemyImpactFrame); // 팝업만 별도 색상 출력
                    return;
                }

                color = ConsoleColor.Red; // 피격 색상
                WriteArtViewport(art, color, renderEnemyAttackOffset, GetFixedEnemyName(enemy)); // 고정 뷰포트 출력
                return;
            }

            if (renderDeadEnemy) // 사망 아트 체크
            {
                art = BuildDeadEnemyArt(enemy, phase, frame); // 사망 아트 생성
                color = ConsoleColor.DarkGray; // 사망 색상
                WriteArtViewport(art, color, 0, GetFixedEnemyName(enemy)); // 고정 뷰포트 출력
                return;
            }

            if (renderEnemyAttackOffset != 0) // 적 공격 모션 체크
            {
                art = BuildAttackEnemyArt(enemy, phase, frame); // 공격 아트 생성
            }
            else if (enemy.IsBoss && phase >= 3) art = BuildBossFinalArt(frame); // 최종 페이즈
            else if (enemy.IsBoss) art = BuildBossArt(phase, frame); // 보스
            else if (enemy.IsElite) art = BuildEliteEnemyArt(enemy.Name, frame); // 엘리트 이름별 아트
            else art = BuildNormalEnemyArt(enemy.Name, frame); // 일반 몬스터 이름별 아트

            if (enemy.IsEncrypted) // 랜섬웨어 암호화 체크
            {
                art = ApplyEncryptedEnemyArt(art, enemy.EncryptionNoiseLevel); // $ 오염 적용
            }

            if (enemy.PopupOverlayActive) // 애드웨어 팝업 체크
            {
                art = ApplyPopupOverlayArt(art, frame); // 팝업 오버레이 적용
            }

            color = renderEnemyAttackOffset != 0 ? ConsoleColor.Red :
                enemy.IsEncrypted ? ConsoleColor.Yellow :
                enemy.PopupOverlayActive ? ConsoleColor.Green :
                enemy.IsBoss ? ConsoleColor.White :
                enemy.IsElite ? ConsoleColor.Magenta : ConsoleColor.Cyan; // 상태별 색상

            WriteArtViewport(art, color, renderEnemyAttackOffset, GetFixedEnemyName(enemy)); // 고정 뷰포트 출력
        }


        private string[] ApplyDamagePopupOverlayArt(string[] art, string damageText, bool critical, int impactFrame)
        {
            if (art == null) art = new string[0]; // null 방지

            string[] result = new string[art.Length]; // 팝업 결과

            for (int i = 0; i < art.Length; i++) // 원본 복사
            {
                result[i] = art[i] ?? string.Empty; // null 줄 방지
            }

            string value = string.IsNullOrEmpty(damageText) ? "-0" : damageText; // 표시 피해값
            string[] popup = critical ? BuildCriticalDamagePopup(value) : BuildNormalDamagePopup(value); // 팝업 종류 선택
            int popupWidth = GetMaxArtWidth(popup); // 팝업 최대 폭
            int artWidth = Math.Max(GetMaxArtWidth(result), popupWidth); // 중앙 배치 기준 폭
            int startRow = Math.Max(0, result.Length / 2 - popup.Length / 2); // 중앙 행
            int startCol = Math.Max(0, (artWidth - popupWidth) / 2); // 중앙 열

            for (int i = 0; i < popup.Length; i++) // 팝업 줄 순회
            {
                int row = startRow + i; // 대상 줄
                if (row < 0 || row >= result.Length) continue; // 범위 체크

                result[row] = OverlayText(result[row], popup[i], startCol); // 팝업 덮기
            }

            return result; // 오버레이 결과
        }

        private string[] BuildNormalDamagePopup(string value)
        {
            string text = " DAMAGE " + value + " "; // 일반 피해 문구
            string border = "+" + new string('-', text.Length) + "+"; // 팝업 테두리

            return new string[]
            {
                border,
                "|" + text + "|",
                border
            };
        }

        private string[] BuildCriticalDamagePopup(string value)
        {
            string text = " CRITICAL " + value + " "; // 치명타 피해 문구
            string border = "#" + new string('=', text.Length) + "#"; // 치명타 전용 테두리

            return new string[]
            {
                border,
                "#" + text + "#",
                border
            };
        }

        private void OverlayDamageFragment(string[] target, int row, int startCol, string fragment)
        {
            if (target == null) return; // null 방지
            if (row < 0 || row >= target.Length) return; // 범위 체크

            target[row] = OverlayText(target[row], fragment, Math.Max(0, startCol)); // 파편 덮기
        }


        private string[] ApplyEncryptedEnemyArt(string[] art, int noiseLevel)
        {
            string[] result = new string[art.Length]; // 오염 결과
            int percent = Math.Max(0, Math.Min(80, noiseLevel)); // 오염 강도 제한

            for (int y = 0; y < art.Length; y++) // 줄 순회
            {
                char[] chars = art[y].ToCharArray(); // 문자 배열

                for (int x = 0; x < chars.Length; x++) // 문자 순회
                {
                    if (chars[x] == ' ') continue; // 공백 유지
                    if (glitchRandom.Next(0, 100) < percent) chars[x] = '$'; // $ 오염
                }

                result[y] = new string(chars); // 줄 저장
            }

            return result; // 오염 아트
        }


        private string[] ApplyPopupOverlayArt(string[] art, int frame)
        {
            string[] result = new string[art.Length]; // 팝업 결과

            for (int i = 0; i < art.Length; i++) // 원본 복사
            {
                result[i] = art[i]; // 줄 복사
            }

            string[] popupA = new string[] // 첫 번째 팝업
            {
                "+------------+",
                "|  POP-UP!!  |",
                "| CLICK_AD   |",
                "+------------+"
            };

            string[] popupB = new string[] // 두 번째 팝업
            {
                "+----------+",
                "|  AD AD   |",
                "|  FREE!!  |",
                "+----------+"
            };

            int offset = frame % 2 == 0 ? 0 : 2; // 흔들림 보정
            OverlayPopup(result, popupA, 2, 4 + offset); // 첫 팝업 배치
            OverlayPopup(result, popupB, 5, 18 - offset); // 둘째 팝업 배치

            return result; // 팝업 아트
        }


        private void OverlayPopup(string[] target, string[] popup, int startRow, int startCol)
        {
            for (int i = 0; i < popup.Length; i++) // 팝업 줄 순회
            {
                int row = startRow + i; // 대상 줄
                if (row < 0 || row >= target.Length) continue; // 범위 체크

                target[row] = OverlayText(target[row], popup[i], startCol); // 텍스트 덮기
            }
        }


        private string OverlayText(string source, string overlay, int startCol)
        {
            if (source == null) source = string.Empty; // null 방지
            if (overlay == null) overlay = string.Empty; // null 방지
            if (startCol < 0) startCol = 0; // 시작 위치 보정

            int required = startCol + overlay.Length; // 필요 길이
            if (source.Length < required) source = source.PadRight(required); // 길이 보정

            char[] chars = source.ToCharArray(); // 대상 문자

            for (int i = 0; i < overlay.Length; i++) // 오버레이 순회
            {
                chars[startCol + i] = overlay[i]; // 문자 덮기
            }

            return new string(chars);
        }


        private string GetFixedEnemyName(Enemy enemy)
        {
            if (enemy == null) return null; // null 방지
            if (enemy.IsBoss || enemy.IsElite) return null; // 보스/엘리트는 기존 아트 기준 유지

            return enemy.Name; // 일반 몬스터 이름 하단 고정
        }


        private class DamagePopupOverlayLine
        {
            public int Row; // 뷰포트 기준 행
            public int Col; // 아트 기준 열
            public string Text; // 출력 문자열
            public ConsoleColor Color; // 출력 색상

            public DamagePopupOverlayLine(int row, int col, string text, ConsoleColor color)
            {
                Row = row; // 행 저장
                Col = col; // 열 저장
                Text = text; // 문자열 저장
                Color = color; // 색상 저장
            }
        }


        private void WriteArtViewportWithDamagePopup(string[] art, ConsoleColor baseColor, int offsetX, string fixedName, string damageText, bool critical, int impactFrame)
        {
            if (art == null) art = new string[0]; // null 방지

            bool hasFixedName = !string.IsNullOrEmpty(fixedName); // 이름 고정 여부
            string[] bodyArt = art; // 실제 모션 아트

            if (hasFixedName && art.Length > 0) // 일반 몬스터 이름 줄 분리
            {
                bodyArt = new string[art.Length - 1]; // 마지막 줄 제외

                for (int i = 0; i < bodyArt.Length; i++) // 본문 복사
                {
                    bodyArt[i] = art[i]; // 모션 대상 아트만 복사
                }
            }

            string value = string.IsNullOrEmpty(damageText) ? "-0" : damageText; // 표시 피해값
            string[] popup = critical ? BuildCriticalDamagePopup(value) : BuildNormalDamagePopup(value); // 팝업 종류 선택
            int popupWidth = GetMaxArtWidth(popup); // 팝업 폭
            int viewportRows = hasFixedName ? BattleEnemyViewportLines - 1 : BattleEnemyViewportLines; // 이름줄 예약
            int maxWidth = Math.Max(GetMaxArtWidth(bodyArt), popupWidth); // 팝업 포함 폭
            int left = Math.Max(0, (InnerWidth - maxWidth) / 2 + offsetX); // 모션 아트 중앙 위치
            int topPadding = Math.Max(0, (viewportRows - bodyArt.Length) / 2); // 위쪽 여백
            int sourceStart = Math.Max(0, (bodyArt.Length - viewportRows) / 2); // 초과 아트 중앙 시작
            int popupTop = Math.Max(0, viewportRows / 2 - popup.Length / 2); // 팝업 중앙 행
            int popupLeft = Math.Max(0, (maxWidth - popupWidth) / 2); // 팝업 중앙 열
            List<DamagePopupOverlayLine> overlays = BuildDamagePopupOverlayLines(popup, popupTop, popupLeft, popupWidth, critical); // 색상 오버레이 목록

            for (int row = 0; row < viewportRows; row++) // 고정 몬스터 뷰포트 출력
            {
                int sourceIndex = row - topPadding + sourceStart; // 원본 아트 줄 위치

                if (sourceIndex >= 0 && sourceIndex < bodyArt.Length && row >= topPadding) // 출력 범위 체크
                {
                    string line = TextUtil.Fit(bodyArt[sourceIndex], maxWidth); // 아트 줄 폭 보정
                    string finalLine = TextUtil.Fit(new string(' ', left) + line, InnerWidth); // 중앙 배치
                    WriteDamagePopupOverlayLine(finalLine, baseColor, overlays, row, left); // 팝업만 별도 색상 출력
                }
                else
                {
                    string finalLine = TextUtil.Fit(string.Empty, InnerWidth); // 빈 줄 구성
                    WriteDamagePopupOverlayLine(finalLine, baseColor, overlays, row, left); // 빈 줄에도 팝업 출력
                }
            }

            if (hasFixedName) // 일반 몬스터 이름 고정 출력
            {
                int nameWidth = TextUtil.GetDisplayWidth(fixedName); // 이름 표시 폭
                int nameLeft = Math.Max(0, (InnerWidth - nameWidth) / 2); // 이름 중앙 고정
                string nameLine = TextUtil.Fit(new string(' ', nameLeft) + fixedName, InnerWidth); // 이름 줄 구성
                WriteLine(nameLine, ConsoleColor.Red); // 이름은 피격색 유지
            }
        }


        private List<DamagePopupOverlayLine> BuildDamagePopupOverlayLines(string[] popup, int popupTop, int popupLeft, int popupWidth, bool critical)
        {
            List<DamagePopupOverlayLine> overlays = new List<DamagePopupOverlayLine>(); // 오버레이 목록

            for (int i = 0; i < popup.Length; i++) // 팝업 줄 순회
            {
                overlays.Add(new DamagePopupOverlayLine(popupTop + i, popupLeft, popup[i], GetDamagePopupLineColor(critical, i))); // 팝업 줄 추가
            }

            return overlays; // 오버레이 반환
        }


        private ConsoleColor GetDamagePopupLineColor(bool critical, int lineIndex)
        {
            if (critical) return ConsoleColor.Yellow; // 치명타 박스 전체 노랑
            return ConsoleColor.White; // 일반 데미지 박스 전체 흰색
        }


        private void WriteDamagePopupOverlayLine(string source, ConsoleColor baseColor, List<DamagePopupOverlayLine> overlays, int row, int absoluteLeft)
        {
            if (source == null) source = string.Empty; // null 방지
            source = TextUtil.Fit(source, InnerWidth); // 출력 폭 보정

            List<DamagePopupOverlayLine> rowOverlays = GetDamagePopupOverlaysForRow(overlays, row); // 현재 줄 오버레이

            if (rowOverlays.Count == 0) // 오버레이 없음 체크
            {
                WriteLine(source, baseColor); // 일반 출력
                return;
            }

            List<ColorSegment> segments = new List<ColorSegment>(); // 색상 세그먼트
            int cursor = 0; // 현재 출력 위치

            for (int i = 0; i < rowOverlays.Count; i++) // 오버레이 순회
            {
                DamagePopupOverlayLine overlay = rowOverlays[i]; // 현재 오버레이
                int start = Math.Max(0, absoluteLeft + overlay.Col); // 실제 출력 시작 열
                string overlayText = overlay.Text ?? string.Empty; // 출력 문자열

                if (start >= InnerWidth) continue; // 화면 밖 체크
                if (start + overlayText.Length > InnerWidth) overlayText = overlayText.Substring(0, InnerWidth - start); // 오른쪽 잘림 보정
                if (overlayText.Length <= 0) continue; // 빈 문자열 체크

                if (start > cursor) // 기본 아트 구간 체크
                {
                    segments.Add(new ColorSegment(source.Substring(cursor, start - cursor), baseColor)); // 기본 아트 색상
                }

                segments.Add(new ColorSegment(overlayText, overlay.Color)); // 팝업 색상
                cursor = Math.Max(cursor, start + overlayText.Length); // 커서 이동
            }

            if (cursor < source.Length) // 남은 기본 아트 체크
            {
                segments.Add(new ColorSegment(source.Substring(cursor), baseColor)); // 남은 구간 출력
            }

            WriteSegmentsLine(segments); // 색상 분리 출력
        }


        private List<DamagePopupOverlayLine> GetDamagePopupOverlaysForRow(List<DamagePopupOverlayLine> overlays, int row)
        {
            List<DamagePopupOverlayLine> result = new List<DamagePopupOverlayLine>(); // 현재 줄 목록

            for (int i = 0; i < overlays.Count; i++) // 전체 오버레이 순회
            {
                if (overlays[i].Row == row) result.Add(overlays[i]); // 같은 행만 추가
            }

            result.Sort(delegate (DamagePopupOverlayLine a, DamagePopupOverlayLine b) { return a.Col.CompareTo(b.Col); }); // 왼쪽부터 정렬
            return result; // 현재 줄 반환
        }


        private void WriteArtViewport(string[] art, ConsoleColor color, int offsetX, string fixedName)
        {
            if (art == null) art = new string[0]; // null 방지

            bool hasFixedName = !string.IsNullOrEmpty(fixedName); // 이름 고정 여부
            string[] bodyArt = art; // 실제 모션 아트

            if (hasFixedName && art.Length > 0) // 일반 몬스터 이름 줄 분리
            {
                bodyArt = new string[art.Length - 1]; // 마지막 줄은 이름/상태줄로 보고 제외

                for (int i = 0; i < bodyArt.Length; i++) // 본문 복사
                {
                    bodyArt[i] = art[i]; // 모션 대상 아트만 복사
                }
            }

            int viewportRows = hasFixedName ? BattleEnemyViewportLines - 1 : BattleEnemyViewportLines; // 이름줄 예약
            int maxWidth = GetMaxArtWidth(bodyArt); // 아트 최대 폭
            int left = Math.Max(0, (InnerWidth - maxWidth) / 2 + offsetX); // 모션 아트 중앙 위치
            int topPadding = Math.Max(0, (viewportRows - bodyArt.Length) / 2); // 위쪽 여백
            int sourceStart = Math.Max(0, (bodyArt.Length - viewportRows) / 2); // 초과 아트 중앙 시작

            for (int row = 0; row < viewportRows; row++) // 고정 몬스터 뷰포트 출력
            {
                int sourceIndex = row - topPadding + sourceStart; // 원본 아트 줄 위치

                if (sourceIndex >= 0 && sourceIndex < bodyArt.Length && row >= topPadding) // 출력 범위 체크
                {
                    string line = TextUtil.Fit(bodyArt[sourceIndex], maxWidth); // 아트 줄 폭 보정
                    string finalLine = TextUtil.Fit(new string(' ', left) + line, InnerWidth); // 중앙 배치
                    WriteLine(finalLine, color); // 아트 줄 출력
                }
                else
                {
                    WriteLine(string.Empty, ConsoleColor.DarkGray); // 뷰포트 공백 유지
                }
            }

            if (hasFixedName) // 일반 몬스터 이름 고정 출력
            {
                int nameWidth = TextUtil.GetDisplayWidth(fixedName); // 이름 표시 폭
                int nameLeft = Math.Max(0, (InnerWidth - nameWidth) / 2); // 이름은 흔들림/공격 offset 없이 중앙 고정
                string nameLine = TextUtil.Fit(new string(' ', nameLeft) + fixedName, InnerWidth); // 이름 줄 구성
                WriteLine(nameLine, color); // 이름 출력
            }
        }


        private int GetMaxArtWidth(string[] art)
        {
            int maxWidth = 0;

            for (int i = 0; i < art.Length; i++) // 최대 폭 계산
            {
                int width = TextUtil.GetDisplayWidth(art[i]);
                if (width > maxWidth) maxWidth = width;
            }

            return maxWidth;
        }


        private string[] BuildHitEnemyArt(Enemy enemy, int phase, int frame)
        {
            if (enemy.IsBoss) return BuildHitBossArt(phase, frame); // 보스 피격
            if (enemy.IsElite) return BuildHitEliteEnemyArt(enemy.Name, frame); // 엘리트 이름별 피격
            return BuildHitNormalEnemyArt(enemy.Name, frame); // 일반 몬스터 이름별 피격
        }


        private string[] BuildDeadEnemyArt(Enemy enemy, int phase, int frame)
        {
            if (enemy.IsBoss) return BuildDeadBossArt(frame); // 보스 사망
            if (enemy.IsElite) return BuildDeadEliteEnemyArt(enemy.Name, frame); // 엘리트 이름별 사망
            return BuildDeadNormalEnemyArt(enemy.Name, frame); // 일반 몬스터 이름별 사망
        }


        private string[] BuildAttackEnemyArt(Enemy enemy, int phase, int frame)
        {
            if (enemy.IsBoss) return BuildAttackBossArt(phase, frame); // 보스 페이즈별 공격 아트
            if (enemy.IsElite) return BuildAttackEliteEnemyArt(enemy.Name, frame); // 엘리트 이름별 공격
            return BuildAttackNormalEnemyArt(enemy.Name, frame); // 일반 몬스터 이름별 공격
        }


    }
}
