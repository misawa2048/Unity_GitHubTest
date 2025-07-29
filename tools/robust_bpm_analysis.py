import librosa
import numpy as np
import os
import sys
import argparse
from scipy import signal

def robust_bpm_analysis(audio_file_path):
    """
    より安定したBPM分析（音声ファイルのみ使用）
    """
    
    # 音楽ファイルを読み込み
    y, sr = librosa.load(audio_file_path)
    
    print(f"\n=== 音声ファイル分析 ===")
    print(f"総長: {len(y)/sr:.3f} 秒")
    
    # 複数の安定した手法でBPM推定
    bpm_estimates = []
    
    # 方法1: 基本的なbeat_track
    try:
        tempo, beats = librosa.beat.beat_track(y=y, sr=sr)
        tempo_val = float(tempo[0]) if isinstance(tempo, np.ndarray) else float(tempo)
        bpm_estimates.append(('beat_track_default', tempo_val))
        print(f"beat_track (デフォルト): {tempo_val:.1f} BPM")
    except:
        print("beat_track (デフォルト): 失敗")
    
    # 方法2: 異なるパラメータでbeat_track
    try:
        tempo2, beats2 = librosa.beat.beat_track(y=y, sr=sr, hop_length=512, start_bpm=150, tightness=100)
        tempo2_val = float(tempo2[0]) if isinstance(tempo2, np.ndarray) else float(tempo2)
        bpm_estimates.append(('beat_track_tuned', tempo2_val))
        print(f"beat_track (調整済み): {tempo2_val:.1f} BPM")
    except:
        print("beat_track (調整済み): 失敗")
    
    # 方法3: RMSエネルギーのピーク検出
    try:
        hop_length = 512
        rms = librosa.feature.rms(y=y, hop_length=hop_length)[0]
        times = librosa.frames_to_time(np.arange(len(rms)), sr=sr, hop_length=hop_length)
        
        # ピーク検出
        peaks, _ = signal.find_peaks(rms, height=np.mean(rms), distance=int(sr/hop_length*0.2))  # 最小200ms間隔
        peak_times = times[peaks]
        
        if len(peak_times) > 1:
            intervals = np.diff(peak_times)
            avg_interval = np.mean(intervals)
            rms_bpm = 60.0 / avg_interval
            bpm_estimates.append(('rms_peaks', rms_bpm))
            print(f"RMSピーク分析: {rms_bpm:.1f} BPM")
    except:
        print("RMSピーク分析: 失敗")
    
    # 方法4: オンセット検出ベース
    try:
        onset_frames = librosa.onset.onset_detect(y=y, sr=sr, hop_length=hop_length)
        onset_times = librosa.frames_to_time(onset_frames, sr=sr, hop_length=hop_length)
        
        if len(onset_times) > 1:
            onset_intervals = np.diff(onset_times)
            # 異常値を除外
            q1, q3 = np.percentile(onset_intervals, [25, 75])
            iqr = q3 - q1
            lower_bound = q1 - 1.5 * iqr
            upper_bound = q3 + 1.5 * iqr
            filtered_intervals = onset_intervals[(onset_intervals >= lower_bound) & (onset_intervals <= upper_bound)]
            
            if len(filtered_intervals) > 0:
                avg_onset_interval = np.mean(filtered_intervals)
                onset_bpm = 60.0 / avg_onset_interval
                bpm_estimates.append(('onset_filtered', onset_bpm))
                print(f"オンセット分析 (フィルタ済み): {onset_bpm:.1f} BPM")
    except:
        print("オンセット分析: 失敗")
    
    # 方法5: スペクトル重心の周期性
    try:
        spectral_centroids = librosa.feature.spectral_centroid(y=y, sr=sr, hop_length=hop_length)[0]
        times = librosa.frames_to_time(np.arange(len(spectral_centroids)), sr=sr, hop_length=hop_length)
        
        # 自己相関でピリオド検出
        autocorr = np.correlate(spectral_centroids, spectral_centroids, mode='full')
        autocorr = autocorr[autocorr.size // 2:]
        
        # 最小・最大BPMに対応するラグ範囲を計算
        min_bpm, max_bpm = 60, 200
        max_lag = int(60.0 / min_bpm / (hop_length / sr))
        min_lag = int(60.0 / max_bpm / (hop_length / sr))
        
        if max_lag < len(autocorr) and min_lag >= 0:
            search_range = autocorr[min_lag:max_lag]
            peak_lag = np.argmax(search_range) + min_lag
            period_seconds = peak_lag * (hop_length / sr)
            spectral_bpm = 60.0 / period_seconds
            bpm_estimates.append(('spectral_autocorr', spectral_bpm))
            print(f"スペクトル自己相関: {spectral_bpm:.1f} BPM")
    except:
        print("スペクトル自己相関: 失敗")
    
    print(f"\n=== 統計分析 ===")
    if bpm_estimates:
        bpm_values = [bpm for _, bpm in bpm_estimates]
        mean_bpm = np.mean(bpm_values)
        median_bpm = np.median(bpm_values)
        std_bpm = np.std(bpm_values)
        
        print(f"平均BPM: {mean_bpm:.1f}")
        print(f"中央値BPM: {median_bpm:.1f}")
        print(f"標準偏差: {std_bpm:.1f}")
        
        # 最も信頼性の高い推定を選択
        print(f"\n=== BPM推定結果詳細 ===")
        for method, bpm in bpm_estimates:
            print(f"{method}: {bpm:.1f} BPM")
        
        # 最も信頼性の高い推定を選択
        # beat_trackの結果を優先し、次に中央値を使用
        if any('beat_track' in method for method, _ in bpm_estimates):
            beat_track_results = [(method, bpm) for method, bpm in bpm_estimates if 'beat_track' in method]
            recommended_method, recommended_bpm = beat_track_results[0]
            confidence_source = "beat_track分析"
        else:
            recommended_bpm = median_bpm
            confidence_source = "中央値"
        
        print(f"\n=== 推奨BPM ===")
        print(f"推奨BPM: {recommended_bpm:.0f} ({confidence_source})")
        
        # 一般的なBPM値との比較
        common_bpms = [120, 128, 140, 150, 154, 160, 170, 180]
        closest_common = min(common_bpms, key=lambda x: abs(x - recommended_bpm))
        print(f"最も近い一般的BPM: {closest_common}")
        
        if abs(recommended_bpm - closest_common) <= 3:
            print(f"✓ 一般的なBPM値 {closest_common} に近い値です")
        
        return recommended_bpm
    else:
        print("有効なBPM推定結果がありませんでした")
        return None
    
    # ビートタイミングの詳細分析
    if 'tempo' in locals() and 'beats' in locals():
        beat_times = librosa.frames_to_time(beats, sr=sr)
        print(f"\n=== ビートタイミング詳細 ===")
        print(f"検出ビート数: {len(beat_times)}")
        
        if len(beat_times) > 1:
            intervals = np.diff(beat_times)
            print(f"平均ビート間隔: {np.mean(intervals):.3f} 秒")
            print(f"ビート間隔の一貫性 (標準偏差): {np.std(intervals):.3f} 秒")
            
            # 推奨BPMの理論値と比較
            if 'recommended_bpm' in locals():
                theoretical_interval = 60.0 / recommended_bpm
                print(f"推奨BPM {recommended_bpm:.0f}の理論間隔: {theoretical_interval:.3f} 秒")
                print(f"理論値との差: {abs(np.mean(intervals) - theoretical_interval):.3f} 秒")

# メイン処理
if __name__ == "__main__":
    # コマンドライン引数の設定
    parser = argparse.ArgumentParser(description='音楽ファイルのBPMを安定した複数手法で分析します')
    parser.add_argument('audio_file', 
                       help='分析する音楽ファイルのパス（相対パス・絶対パス両対応）')
    
    args = parser.parse_args()
    audio_file = args.audio_file
    
    # 相対パスを絶対パスに変換
    if not os.path.isabs(audio_file):
        audio_file = os.path.abspath(audio_file)
    
    # ファイルの存在確認
    if not os.path.exists(audio_file):
        print(f"エラー: ファイルが見つかりません: {audio_file}")
        print(f"現在の作業ディレクトリ: {os.getcwd()}")
        print()
        print("使用方法:")
        print(f"  python {os.path.basename(sys.argv[0])} <音楽ファイルのパス>")
        print()
        print("例:")
        print(f"  python {os.path.basename(sys.argv[0])} Assets\\_MyExamples\\MusicGame\\AudioFiles\\NHKWaltz.mp3")
        print(f"  python {os.path.basename(sys.argv[0])} C:\\path\\to\\music.mp3")
        print(f"  python {os.path.basename(sys.argv[0])} music.mp3")
        exit(1)
    
    try:
        result_bpm = robust_bpm_analysis(audio_file)
        
        if result_bpm:
            print(f"\n🎵 最終結果: {result_bpm:.0f} BPM")
        else:
            print("\n❌ BPM推定に失敗しました")
        
    except Exception as e:
        print(f"エラーが発生しました: {e}")
        import traceback
        traceback.print_exc()
