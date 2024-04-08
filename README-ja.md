# Unity-SimpleSoundSuite

Unity向けのシンプルなサウンドシステムです。

## 主な機能

- 事前指定したID、名前を利用して音を鳴らせる
- Jukeboxファイルを利用してランタイムに自由に音データのグループを追加/削除できる。JukeboxをAssetbundle化することで事後のダウンロードにも対応
- Jukebox内であらかじめ指定しておくことで、音のランダムピッチ変化、ランダムに違う音を再生、同時発音数制御が可能
- (効果音)awaitで再生を待機できる
- (BGM)フェードイン、フェードアウト、クロスフェード再生

# インストール

R3 version: https://github.com/sh-kj/Unity-SimpleSoundSuite.git?path=R3/SimpleSoundSuite  
UniRx version: https://github.com/sh-kj/Unity-SimpleSoundSuite.git?path=UniRx/SimpleSoundSuite  
Package Managerの`Install package from git URL...`で↑どちらかを指定することでインストールできます。

## 依存ライブラリ

(必須) UniTask https://github.com/Cysharp/UniTask.git

(どちらかを選択)  
R3 https://github.com/Cysharp/R3.git  
UniRx https://github.com/neuecc/UniRx

R3とUniRxは使用しているほうのサブフォルダ下をPackageManager登録してください。

