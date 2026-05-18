namespace VirusExe.SystemBreach.Rendering
{
    // 모달 입력 모드
    // Footer 문구와 실제 입력 처리를 같은 기준으로 맞추기 위한 구분값
    public enum ModalInputMode
    {
        NextOnly, // E / Enter 다음
        CloseOnly, // Q 창닫기
        NextOrClose // E / Enter 다음 또는 Q 창닫기
    }
}
