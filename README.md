# EasyVTuber
* Macだけでバ美肉会議するための簡易ツールです。
* アバターはVRMのみ対応しています。 

ビデオチャットモード  
<img src="./images/completion.gif" width="480px">

デスクトップ共有モード  
<img src="./images/completion_new_desk.gif" width="640px">

旧バージョンから大幅に動作が変わっています。旧バージョンは[こちら](OLD_VERSION.md)

# Requirement
以下の環境で動作確認済みです。
* Mac OS Mojave(10.14)
* Mac OS Catalina(10.15)

仮想カメラにCamTwistを利用するためダウンロードしてください。
* [CamTwist 3.4.3](http://camtwiststudio.com/download/)

# How to Use

## フェイストラッキングの開始
* [VRoid Hub](https://hub.vroid.com)などからVRMファイルをダウンロード。
* Macのローカルディスクのユーザのホームディレクトリにavatar.vrmという名前で配置  
  (例： OSユーザがyoshidanの場合、/Users/yoshidan/avatar.vrm)
* 下のリンクからアプリをダウンロードして起動。
https://drive.google.com/file/d/1Y9IkXH5VCh60tBIKv3LuVe7eS908pAL5/view  

* 初回実行時にアクセシビリティの実行権限が求められます。アクセシビリティに権限を与えて、再起動してください。   
  （権限がない場合は、アプリがアクティブになっていない時にデスクトップ共有モードでのキータイプ時の動きがなくなります） 

しばらくするとカメラのパーミッション許可が求められます。  
許可してしばらく待つとアプリにアバターが表示されます。

<img src="./images/snap.png" width="320px">

* アプリはデスクトップの最前面に常駐します。（Keynoteの全画面再生時よりも前面）

## CamTwistの設定
たぶん[このリンク](https://hori-ryota.com/blog/live-broadcasting-with-mac/)が分かりやすいですが、  
まとめると以下の手順を辿ればOKです。

1. Preferencesで解像度を変更
2. Step1でDesktop+を選択
3. SettingsのSelect from exsisting windowで「EasyVTuberNew」を選択

下図のような設定になっていればOKです。

* Cam Twistメイン画面
<img src="./images/camtwist1.png" width="480px">

* Preferences
<img src="./images/camtwist2.png" width="320px">

## 会議ツールでの使用（例:Zoom.us)
[Zoom.us](https://zoom.us/download)を起動してビデオ設定のカメラでCamTwistを選択してください。  
* カメラにCamTwistが出ない場合は再起動してみてください。  
* それでも認識しない場合は、Zoomのバージョンに問題があります。最新版にして、以下のコマンドを実行してみてください。
`$ codesign --remove-signature /Applications/zoom.us.app/` （Xcodeがインストール済みである必要があります；また権限周りのエラーが起きた場合、`sudo codesign` 等で対応可能なはずです。）

<img src="./images/zoom.png" width="480px">

## Zoomでのバーチャル背景の使用
Zoomのバーチャル背景で「グリーンスクリーンがあります」にチェックをつけるとバーチャル背景が完全になります。

<img src="./images/virtual.jpg" width="480px">

## 自前の背景の利用

バーチャル背景が使えないツール用です。  
アバターと同じディレクトリにbackground.jpgという名称で画像を保存  
(例： OSユーザがyoshidanの場合、/Users/yoshidan/background.jpg)

アプリ起動時に、背景を配置した画像に変更します。  
デフォルトでは、画像のアスペクト比を保ったまま横方向は全て表示します。画像のアスペクト比が画面のアスペクト比と合わない場合、縦方向は中央中心に切り取って表示します。
設定画面で 縦方向全表示との切り替えが可能です。 
<img src="./images/bgimage.jpg" width="320px">

## 画面共有時のマスコット化

# 機能

## メニュー(ショートカットキー「m」)
アプリがアクティブになっている時にキーボードの「m」を押すと、以下のようにメニュー画面が開きます。
※ アプリ自体はウインドウの最前面に常駐しますが、アクティブにするにはマウスでウインドウを選択する必要があります。

| 機能 | 説明 | ショートカットーキー | 
| ---- | ---- | ----------- | 
| 背景透過 | 背景を透明にします。主にデスクトップ共有モードでの利用します。 | t |
| 背景画像を横方向全表示 | 縦長の背景画像を使った場合で、縦方向に全部表示するようにします | w |
| デスクトップ共有モード | カメラを引き、タイピングやマウス操作をアバターに反映するようになります | z |
| フェイストラッキング開始 | カメラを切り替える場合に使用します |  |
| 音声リップシンク有効 | デフォルトではフェイストラッキングで口を動かしますが、オンにするとOVRLipSyncで動かすようになります |  |
| カメラコントロール有効 | デフォルトではウインドウをドラッグするとアプリを移動しますが、オンにするとアプリ内でYaw方向のカメラ回転ができます | c |


## 表情ショートカット
アプリをアクティブにした状態で以下のキーコードを認識します。

| キーコード | 機能 |
|----------|------|
| a | 表情をAngryにする |
| j | 表情をJoyにする |
| f | 表情をFunにする |
| s | 表情をsurprisedにする |
| e | 表情をExtraにする |

