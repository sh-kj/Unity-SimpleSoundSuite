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

(必須)  
UniTask https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask  

(どちらかを選択)  
R3 https://github.com/Cysharp/R3.git  
UniRx https://github.com/neuecc/UniRx

R3とUniRxは使用しているほうのサブフォルダ下をPackageManager登録してください。

# 使用法(事前準備)

## Jukeboxの作成

AssetsメニューまたはProjects右クリックから、`Create/radiants/SimpleSoundSuite/Make Jukebox ScriptableObject` でJukeboxのScriptableObjectを作成します。

Jukeboxは複数のAudioClipを格納してそれぞれにIDと名前を割り振ることができます。  

## Jukeboxへの音の追加、編集

Windowメニューより`SimpleSoundSuite/Open Jukebox Editor`から、専用のウィンドウでJukeboxの編集を行えます。ウィンドウが開いたら`Target`で作成したJukeboxを指定してください。

AudioClipをドラッグ&ドロップ、または`+`ボタンで`Sound Element`を追加できます。  

### SoundElement

`SoundElement`は、ユニークなID(long)とName(string)を持った音ファイルの単位です。  
一つの`Sound Element`は複数のClipを持つことができ、再生が呼ばれた時にそれらのうち1つを(ランダム等で)選んで再生します。

#### パラメータ

- Name(string)  
音を呼び出す時に使用する名前
- ID(long)  
音を呼び出す時に使用するID。名前もしくはIDで音を検索し、呼び出せる
- Polyphony  
このSoundElementはいくつまで同時に発音できるか？の設定。0で無制限  
敵撃破時の爆発音などで同時発音数を制限したい場合に使う
- Order
Clipを複数持っている時に、どういった法則で再生するClipを選択するかの設定。  
Random Not Repeatでは直前に発した音は抽選されなくなる。

#### パラメータ(Clipごと)

- Volume  
Clip自体の音量調整用ボリューム
- Change Pitch  
ピッチをランダム変化させるか、またそのランダム範囲
- Change Pan  
パンをランダム変化させるか、またそのランダム範囲

## SoundPlayerの準備

ランタイムでの音の再生はMonobehaviourの`SoundPlayer`クラスが担います。これはゲーム中に1つ存在すればほぼ充分なので、起動シーンや起動直後にAdditiveで読み込むシーンに存在させておき`DontDestroyOnLoad`しておくことを推奨します。

### Jukeboxの登録

SoundPlayerにはInspector参照でJukeboxをシーン保持できる`DefaultJukeboxes`と、ランタイムにいつでもJukeboxを追加可能な`AddJukebox()`メソッドが存在します。後者はAssetBundle等でJukeboxをランタイムに読み込んだ時に使う想定です。  
なお、別々のJukeboxで同一のIDもしくはNameを持つ`SoundElement`があり、それを呼び出そうとした場合は`AddJukebox()`でランタイム追加したほうの音が優先されます。

### (オプション)static呼び出し

SoundPlayerはMonobehaviourなのでstaticではありませんが、音はゲームの様々なタイミングで再生したいので音の再生呼び出しはstatic化しておいた方が便利です。  
interfaceを介してstaticクラスから音を再生できるようにしておくと、設計を汚さずに利便性を得られます。

設計の性質上、こちらはライブラリに入れておくべきではなく使用プロジェクト側に配置すべきですので、サンプルソースを以下に置いておきます。

https://github.com/sh-kj/Unity-SimpleSoundSuite.git?path=static-sample

SoundPlayerInjector(Monobehaviour)をSoundPlayerを参照させるように配置しておくことでstaticなSoundManagerにSoundPlayerをInjectし、どこからでも`SoundManager.Play()`で音を鳴らすことができるようになります。

# 使用法(ランタイム)

## 効果音の再生

`SoundPlayer`の`Play()`で、IDまたはNameを指定して効果音を再生します。全体の音量ボリューム調整等はAudioMixerで行う想定で、`AudioMixerGroup`が指定できます。

`PlayAwaitable()`で、await可能な効果音再生を行います。

## BGMの再生

`SoundPlayer`の`PlayMusic()`でBGMをループ再生できます。既にBGMが流れている場合は前のBGMとクロスフェードして切り替わります。  
`forceReplay=true`としない限り、今流れているものと同一のBGMを指定してもBGMは止まりません。

`StopMusic()`でBGMを中断します。