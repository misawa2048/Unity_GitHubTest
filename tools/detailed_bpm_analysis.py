import librosa
import numpy as np
import os
import sys
import argparse

def detect_initial_silence(y, sr, silence_threshold=0.01, frame_length=1024, hop_length=512):
    """
    音楽ファイルの最初の無音時間を検出する
    
    Parameters:
    y: 音声信号
    sr: サンプリングレート
    silence_threshold: 無音と判定する振幅の閾値
    frame_length: フレーム長
    hop_length: ホップ長
    
    Returns:
    silence_duration: 最初の無音時間（秒）
    """
    # RMS（Root Mean Square）エネルギーを計算
    rms = librosa.feature.rms(y=y, frame_length=frame_length, hop_length=hop_length)[0]
    
    # 各フレームの時間を計算
    times = librosa.frames_to_time(np.arange(len(rms)), sr=sr, hop_length=hop_length)
    
    # 無音でないフレームを見つける
    non_silent_frames = np.where(rms > silence_threshold)[0]
    
    if len(non_silent_frames) == 0:
        # 全て無音の場合
        return len(y) / sr
    
    # 最初の非無音フレームの時間
    first_sound_time = times[non_silent_frames[0]]
    
    return first_sound_time

def detailed_bpm_analysis(audio_file_path):
    """
    音楽ファイルのみを使用してBPMを詳細分析
    """
    
    # 音楽ファイルを読み込み
    y, sr = librosa.load(audio_file_path)
    
    print(f"\n=== 音声ファイル分析 ===")
    print(f"音楽ファイル: {audio_file_path}")
    print(f"サンプリングレート: {sr} Hz")
    print(f"総長: {len(y)/sr:.3f} 秒")
    
    # 最初の無音時間を検出
    silence_duration = detect_initial_silence(y, sr)
    print(f"最初の無音時間: {silence_duration:.3f} 秒")
    
    # より細かい閾値での無音検出も実行
    silence_thresholds = [0.001, 0.005, 0.01]
    print(f"詳細無音分析:")
    for threshold in silence_thresholds:
        silence_detail = detect_initial_silence(y, sr, silence_threshold=threshold)
        print(f"  閾値 {threshold}: {silence_detail:.3f} 秒")
    
    # 無音時間を考慮した音声データ（オプション：無音をスキップして分析）
    if silence_duration > 0.1:  # 100ms以上の無音がある場合
        silence_samples = int(silence_duration * sr)
        y_trimmed = y[silence_samples:]
        print(f"無音部分をスキップして分析: {silence_duration:.3f}秒後から開始")
        analysis_y = y_trimmed
        analysis_note = " (無音スキップ後)"
    else:
        analysis_y = y
        analysis_note = ""
    
    # 複数の方法でBPMを推定
    methods_results = {}
    
    print(f"\n=== BPM推定{analysis_note} ===")
    
    # 方法1: 基本的なbeat_track
    tempo1, beats1 = librosa.beat.beat_track(y=analysis_y, sr=sr)
    tempo1_value = float(tempo1[0]) if isinstance(tempo1, np.ndarray) else float(tempo1)
    methods_results['beat_track'] = tempo1_value
    
    # 方法2: より厳密なパラメータでbeat_track
    tempo2, beats2 = librosa.beat.beat_track(y=analysis_y, sr=sr, hop_length=512, start_bpm=150)
    tempo2_value = float(tempo2[0]) if isinstance(tempo2, np.ndarray) else float(tempo2)
    methods_results['beat_track_strict'] = tempo2_value
    
    # 方法3: tempogram分析
    hop_length = 512
    tempogram = librosa.feature.tempogram(y=analysis_y, sr=sr, hop_length=hop_length)
    tempo_frequencies = librosa.tempo_frequencies(len(tempogram), sr=sr, hop_length=hop_length)
    avg_tempogram = np.mean(tempogram, axis=1)
    peak_tempo_idx = np.argmax(avg_tempogram)
    tempo3_value = tempo_frequencies[peak_tempo_idx]
    methods_results['tempogram'] = tempo3_value
    
    # 方法4: オンセット検出を使用した分析
    try:
        onset_frames = librosa.onset.onset_detect(y=analysis_y, sr=sr, hop_length=hop_length)
        if len(onset_frames) > 1:
            onset_times = librosa.frames_to_time(onset_frames, sr=sr, hop_length=hop_length)
            onset_intervals = np.diff(onset_times)
            # 異常値を除外
            filtered_intervals = onset_intervals[(onset_intervals > 0.2) & (onset_intervals < 2.0)]
            if len(filtered_intervals) > 0:
                avg_onset_interval = np.mean(filtered_intervals)
                onset_bpm = 60.0 / avg_onset_interval
                methods_results['onset_based'] = onset_bpm
    except:
        pass
    
    print(f"\n=== BPM推定結果 ===")
    for method, bpm in methods_results.items():
        print(f"{method}: {bpm:.1f} BPM")
    
    print(f"\n=== 統合BPM分析 ===")
    # tempogramの結果が異常値の場合は除外
    valid_results = {method: bpm for method, bpm in methods_results.items() 
                    if bpm > 60 and bpm < 300 and not np.isinf(bpm)}
    
    if valid_results:
        bpm_values = list(valid_results.values())
        mean_bpm = np.mean(bpm_values)
        median_bpm = np.median(bpm_values)
        std_bpm = np.std(bpm_values)
        
        print(f"有効なBPM推定:")
        for method, bpm in valid_results.items():
            print(f"  {method}: {bpm:.1f} BPM")
        
        print(f"\n統計値:")
        print(f"平均BPM: {mean_bpm:.1f}")
        print(f"中央値BPM: {median_bpm:.1f}")
        print(f"標準偏差: {std_bpm:.1f}")
        
        # 最も信頼性の高い推定を選択
        # beat_trackの結果を優先し、次に中央値を使用
        if 'beat_track' in valid_results:
            recommended_bpm = valid_results['beat_track']
            confidence_source = "beat_track分析"
        elif 'beat_track_strict' in valid_results:
            recommended_bpm = valid_results['beat_track_strict']
            confidence_source = "beat_track_strict分析"
        else:
            recommended_bpm = median_bpm
            confidence_source = "中央値"
        
        print(f"\n=== 推奨BPM ===")
        print(f"推奨BPM: {recommended_bpm:.0f} ({confidence_source})")
        
        # 無音時間を考慮した補正
        if silence_duration > 0.001:  # 1ms以上の無音がある場合
            print(f"\n=== 無音時間による補正情報 ===")
            print(f"検出された無音時間: {silence_duration:.3f} 秒")
            
            # 無音時間がビート同期に影響する可能性をチェック
            beat_interval = 60.0 / recommended_bpm
            silence_in_beats = silence_duration / beat_interval
            
            print(f"推奨BPM {recommended_bpm:.0f} での1ビート間隔: {beat_interval:.3f} 秒")
            print(f"無音時間のビート数相当: {silence_in_beats:.2f} ビート")
            
            if silence_in_beats > 0.1:
                print(f"⚠ 無音時間が存在するため、楽曲開始時にオフセット調整が必要な可能性があります")
                offset_ms = silence_duration * 1000
                print(f"推奨オフセット: {offset_ms:.0f} ms")
            else:
                print(f"✓ 無音時間は短く、オフセット調整は不要です")
        
        # 一般的なBPM値との比較
        common_bpms = [120, 128, 140, 150, 154, 160, 170, 180]
        closest_common = min(common_bpms, key=lambda x: abs(x - recommended_bpm))
        print(f"\n最も近い一般的BPM: {closest_common}")
        
        if abs(recommended_bpm - closest_common) <= 3:
            print(f"✓ 一般的なBPM値 {closest_common} に近い値です")
        
        return {
            'bpm': recommended_bpm,
            'silence_duration': silence_duration,
            'confidence_source': confidence_source,
            'closest_common_bpm': closest_common
        }
    else:
        print("有効なBPM推定結果がありませんでした")
        return None

# メイン処理
if __name__ == "__main__":
    # コマンドライン引数の設定
    parser = argparse.ArgumentParser(description='音楽ファイルのBPMを詳細分析します')
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
        result = detailed_bpm_analysis(audio_file)
        
        if result:
            print(f"\n🎵 最終結果:")
            print(f"   BPM: {result['bpm']:.0f}")
            print(f"   無音時間: {result['silence_duration']:.3f} 秒")
            print(f"   信頼度: {result['confidence_source']}")
            if result['silence_duration'] > 0.001:
                offset_ms = result['silence_duration'] * 1000
                print(f"   推奨オフセット: {offset_ms:.0f} ms")
        else:
            print("\n❌ BPM推定に失敗しました")
        
    except Exception as e:
        print(f"エラーが発生しました: {e}")
        import traceback
        traceback.print_exc()
