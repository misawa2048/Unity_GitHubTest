import librosa
import numpy as np
import os

def detect_initial_silence_precise(audio_file_path):
    """
    音楽ファイルの最初の無音時間をより精密に検出する
    """
    
    # 音楽ファイルを読み込み
    y, sr = librosa.load(audio_file_path)
    
    # より細かい解析のため、小さなホップ長を使用
    hop_length = 256  # より細かい時間解像度
    frame_length = 512
    
    # RMS（Root Mean Square）エネルギーを計算
    rms = librosa.feature.rms(y=y, frame_length=frame_length, hop_length=hop_length)[0]
    
    # 各フレームの時間を計算
    times = librosa.frames_to_time(np.arange(len(rms)), sr=sr, hop_length=hop_length)
    
    print("最初の3秒間の詳細分析:")
    print("時間(秒)\tRMSエネルギー")
    print("-" * 30)
    
    silence_threshold = 0.0001  # 非常に低い閾値
    first_sound_index = None
    
    for i, (time, energy) in enumerate(zip(times, rms)):
        if time > 3.0:  # 最初の3秒のみ表示
            break
            
        print(f"{time:.6f}\t{energy:.8f}")
        
        # 最初に閾値を超えるフレームを検出
        if first_sound_index is None and energy > silence_threshold:
            first_sound_index = i
    
    if first_sound_index is not None:
        first_sound_time = times[first_sound_index]
        print(f"\n最初の音（閾値 {silence_threshold}）: {first_sound_time:.6f} 秒")
    else:
        print(f"\n閾値 {silence_threshold} を超える音が見つかりませんでした")
    
    # 統計情報
    print(f"\nRMSエネルギーの統計（最初の3秒）:")
    first_3s_mask = times <= 3.0
    rms_3s = rms[first_3s_mask]
    print(f"最小値: {np.min(rms_3s):.8f}")
    print(f"最大値: {np.max(rms_3s):.8f}")
    print(f"平均値: {np.mean(rms_3s):.8f}")
    print(f"標準偏差: {np.std(rms_3s):.8f}")
    
    # 振幅の直接分析
    print(f"\n振幅の直接分析（最初の1000サンプル）:")
    first_samples = y[:1000]
    print(f"最初の1000サンプルの最大振幅: {np.max(np.abs(first_samples)):.8f}")
    print(f"最初の1000サンプルの平均振幅: {np.mean(np.abs(first_samples)):.8f}")
    
    # ゼロでない最初のサンプルを見つける
    non_zero_samples = np.where(np.abs(y) > 0.00001)[0]
    if len(non_zero_samples) > 0:
        first_non_zero_time = non_zero_samples[0] / sr
        print(f"最初の非ゼロサンプル（閾値 0.00001）: {first_non_zero_time:.6f} 秒")
    
    return first_sound_time if first_sound_index is not None else 0

# メイン処理
if __name__ == "__main__":
    audio_file = r"Assets\_MyExamples\MusicGame\AudioFiles\NHKWaltz.mp3"
    
    print(f"音楽ファイル: {audio_file}")
    print("精密分析中...")
    
    try:
        silence_duration = detect_initial_silence_precise(audio_file)
        
    except Exception as e:
        print(f"エラーが発生しました: {e}")
