import librosa
import numpy as np
import os
import sys
import argparse

def detect_initial_silence(audio_file_path, silence_threshold=0.01, frame_length=1024, hop_length=512):
    """
    音楽ファイルの最初の無音時間を検出する
    
    Parameters:
    audio_file_path: 音楽ファイルのパス
    silence_threshold: 無音と判定する振幅の閾値（デフォルト: 0.01）
    frame_length: フレーム長
    hop_length: ホップ長
    
    Returns:
    silence_duration: 最初の無音時間（秒）
    """
    
    # 音楽ファイルを読み込み
    y, sr = librosa.load(audio_file_path)
    
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

# メイン処理
if __name__ == "__main__":
    # コマンドライン引数の設定
    parser = argparse.ArgumentParser(description='音楽ファイルの最初の無音時間を検出します')
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
    
    print(f"音楽ファイル: {audio_file}")
    print("分析中...")
    
    try:
        # 複数の閾値で試してみる
        thresholds = [0.001, 0.005, 0.01, 0.02]
        
        for threshold in thresholds:
            silence_duration = detect_initial_silence(audio_file, silence_threshold=threshold)
            print(f"閾値 {threshold}: 最初の無音時間 = {silence_duration:.3f} 秒")
        
        # 詳細な分析（推奨の閾値で）
        print("\n詳細分析:")
        y, sr = librosa.load(audio_file)
        print(f"サンプリングレート: {sr} Hz")
        print(f"総長: {len(y)/sr:.3f} 秒")
        
        # RMSエネルギーの最初の10秒分を表示
        rms = librosa.feature.rms(y=y, frame_length=1024, hop_length=512)[0]
        times = librosa.frames_to_time(np.arange(len(rms)), sr=sr, hop_length=512)
        
        print("\n最初の10秒のRMSエネルギー:")
        for i, (time, energy) in enumerate(zip(times[:100], rms[:100])):
            if time > 10:
                break
            print(f"{time:.3f}秒: {energy:.6f}")
            
    except Exception as e:
        print(f"エラーが発生しました: {e}")
