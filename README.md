# EasyVTuber
* Macだけでバ美肉会議するための簡易ツールです。
* アバターはVRMのみ対応しています。

<img src="./images/completion.gif" width="480px">

# Requirement
Mac OS X 10.14 or 10.15で動作確認済みです。

仮想カメラにCamTwistを利用するためダウンロードしてください。
* [CamTwist 3.4.3](http://camtwiststudio.com/download/)


# How to Use

## フェイストラッキングの開始
* [VRoid Hub](https://hub.vroid.com)などからVRMファイルをダウンロード。
* Macのローカルディスクのユーザのホームディレクトリにavatar.vrmという名前で配置  
  (例： OSユーザがyoshidanの場合、/Users/yoshidan/avatar.vrm)
* 下のリンクからアプリをダウンロードして起動。
https://drive.google.com/file/d/1Y9IkXH5VCh60tBIKv3LuVe7eS908pAL5/view  

画面サイズが聞かれるので任意のサイズを設定してPlayを押してください。  
Windowedを忘れると全画面になってしまうのでチェックをつけた方がよいです。  
解像度は1280 * 800くらいがよいと思います。  

<img src="./images/boot.png" width="320px"> 

しばらくするとカメラのパーミッション許可が求められます。  
許可してしばらく待つとアプリにアバターが表示されます。

<img src="./images/snap.png" width="320px">

## CamTwistの設定
たぶん[このリンク](https://hori-ryota.com/blog/live-broadcasting-with-mac/)が分かりやすいですが、  
まとめると以下の手順を辿ればOKです。

1. Preferencesで解像度を変更
2. Step1でDesktop+を選択
3. SettingsのSelect from exsisting windowで「EasyVTuber」を選択
4. Select Capture areaボタンを押してウインドウのタイトル部が入らないようにエリアを設定

<img src="./images/camtwist3.png" width="320px">

5. Done Selectionしてエリアを確定
6. Step3でSave Setupを実行

下図のような設定になっていればOKです。一度設定した後は

* Cam Twistメイン画面
<img src="./images/camtwist1.png" width="320px">

* Preferences
<img src="./images/camtwist2.png" width="320px">

## 会議ツールでの使用（例:Zoom.us)
[Zoom.us](https://zoom.us/download)を起動してビデオ設定のカメラでCamTwistを選択してください。

<img src="./images/zoom.png" width="480px">


# その他 
* EasyVTuberのウインドウをアクティブにしていないといけませんがキーを押してる間は表情を変えます。
  - A -> Angry
  - J -> Joy
  - F -> Fun
  - S -> Surprised
  - E -> Extra
* トラックパッドでクリックした状態でスクロールすると横回転と縦方向移動します。

* バ美声は対応していませんので、Voidolとsoundflowerを使って仮想マイクを作るなどしてください。
