import librosa
import numpy as np
import os

def calculate_bpm(audio_file_path):
    """
    音楽ファイルのBPMを計算する
    
    Parameters:
    audio_file_path: 音楽ファイルのパス
    
    Returns:
    bpm: 推定されたBPM値
    """
    
    # 音楽ファイルを読み込み
    y, sr = librosa.load(audio_file_path)
    
    print(f"音楽ファイル: {audio_file_path}")
    print(f"サンプリングレート: {sr} Hz")
    print(f"総長: {len(y)/sr:.3f} 秒")
    print("BPM分析中...")
    
    # テンポ（BPM）を推定
    tempo, beats = librosa.beat.beat_track(y=y, sr=sr)
    
    # tempoが配列の場合は最初の値を取得
    if isinstance(tempo, np.ndarray):
        tempo_value = float(tempo[0])
    else:
        tempo_value = float(tempo)
    
    print(f"\n基本的なBPM推定: {tempo_value:.1f} BPM")
    
    # より詳細なテンポ分析
    # 複数の候補を取得
    tempo_candidates = librosa.beat.tempo(y=y, sr=sr, max_tempo=300)
    
    print(f"\nテンポ候補:")
    for i, candidate in enumerate(tempo_candidates[:5]):  # 上位5候補を表示
        print(f"  候補 {i+1}: {candidate:.1f} BPM")
    
    # ビート位置を時間に変換
    beat_times = librosa.frames_to_time(beats, sr=sr)
    
    print(f"\n検出されたビート数: {len(beat_times)}")
    
    if len(beat_times) > 1:
        # ビート間隔から手動でBPMを計算
        beat_intervals = np.diff(beat_times)
        avg_beat_interval = np.mean(beat_intervals)
        manual_bpm = 60.0 / avg_beat_interval
        
        print(f"平均ビート間隔: {avg_beat_interval:.3f} 秒")
        print(f"手動計算BPM: {manual_bpm:.1f} BPM")
        
        # 統計情報
        print(f"ビート間隔の標準偏差: {np.std(beat_intervals):.3f} 秒")
        print(f"最小ビート間隔: {np.min(beat_intervals):.3f} 秒")
        print(f"最大ビート間隔: {np.max(beat_intervals):.3f} 秒")
    
    # オンセット（音の開始点）を検出してより精密な分析
    onset_frames = librosa.onset.onset_detect(y=y, sr=sr, units='frames')
    onset_times = librosa.frames_to_time(onset_frames, sr=sr)
    
    print(f"\n検出されたオンセット数: {len(onset_times)}")
    
    if len(onset_times) > 1:
        onset_intervals = np.diff(onset_times)
        # 短すぎる間隔を除外（装飾音符など）
        filtered_intervals = onset_intervals[onset_intervals > 0.1]
        
        if len(filtered_intervals) > 0:
            avg_onset_interval = np.mean(filtered_intervals)
            onset_bpm = 60.0 / avg_onset_interval
            print(f"オンセット平均間隔: {avg_onset_interval:.3f} 秒")
            print(f"オンセットベースBPM: {onset_bpm:.1f} BPM")
    
    # スペクトログラム解析による確認
    print(f"\n=== 最終推定結果 ===")
    print(f"推奨BPM: {tempo_value:.1f} BPM")
    
    # 一般的なBPM値との比較
    common_bpms = [120, 128, 140, 150, 154, 160, 170, 180]
    closest_common = min(common_bpms, key=lambda x: abs(x - tempo_value))
    print(f"最も近い一般的BPM: {closest_common} BPM")
    
    return tempo_value, beat_times

# メイン処理
if __name__ == "__main__":
    audio_file = r"Assets\_MyExamples\MusicGame\AudioFiles\NHKWaltz.mp3"
    
    # ファイルの存在確認
    if not os.path.exists(audio_file):
        print(f"ファイルが見つかりません: {audio_file}")
        exit(1)
    
    try:
        bpm, beat_times = calculate_bpm(audio_file)
        
        # 追加分析：最初の10秒のビートパターン
        print(f"\n=== 最初の10秒のビートパターン ===")
        early_beats = beat_times[beat_times <= 10.0]
        print(f"最初の10秒のビート時刻:")
        for i, beat_time in enumerate(early_beats):
            print(f"  ビート {i+1}: {beat_time:.3f} 秒")
            
    except Exception as e:
        print(f"エラーが発生しました: {e}")
        import traceback
        traceback.print_exc()
